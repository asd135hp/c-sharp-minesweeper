using System;
using System.Threading;

namespace MultiplayerMinesweeper.Core.Multiplayer.TaskManagement
{
    public class DownloadTask : TaskAction<string>
    {
        private DownloadTask(CancellationToken token, Action<string> action = null, int mergeDataRate = 1000)
            : base(token, action, (result) => result != "" && result != "null", mergeDataRate)
        {

        }

        /// <summary>
        /// Singleton design pattern, create a new single instance for tasks management only
        /// </summary>
        /// <param name="token">Cancellation token for cancelling current task action</param>
        /// <param name="mergeDataRate">Data merging rate (in milliseconds), is only used for downloading</param>
        /// <param name="action">An action that has a result of type T</param>
        public static TaskAction<string> CreateInstance(
            CancellationToken token,
            Action<string> action = null,
            int mergeDataRate = 1000
        )
        {
            if(_instance == null) _instance = new DownloadTask(token, action, mergeDataRate);
            return _instance;
        }
    }
}
