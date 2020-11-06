using System.Collections.Generic;
using SplashKitSDK;

namespace MultiplayerMinesweeper.Drawing.Component
{
    public class Range : UIRectangle
    {
        private List<Button> _buttons = new List<Button>();
        private int _min, _max, _objectSpacing;

        public Range(int min, int max, int objectSpacing = 20) : this(min, max, min, objectSpacing) { }
        public Range(int min, int max, int value, int objectSpacing = 20)
        {
            _min = min;
            _max = max;
            CurrentValue = value;
            _objectSpacing = objectSpacing;

            // pushing objects
            _buttons.Add(new Button("-")
            {
                Action = Decrement
            });
            _buttons.Add(new Button(CurrentValue.ToString())
            {
                X = _buttons[0].Width + objectSpacing,
                PaddingLeft = 100
            });
            _buttons.Add(new Button("+")
            {
                X = _buttons[1].X + _buttons[1].Width + objectSpacing, 
                Action = Increment
            });

            // set primal size
            Width = (int)_buttons[2].X + _buttons[2].Width;
            Height = _buttons[2].Height + 10;
        }

        public int CurrentValue { get; private set; }

        public void Increment()
        {
            // increment first (if it gets over upper boundary, reject the action)
            if(CurrentValue + 1 > _max) return;
            // second object always has mutable text
            _buttons[1].ChangeText((++CurrentValue).ToString());
        }

        public void Decrement()
        {
            // decrement first (if it gets over lower boundary, reject the action)
            if (CurrentValue - 1 < _min) return;

            // second object always has mutable text
            _buttons[1].ChangeText((--CurrentValue).ToString());
        }

        /// <summary>
        /// Automatically check which button to be clicked upon
        /// </summary>
        public void Click() => _buttons.ForEach((button) => button.Click());

        public override void AlignHorizontally(int windowWidth = 1000)
        {
            base.AlignHorizontally(windowWidth);

            // 1st button
            _buttons[0].X = X;
            _buttons[0].Text.X = X + _buttons[0].PaddingLeft;

            // 2nd button
            _buttons[1].X = X + _buttons[0].Width + _objectSpacing;
            _buttons[1].Text.X = _buttons[1].X + _buttons[1].PaddingLeft;
            
            // 3rd button
            _buttons[2].X = _buttons[1].X + _buttons[1].Width + _objectSpacing;
            _buttons[2].Text.X = _buttons[2].X + _buttons[2].PaddingLeft;
        }

        public override void AlignVertically(int windowHeight = 600)
        {
            base.AlignVertically(windowHeight);
            _buttons.ForEach((button) =>
            {
                button.Y = Y;
                button.Text.Y = Y + button.PaddingTop;
            });
        }

        public override void Draw() =>  _buttons.ForEach((button) => button.Draw());
    }
}
