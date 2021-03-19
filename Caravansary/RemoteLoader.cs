using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Runtime.Loader;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

public class RemoteLoader : MarshalByRefObject
{
    private Assembly _pluginAssembly;
    private ICoreModule _instance;
    public ICoreModule Instance
    {
        get { return _instance; }
        private set
        {
            _instance = value;
        }
    }

    private string _name;
    public string Name => _name;
    public bool IsStarted { get; private set; }

    //private UserControl _view;
    //public UserControl View { get { return _view; } private set { _view = value; } }

    public ICoreModule _instanceSettings;

    public void Init(IModuleController host, AssemblyLoadContext alc, string assemblyDllPath)
    {
        _name = Path.GetFileNameWithoutExtension(assemblyDllPath);



        _pluginAssembly = alc.LoadFromAssemblyName(new AssemblyName(Path.GetFileNameWithoutExtension(assemblyDllPath)));




        Type[] types;
        try
        {
            types = _pluginAssembly.GetTypes();
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


        //var stream = _pluginAssembly.GetManifestResourceStream(_pluginAssembly.GetName().Name + ".g.resources");

        //var resourceReader = new ResourceReader(stream);


        //foreach (DictionaryEntry resource in resourceReader)
        //{
        //    if (new FileInfo(resource.Key.ToString()).Extension.Equals(".baml"))
        //    {

        //        Uri uri = new Uri("/" + _pluginAssembly.GetName().Name + ";component/" + resource.Key.ToString().Replace(".baml", ".xaml"), UriKind.Relative);

        //        Debug.WriteLine(resourceReader.ToString());
        //        UserControl currentUserControl = Application.LoadComponent(uri) as UserControl;




        //        currentUserControl.DataContext = _instance;


        //        View = currentUserControl;

        //        break;
        //    }
        //}
    }

    public void Start()
    {
        if (_instance == null)
        {
            return;
        }
        _instance.Start();
        IsStarted = true;
    }

    public void ReceiveMessage(string message)
    {
        if (_instance == null)
        {
            return;
        }
        _instance.ReceiveMessage(message);
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
