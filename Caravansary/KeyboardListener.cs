using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Input;

namespace Caravansary
{
    public class KeyboardListener
    {

        private static KeyboardListener _instance;
        public static KeyboardListener Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new KeyboardListener();

                return _instance;
            }
        }


        private const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYDOWN = 0x0100;
        private const int WM_SYSKEYDOWN = 0x0104;
        private const int WM_KEYUP = 0x0101;
        private const int WM_SYSKEYUP = 0x0105;


        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);


        public delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

        public  event EventHandler<KeyPressedArgs> OnKeyPressed;
        public  event EventHandler<KeyReleasedArgs> OnKeyReleased;

        private LowLevelKeyboardProc _proc;
        private IntPtr _hookID = IntPtr.Zero;

        public KeyboardListener()
        {

            _proc = HookCallback;

            HookKeyboard();
        }



        public void HookKeyboard()
        {
            _hookID = SetHook(_proc);
            //Debug.WriteLine("Last error kb " + Marshal.GetLastWin32Error());

        }

        public void UnHookKeyboard()
        {
            OnKeyPressed = null;
            OnKeyReleased = null;
            WinApi.UnhookWindowsHookEx(_hookID);
        }

        private IntPtr SetHook(LowLevelKeyboardProc proc)
        {
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule)
            {
                return SetWindowsHookEx(WH_KEYBOARD_LL, proc, WinApi.GetModuleHandle(curModule.ModuleName), 0);
            }
        }

        private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && wParam == (IntPtr)WM_KEYDOWN || wParam == (IntPtr)WM_SYSKEYDOWN)
            {
                int vkCode = Marshal.ReadInt32(lParam);

                if (OnKeyPressed != null) { OnKeyPressed(this, new KeyPressedArgs(KeyInterop.KeyFromVirtualKey(vkCode))); }
            }

            if (nCode >= 0 && wParam == (IntPtr)WM_KEYUP || wParam == (IntPtr)WM_SYSKEYUP)
            {
                int vkCode = Marshal.ReadInt32(lParam);

                if (OnKeyReleased != null) { OnKeyReleased(this, new KeyReleasedArgs(KeyInterop.KeyFromVirtualKey(vkCode))); }
            }

            return WinApi.CallNextHookEx(_hookID, nCode, wParam, lParam);
        }
    }

    public class KeyReleasedArgs : EventArgs
    {
        public Key KeyReleased { get; private set; }

        public KeyReleasedArgs(Key key)
        {
            KeyReleased = key;
        }
    }

    public class KeyPressedArgs : EventArgs
    {
        public Key KeyPressed { get; private set; }

        public KeyPressedArgs(Key key)
        {
            KeyPressed = key;
        }
    }
}