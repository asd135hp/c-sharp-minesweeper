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
        /// If the representative character is supported, the square will change its state
        /// Could be unfit for this class but we will see about that
        /// </summary>
        /// <param name="representation"></param>
        public static Square FromChar(string representation, Square previousSquare)
        {
            switch (representation)
            {
                case "c":
                    return new CoveredSquare(previousSquare.Value);
                case "u":
                case "b":
                    return new UncoveredSquare(previousSquare);
                case "f":
                    return new FlaggedSquare(previousSquare);
                default:
                    throw new ArgumentException("Unsupported character representation!");
            }
        }

        public bool IsTrivialType() => Value != -1 && _type == SquareType.Normal;
    }
}
