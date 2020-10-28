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
        public bool Establish()
        {
            _tokenSource = new CancellationTokenSource();
            _cancelToken = _tokenSource.Token;

            // change multiplayer game state
            CurrentData.ChangeGameState(GameState.Waiting);

            // overall upload task
            _uploadTask = UploadTask.CreateInstance(_cancelToken);

            // overall download task
            _downloadTask = DownloadTask.CreateInstance(_cancelToken, (result) =>
            {
                // must try here because exception could throw in following code of try block
                try
                {
                    // the result will be merged
                    if (result != "") MergeData(result);
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
                while (!_cancelToken.IsCancellationRequested)
                {
                    Logger.Log("Doing main task here...");
                    UploadData(_role == MultiplayerRole.Host ? CurrentData.Host : CurrentData.Guest);
                    DownloadData();
                    Thread.Sleep(1000);
                }
            }, _cancelToken);

            return true;
        }

        /// <summary>
        /// Just separate this from the Establish method
        /// for better code reading
        /// </summary>
        /// <param name="result"></param>
        private void MergeData(string result)
        {
            bool isHost = _role == MultiplayerRole.Host;
            PlayerData player = isHost ? CurrentData.Host : CurrentData.Guest;
            player.FromJson(result);

            // get the state number
            var task = Firebase.Get(GetLink("state"));
            if (task.Wait(Constants.TIMEOUT)
            && int.TryParse(task.Result, out int _state))
            {
                GameState state = (GameState)_state;

                switch (state)
                {
                    case GameState.Waiting:
                        // if game state is waiting, switch it to playing
                        // because the result starts to be other than an empty string
                        // -> a simplified board received
                        CurrentData.ChangeGameState(GameState.Playing);
                        _stopWatch.Start();
                        break;
                    case GameState.Playing:
                        // while playing, if the host receives a state of GuestWin
                        // or the guest receives a state of HostWin
                        // -> loses nonetheless
                        if ((isHost && state == GameState.GuestWin)
                        || (!isHost && state == GameState.HostWin))
                        {
                            Close();
                            UI.ChangeToResultPage(
                                player.Flag,
                                (player.Board as MinesweeperBoard).Bomb,
                                player.Time,
                                false
                            );
                            return;
                        }
                        break;
                }
            }
        }

        /// <summary>
        /// Push new upload data task into the task list
        /// </summary>
        private void UploadData(IJson data)
        {
            if (_uploadTask != null)
            {
                string role = _role == MultiplayerRole.Host ? "host" : "opponent";

                _uploadTask.AddTask(Task.Run(() =>
                {
                    if (_cancelToken.IsCancellationRequested) return false;

                    var task = Firebase.Put(data.ToJsonString(), GetLink(role));
                    return task.Wait(Constants.TIMEOUT) && task.Result;
                }, _cancelToken));
            }
        }

        /// <summary>
        /// Push new download data task into the task list
        /// </summary>
        private void DownloadData()
        {
            if (_downloadTask != null)
            {
                string role = _role == MultiplayerRole.Host ? "opponent" : "host";

                _downloadTask.AddTask(Task.Run(() =>
                {
                    if (_cancelToken.IsCancellationRequested) return "";

                    // get game state
                    // malformed game state will make the game unplayable
                    var task = Firebase.Get(GetLink("state"));
                    if (!task.Wait(Constants.TIMEOUT)
                    || int.TryParse(task.Result, out int state)
                    || !Enum.IsDefined(typeof(GameState), state)) return "";

                    CurrentData.ChangeGameState((GameState)state);

                    // get new data from role
                    task = Firebase.Get(GetLink(role));
                    return task.Wait(Constants.TIMEOUT) ? task.Result : "";
                }, _cancelToken));
            }
        }

        public void Close()
        {
            string role = _role.ToString();

            // sudden close
            if(CurrentData.State == GameState.Playing)
                CurrentData.ChangeGameState(_role == MultiplayerRole.Host ? GameState.HostExited : GameState.GuestExited);

            // stop the stop watch
            _stopWatch.Stop();

            // cancel the token source first
            _tokenSource.Cancel();
            Logger.Log($"Cancel token requested from {role}!");
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

            // prevent creating ghost rooms
            Firebase.Delete(GetLink("")).Wait(Constants.TIMEOUT);
            // set flag
            IsConnectionClosed = true;

            // log
            Logger.Log($"Successfully closed connection to the server from {role}");
        }
    }
}
