namespace MultiplayerMinesweeper.Core.Mode
{
    public class MediumMode : Mode
    {
        public MediumMode() : base(ModeType.Medium, 20, 15, 35) { }

        public override int SquareSize => 30;
    }
}
