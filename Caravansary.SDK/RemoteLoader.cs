using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;


public class RemoteLoader : MarshalByRefObject
{
    private Assembly _pluginAassembly;
    private ICoreModule _instance;
    private string _name;

    public void Init(IModuleController host, string assemblyPath)
    {
        // note that you pass reference to controller here
        _name = Path.GetFileNameWithoutExtension(assemblyPath);
        if (_pluginAassembly == null)
        {
            _pluginAassembly = AppDomain.CurrentDomain.Load(File.ReadAllBytes(assemblyPath));
        }

        // Required to identify the types when obfuscated
        Type[] types;
        try
        {
            types = _pluginAassembly.GetTypes();
        }
        catch (ReflectionTypeLoadException e)
        {
            types = e.Types.Where(t => t != null).ToArray();
        }

        var type = types.FirstOrDefault(t => t.GetInterface("ICoreModule") != null);
        if (type != null && _instance == null)
        {
            _instance = (ICoreModule)Activator.CreateInstance(type, null, null);
            // propagate reference to controller futher
            _instance.Init(host);
        }
    }

    public string Name => _name;
    public bool IsStarted { get; private set; }

    public void Start()
    {
        if (_instance == null)
        {
            return;
        }
        _instance.Start();
        IsStarted = true;
    }

    public void Stop()
    {
        if (_instance == null)
        {
            return;
        }
        _instance.Stop();
        IsStarted = false;
    }
}
