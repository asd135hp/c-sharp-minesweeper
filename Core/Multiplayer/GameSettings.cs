using SplashKitSDK;

namespace MultiplayerMinesweeper.Core.Multiplayer
{
    public class GameSettings : IJson
    {
        public GameSettings(Json json, int gameId)
        {
            BoardWidth = json.ReadInteger("boardWidth");
            BoardHeight = json.ReadInteger("boardHeight");
            Bomb = json.ReadInteger("bombNumber");
            GameID = gameId;
        }
        public GameSettings(string jsonString, int gameId)
            : this(SplashKit.CreateJson(jsonString), gameId) { }
        public GameSettings(int boardWidth, int boardHeight, int bomb, int gameId)
        {
            BoardWidth = boardWidth;
            BoardHeight = boardHeight;
            Bomb = bomb;
            GameID = gameId;
        }

        public int BoardWidth { get; private set; }
        public int BoardHeight { get; private set; }
        public int Bomb { get; private set; }
        public int GameID { get; private set; }

        public void FromJson(string jsonString) => FromJson(SplashKit.CreateJson(jsonString));
        public void FromJson(Json json)
        {
            BoardWidth = json.ReadInteger("boardWidth");
            BoardHeight = json.ReadInteger("boardHeight");
            Bomb = json.ReadInteger("bombNumber");
        }

        public string ToJsonString()
        {
            var json = SplashKit.CreateJson();
            json.AddNumber("boardWidth", BoardWidth);
            json.AddNumber("boardHeight", BoardHeight);
            json.AddNumber("bombNumber", Bomb);
            return SplashKit.JsonToString(json);
        }
    }
}
