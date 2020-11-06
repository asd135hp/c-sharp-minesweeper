using System;
using System.Text;
using MultiplayerMinesweeper.Core;
using MultiplayerMinesweeper.Drawing.Component;

namespace MultiplayerMinesweeper.Drawing.UI
{
    public class ResultPage : GraphicalPage
    {
        private string _rank;
        private readonly bool _isWin;

        public ResultPage(int usedFlags, int totalFlags, int timePlayed, bool isWin)
        {
            _isWin = isWin;

            GetRank(usedFlags, totalFlags, timePlayed);

            _drawingObjects.AddRange(new UIRectangle[]
            {
                new Button(0, 0, "You " + (_isWin ? "win!" : "lose!"), Constants.HEADER_FONT_SIZE)
                {
                    HorizontalAlign = 0.5f,
                    VerticalAlign = 0.15f,
                    BorderColor = Constants.BACKGROUND_COLOR
                },
                new Button($"Used flags: {usedFlags}", 0, 0)
                {
                    HorizontalAlign = 0.5f,
                    VerticalAlign = 0.35f,
                    BorderColor = Constants.BACKGROUND_COLOR
                },
                new Button($"Time used: {TimeFormatter.GetTime(timePlayed)}", 0, 0)
                {
                    HorizontalAlign = 0.5f,
                    VerticalAlign = 0.45f,
                    BorderColor = Constants.BACKGROUND_COLOR
                },
                new Button($"Rank: {_rank}", 0, 0)
                {
                    HorizontalAlign = 0.5f,
                    VerticalAlign = 0.55f,
                    BorderColor = Constants.BACKGROUND_COLOR
                },
                new Button("Back to main menu", 0, 0)
                {
                    HorizontalAlign = 0.5f,
                    VerticalAlign = 0.8f,
                    Action = () => MultiplayerMinesweeper.UI.Back()
                }
            });
            _drawingObjects.ForEach((rect) => { rect.AlignHorizontally(); rect.AlignVertically(); });
        }

        /// <summary>
        /// A method for getting rank from game session
        /// </summary>
        private void GetRank(int usedFlags, int totalFlags, int timePlayed)
        {
            int flagRank = (int)(usedFlags * 6.0 / totalFlags),
                timeRank = (int)Math.Floor(timePlayed / 30.0),
                finalRank = (int)Math.Ceiling((flagRank + timeRank) / 2.0);

            // 64 - before A in ASCII table to 64 + 5 = 69 or E in ASCII table
            _rank = finalRank >= 6 || !_isWin ? "F" : (
                finalRank == 0 ? "???" : Encoding.ASCII.GetString(new byte[] { (byte)(64 + finalRank) })
            );
        }

        public override void Draw()
        {
            foreach (var obj in _drawingObjects)
            {
                if (obj is Button) { (obj as Button).Draw(); continue; }
                if (obj is Range) { (obj as Range).Draw(); continue; }
            }
        }
    }
}
