using System;
using System.Windows.Input;

public class KeyReleasedArgs : EventArgs
{
    public Key KeyReleased { get; private set; }

    public KeyReleasedArgs(Key key)
    {
        KeyReleased = key;
    }
}
