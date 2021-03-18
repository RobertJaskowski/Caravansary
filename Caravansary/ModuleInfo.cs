using Caravansary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Loader;
using System.Text;
using System.Threading.Tasks;

public class ModuleInfo
{
    public PluginLoadContext AssemblyLoadContext { get; set; }
    public RemoteLoader Loader { get; set; }
}
