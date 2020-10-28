namespace MultiplayerMinesweeper.Core.Mode
{
    public class HardMode : Mode
    {
        public HardMode() : base(ModeType.Hard, 30, 17, 100) { }

        public override int SquareSize => 25;
    }
}
