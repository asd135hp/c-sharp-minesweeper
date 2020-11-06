using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using MultiplayerMinesweeper.Core;
using MultiplayerMinesweeper.Core.Mode;
using MultiplayerMinesweeper.Core.Multiplayer;
using MultiplayerMinesweeper.Drawing.UI;
using MultiplayerMinesweeper.Drawing.Component;

namespace MultiplayerMinesweeper
{
    public static class UI
    {
        private static GraphicalPage _currentPage;
        private static readonly Dictionary<string, UIPage> Pages = new Dictionary<string, UIPage>();

        /// <summary>
        /// Gets currently drawn page
        /// </summary>
        public static GraphicalPage CurrentPage
        {
            get
            {
                if (!Pages.ContainsKey("base")) Initialize();
                if (_currentPage is UIPage) return _currentPage as UIPage;
                if (_currentPage is GamePage) return _currentPage as GamePage;
                if (_currentPage is MultiplayerPage) return _currentPage as MultiplayerPage;
                return _currentPage;
            }
            private set => _currentPage = value;
        }

        /// <summary>
        /// If this method is called, the page will be the previous page provided of current page
        /// </summary>
        public static void Back()
        {
            if (_currentPage is GamePage) (_currentPage as GamePage).CleanUp();
            if (_currentPage is MultiplayerPage) (_currentPage as MultiplayerPage).CloseConnection();

            _currentPage = _currentPage?.PreviousPage as UIPage ?? _currentPage;
        }

        /// <summary>
        /// General pages initializer
        /// </summary>
        private static void Initialize()
        {
            _currentPage = Pages["base"] = Welcome();
            Pages["pre-custom"] = CustomizeGamePage((width, height, bomb) =>
            {
                CurrentPage = new GamePage(new CustomMode(width, height, bomb))
                {
                    PreviousPage = CurrentPage
                };
            });
            Pages["pre-multiplayer"] = PreMultiplayerPage();
        }

        /// <summary>
        /// Welcome page initializer
        /// </summary>
        /// <returns></returns>
        private static UIPage Welcome()
        {
            UIPage page = new UIPage();
            int width = 350;

            page.PushUIRectangle(new UIRectangle[]{
                new Button(0, 0, "Welcome to Minesweeper", Constants.HEADER_FONT_SIZE)
                {
                    HorizontalAlign = 0.5f,
                    VerticalAlign = 0.1f,
                    BorderColor = Constants.BACKGROUND_COLOR
                },
                new Button("Easy", width, 0)
                {
                    HorizontalAlign = 0.5f,
                    VerticalAlign = 0.3f,
                    Action = () => CurrentPage = new GamePage(new EasyMode())
                    {
                        PreviousPage = CurrentPage
                    }
                },
                new Button("Medium", width, 0)
                {
                    HorizontalAlign = 0.5f,
                    VerticalAlign = 0.45f,
                    Action = () => CurrentPage = new GamePage(new MediumMode())
                    {
                        PreviousPage = CurrentPage
                    }
                },
                new Button("Hard", width, 0)
                {
                    HorizontalAlign = 0.5f,
                    VerticalAlign = 0.6f,
                    Action = () => CurrentPage = new GamePage(new HardMode())
                    {
                        PreviousPage = CurrentPage
                    }
                },
                new Button("Custom", width, 0)
                {
                    HorizontalAlign = 0.5f,
                    VerticalAlign = 0.75f,
                    Action = () => CurrentPage = Pages["pre-custom"]
                },
                new Button("Multiplayer", width, 0)
                {
                    HorizontalAlign = 0.5f,
                    VerticalAlign = 0.9f,
                    Action = () => CurrentPage = Pages["pre-multiplayer"]
                }
            });

            return page;
        }

