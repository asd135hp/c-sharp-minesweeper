using SplashKitSDK;
using System;
using System.Collections.Generic;
using MultiplayerMinesweeper.Core;
using MultiplayerMinesweeper.Core.Multiplayer;
using MultiplayerMinesweeper.Drawing.Component;
using System.Threading.Tasks;

namespace MultiplayerMinesweeper.Drawing.UI
{
    public class LoadingPage : GraphicalPage
    {
        private readonly GraphicalPage _prevPage;
        private GameSettings _settings;

        // mitigate a potential error where user might exit when the game is loading information to the server
        // as it will greatly polute server if it happens
        private LoadingPage()
        {
            _drawingObjects.AddRange(new UIRectangle[]
            {
                new Button("Your game id is:")
                {
                    HorizontalAlign = 0.05f,
                    VerticalAlign = 0.05f,
                    BorderColor = Constants.BACKGROUND_COLOR
                },
                new Button(0, 0, "Loading...", Constants.HEADER_FONT_SIZE)
                {
                    HorizontalAlign = 0.5f,
                    VerticalAlign = 0.5f,
                    BorderColor = Constants.BACKGROUND_COLOR
                }
            });
            _drawingObjects.ForEach((button) =>
            {
                button.AlignHorizontally();
                button.AlignVertically();
            });
        }
        public LoadingPage(GraphicalPage prevPage) : this()
        {
            _prevPage = prevPage;

            // guest
            Task.Run(() =>
            {
                var queue = GetQueue();
                if(queue.Count == 0)
                {
                    DisplayErrorMessage("There is currently no game queued. Please try again!");
                    return;
                }

                var randIdTask = GetRandomGameIDFromQueue(queue);
                randIdTask.Wait();
                int id = randIdTask.Result;

                var text = _drawingObjects[0] = new Button($"Your game id is: {id}")
                {
                    HorizontalAlign = 0.05f,
                    VerticalAlign = 0.05f,
                    BorderColor = Constants.BACKGROUND_COLOR
                };
                text.AlignHorizontally();
                text.AlignVertically();

                Upload(queue);

                // get the game information
                var task = Firebase.Get($"{Constants.GAMENAME}{id}/settings");
                if (!task.Wait(Constants.TIMEOUT))
                    DisplayErrorMessage("Connection timed out. Please try again later!");

                _settings = new GameSettings(task.Result, id);
            });
        }
        public LoadingPage(int width, int height, int bomb, GraphicalPage prevPage) : this()
        {
            _prevPage = prevPage;

            Task.Run(() =>
            {
                // host - only host is able to customize the game
                var queue = GetQueue();
                int id = GetCurrentGlobalGameID();
                var text = _drawingObjects[0] = new Button($"Your game id is: {id}")
                {
                    HorizontalAlign = 0.05f,
                    VerticalAlign = 0.05f,
                    BorderColor = Constants.BACKGROUND_COLOR
                };

                text.AlignHorizontally();
                text.AlignVertically();

                // get settings and also upload new settings too
                queue.Add(id);

                GameSettings settings = new GameSettings(width, height, bomb, id);
                Upload(queue, id, settings.ToJsonString());
                _settings = settings;
            });
        }

        /// <summary>
        /// Await the provided task. Set out error message if fails after Constants.TIMOUT
        /// </summary>
        private bool AwaitTask(Task task)
        {
            if (!task.Wait(Constants.TIMEOUT))
            {
                DisplayErrorMessage("Unstable internet. Please try again later!");
                return false;
            }
            return true;
        }

