using DFA.Properties;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Threading;
using static DFA.WinApi;
using System.Linq;
using System.Windows.Automation;

namespace DFA
{
    class ArtistTimeController
    {


        WinEventDelegate dele = null;
        delegate void WinEventDelegate(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime);

        [DllImport("user32.dll")]
        static extern IntPtr SetWinEventHook(uint eventMin, uint eventMax, IntPtr hmodWinEventProc, WinEventDelegate lpfnWinEventProc, uint idProcess, uint idThread, uint dwFlags);

        private const uint WINEVENT_OUTOFCONTEXT = 0;
        private const uint EVENT_SYSTEM_FOREGROUND = 3;

        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);

        private string GetActiveWindowTitle()
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
            //TODO refactor and make async


            if (!Settings.Default.CheckBoxBlacklistEnabled) return;
            if (!(GlobalSettings.SettingsBlackListItems.Count > 0)) return;



            var windowTitle = GetActiveWindowTitle();

            if (windowTitle == null) return;


            windowTitle = windowTitle.ToLower().Trim();
            Debug.WriteLine("window Title " + windowTitle + "\r\n");



            //if (IsChrome(windowTitle))
            //{
            for (int i = 0; i < GlobalSettings.SettingsBlackListItems.Count; i++)
            {

                BlacklistItem item = GlobalSettings.SettingsBlackListItems[i];
                if (IsChrome(windowTitle))//called alot in debug
                {
                    if (IsChromeTab(item.Rule.ToLower(), out string settingsTabUrl))
                    {

                        Process procWithWindow = null;
                        string sURL = null;
                        Process[] procsChrome = Process.GetProcessesByName("chrome");
                        foreach (Process chrome in procsChrome)
                        {
                            if (chrome.MainWindowHandle == IntPtr.Zero)
                            {
                                continue;
                            }
                            else
                            {
                                procWithWindow = chrome;
                            }
                        }

                        if (procWithWindow == null) continue;


                        var ActiveTabUrl = "";


                        ActiveTabUrl = GetActiveTabUrl(procWithWindow.Handle);


                        //mainWindow.Label2 = "itu " + ActiveTabUrl.Substring(0, 10);


                        if (ActiveTabUrl.Contains(settingsTabUrl))
                        {
                            mainWindow.PausedReason = settingsTabUrl;
                            mainWindow.ArtistPause.Execute(null);
                            Debug.WriteLine("pausing");
                            return;
                        }
                        else
                        {
                            if (mainWindow.Artist.ArtistState == ArtistState.PAUSED)
                            {
                                mainWindow.PausedReason = "";

                                mainWindow.ArtistActivate.Execute(null);

                            }
                        }
                    }
                }
                else
                {
                    if (windowTitle.Contains(item.Rule.ToLower()))
                    {
                        mainWindow.PausedReason = item.Rule;

                        mainWindow.ArtistPause.Execute(null);
                        return;
                    }

                    if (mainWindow.Artist.ArtistState == ArtistState.PAUSED)
                    {
                        mainWindow.PausedReason = "";

                        mainWindow.ArtistActivate.Execute(null);
                        return;
                    }
                }
            }

        }
        private string GetActiveTabUrl(IntPtr process)
        {
            string sURL = null;
            Process[] procsChrome = Process.GetProcessesByName("chrome");
            foreach (Process chrome in procsChrome)
            {
                if (chrome.MainWindowHandle == IntPtr.Zero)
                {
                    continue;
                }
                AutomationElement element = AutomationElement.FromHandle(chrome.MainWindowHandle);
                AutomationElement elm1 = element.FindFirst(TreeScope.Children, new PropertyCondition(AutomationElement.NameProperty, "Google Chrome"));
                AutomationElement elm2 = TreeWalker.RawViewWalker.GetLastChild(elm1);
                AutomationElement elm3 = elm2.FindFirst(TreeScope.Children, new PropertyCondition(AutomationElement.NameProperty, ""));
                AutomationElement elm4 = elm3.FindFirst(TreeScope.Children, new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.ToolBar));
                AutomationElement elementx = elm1.FindFirst(TreeScope.Descendants, new PropertyCondition(AutomationElement.NameProperty, "Pasek adresu i wyszukiwania"));
                if (!(bool)elementx.GetCurrentPropertyValue(AutomationElement.HasKeyboardFocusProperty))
                {
                    sURL = ((ValuePattern)elementx.GetCurrentPattern(ValuePattern.Pattern)).Current.Value as string;
                }
            }
            return sURL;
        }
        private void SplitProcessAndParameter(string title, out string process, out string parameter)
        {
            if (title.Contains(':'))
            {


                List<string> spl = title.Split(':').ToList<string>();
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
                process = title;
                parameter = "";
            }


        }




        private bool IsChrome(string title)
        {
            return title.Contains("chrome");
        }

        private bool IsChromeTab(string title)
        {
            if (!IsChrome(title)) return false;

            SplitProcessAndParameter(title, out string process, out string param);
            return !string.IsNullOrEmpty(param);
        }

        private bool IsChromeTab(string title, out string url)
        {
            if (!IsChrome(title))
            {
                url = "";
                return false;
            }
            SplitProcessAndParameter(title, out string process, out url);
            return !string.IsNullOrEmpty(url);
        }


        public event EventHandler<WindowSwitchedArgs> OnWindowSwitched;
        private IntPtr _hookID = IntPtr.Zero;

        public class WindowSwitchedArgs : EventArgs
        {
            //public Key KeyPressed { get; private set; }

            //public KeyPressedArgs(Key key)
            //{
            //    KeyPressed = key;
            //}
        }


        private MainWindowViewModel mainWindow;

        private const int maxSecAfkTime = 2;
        private int currentCheckingAfkTime = 0;

        public ArtistTimeController(MainWindowViewModel mainWindowViewModel)
        {
            mainWindow = mainWindowViewModel;

            CreateArtistStateTimers();

            dele = new WinEventDelegate(WinEventProc);
            IntPtr m_hhook = SetWinEventHook(EVENT_SYSTEM_FOREGROUND, EVENT_SYSTEM_FOREGROUND, IntPtr.Zero, dele, 0, 0, WINEVENT_OUTOFCONTEXT);


            OnWindowSwitched += test;


        }

        private void test(object sender, WindowSwitchedArgs e)
        {

        }





        private void CreateArtistStateTimers()
        {

            DispatcherTimer timerArtistStateCheck = new DispatcherTimer();
            timerArtistStateCheck.Interval = TimeSpan.FromSeconds(1);
            timerArtistStateCheck.Tick += new EventHandler(TimerTick);
            timerArtistStateCheck.Start();

        }
        private void TimerTick(object sender, EventArgs e)
        {


            // ActiveTimeUpdate.Execute(null);

            CheckStateChanges();

            OnTick();
        }


        private void OnArtistStateInactiveTick()
        {

        }


        private void CheckStateChanges()
        {


            if (mainWindow.Artist.ArtistState == ArtistState.PAUSED)
            {
                return;
            }
            else if (!mainWindow.Artist.ArtistActive)
            {
                var r = CheckIfAnyInputReceived();

                if (r)
                {

                    mainWindow.ArtistActivate.Execute(null);
                }
            }
            else//active
            {
                var lastInputState = CheckIfAnyInputReceived();

                if (lastInputState)
                {
                    currentCheckingAfkTime = 0;
                }
                else
                {
                    currentCheckingAfkTime++;
                    if (currentCheckingAfkTime > maxSecAfkTime)
                    {
                        currentCheckingAfkTime = 0;
                        mainWindow.ArtistDeactivate.Execute(null);

                    }
                }
            }
        }



        uint lastActive = 0;
        private bool CheckIfAnyInputReceived()
        {

            LASTINPUTINFO lastInputInfo = new LASTINPUTINFO();
            lastInputInfo.cbSize = (UInt32)Marshal.SizeOf(lastInputInfo);
            lastInputInfo.dwTime = 0;

            if (GetLastInputInfo(ref lastInputInfo))
            {
                if (lastInputInfo.dwTime != lastActive)
                {
                    lastActive = lastInputInfo.dwTime;
                    return true;
                }
            }


            return false;
        }


        private void OnTick()
        {
            switch (mainWindow.Artist.ArtistState)
            {
                case ArtistState.ACTIVE:
                    OnArtistStateActiveTick();
                    break;
                case ArtistState.INACTIVE:
                    OnArtistStateInactiveTick();
                    break;
                case ArtistState.PAUSED:
                    break;
            }
        }

        private void OnArtistStateActiveTick()
        {
            mainWindow.ActiveTimeUpdate1Sec.Execute(null);



            mainWindow.UpdateTopBar();
            //UpdateBottomBar();
            //StartAnimatingBottomBar();
        }
    }
}
