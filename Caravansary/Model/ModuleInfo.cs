using System;

public class ModuleInfo
{
    public WeakReference AssemblyLoadContext { get; set; }
    public RemoteLoader Loader { get; set; }
}
