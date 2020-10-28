using System;

namespace MultiplayerMinesweeper.Core.BoardComponent
{
    class FlaggedSquare : Square
    {
        public FlaggedSquare(Square square) : base(square.Value)
        {
            if (!square.Covered)
                throw new ArgumentException("Could not flag an UncoveredSquare");
            _type = SquareType.Flag;
        }

        public override string AsChar => "f";
        public override bool Covered => true;
    }
}
