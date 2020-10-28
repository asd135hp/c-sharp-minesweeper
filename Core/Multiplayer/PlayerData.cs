using SplashKitSDK;
using System.Collections.Generic;

namespace MultiplayerMinesweeper.Core.Multiplayer
{
    public class PlayerData : IJson
    {
        public PlayerData(GameSettings settings)
        {
            Time = 0;
            Flag = settings.Bomb;
            Board = new MinesweeperBoard(settings.BoardWidth, settings.BoardHeight, settings.Bomb);
        }

        public int Time, Flag;
        public readonly Board Board;

        public string ToJsonString()
        {
            // take a chance to update Flag
            Flag = (Board as MinesweeperBoard).Flag;

            var json = SplashKit.CreateJson();
            json.AddNumber("time", Time);
            json.AddNumber("flag", Flag);
            json.AddArray("board", new List<string>(Board.DrawableBoard));
            return SplashKit.JsonToString(json);
        }

        public void FromJson(Json json)
        {
            Time = json.ReadInteger("time");
            Flag = json.ReadInteger("flag");

            // merge current board
            List<string> temp = new List<string>();
            json.ReadArray("board", ref temp);
            Board.MergeBoard(temp.ToArray());
        }
        public void FromJson(string jsonString)
        {
            try
            {
                FromJson(SplashKit.CreateJson(jsonString));
            }
            finally
            {
                // empty block here, just to catch the exception without much penalty
            }
        }
    }
}
