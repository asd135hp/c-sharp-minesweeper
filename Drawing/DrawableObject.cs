using SplashKitSDK;

namespace MultiplayerMinesweeper.Drawing
{
    public abstract class DrawableObject
    {
        public DrawableObject()
        {
            X = 0;
            Y = 0;
            Color = Color.White;
        }
        public DrawableObject(Color color) : this()
        {
            Color = color;
        }
        public DrawableObject(float x, float y) : this()
        {
            X = x;
            Y = y;
        }
        public DrawableObject(float x, float y, Color color) : this(x, y)
        {
            Color = color;
        }

        public float X, Y;
        public readonly Color Color;

        public abstract void Draw();
    }
}
