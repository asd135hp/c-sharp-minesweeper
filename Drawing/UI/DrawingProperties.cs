using SplashKitSDK;
using MultiplayerMinesweeper.Core;
using MultiplayerMinesweeper.Drawing.Reusable;

namespace MultiplayerMinesweeper.Drawing.UI
{
    internal class DrawingProperties
    {
        private static Bitmap _bitmap = BitmapList.GetBitmap("0");
        private bool _mlOff = false, _mtOff = true;

        public DrawingProperties(
            int width, int height, int squareSize,
            int windowWidth = Constants.WINDOW_WIDTH,
            int windowHeight = Constants.WINDOW_HEIGHT)
        {
            int widthPixels = width * squareSize,
                heightPixels = height * squareSize,
                barHeight = Constants.INFORMATION_BAR_HEIGHT;

            SquareSize = squareSize;
            MarginLeft = (windowWidth - widthPixels) / 2;
            MarginTop = (windowHeight - heightPixels - barHeight) / 2 + barHeight;
            WindowWidth = windowWidth;
            WindowHeight = windowHeight;

            // specify square options and also square offset dimensions
            SquareOptions = new DrawingOptions()
            {
                ScaleX = (float)squareSize / _bitmap.Width,
                ScaleY = (float)squareSize / _bitmap.Height
            };

            // offset is made from how much the bitmap has shifted from its original position both on x and y axis.
            // This offset has been tested as the scaling point of all bitmaps are at the center of that bitmap!
            // (bitmap scaled horizontally 0.75 means that the square has shifted totally 0.25/2 = 0.125 to the left)
            SquareOffsetX = _bitmap.Width * (1 - SquareOptions.ScaleX) / 2;
            SquareOffsetY = _bitmap.Height * (1 - SquareOptions.ScaleY) / 2;
        }

        public int MarginLeftOffset
        {
            set
            {
                if (!_mlOff)
                {
                    MarginLeft += value;
                    _mlOff = true;
                }
            }
        }
        public int MarginTopOffset
        {
            set
            {
                if (!_mtOff)
                {
                    MarginTop += value;
                    _mtOff = true;
                }
            }
        }
        public bool IsRotated
        {
            set
            {
                if (value)
                {
                    SquareOffsetX = _bitmap.Height * (1 - SquareOptions.ScaleY) / 2;
                    SquareOffsetY = _bitmap.Width * (1 - SquareOptions.ScaleX) / 2;
                }
            }
        }

        public int MarginLeft { get; private set; }
        public int MarginTop { get; private set; }
        public double SquareOffsetX { get; private set; }
        public double SquareOffsetY { get; private set; }
        public readonly int SquareSize;
        public readonly int WindowWidth, WindowHeight;
        public readonly DrawingOptions SquareOptions;
    }
}
