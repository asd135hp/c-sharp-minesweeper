using System.Collections.Generic;
using MultiplayerMinesweeper.Drawing.Component;
using SplashKitSDK;

namespace MultiplayerMinesweeper.Drawing.UI
{
    public abstract class GraphicalPage
    {
        protected List<UIRectangle> _drawingObjects;

        public GraphicalPage()
        {
            _drawingObjects = new List<UIRectangle>();
            PreviousPage = null;
        }

        public GraphicalPage PreviousPage;

        /// <summary>
        /// Clicking is the only way to proceed (Monodirectional list datastructure ????)
        /// </summary>
        /// <returns></returns>
        public virtual void Click(MouseButton clickedButton)
        {
            foreach(var obj in _drawingObjects)
            {
                if (obj is Button)
                {
                    (obj as Button).Click();
                    continue;
                }
                if (obj is Range) (obj as Range).Click();
            }
        }

        public void Draw() => Draw(SplashKit.CurrentWindow());
        public abstract void Draw(Window window);
    }
}
