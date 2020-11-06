using System;
using System.Threading;
using System.Threading.Tasks;
using MultiplayerMinesweeper.Core.Multiplayer.TaskManagement;

namespace MultiplayerMinesweeper.Core.Multiplayer
{
    public enum MultiplayerRole
    {
        Host,
        Guest
    }

    public class MultiplayerConnection
    {
        private Task _mainTask;
        private TaskAction<bool> _uploadTask;
        private TaskAction<string> _downloadTask;
        private CancellationToken _cancelToken;
        private CancellationTokenSource _tokenSource;

        private readonly MultiplayerRole _role;
        private readonly StopWatch.StopWatch _stopWatch;

        private bool _requestCloseConnection = false;

        public MultiplayerConnection(MultiplayerRole role, GameSettings settings)
        {
            _uploadTask = null;
            _downloadTask = null;
            _role = role;
            _stopWatch = new StopWatch.StopWatch();
            CurrentData = new MultiplayerData(settings);

            // set initial data
            IsConnectionClosed = false;

            switch (role)
            {
                case MultiplayerRole.Host:
                    (CurrentData.Host.Board as MinesweeperBoard).PopulateBomb();
                    break;
                case MultiplayerRole.Guest:
                    (CurrentData.Guest.Board as MinesweeperBoard).PopulateBomb();
                    break;
            }
        }

        public readonly MultiplayerData CurrentData;
        public int TimePlayed => _stopWatch.TimeElapsed;
        public bool IsConnectionEstablished => _uploadTask != null && _downloadTask != null;
        public bool IsConnectionClosed { get; private set; }

        private string GetLink(string jsonKey)
            => $"{Constants.GAMENAME}{CurrentData.GameSettings.GameID}{(jsonKey == "" ? "" : "/" + jsonKey)}";

        /// <summary>
        /// Establish new game session.
        /// If the player is a guest, they must first download the game settings first (not implemented)
        /// </summary>
        /// <returns>True if the establishment is successful or false on failure</returns>
        public void Establish()
        {
            _tokenSource = new CancellationTokenSource();
            _cancelToken = _tokenSource.Token;

            // change multiplayer game state depending on role
            switch (_role)
            {
                case MultiplayerRole.Guest:
                    var task = Firebase.Get(GetLink("state"));
                    task.Wait();

                    // malformed state
                    if (!int.TryParse(task.Result, out int _state)) Close();
                    GameState state = (GameState)_state;
                    if(state == GameState.Connecting || state == GameState.Waiting)
                        CurrentData.ChangeGameState(GameState.Playing);

                    break;
                case MultiplayerRole.Host:
                    CurrentData.ChangeGameState(GameState.Waiting);
                    break;
            }

            // overall upload task
            _uploadTask = UploadTask.CreateInstance(_cancelToken);

            // overall download task
            _downloadTask = DownloadTask.CreateInstance(_cancelToken, (result) =>
            {
                // must try here because exception could throw in following code of try block
                try
                {
                    // the result will be merged
                    Logger.Log(result);
                    MergeData(result);
                }
                catch (Exception e)
                {
                    Logger.Log("Exception raised: " + e.Message);
                    Close();
                }
            });

            // main task to initiate both uploading and downloading tasks
            _mainTask = Task.Run(() =>
            {
                PlayerData data = _role == MultiplayerRole.Host ? CurrentData.Host : CurrentData.Guest;
                while (!_cancelToken.IsCancellationRequested)
                {
                    Logger.Log("Doing main task here...");
                    data.Time = TimePlayed;
                    UploadData(data);
                    DownloadData();
                    Thread.Sleep(1000);
                }
            }, _cancelToken);

            // set a running thread for closing connection from another thread
            Task.Run(() =>
            {
                // preventing multiple threads "data racing" with closing this connection
                // only one thread can request closing the connection
                // the thread that is trying to close is actually a part
                // of one player's program that is passively waiting
                // for further instruction on closing the multiplayer game
                // -> the last one to close is the one to delete data, simple
                while (!_requestCloseConnection && !IsConnectionClosed) { }
                Close(true);
            });
        }

