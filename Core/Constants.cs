using SplashKitSDK;

namespace MultiplayerMinesweeper.Core
{
    public static class Constants
    {
        // visual related constants
        public const int WINDOW_WIDTH = 1000;
        public const int WINDOW_HEIGHT = 600;
        public const int INFORMATION_BAR_HEIGHT = 100;
        public const double SCREENS_RATIO = 0.75;
        // board size
        public const int MAX_BOARD_WIDTH_PIXELS = MIN_SQUARE_SIZE * MAX_BOARD_WIDTH;
        public const int MAX_BOARD_HEIGHT_PIXELS = MIN_SQUARE_SIZE * MAX_BOARD_HEIGHT;
        // square size
        public const int MIN_SQUARE_SIZE = 17;
        public const int MAX_SQUARE_SIZE = 45;
        public const int OPPONENT_SQUARE_SIZE = 12;
        // padding
        public const int MAX_BOARD_PADDING_LEFT
            = (WINDOW_WIDTH - MAX_BOARD_WIDTH_PIXELS) / 2;
        public const int MAX_BOARD_PADDING_TOP
            = (WINDOW_HEIGHT - INFORMATION_BAR_HEIGHT - MAX_BOARD_HEIGHT_PIXELS) / 2;

        // data related constants
        public const int MAX_BOARD_WIDTH = 40;
        public const int MAX_BOARD_HEIGHT = 25;
        public const int MAX_BOARD_BOMB = MAX_BOARD_WIDTH * MAX_BOARD_HEIGHT - 10;

        // global settings
        public const int NORMAL_FONT_SIZE = 20;
        public const int HEADER_FONT_SIZE = 40;
        public const string DEFAULT_FONT_NAME = "Roboto-Regular";
        public static readonly Color BACKGROUND_COLOR = Color.Black;
        public static readonly Color TEXT_COLOR = Color.White;

        // multiplayer link
        public const string FIREBASE_URL = "https://uni-project-43896.firebaseio.com/minesweeper";

        // multiplayer constant
        public const int TIMEOUT = 5000;
        public const string GAMENAME = "OOPGame";

        // switch for logs
        public const bool ENABLE_LOGGER = true;
        public const bool LOG_TO_TXT_FILE = false;
    }
}
