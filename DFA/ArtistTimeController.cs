using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Threading;
using static DFA.WinApi;

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
            Debug.WriteLine(GetActiveWindowTitle() + "\r\n");
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
