using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Automation;

namespace DFA
{
    public class WindowsWindowApi
    {
        static WinEventDelegate dele = null;
        delegate void WinEventDelegate(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime);

        [DllImport("user32.dll")]
        static extern IntPtr SetWinEventHook(uint eventMin, uint eventMax, IntPtr hmodWinEventProc, WinEventDelegate lpfnWinEventProc, uint idProcess, uint idThread, uint dwFlags);

        private const uint WINEVENT_OUTOFCONTEXT = 0;
        private const uint EVENT_SYSTEM_FOREGROUND = 3;

        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);



        public static event EventHandler<WindowSwitchedArgs> OnWindowSwitched;
        private static IntPtr _hookID = IntPtr.Zero;


        public WindowsWindowApi()
        {

            dele = new WinEventDelegate(WinEventProc);
            IntPtr m_hhook = SetWinEventHook(EVENT_SYSTEM_FOREGROUND, EVENT_SYSTEM_FOREGROUND, IntPtr.Zero, dele, 0, 0, WINEVENT_OUTOFCONTEXT);

        }




        private static string GetActiveWindowTitle()
        {
            const int nChars = 256;
            IntPtr handle = IntPtr.Zero;
            StringBuilder Buff = new StringBuilder(nChars);
            handle = GetForegroundWindow();

            if (GetWindowText(handle, Buff, nChars) > 0)
            {
                return Buff.ToString();
            }
            return null;
        }

        public void WinEventProc(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime)
        {



            var windowTitle = GetActiveWindowTitle();
            if (windowTitle == null) return;

            windowTitle = windowTitle.ToLower().Trim();


            if (OnWindowSwitched != null)
            {
                OnWindowSwitched(this, new WindowSwitchedArgs(windowTitle));
            }


        }
        public static string GetChromeActiveTabUrl()
        {
            Process[] procsChrome = Process.GetProcessesByName("chrome");

            if (procsChrome.Length <= 0)
                return null;

            foreach (Process proc in procsChrome)
            {
                // the chrome process must have a window 
                if (proc.MainWindowHandle == IntPtr.Zero)
                    continue;

                // to find the tabs we first need to locate something reliable - the 'New Tab' button 

                try
                {
                    AutomationElement root = AutomationElement.FromHandle(proc.MainWindowHandle);
                    var SearchBar =
                         root.FindFirst(TreeScope.Descendants, new PropertyCondition(AutomationElement.NameProperty, "Address and search bar"));
                    if (SearchBar != null)
                        return (string)SearchBar.GetCurrentPropertyValue(ValuePatternIdentifiers.ValueProperty);
                }
                catch
                {
                    Debug.WriteLine("exception chrome tab");
                }
                
            }

            return null;
        }


        public static async Task<string> AsyncGetChromeActiveTabUrl()
        {
            Process[] procsChrome = Process.GetProcessesByName("chrome");

            if (procsChrome.Length <= 0)
                return null;

            foreach (Process proc in procsChrome)
            {
                // the chrome process must have a window 
                if (proc.MainWindowHandle == IntPtr.Zero)
                    continue;

                // to find the tabs we first need to locate something reliable - the 'New Tab' button 
                AutomationElement root = AutomationElement.FromHandle(proc.MainWindowHandle);
                var SearchBar = 
                    await Task.Run(() => root.FindFirst(TreeScope.Descendants, new PropertyCondition(AutomationElement.NameProperty, "Address and search bar")));
                if (SearchBar != null)
                    return (string)SearchBar.GetCurrentPropertyValue(ValuePatternIdentifiers.ValueProperty);
            }

            return null;
        }
        public static bool IsChrome(string title)
        {
            return title.Contains("chrome");
        }

        public static bool IsChromeTab(string title)
        {
            if (!IsChrome(title)) return false;

            SplitProcessAndParameter(title, out string process, out string param);
            return !string.IsNullOrEmpty(param);
        }
        public static void SplitProcessAndParameter(string stringToSplit, out string process, out string parameter)
        {
            if (stringToSplit.Contains(':'))
            {


                List<string> spl = stringToSplit.Split(':').ToList<string>();
                if (spl[1].Length > 0)
                {
                    process = spl[0];
                    parameter = spl[1];
                }
                else
                {
                    process = spl[0];
                    parameter = "";
                }

            }
            else
            {
                process = stringToSplit;
                parameter = "";
            }


        }


        public static bool IsChromeTab(string title, out string url)
        {
            if (!IsChrome(title))
            {
                url = "";
                return false;
            }
            SplitProcessAndParameter(title, out string process, out url);
            return !string.IsNullOrEmpty(url);
        }


        public class WindowSwitchedArgs : EventArgs
        {
            public string Title { get; private set; }

            public WindowSwitchedArgs(string title)
            {
                Title = title;
            }

        }

    }
}
