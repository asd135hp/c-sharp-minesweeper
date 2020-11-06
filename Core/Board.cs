using System;
using System.Collections.Generic;
using MultiplayerMinesweeper.Core.BoardComponent;

namespace MultiplayerMinesweeper.Core
{
    public abstract class Board
    {
        protected List<List<Square>> _board;
        protected int _width, _height;

        public Board(int width, int height)
        {
            // initialize main board
            var drawableBoard = new List<string>();
            _board = new List<List<Square>>();
            for(int y = 0; y < height; y++)
            {
                var temp = new List<Square>();
                for(int x = 0; x < width; x++)
                {
                    // we will decide on square's X and Y pos later
                    var newSquare = new CoveredSquare(0);
                    temp.Add(newSquare);
                    drawableBoard.Add(newSquare.AsChar);
                }
                _board.Add(temp);
            }
            DrawableBoard = drawableBoard.ToArray();

            _width = width;
            _height = height;
        }

        public int Width => _width;
        public int Height => _height;
        public readonly string[] DrawableBoard;

        /// <summary>
        /// Merge a stringified board to the current board object.
        /// Throws an exception when the board is wrong in number of squares
        /// </summary>
        /// <param name="board"></param>
        public void MergeBoard(string[] board)
        {
            if (board.Length == Width * Height)
            {
                for(int x = 0; x < Width; x++)
                {
                    for(int y = 0; y < Height; y++)
                    {
                        int pos = y * Width + x;
                        if(DrawableBoard[pos] != board[pos])
                        {
                            // create new square to replace an old square in the board
                            // most likely from covered -> flag/uncovered
                            string newRepresentation = board[pos];
                            _board[y][x] = Square.FromChar(newRepresentation, _board[y][x]);
                            DrawableBoard[pos] = newRepresentation;
                        }
                    }
                }
            }
            else throw new ArgumentException("Inconsistent width, height and possibly simplified board specified!");
        }
    }
}
