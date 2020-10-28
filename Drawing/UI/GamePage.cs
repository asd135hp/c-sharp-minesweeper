using SplashKitSDK;
using System;
using System.Threading;
using System.Threading.Tasks;
using MultiplayerMinesweeper.Core;
using MultiplayerMinesweeper.Core.Mode;
using MultiplayerMinesweeper.Core.StopWatch;
using MultiplayerMinesweeper.Drawing.Reusable;

namespace MultiplayerMinesweeper.Drawing.UI
{
    public class GamePage : GraphicalPage
    {
        private readonly Mode _mode;
        private readonly StopWatch _stopWatch;
        private readonly DrawingProperties _properties;

        public GamePage(Mode mode)
        {
            _mode = mode;
            _stopWatch = new StopWatch();
            _properties = new DrawingProperties(mode.Board.Width, mode.Board.Height, mode.SquareSize);
        }

        public void CleanUp() => _stopWatch.Stop();
        
        private void ToResultPage(MinesweeperBoard board, bool isWin)
        {
            CleanUp();
            Task.Run(() =>
            {
                // 1500 millisecs is the reasonable time for player to see the bombs on the screen
                Thread.Sleep(1500);
                // go to result page! because you lose/win, you know...
                MultiplayerMinesweeper.UI.ChangeToResultPage(
                    board.Bomb - board.Flag,
                    board.Bomb,
                    _stopWatch.TimeElapsed,
                    isWin
                );
            });
        }

        public override void Click(MouseButton clickedButton)
        {
            var position = SplashKit.MousePosition();
            int size = _mode.SquareSize,
                x = (int)Math.Floor((position.X - _properties.MarginLeft) / size),
                y = (int)Math.Floor((position.Y - _properties.MarginTop) / size);
            MinesweeperBoard board = _mode.Board as MinesweeperBoard;

            switch (clickedButton)
            {
                case MouseButton.LeftButton:
                    // the game should stop when player click on a bomb square or is considered win
                    int result = board.RevealSquare(x, y);
                    if (result != -2 && !_stopWatch.IsStarted && !_stopWatch.IsStopped)
                        _stopWatch.Start();
                    if (result == -1) ToResultPage(board, false);

                    // triggering whole board check for winning condition
                    bool isWin = board.IsWin;
                    if (isWin) ToResultPage(board, true);

                    break;
                case MouseButton.RightButton:
                    board.ToggleFlag(x, y);
                    break;
            }
        }

        public override void Draw(Window window)
        {
            int size = _mode.SquareSize, x = 0, y = 0;
            var p = _properties;

            // draw information bar
            InformationBar.Draw(_stopWatch.GetTime(), (_mode.Board as MinesweeperBoard).Flag, window);

            // draw each squares on the board
            foreach (string representativeChar in _mode.Board.DrawableBoard)
            {
                // draw the whole square
                Bitmap image = BitmapList.GetBitmap(representativeChar);
                // x and y must all be substracted with its respective offsets
                window.DrawBitmap(
                    image,
                    p.MarginLeft + x * size - p.SquareOffsetX,
                    p.MarginTop + y * size - p.SquareOffsetY,
                    p.SquareOptions
                );

                // advances to another row
                if(x++ == _mode.Board.Width - 1)
                {
                    x = 0;
                    y++;
                }
            }
        }
    }
}
