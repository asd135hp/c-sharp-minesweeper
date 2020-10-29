using SplashKitSDK;
using System.Threading.Tasks;
using MultiplayerMinesweeper;
using MultiplayerMinesweeper.Core;
using MultiplayerMinesweeper.Drawing.UI;

public class Program
{
    public static void Main()
    {
        new Window("Minesweeper", Constants.WINDOW_WIDTH, Constants.WINDOW_HEIGHT);

        do
        {
            /*
                this solution seems to work fine.
                Normal loop usually runs under 1 milliseconds in each loop (for fast cases)
                and may be because of large overhead
                when the multiplayer game is trying to connect to the server
                that SplashKit will instantly froze the whole window at that time.
                Therefore, by adding a little delay
                that is unnoticeable to most players (in this case, 5 millisecs),
                it helps the loop go over that huge overhead
                (my hunch for something can never be true, though)
                NOTE: this is entirely my trial and errors attempts
                and even if I search this situation up on Google,
                nothing will even match what I am going through...
            */
            Task.Delay(5).Wait();

            // refresh window
            SplashKit.RefreshScreen();
            SplashKit.ProcessEvents();

            if (SplashKit.MouseClicked(MouseButton.LeftButton))
                UI.CurrentPage.Click(MouseButton.LeftButton);

            if (SplashKit.MouseClicked(MouseButton.RightButton))
            {
                if(UI.CurrentPage is GamePage)
                    (UI.CurrentPage as GamePage).Click(MouseButton.RightButton);
                if (UI.CurrentPage is MultiplayerPage)
                    (UI.CurrentPage as MultiplayerPage).Click(MouseButton.RightButton);
            }
                
            if (SplashKit.KeyTyped(KeyCode.EscapeKey)) UI.Back();

            // clear window with background color
            SplashKit.ClearScreen(Constants.BACKGROUND_COLOR);

            // draw everything to the screen(s)
            UI.CurrentPage.Draw();
        } while (!SplashKit.WindowCloseRequested("Minesweeper"));

        // trigger automatic cleanup so that there is no need to copy code anymore
        UI.Back();
        SplashKit.FreeAllJson();
    }
}