        /// <summary>
        /// Display error message and provide a way out for player
        /// </summary>
        private void DisplayErrorMessage(string message)
        {
            _drawingObjects[1] = new Button(0, 0, message, Constants.HEADER_FONT_SIZE)
            {
                HorizontalAlign = 0.5f,
                VerticalAlign = 0.5f,
                BorderColor = Constants.BACKGROUND_COLOR
            };

            // adding new button that tells user to go back to previous page
            _drawingObjects.Add(new Button("Back")
            {
                HorizontalAlign = 0.5f,
                VerticalAlign = 0.75f,
                Action = () => MultiplayerMinesweeper.UI.Back()
            });

            // align every objects manually...
            _drawingObjects.ForEach((button) => { button.AlignHorizontally(); button.AlignVertically(); });

            // there is an error => emergency exit for user
            PreviousPage = _prevPage;
        }

        /// <summary>
        /// Get the queue stored on the server
        /// </summary>
        /// <returns></returns>
        private List<double> GetQueue()
        {
            var task = Firebase.Get("queue");
            List<double> queue = new List<double>();
            
            if (!AwaitTask(task) || task.Result == "null") return queue;

            foreach(string _id in task.Result.Trim('[', ']').Split(','))
                if (int.TryParse(_id.Trim(), out int id) && id >= 0)
                    queue.Add(id);

            return queue;
        }

        /// <summary>
        /// Upload everything specified as parameters
        /// </summary>
        /// <param name="queue">New queue</param>
        /// <param name="id">New global game ID</param>
        /// <param name="settings">New game settings</param>
        private void Upload(List<double> queue, int id = -1, string settings = "")
        {
            string queueStr = "[";
            foreach (var _id in queue) queueStr += $"{_id},";
            queueStr = queueStr.Trim(',') + ']';

            AwaitTask(Firebase.Put(queueStr, "queue"));
            if (id >= 0)
            {
                AwaitTask(Firebase.Put((id + 1).ToString(), "game_id"));
                if (settings != "")
                    using (Json gameSettings = SplashKit.CreateJson())
                    {
                        // adding settings to the game
                        gameSettings.AddObject("settings", SplashKit.CreateJson(settings));
                        AwaitTask(Firebase.Put(gameSettings, $"{Constants.GAMENAME}{id}"));
                    }
            }
        }

        /// <summary>
        /// Get global game id for the next multiplayer game
        /// </summary>
        /// <returns>Game ID</returns>
        private int GetCurrentGlobalGameID()
        {
            var task = Firebase.Get("game_id");
            if(!task.Wait(Constants.TIMEOUT)
            || !int.TryParse(task.Result, out int id))
            {
                DisplayErrorMessage("Unstable internet. Please try again later!");
                return -1;
            }

            if(id < 0) DisplayErrorMessage("Error from database. Please try again later!");
            return id;
        }

        /// <summary>
        /// Get random game id from the provided queue
        /// </summary>
        /// <param name="queue">Queue pulled from the server</param>
        /// <returns>New game id as a guest</returns>
        private async Task<int> GetRandomGameIDFromQueue(List<double> queue)
        {
            string state;
            int id;

            do
            {
                int random = new Random().Next(0, queue.Count - 1);
                id = (int)queue[random];
                queue.RemoveAt(random);

                // prevent getting ghost ids
                state = await Firebase.Get($"{Constants.GAMENAME}{id}/state");
            } while (state == "null");

            return id;
        }

        /// <summary>
        /// Get multiplayer page but must be awaited because the main thread must not be blocked
        /// when multiplayer page is processing in this method
        /// </summary>
        /// <param name="mainMenuPage">Exit page from multiplayer page</param>
        /// <param name="role">Role of multiplayer connection</param>
        /// <returns>New multiplayer page</returns>
        public async Task<MultiplayerPage> GetMultiplayerPage(GraphicalPage mainMenuPage, MultiplayerRole role)
            => await Task.Run(() =>
            {
                while (_settings == null) { }
                var page = new MultiplayerPage(_settings) { PreviousPage = mainMenuPage };
                page.CreateConnection(role);
                return page;
            });

        public override void Draw()
        {
            // data race happens here where the collection is altered while the whole page is drawing
            // therefore, using collection type loop is inappropriate
            for (int i = 0; i < _drawingObjects.Count; i++) _drawingObjects[i].Draw();
        }
    }
}
