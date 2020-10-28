namespace MultiplayerMinesweeper.Core.Mode
{
    public class EasyMode : Mode
    {
        public EasyMode() : base(ModeType.Easy, 10, 10, 10) { }

        public override int SquareSize => 45;
    }
}