using System;



public class WindowSwitchedArgs : EventArgs
{
    public string Title { get; private set; }

    public WindowSwitchedArgs(string title)
    {
        Title = title;
    }

}