        /// <summary>
        /// Pre of any custom game mode (like custom or multiplayer)
        /// But now it is only for custom game mode
        /// because I can not prove the genuinity of Multiplayer mode
        /// SplashKit does not allow me to have 2 windows be drawn at the same time
        /// -> Multiplayer mode is abandoned
        /// </summary>
        /// <param name="actionToNewPage"></param>
        /// <param name="prevPage"></param>
        /// <returns></returns>
        private static UIPage CustomizeGamePage(Action<int, int, int> actionToNewPage, GraphicalPage prevPage = null)
        {
            UIPage page = new UIPage()
            {
                PreviousPage = prevPage ?? Pages["base"]
            };

            var objects = new List<UIRectangle>();

            objects.AddRange(new UIRectangle[]{
                new Button(0, 0, "Customize your game", Constants.HEADER_FONT_SIZE)
                {
                    HorizontalAlign = 0.5f,
                    VerticalAlign = 0.05f,
                    BorderColor = Constants.BACKGROUND_COLOR
                },
                new Button(0, 0, "Width", 25)
                {
                    HorizontalAlign = 0.2f,
                    VerticalAlign = 0.29f,
                    BorderColor = Constants.BACKGROUND_COLOR
                },
                new Range(2, Constants.MAX_BOARD_WIDTH, 10, 20)
                {
                    HorizontalAlign = 0.65f,
                    VerticalAlign = 0.3f
                },
                new Button(0, 0, "Height", 25)
                {
                    HorizontalAlign = 0.2f,
                    VerticalAlign = 0.44f,
                    BorderColor = Constants.BACKGROUND_COLOR
                },
                new Range(2, Constants.MAX_BOARD_HEIGHT, 10, 20)
                {
                    HorizontalAlign = 0.65f,
                    VerticalAlign = 0.45f
                },
                new Button(0, 0, "Bomb", 25)
                {
                    HorizontalAlign = 0.2f,
                    VerticalAlign = 0.59f,
                    BorderColor = Constants.BACKGROUND_COLOR
                },
                new Range(1, Constants.MAX_BOARD_BOMB, 10, 20)
                {
                    HorizontalAlign = 0.65f,
                    VerticalAlign = 0.6f
                },
                new Button("Back", 300, 0)
                {
                    HorizontalAlign = 0.25f,
                    VerticalAlign = 0.85f,
                    Action = Back
                },
                new Button("Confirm", 300, 0)
                {
                    HorizontalAlign = 0.75f,
                    VerticalAlign = 0.85f,
                    Action = () => {
                        int width = (objects[2] as Range).CurrentValue,
                            height = (objects[4] as Range).CurrentValue,
                            bomb = (objects[6] as Range).CurrentValue;

                        Button lastButton = objects[objects.Count - 1] as Button;
                        int maxSquare = Constants.MAX_BOARD_WIDTH * Constants.MAX_BOARD_HEIGHT,
                            localMaxSquare = width * height,
                            localMaxBombs = maxSquare - 10 * localMaxSquare / maxSquare;
                        if(lastButton.BorderThickness != 0 && bomb > localMaxBombs)
                        {
                            var text = new Button("Selected bombs are higher than maximum possible bomb now!")
                            {
                                HorizontalAlign = 0.5f,
                                VerticalAlign = 0.75f,
                                BorderThickness = 0,
                                BorderColor = Constants.BACKGROUND_COLOR
                            };
                            objects.Add(text);
                        }

                        actionToNewPage.Invoke(width, height, bomb);
                    }
                }
            });
            page.PushUIRectangle(objects);

            return page;
        }

        /// <summary>
        /// Pre of multiplayer page initializer
        /// </summary>
        private static UIPage PreMultiplayerPage()
        {
            UIPage page = new UIPage() { PreviousPage = Pages["base"] };

            page.PushUIRectangle(new UIRectangle[]
            {
                new Button(0, 0, "Welcome to multiplayer lobby creation", Constants.HEADER_FONT_SIZE)
                {
                    HorizontalAlign = 0.5f,
                    VerticalAlign = 0.1f,
                    BorderColor = Constants.BACKGROUND_COLOR
                },
                new Button("Create new game as host", 450, 0)
                {
                    HorizontalAlign = 0.5f,
                    VerticalAlign = 0.35f,
                    Action = () =>
                    {
                        // customize game first
                        CurrentPage = CustomizeGamePage((width, height, bomb) =>
                            Task.Run(() =>
                            {
                                // then to loading and to multiplayer page
                                CurrentPage = new LoadingPage(width, height, bomb, CurrentPage);
                                using(var task = (CurrentPage as LoadingPage).GetMultiplayerPage(
                                    Pages["base"],
                                    MultiplayerRole.Host
                                ))
                                {
                                    task.Wait();
                                    CurrentPage = task.Result;
                                }
                            }), CurrentPage);
                    }
                },
                new Button("Join a random game as guest", 450, 0)
                {
                    HorizontalAlign = 0.5f,
                    VerticalAlign = 0.55f,
                    Action = () => Task.Run(() =>
                    {
                        // landing on loading page first and then multiplayer page
                        CurrentPage = new LoadingPage(CurrentPage);
                        using(var task = (CurrentPage as LoadingPage).GetMultiplayerPage(
                            Pages["base"],
                            MultiplayerRole.Guest
                        ))
                        {
                            task.Wait();
                            CurrentPage = task.Result;
                        }
                    })
                },
                new Button("Back to main menu", 450, 0)
                {
                    HorizontalAlign = 0.5f,
                    VerticalAlign = 0.75f,
                    Action = Back
                }
            });

            return page;
        }

        /// <summary>
        /// Change to result page with used flags number, total flags number, time played and is win or not
        /// to get the accordingly result
        /// </summary>
        public static void ChangeToResultPage(int usedFlags, int totalFlags, int timePlayed, bool isWin)
            => CurrentPage = new ResultPage(usedFlags, totalFlags, timePlayed, isWin)
            {
                PreviousPage = Pages["base"]
            };
    }
}
