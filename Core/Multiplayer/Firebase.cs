using SplashKitSDK;
using System;
using System.IO;
using System.Text;
using System.Net.Http;
using System.Threading.Tasks;

namespace MultiplayerMinesweeper.Core.Multiplayer
{
    public static class Firebase
    {
        public static async Task<string> Get(string jsonRelativePath)
        {
            using (var client = new HttpClient())
            {
                var res = await client.GetAsync($"{Constants.FIREBASE_URL}/{jsonRelativePath}.json");
                using (var reader = new StreamReader(await res.Content.ReadAsStreamAsync()))
                {
                    string result = await reader.ReadToEndAsync();
                    Logger.Log($"{result} from ./{jsonRelativePath}.json");
                    return result;
                }
            }
        }

        public static async Task<bool> Put(Json json, string jsonRelativePath)
            => await Put(SplashKit.JsonToString(json), jsonRelativePath);
        public static async Task<bool> Put(string stringToWrite, string jsonRelativePath)
        {
            var client = new HttpClient();
            string path = jsonRelativePath == "" ? "" : $"/{jsonRelativePath}";
            var putContent = new StringContent(stringToWrite, Encoding.UTF8, "application/json");
            var req = new HttpRequestMessage()
            {
                Method = System.Net.Http.HttpMethod.Put,
                RequestUri = new Uri($"{Constants.FIREBASE_URL}{path}.json"),
                Content = putContent
            };

            // run new task
            var newTask = Task.Run(() => {
                var asyncTask = client.SendAsync(req)
                    .Result
                    .Content
                    .ReadAsStringAsync();
                asyncTask.Wait();

                using (var resultJson = Json.FromJsonString(asyncTask.Result))
                {
                    bool hasErrorKey = resultJson.HasKey("error");
                    if(hasErrorKey) Logger.Log(Json.ToJsonString(resultJson));
                    return !hasErrorKey;
                }
            });

            // wait till 5 secs (default) and you are out
            bool result = newTask.Wait(Constants.TIMEOUT) && await newTask;
            Logger.Log((result ? "Succesfully" : "Failed") + $" upload data to {req.RequestUri.OriginalString}");
            return result;
        }

        public static async Task<bool> Delete(string jsonRelativePath)
        {
            using (var client = new HttpClient())
            {
                bool result = await client.DeleteAsync($"{Constants.FIREBASE_URL}/{jsonRelativePath}.json")
                    .Result
                    .Content
                    .ReadAsStringAsync() == "null";
                Logger.Log(result ? "Deleted data on..." : "???");
                return result;
            }
        }
    }
}
