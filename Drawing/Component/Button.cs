using SplashKitSDK;
using MultiplayerMinesweeper.Core;

namespace MultiplayerMinesweeper.Drawing.Component
{
    public class Button : UIRectangle
    {
        private int _paddingTop, _paddingLeft;

        public Button(string text)
            : this(0, 0, text, Constants.NORMAL_FONT_SIZE, Constants.DEFAULT_FONT_NAME)
        {

        }
        public Button(string text, int width, int height)
            : this(width, height, text, Constants.NORMAL_FONT_SIZE, Constants.DEFAULT_FONT_NAME)
        {

        }
        public Button(int width, int height, string text, int fontSize, string fontName = "")
            : base(width, height)
        {
            // initialize text at the start
            // (padding top and padding left will automatically centralize the text)
            Text = fontName != "" ?
                new Text(text, fontSize, fontName) :
                new Text(text, fontSize);

            // set padding top and padding left, trigger automatic functionalities
            PaddingTop = 20;
            PaddingLeft = 40;

            Size textSize = Text.GetTextSize();

            // determine path way, depending on how width is specified
            // padding based UI automatic alignment
            // if static dimension is provided, corresponding padding dimension is automatically adjusted
            // or else, just set the corresponding size dimension
            // (padding is set previously so set it again is not a good choice)
            // at least this is not HTML, or else this would pose a lot of problems
            if (width == 0) Width = PaddingLeft * 2 + textSize.Width;
            else PaddingLeft = (width - textSize.Width) / 2;

            if (height == 0) Height = PaddingTop * 2 + textSize.Height;
            else PaddingTop = (height - textSize.Height) / 2;
        }

        /// <summary>
        /// Setting this property will automatically trigger vertical alignment if alignment is not 0
        /// </summary>
        public int PaddingTop
        {
            get => _paddingTop;
            set
            {
                AlignVertically();
                _paddingTop = value;
                Height = value * 2 + Text.GetTextSize().Height;
                Text.Y = Y + value;
            }
        }
        /// <summary>
        /// Setting this property will automatically trigger horizontal alignment if alignment is not 0
        /// </summary>
        public int PaddingLeft
        {
            get => _paddingLeft;
            set
            {
                AlignHorizontally();
                _paddingLeft = value;
                // width shrinks as the range's number changes
                Width = value * 2 + Text.GetTextSize().Width;
                Text.X = X + value;
            }
        }
        public Text Text { get; private set; }
        public System.Action Action = null;

        /// <summary>
        /// Open the possibility of changing text (simplified, only the text's content is changed)
        /// because button's text is initially immutable.
        /// </summary>
        /// <param name="newText">New text to be changed</param>
        /// <param name="noSizeChange">
        /// Set true if the new text will be centralized in current border.
        /// Or false on dynamically resize the button
        /// </param>
        public void ChangeText(string newText, bool noSizeChange = true)
        {
            Text = new Text(newText, Text.FontSize, Text.Color);

            Size textSize = Text.GetTextSize();
            if (noSizeChange)
            {
                // static size
                PaddingLeft = (Width - textSize.Width) / 2;
                PaddingTop = (Height - textSize.Height) / 2;
                return;
            }

            // dynamic size
            Width = PaddingLeft * 2 + textSize.Width;
            Height = PaddingTop * 2 + textSize.Height;
            AlignHorizontally();
            AlignVertically();
        }

        /// <summary>
        /// Check if mouse position is inside the button.
        /// This method does not count border's pixels
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public bool IsMouseOver(Point2D point)
        {
            double x = point.X,
                y = point.Y;

            // does not include border (to be changed)
            return x >= X && x <= X + Width && y >= Y && y <= Y + Height;
        }

        public void ExecuteAction() => Action?.Invoke();
        public void Click()
        {
            if (IsMouseOver(SplashKit.MousePosition())) ExecuteAction();
        }

        public override void AlignHorizontally(int windowWidth = 1000)
        {
            base.AlignHorizontally(windowWidth);
            Text.X += X;
        }

        public override void AlignVertically(int windowHeight = 600)
        {
            base.AlignVertically(windowHeight);
            Text.Y += Y;
        }

        public override void Draw() => Draw(SplashKit.CurrentWindow());
        public override void Draw(Window window)
        {
            base.Draw(window);
            Text.Draw(window);
        }
    }
}
