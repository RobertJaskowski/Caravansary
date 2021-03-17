using Caravansary.SDK;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;


public class ModuleController : MarshalByRefObject, IModuleController
{
    private readonly Dictionary<string, ModuleInfo> _coreModules = new Dictionary<string, ModuleInfo>();

    public void ScanAssemblies(params string[] paths)
    {
        foreach (var path in paths)
        {
            //var setup = new AppDomainSetup();
            //var domain = AppDomain.CreateDomain(Path.GetFileNameWithoutExtension(path), null, setup);
            var domain = AppDomain.CreateDomain(Path.GetFileNameWithoutExtension(path));
            var assemblyPath = Assembly.GetExecutingAssembly().Location;
            var loader = (RemoteLoader)domain.CreateInstanceFromAndUnwrap(assemblyPath, typeof(RemoteLoader).FullName);
            // you are passing "this" (which is IHostController) to your plugin here
            loader.Init(this, path);
            _coreModules.Add(loader.Name, new ModuleInfo
            {
                Domain = domain,
                Loader = loader
            });
        }
    }


    public string[] CoreModules => _coreModules.Keys.ToArray();

    public void StartCoreModule(string name)
    {
        if (_coreModules.ContainsKey(name))
        {
            var p = _coreModules[name].Loader;
            if (!p.IsStarted)
            {
                p.Start();
            }
        }
    }

    public void StopCoreModule(string name)
    {
        if (_coreModules.ContainsKey(name))
        {
            var p = _coreModules[name].Loader;
            if (p.IsStarted)
            {
                p.Stop();
            }
        }
    }

    public void SendMessage(string ModuleName, string message)
    {
        if (_coreModules.ContainsKey(ModuleName))
        {
            var p = _coreModules[ModuleName].Loader;
            if (p.IsStarted)
            {
                p.Stop();
            }
        }
    }
}
