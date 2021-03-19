using Caravansary;
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
using System.Xml.Serialization;
using static WindowsWindowApi;

public class ModuleController : MarshalByRefObject, IModuleController
{
    
    public ModuleController()
    {

    }

    private Dictionary<string, ModuleInfo> _coreModules = new Dictionary<string, ModuleInfo>();

    public Action<ModuleInfo> OnModulesChanged;
    public Action<ModuleInfo> OnModuleAdded;
    public Action<ModuleInfo> OnModuleRemoved;

    public string[] CoreModulesKeys => _coreModules.Keys.ToArray();
    public ModuleInfo[] CoreModuleValues => _coreModules.Values.ToArray();

    #region Listeners
    public void HookWindowSwitchEvent(Action<WindowSwitchedArgs> method)
    {
        WindowsWindowApi.Instance.OnWindowSwitched += method;
    }

    public void UnHookWindowSwitchEvent(Action<WindowSwitchedArgs> method)
    {
        WindowsWindowApi.Instance.OnWindowSwitched -= method;
    }

    public void HookKeyboardPressedEvent(Action<KeyPressedArgs> method)
    {
        KeyboardListener.Instance.OnKeyPressed += method;
    }

    public void UnHookKeyboardPressedEvent(Action<KeyPressedArgs> method)
    {
        KeyboardListener.Instance.OnKeyPressed -= method;
    }

    public void HookKeyboardReleaseEvent(Action<KeyPressedArgs> method)
    {
        KeyboardListener.Instance.OnKeyPressed += method;
    }

    public void UnHookKeyboardReleasedEvent(Action<KeyPressedArgs> method)
    {
        KeyboardListener.Instance.OnKeyPressed -= method;
    }

    #endregion

    public void ScanDirectory(string path)
    {

        List<string> SpecificModuleDirectories = new List<string>();

        if (IsModulesDirectory(path))
        {
            var tempDir = Directory.GetDirectories(path).ToList();

            foreach (var td in tempDir)
            {
                if (CheckIfDirectoryIsAValidModule(td))
                    SpecificModuleDirectories.Add(td);
            }

        }
        else
        {
            if (CheckIfDirectoryIsAValidModule(path))
                SpecificModuleDirectories.Add(path);
        }


        if (SpecificModuleDirectories.Count <= 0)
            return;




        foreach (var dllDirectory in SpecificModuleDirectories)
        {
            string dllPath = Directory.GetFiles(dllDirectory, "*.dll").FirstOrDefault();
            if (string.IsNullOrEmpty(dllPath))
                continue;



            try
            {

                RemoteLoader remoteLoader = new RemoteLoader();

                PluginLoadContext loadContext = new PluginLoadContext(dllPath);




                remoteLoader.Init(this, loadContext, dllPath);



                _coreModules.Add(remoteLoader.Name, new ModuleInfo
                {
                    AssemblyLoadContext = loadContext,
                    Loader = remoteLoader
                });


            }
            catch (Exception e)
            {
                Debug.WriteLine("failed loading plugin " + dllDirectory + " " +e.Message);
            }




        }

    }


    private bool CheckIfDirectoryIsAValidModule(string dirName)
    {
        var files = Directory.GetFiles(dirName, "*.dll");

        if (files.Count() <= 0)
            return false;




        string moduleName = Path.GetFileNameWithoutExtension(dirName);

        bool foundSameNamedDll = false;

        foreach (var fn in files)
        {
            if (fn.ToLower().Contains(moduleName.ToLower() + ".dll"))
            {
                foundSameNamedDll = true;
                break;
            }
        }
        if (!foundSameNamedDll) return false;




        return true;
    }
    private bool IsModulesDirectory(string path)
    {
        var p = Path.GetFileName(path);
        
        return p.Contains("Modules");
    }


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

    
    public void SendMessage(string ModuleName, string message)
    {
        if (_coreModules.ContainsKey(ModuleName))
        {
            var p = _coreModules[ModuleName].Loader;
            if (p.IsStarted)
            {
                p.ReceiveMessage(message);
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

    internal void StopAllModules()
    {
        foreach (var item in CoreModulesKeys)
        {

            OnModuleRemoved?.Invoke(_coreModules[item]);
            _coreModules[item].Loader.Stop();
            _coreModules.Remove(item);

        }
    }

    public bool SaveModuleInformation(string ModuleName, string saveFileName, object objectToSave)
    {
        return Saves.Save(ModuleName, saveFileName, objectToSave);
    }

    public T LoadModuleInformation<T>(string ModuleName, string saveFileName)
    {
        return Saves.Load<T>(ModuleName, saveFileName);
    }
}
