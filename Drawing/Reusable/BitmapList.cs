using SplashKitSDK;
using System.Collections.Generic;

namespace MultiplayerMinesweeper.Drawing.Reusable
{
    class BitmapList
    {
        private static Dictionary<string, Bitmap> _bitmaps = new Dictionary<string, Bitmap>()
        {
            { "b", SplashKit.LoadBitmap("bomb", "bomb.png") },
            { "c", SplashKit.LoadBitmap("cover", "cover.png") },
            { "f", SplashKit.LoadBitmap("flag", "flag.png") },
            { "0", SplashKit.LoadBitmap("0", "0.png") },
            { "1", SplashKit.LoadBitmap("1", "1.png") },
            { "2", SplashKit.LoadBitmap("2", "2.png") },
            { "3", SplashKit.LoadBitmap("3", "3.png") },
            { "4", SplashKit.LoadBitmap("4", "4.png") },
            { "5", SplashKit.LoadBitmap("5", "5.png") },
            { "6", SplashKit.LoadBitmap("6", "6.png") },
            { "7", SplashKit.LoadBitmap("7", "7.png") },
            { "8", SplashKit.LoadBitmap("8", "8.png") },
            { "clock", SplashKit.LoadBitmap("clockIcon", "display_clock.png") },
            { "flag", SplashKit.LoadBitmap("flagIcon", "display_flag.png") }
        };
        public static Bitmap GetBitmap(string index) => _bitmaps[index];
    }
}
