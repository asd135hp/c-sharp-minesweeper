using SplashKitSDK;
using MultiplayerMinesweeper.Core;

namespace MultiplayerMinesweeper.Drawing.Reusable
{
    static class InformationBar
    {
        // bitmaps (preload so that the game does not have heavy impact on performace
        private const string CLOCK = "clock", FLAG = "flag";

        private static DrawingOptions CLOCK_ICON_OPTIONS = new DrawingOptions()
        {
            ScaleX = 35f / BitmapList.GetBitmap(CLOCK).Width,
            ScaleY = 35f / BitmapList.GetBitmap(CLOCK).Height
        };
        private static DrawingOptions FLAG_ICON_OPTIONS = new DrawingOptions()
        {
            ScaleX = 35f / BitmapList.GetBitmap(FLAG).Width,
            ScaleY = 35f / BitmapList.GetBitmap(FLAG).Height
        };
        private static DrawingOptions SMALL_CLOCK_ICON_OPTIONS = new DrawingOptions()
        {
            ScaleX = 25f / BitmapList.GetBitmap(CLOCK).Width,
            ScaleY = 25f / BitmapList.GetBitmap(CLOCK).Height
        };
        private static DrawingOptions SMALL_FLAG_ICON_OPTIONS = new DrawingOptions()
        {
            ScaleX = 25f / BitmapList.GetBitmap(FLAG).Width,
            ScaleY = 25f / BitmapList.GetBitmap(FLAG).Height
        };

        public static void Draw(string time, int flagNumber, Window window,
            int screenWidth = Constants.WINDOW_WIDTH, bool smaller = false, int marginLeft = -1)
        {

            // pre-calculated values for information bar
            double quartX = screenWidth * 0.25,
                centerX = screenWidth * 0.5,
                centerY = Constants.INFORMATION_BAR_HEIGHT * 0.5;

            Bitmap clockIcon = BitmapList.GetBitmap(CLOCK),
                flagIcon = BitmapList.GetBitmap(FLAG);

            // votile texts - a little ineffient
            // may be pushing these into drawing objects...
            Component.Text timer = new Component.Text(time),
                flags = new Component.Text(flagNumber.ToString());

            Size flagsTextSize = flags.GetTextSize();

            if (smaller)
            {
                marginLeft -= 15;
                // for smaller sized window
                timer.X = (float)(centerX + quartX / 2 + marginLeft);
                timer.Y = (float)(centerY / 4);
                flags.X = (float)(centerX + quartX / 2 + marginLeft);
                flags.Y = (float)(centerY * 0.9);

                DrawingOptions cOpts = SMALL_CLOCK_ICON_OPTIONS,
                    fOpts = SMALL_FLAG_ICON_OPTIONS;

                // draw information bar
                // draw clock icon and its timer value
                window.DrawBitmap(
                    clockIcon,
                    centerX - quartX / 2 + marginLeft - clockIcon.Width * (1 - cOpts.ScaleX) / 2,
                    centerY / 4 - clockIcon.Height * (1 - cOpts.ScaleY) / 2,
                    cOpts
                );
                timer.Draw();

                // draw flag icon and flags value from MinesweeperBoard
                window.DrawBitmap(
                    flagIcon,
                    centerX - quartX / 2 + marginLeft - flagIcon.Width * (1 - fOpts.ScaleX) / 2,
                    centerY * 0.9 - flagIcon.Width * (1 - fOpts.ScaleY) / 2,
                    fOpts
                );
                flags.Draw();
                return;
            }

            timer.X = (float)(centerX - quartX + clockIcon.Width + 10);
            timer.Y = (float)(centerY - timer.GetTextSize().Height / 2);
            flags.X = (float)(centerX + quartX);
            flags.Y = (float)(centerY - flagsTextSize.Height / 2);

            // draw information bar
            // draw clock icon and its timer value
            window.DrawBitmap(
                clockIcon,
                centerX - quartX,
                centerY - clockIcon.Height / 2,
                CLOCK_ICON_OPTIONS
            );
            timer.Draw();

            // draw flag icon and flags value from MinesweeperBoard
            window.DrawBitmap(
                flagIcon,
                centerX + quartX - 110,
                centerY - flagIcon.Height / 2,
                FLAG_ICON_OPTIONS
            );
            flags.Draw();
        }
    }
}
