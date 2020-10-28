using System;

namespace MultiplayerMinesweeper.Core.Mode
{
    public class CustomMode : Mode
    {
        public CustomMode(int width, int height, int bomb)
            : base(ModeType.Custom, width, height, bomb) { }

        public override int SquareSize
        {
            get
            {
                int maxSizeOnWidth = Constants.MAX_BOARD_WIDTH_PIXELS / Board.Width,
                    maxSizeOnHeight = Constants.MAX_BOARD_HEIGHT_PIXELS / Board.Height;
                return Math.Min(maxSizeOnHeight < maxSizeOnWidth ? maxSizeOnHeight : maxSizeOnWidth, Constants.MAX_SQUARE_SIZE);
            }
        }
    }
}
