using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using static DFA.WinApi;

namespace DFA
{
    class ArtistTimeController
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr SetWindowsHookEx(int idHook, CBTProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("kernel32.dll")]
        static extern uint GetLastError();

        [DllImport("kernel32.dll")]
        static extern uint GetCurrentThreadId();
        //[DllImport("DFALibrary.dll")]
        //public static extern bool fibTest();



        [DllImport("DLib.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool test();

        public delegate double CallbackDelegate(double x);

        // PInvoke declaration for the native DLL exported function
        [DllImport("DLib.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern double TestDelegate(CallbackDelegate func);

        private double MyFunctionCallback(double x)
        {
            
            
            
            Debug.WriteLine("test cb" + x);
            return 5;
        }



        public delegate IntPtr CBTProc(int nCode, IntPtr wParam, IntPtr lParam);
        private CBTProc _proc;

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

        CallbackDelegate managedDelegate;
        public ArtistTimeController(MainWindowViewModel mainWindowViewModel)
        {
            mainWindow = mainWindowViewModel;

            CreateArtistStateTimers();



            bool ret = test();
            if (ret)
            { 
                Application.Current.MainWindow.Background = new System.Windows.Media.SolidColorBrush(Color.FromRgb(124, 252, 0));
                Debug.WriteLine(ret);
            }
            else
            {
                Debug.WriteLine(ret);
                Application.Current.MainWindow.Background = new System.Windows.Media.SolidColorBrush(Color.FromRgb(255, 160, 122));

            }


             managedDelegate = new CallbackDelegate(MyFunctionCallback);

            var tt =TestDelegate(managedDelegate);


            _proc = HookCallback;

            OnWindowSwitched += test;


        }

        private void test(object sender, WindowSwitchedArgs e)
        {

        }

        public void HookWindowFocus()
        {
            _hookID = SetHook(_proc);

            Debug.WriteLine("Last error " + Marshal.GetLastWin32Error());
        }

        public void UnHookWindowFocus()
        {
            WinApi.UnhookWindowsHookEx(_hookID);
        }

        private IntPtr SetHook(CBTProc proc)
        {
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule)
            {
                //return SetWindowsHookEx(5, proc, WinApi.GetModuleHandle(curModule.ModuleName), 0);
                return SetWindowsHookEx(5, proc, WinApi.GetModuleHandle(string.Empty), 0);
            }
        }

        private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            //if((IntPtr)nCode!=9)
            Debug.WriteLine("Hook callback" + (IntPtr)nCode + " " + (IntPtr)wParam + " " + (IntPtr)lParam);

            //if (nCode >= 0 && wParam == (IntPtr)WM_KEYUP || wParam == (IntPtr)WM_SYSKEYUP)
            //{
            //    int vkCode = Marshal.ReadInt32(lParam);

            //    if (OnKeyReleased != null) { OnKeyReleased(this, new KeyReleasedArgs(KeyInterop.KeyFromVirtualKey(vkCode))); }
            //}

            return WinApi.CallNextHookEx(_hookID, nCode, wParam, lParam);
        }

        private void CreateArtistStateTimers()
        {

            DispatcherTimer timerArtistStateCheck = new DispatcherTimer();
            timerArtistStateCheck.Interval = TimeSpan.FromSeconds(1);
            timerArtistStateCheck.Tick += new EventHandler(TimerTick);
            timerArtistStateCheck.Start();

        }
        int unk = 0;
        private void TimerTick(object sender, EventArgs e)
        {
            unk++;
            if (unk > 10)
            {
                UnHookWindowFocus();
                if (unk == 10)
                    Debug.WriteLine("unhooked");
            }

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
