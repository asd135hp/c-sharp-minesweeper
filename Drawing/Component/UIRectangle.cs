using SplashKitSDK;
using MultiplayerMinesweeper.Core;

namespace MultiplayerMinesweeper.Drawing.Component
{
    public abstract class UIRectangle : DrawableObject
    {
        public UIRectangle() : base(0, 0)
        {
            BorderColor = Constants.TEXT_COLOR;
            BackgroundColor = Constants.BACKGROUND_COLOR;
        }
        public UIRectangle(int width, int height) : this()
        {
            Width = width;
            Height = height;
        }

        public int Width { get; protected set; }
        public int Height { get; protected set; }
        public float VerticalAlign, HorizontalAlign;
        public Color BorderColor, BackgroundColor;

        /// <summary>
        /// Outset border (in constrast to inset border - CSS definition)
        /// </summary>
        public int BorderThickness = 1;

        public virtual void AlignHorizontally(int windowWidth = Constants.WINDOW_WIDTH)
        {
            if (HorizontalAlign != 0) X = (windowWidth - Width) * HorizontalAlign;
        }

        public virtual void AlignVertically(int windowHeight = Constants.WINDOW_HEIGHT)
        {
            if (VerticalAlign != 0) Y = (windowHeight - Height) * VerticalAlign;
        }

        public override void Draw() => Draw(SplashKit.CurrentWindow());
        public override void Draw(Window window)
        {
            // draw outline
            SplashKit.DrawRectangleOnWindow(window, BorderColor, X, Y, Width, Height, new DrawingOptions()
            {
                LineWidth = BorderThickness
            });
        }
    }
}
