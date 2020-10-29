using SplashKitSDK;
using MultiplayerMinesweeper;
using MultiplayerMinesweeper.Core;
using MultiplayerMinesweeper.Drawing.UI;
using MultiplayerMinesweeper.Drawing.Reusable;

public class Program
{
    public static void Main()
    {
        new Window("Minesweeper", Constants.WINDOW_WIDTH, Constants.WINDOW_HEIGHT);

        do
        {
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
