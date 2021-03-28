using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Input;


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

    private static List<Key> AllKeyPressed;


    private const int WH_KEYBOARD_LL = 13;
    private const int WM_KEYDOWN = 0x0100;
    private const int WM_SYSKEYDOWN = 0x0104;
    private const int WM_KEYUP = 0x0101;
    private const int WM_SYSKEYUP = 0x0105;


    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    public static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);


    public delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

    public event Action<KeyPressedArgs> OnKeyPressed;
    public event Action<KeyReleasedArgs> OnKeyReleased;

    private LowLevelKeyboardProc _proc;
    private IntPtr _hookID = IntPtr.Zero;

    public KeyboardListener()
    {
        AllKeyPressed = new List<Key>();

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
            Key keyPressed = KeyInterop.KeyFromVirtualKey(vkCode);
            if (!AllKeyPressed.Contains(keyPressed))
                AllKeyPressed.Add(keyPressed);

            if (OnKeyPressed != null) { OnKeyPressed.Invoke(new KeyPressedArgs(keyPressed)); }
        }

        if (nCode >= 0 && wParam == (IntPtr)WM_KEYUP || wParam == (IntPtr)WM_SYSKEYUP)
        {
            int vkCode = Marshal.ReadInt32(lParam);
            Key keyPressed = KeyInterop.KeyFromVirtualKey(vkCode);
            if (AllKeyPressed.Contains(keyPressed))
                AllKeyPressed.Remove(keyPressed);

            if (OnKeyReleased != null) { OnKeyReleased.Invoke(new KeyReleasedArgs(keyPressed)); }
        }

        return WinApi.CallNextHookEx(_hookID, nCode, wParam, lParam);
    }

    public bool IsKeyPressed(Key keyToCheck)
    {
        if (AllKeyPressed.Contains(keyToCheck))
            return true;

        return false;

    }
}
