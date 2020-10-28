namespace MultiplayerMinesweeper.Core.Multiplayer
{
    public interface IJson
    {
        string ToJsonString();
        void FromJson(string jsonString);
        void FromJson(SplashKitSDK.Json jsonString);
    }
}
