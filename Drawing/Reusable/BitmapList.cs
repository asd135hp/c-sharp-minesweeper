using SplashKitSDK;
using System.Collections.Generic;

namespace MultiplayerMinesweeper.Drawing.Reusable
{
    class BitmapList
    {
        private static bool _isLoaded = false;
        private static void LoadBitmaps()
        {
            if (!_isLoaded)
            {
                SplashKit.LoadBitmap("b", "bomb.png");
                SplashKit.LoadBitmap("c", "cover.png");
                SplashKit.LoadBitmap("f", "flag.png");
                SplashKit.LoadBitmap("0", "0.png");
                SplashKit.LoadBitmap("1", "1.png");
                SplashKit.LoadBitmap("2", "2.png");
                SplashKit.LoadBitmap("3", "3.png");
                SplashKit.LoadBitmap("4", "4.png");
                SplashKit.LoadBitmap("5", "5.png");
                SplashKit.LoadBitmap("6", "6.png");
                SplashKit.LoadBitmap("7", "7.png");
                SplashKit.LoadBitmap("8", "8.png");
                SplashKit.LoadBitmap("clock", "display_clock.png");
                SplashKit.LoadBitmap("flag", "display_flag.png");
                _isLoaded = true;
            }
        }

        public static Bitmap GetBitmap(string name)
        {
            LoadBitmaps();
            return SplashKit.BitmapNamed(name);
        }
    }
}
