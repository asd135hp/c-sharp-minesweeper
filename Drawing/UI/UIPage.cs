using SplashKitSDK;
using System.Collections.Generic;
using MultiplayerMinesweeper.Drawing.Component;

namespace MultiplayerMinesweeper.Drawing.UI
{
    public class UIPage : GraphicalPage
    {
        public UIPage() : base() { }

        public void PushUIRectangle(UIRectangle rectangle)
        {
            rectangle.AlignVertically();
            rectangle.AlignHorizontally();
            _drawingObjects.Add(rectangle);
        }
        public void PushUIRectangle(IEnumerable<UIRectangle> rectangles)
        {
            foreach (var rectangle in rectangles) PushUIRectangle(rectangle);
        }

        public override void Draw(Window window)
        {
            foreach(var obj in _drawingObjects)
            {
                if (obj is Button) { (obj as Button).Draw(window); continue; }
                if (obj is Range) { (obj as Range).Draw(window); continue; }
            }
        }
    }
}