        /// <summary>
        /// Just separate this from the Establish method
        /// for better code reading
        /// </summary>
        /// <param name="result"></param>
        private void MergeData(string result)
        {
            GameState state = CurrentData.State;
            bool isHost = _role == MultiplayerRole.Host;
            PlayerData player = isHost ? CurrentData.Host : CurrentData.Guest,
                opponent = isHost ? CurrentData.Guest : CurrentData.Host;

            // merge the right hand side board with downloaded data
            Logger.Log("Merge downloaded JSON...");
            opponent.FromJson(result);

            if (state == GameState.Playing) _stopWatch.Start();

            // because the result is received passively, we also need to specify the winning condition
            // through game state received
            if (state == GameState.GuestWin || state == GameState.HostWin
            || state == GameState.HostExited || state == GameState.GuestExited)
            {
                if (_requestCloseConnection) return;

                // you don't want 2+ times below code executing
                _requestCloseConnection = true;
                UI.ChangeToResultPage(
                    player.Flag,
                    (player.Board as MinesweeperBoard).Bomb,
                    player.Time,
                    state == (isHost ? GameState.HostWin : GameState.GuestWin) ||
                    state == (isHost ? GameState.GuestExited : GameState.HostExited)
                );
            }
        }

        /// <summary>
        /// Pushes new upload data task into the task list
        /// </summary>
        private void UploadData(IJson data)
        {
            string role = _role == MultiplayerRole.Host ? "host" : "opponent";

            _uploadTask?.AddTask(Task.Run(() =>
            {
                if (_cancelToken.IsCancellationRequested) return false;

                var task = Firebase.Put(data.ToJsonString(), GetLink(role));
                return task.Wait(Constants.TIMEOUT) && task.Result;
            }, _cancelToken));
        }

        /// <summary>
        /// Pushes new download data task into the task list
        /// </summary>
        private void DownloadData()
        {
            string role = _role == MultiplayerRole.Host ? "opponent" : "host";

            _downloadTask?.AddTask(Task.Run(() =>
            {
                if (_cancelToken.IsCancellationRequested) return "";

                // get game state
                // malformed game state will make the game unplayable
                var task = Firebase.Get(GetLink("state"));
                if (!task.Wait(Constants.TIMEOUT)
                || task.Result == "null"
                || !int.TryParse(task.Result, out int state)
                || !Enum.IsDefined(typeof(GameState), state)) return "";

                CurrentData.ChangeGameState((GameState)state, false);
                Logger.Log("GameState changed");

                // get new data from role
                task = Firebase.Get(GetLink(role));
                return task.Wait(Constants.TIMEOUT) ? task.Result : "";
            }, _cancelToken));
        }

        /// <summary>
        /// Closes every possible connections from this object to the server.
        /// This method also set appropriate flags for stored game data on server,
        /// as well as (might be) triggering GC with null values
        /// </summary>
        public void Close(bool deleteData = false)
        {
            if(!IsConnectionClosed)
            {
                string role = _role.ToString();
                bool isHost = _role == MultiplayerRole.Host;

                // sudden close
                if (CurrentData?.State == GameState.Playing)
                    CurrentData?.ChangeGameState(
                        isHost ? GameState.HostExited : GameState.GuestExited,
                        true, true
                    );

                // stop the stop watch
                _stopWatch.Stop();

                // cancel the token source first
                _tokenSource.Cancel();
                _mainTask.Wait();
                _mainTask.Dispose();
                Logger.Log($"Main task finished from {role}");

                // then free up every tasks
                _uploadTask.Close();
                _downloadTask.Close();
                Logger.Log($"Other tasks also finished from {role}");

                // set to null for next check / gc invoker
                _tokenSource = null;
                _mainTask = null;
                _uploadTask = null;
                _downloadTask = null;

                // leave only settings and game state behind, not deleting all of it...
                if (deleteData)
                {
                    // delete parts of the game
                    Firebase.Delete(GetLink("host")).Wait(Constants.TIMEOUT);
                    Firebase.Delete(GetLink("opponent")).Wait(Constants.TIMEOUT);
                }

                // set flag
                IsConnectionClosed = true;

                // log
                Logger.Log($"Successfully closed connection to the server from {role}");
            }
        }
    }
}
