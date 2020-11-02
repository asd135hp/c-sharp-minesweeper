using SplashKitSDK;
using System;
using MultiplayerMinesweeper.Core;
using MultiplayerMinesweeper.Core.Multiplayer;
using MultiplayerMinesweeper.Drawing.Reusable;
using MultiplayerMinesweeper.Core.Mode;

namespace MultiplayerMinesweeper.Drawing.UI
{
    public class MultiplayerPage : GraphicalPage
    {
        // drawing part of multiplayer page
        private DrawingProperties _playerProps, _opponentProps;

        // the core of multiplayer page
        private readonly GameSettings _settings;
        private MultiplayerRole _role;
        private MultiplayerConnection _connection;

        public MultiplayerPage(GameSettings settings)
        {
            _settings = settings;

            var mode = new CustomMode(settings.BoardWidth, settings.BoardHeight, settings.Bomb);
            int playerWindowWidth = (int)(Constants.WINDOW_WIDTH * Constants.SCREENS_RATIO);

            _playerProps = new DrawingProperties(
                settings.BoardWidth, settings.BoardHeight, mode.SquareSize, playerWindowWidth
            );

            _opponentProps = new DrawingProperties(
                settings.BoardHeight, settings.BoardHeight, Constants.OPPONENT_SQUARE_SIZE,
                (int)(Constants.WINDOW_WIDTH * (1 - Constants.SCREENS_RATIO)))
            {
                MarginLeftOffset = playerWindowWidth,
                IsRotated = true
            };
        }

        public bool CreateConnection(MultiplayerRole role)
        {
            _role = role;
            _connection = new MultiplayerConnection(role, _settings);
            _connection.Establish();
            return _connection.IsConnectionEstablished;
        }

        public void CloseConnection()
        {
            if (!_connection.IsConnectionClosed) _connection.Close();
        }

        public override void Click(MouseButton clickedButton)
        {
            if (_connection.CurrentData.State == GameState.Playing)
            {
                var position = SplashKit.MousePosition();
                var p = _playerProps;
                int squareValue = 0,
                    size = p.SquareSize,
                    x = (int)Math.Floor((position.X - p.MarginLeft) / size),
                    y = (int)Math.Floor((position.Y - p.MarginTop) / size);

                MinesweeperBoard board = (_role == MultiplayerRole.Host ?
                    _connection.CurrentData.Host :
                    _connection.CurrentData.Guest).Board as MinesweeperBoard;

                switch (clickedButton)
                {
                    case MouseButton.LeftButton:
                        squareValue = board.RevealSquare(x, y);
                        break;
                    case MouseButton.RightButton:
                        board.ToggleFlag(x, y);
                        break;
                }

                // if player is winning now (except for 1 edge case on my mind now),
                // change state of the game
                // then closes the connection
                bool isLose = squareValue == -1;
                if (board.IsWin || isLose)
                {
                    _connection.CurrentData.ChangeGameState(
                        _role == MultiplayerRole.Host ?
                            (isLose ? GameState.GuestWin : GameState.HostWin) :
                            (isLose ? GameState.HostWin : GameState.GuestWin)
                    );
                    CloseConnection();

                    // moves the player to result page
                    MultiplayerMinesweeper.UI.ChangeToResultPage(
                        board.Flag,
                        board.Bomb,
                        _connection.TimePlayed,
                        board.IsWin
                    );
                }
            }
        }

        private void DrawBoard(string[] board, bool rotate90Deg = false)
        {
            int x = 0, y = 0;
            var props = rotate90Deg ? _opponentProps : _playerProps;

            // draw each squares on the board
            foreach (string representativeChar in board)
            {
                // draw the whole square
                Bitmap image = BitmapList.GetBitmap(representativeChar);
                // x and y must all be substracted with its respective offsets
                SplashKit.DrawBitmapOnWindow(
                    SplashKit.CurrentWindow(),
                    image,
                    props.MarginLeft + x * props.SquareSize - props.SquareOffsetX,
                    props.MarginTop + y * props.SquareSize - props.SquareOffsetY,
                    props.SquareOptions
                );

                // advances to another row
                // player
                if(!rotate90Deg && x++ == _settings.BoardWidth - 1)
                {
                    x = 0;
                    y++;
                    continue;
                }

                // opponent
                if (rotate90Deg && y++ == _settings.BoardHeight - 1)
                {
                    y = 0;
                    x++;
                }
            }
        }

        public override void Draw()
        {
            // may be the luck with drawing on two windows parallel-ly ran out...
            bool isHost = _role == MultiplayerRole.Host;
            MultiplayerData data = _connection.CurrentData;
            PlayerData host = isHost ? data.Host : data.Guest,
                guest = isHost ? data.Guest : data.Host;

            // draw player side
            InformationBar.Draw(
                TimeFormatter.GetTime(_connection.TimePlayed),
                (host.Board as MinesweeperBoard).Flag,
                _playerProps.WindowWidth
            );
            DrawBoard(host.Board.DrawableBoard);

            // a line in between screens
            int wW = _playerProps.WindowWidth, wH = _playerProps.WindowHeight;
            SplashKit.DrawLine(Constants.TEXT_COLOR, wW, 0, wW, wH);

            // draw opponent side
            InformationBar.Draw(
                TimeFormatter.GetTime(guest.Time),
                guest.Flag,
                _opponentProps.WindowWidth, true,
                _playerProps.WindowWidth
            );
            DrawBoard(guest.Board.DrawableBoard, true);
        }
    }
}
