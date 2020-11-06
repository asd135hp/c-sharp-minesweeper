namespace MultiplayerMinesweeper.Core.Multiplayer
{
    public interface IJson
    {
        /// <summary>
        /// Turns any object into a JSON string for better transportation
        /// </summary>
        /// <returns>JSON string from object's properties</returns>
        string ToJsonString();

        /// <summary>
        /// Merge json string with current object.
        /// SplashKit will echo warning to any possible errors.
        /// </summary>
        /// <param name="jsonString">Expected JSON string for object</param>
        void FromJson(string jsonString);

        /// <summary>
        /// Merge json string with current object.
        /// SplashKit will echo warning to any possible errors.
        /// </summary>
        /// <param name="json">Expected JSON object for object</param>
        void FromJson(SplashKitSDK.Json json);
    }
}
