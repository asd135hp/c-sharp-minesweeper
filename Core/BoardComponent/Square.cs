using System;

namespace MultiplayerMinesweeper.Core.BoardComponent
{
    public enum SquareType
    {
        Bomb,
        Flag,
        Normal
    }

    public abstract class Square
    {
        protected SquareType _type;

        public Square()
        {
            _type = SquareType.Normal;
            Value = 0;
        }
        public Square(int value) : this()
        {
            Value = value;
        }

        public SquareType Type => _type;
        public readonly int Value;
        public abstract bool Covered { get; }
        public abstract string AsChar { get; }

        /// <summary>
        /// There must be a way to change square's state indirectly. So this method exists.
        /// If the representative character is supported, a new square will be returned
        /// </summary>
        /// <param name="representation">A string representation of the square</param>
        /// <returns>A new square based on provided string representation</returns>
        public static Square FromChar(string representation, Square previousSquare)
        {
            switch (representation)
            {
                case "c":
                    return new CoveredSquare(previousSquare.Value);
                case "b":
                    return new UncoveredSquare(-1);
                case "f":
                    return new FlaggedSquare(previousSquare);
                default:
                    if(!int.TryParse(representation, out int value))
                        throw new ArgumentException("Unsupported character representation!");

                    return new UncoveredSquare(value);
            }
        }

        /// <summary>
        /// Checking if this object is a trivial type (not a bomb nor a flag)
        /// </summary>
        public bool IsTrivialType() => Value != -1 && _type == SquareType.Normal;
    }
}
