using System;

namespace MultiplayerMinesweeper.Core
{
    public static class Logger
    {
        private static int _maxBytes = 16 * 1024 * 1024; // 16MB
        private static int _file = 0;
        private static void LogToTextFile(string message)
        {
            // i'm lazy so I will let it be for now
        }

        public static void Log(string message)
        {
            if (Constants.ENABLE_LOGGER)
            {
                if (Constants.LOG_TO_TXT_FILE) LogToTextFile(message);
                else Console.WriteLine("[{0}] {1}", DateTime.Now.ToString(), message);
            }
        }
    }
}
