namespace MultiplayerMinesweeper.Core.BoardComponent
{
    class CoveredSquare : Square
    {
        public CoveredSquare(int value) : base(value)
        {
            _type = value == -1 ? SquareType.Bomb : SquareType.Normal;
        }

        public override string AsChar => "c";
        public override bool Covered => true;
    }
}
