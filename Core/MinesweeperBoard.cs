using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using MultiplayerMinesweeper.Core.BoardComponent;
using MultiplayerMinesweeper.Core.Multiplayer;

namespace MultiplayerMinesweeper.Core
{
    public class MinesweeperBoard : Board
    {
        public MinesweeperBoard() : base(2, 2)
        {
            Bomb = Flag = 1;
        }
        public MinesweeperBoard(int width, int height, int bomb) : base(width, height)
        {
            Bomb = Flag = bomb;
        }

        public int Bomb { get; private set; }
        public int Flag { get; private set; }

        /// <summary>
        /// Condition: if the board shows that the number of covered square and flagged squares
        /// is the same as the total flags, it will be a win. Else, a lose instead
        /// </summary>
        public bool IsWin
        {
            get
            {
                int count = 0;
                foreach (var repChar in DrawableBoard)
                    if (repChar == "c" || repChar == "f") count++;

                return count == Bomb;
            }
        }

        /// <summary>
        /// For multiplayer only
        /// Reinitialize the board from a provided multiplayer settings
        /// </summary>
        /// <param name="settings">Multiplayer game settings. Specified by the host</param>
        public void FromSettings(GameSettings settings)
        {
            _width = settings.BoardWidth;
            _height = settings.BoardHeight;
            Bomb = Flag = settings.Bomb;
        }

        public void PopulateBomb()
        {
            byte[] randomBytes = new byte[2];
            var generator = RandomNumberGenerator.Create();

            // read 2 numbers at a time
            for(int i = 0; i < Bomb; i++)
            {
                // get new random bytes
                generator.GetBytes(randomBytes);

                // get new x and y position
                int x = (int)Math.Floor(randomBytes[0] * _width / 256.0),
                    y = (int)Math.Floor(randomBytes[1] * _height / 256.0);
                Square oldSquare = _board[y][x];

                if(oldSquare.Value == -1)
                {
                    // try again
                    i--;
                    continue;
                }

                // set bomb
                _board[y][x] = new CoveredSquare(-1);

                // set surrounding squares 1 value higher than it used to be
                // a bit inefficient because CoveredSquare is called many times
                CheckSurroundingSquares(x, y, (newX, newY) =>
                {
                    if (newX >= 0 && newY >= 0
                    && newX < _width && newY < _height
                    && _board[newY][newX].Value != -1)
                        _board[newY][newX] = new CoveredSquare(_board[newY][newX].Value + 1);
                });
            }
        }

        /// <summary>
        /// Checking all of surrounding squares (8 squares around it)
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="actionOnEachSquare"></param>
        private void CheckSurroundingSquares(int x, int y, Action<int, int> actionOnEachSquare)
        {
            for (int newX = x - 1; newX <= x + 1; newX++)
                for (int newY = y - 1; newY <= y + 1; newY++)
                    actionOnEachSquare.Invoke(newX, newY);
        }

        private void RevealBombs()
        {
            List<string> tempBoard = new List<string>();
            foreach(var list in _board)
            {
                foreach(Square s in list)
                {
                    tempBoard.Add(s.Value == -1 ? "b" : s.AsChar);
                }
            }

            MergeBoard(tempBoard.ToArray());
        }

        /// <summary>
        /// By using this method, it is guaranteed that mouse position calculation will not exceed the
        /// board's capacity.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns>-2 when player click on the unallowed squares. Else, it will range from -1 to 8 inclusive</returns>
        public int RevealSquare(int x, int y, bool isRevealing = false)
        {
            Square selectedSquare;

            // prevent out of bounds
            if (x >= 0 && y >= 0 && x < _width && y < _height)
                selectedSquare = _board[y][x];
            else return -3;

            if (selectedSquare.Covered && selectedSquare.Type != SquareType.Flag)
            {
                if (selectedSquare.IsTrivialType())
                {
                    var temp = _board[y][x] = new UncoveredSquare(selectedSquare);
                    DrawableBoard[y * _width + x] = temp.AsChar;

                    // check all of 8 squares
                    if (selectedSquare.Value == 0)
                        CheckSurroundingSquares(x, y, (newX, newY) => RevealSquare(newX, newY, true));

                    return selectedSquare.Value;
                }

                if (!isRevealing)
                {
                    RevealBombs();
                    return -1;
                }
            }

            return -2;
        }

        /// <summary>
        /// Combined method from SetFlag and RemoveFlag.
        /// The method will do something if the selected square is covered
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void ToggleFlag(int x, int y)
        {
            Square selectedSquare = _board[y][x];
            if (selectedSquare.Covered)
            {
                int flattenedPosition = y * _width + x;
                bool isFlag = selectedSquare.Type == SquareType.Flag;
                Square newSquare = isFlag ?
                    new CoveredSquare(selectedSquare.Value) :
                    (Square)new FlaggedSquare(selectedSquare);
                Flag += isFlag ? 1 : -1;

                _board[y][x] = newSquare;
                DrawableBoard[flattenedPosition] = newSquare.AsChar;
            }
        }
    }
}
