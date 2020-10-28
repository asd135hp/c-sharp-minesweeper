namespace MultiplayerMinesweeper.Core.Mode
{
    public enum ModeType
    {
        Easy,
        Medium,
        Hard,
        Custom
    }

    public abstract class Mode
    {
        public Mode(ModeType type, int width, int height, int bomb)
        {
            var board = new MinesweeperBoard(width, height, bomb);
            board.PopulateBomb();

            Type = type;
            Board = board;
        }

        public readonly ModeType Type;
        public readonly Board Board;
        public abstract int SquareSize { get; }
    }
}
