using System;

namespace MultiplayerMinesweeper.Core.BoardComponent
{
    class UncoveredSquare : Square
    {
        public UncoveredSquare(Square square) : base(square.Value)
        {
            if (!square.Covered)
                throw new ArgumentException("Could not convert any other kind of squares than CoveredSquare to UncoveredSquare");
            _type = SquareType.Normal;
        }

        public override string AsChar => Value == -1 ? "b" : Value.ToString();
        public override bool Covered => false;
    }
}
