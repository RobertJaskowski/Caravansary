using System;
using System.Windows.Input;

namespace Caravansary.SDK
{
    public class KeyReleasedArgs : EventArgs
    {
        public Key KeyReleased { get; private set; }

        public KeyReleasedArgs(Key key)
        {
            KeyReleased = key;
        }
    }
}