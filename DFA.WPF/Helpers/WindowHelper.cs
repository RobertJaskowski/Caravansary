
using System.Windows;

namespace DFA
{
    public static class WindowHelper
    {
        public static double DefaultWindowX(Window Window)
        {
          return  ((SystemParameters.WorkArea.Width / 2) - (Window.Width / 2));
        }
        // Screen.FromControl(this).WorkingArea.Width / 2;

        public static int DefaultWindowY => 0;


        public static void ResetWindowPosition(this Window Window)
        {
            Window.WindowStartupLocation = WindowStartupLocation.Manual;
            SetWindowPosition(Window, DefaultWindowX(Window), DefaultWindowY);
        }
        public static void SetWindowPosition(this Window Window, double leftX, double topY)
        {
            Window.Left = leftX;
            Window.Top = topY;
        }
    }
}
