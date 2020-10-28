﻿using MultiplayerMinesweeper.Core;
using SplashKitSDK;

namespace MultiplayerMinesweeper.Drawing.Component
{
    public class Text : DrawableObject
    {
        private readonly string _text;

        public Text(string text)
            : this(text, Constants.NORMAL_FONT_SIZE, Constants.DEFAULT_FONT_NAME, Color.White)
        {

        }
        public Text(string text, int fontSize)
            : this(text, fontSize, Constants.DEFAULT_FONT_NAME, Color.White)
        {
            
        }
        public Text(string text, int fontSize, Color color)
            : this(text, fontSize, Constants.DEFAULT_FONT_NAME, color)
        {

        }
        public Text(string text, int fontSize, string fontName)
            : this(text, fontSize, fontName, Color.White)
        {

        }
        public Text(string text, int fontSize, string fontName, Color color)
            : base(color)
        {
            _text = text;
            FontSize = fontSize;
            FontName = fontName;
            if (Font == null) Font = SplashKit.LoadFont(FontName, FontName);
        }

        public static Font Font { get; private set; }
        public string FontName { get; }
        public int FontSize { get; }

        public Size GetTextSize()
        {
            int width = SplashKit.TextWidth(_text, FontName, FontSize),
                height = SplashKit.TextHeight(_text, FontName, FontSize);

            return new Size(width, height);
        }

        public override void Draw() => Draw(SplashKit.CurrentWindow());
        public override void Draw(Window window)
        {
            SplashKit.DrawTextOnWindow(window, _text, Color, Font, FontSize, X, Y);
        }
    }
}
