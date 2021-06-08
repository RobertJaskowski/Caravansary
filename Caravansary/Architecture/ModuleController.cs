using Caravansary;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Mime;
using System.Reflection;
using System.Runtime.Loader;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

public class ModuleController : MarshalByRefObject, IModuleController
{
    private static ModuleController _instance;

    public static ModuleController Instance
    {
        get
        {
            if (_instance == null) _instance = new ModuleController();

            return _instance;
        }
    }

    public ModuleController()
    {
    }

    private Dictionary<string, ModuleInfo> _coreModules = new Dictionary<string, ModuleInfo>();

    private Dictionary<string, Dictionary<string, Action<object>>> _events = new Dictionary<string, Dictionary<string, Action<object>>>();

    public Action<ModuleInfo> OnModulesChanged;
    public Action<ModuleInfo> OnModuleStarted;
    public Action<ModuleInfo> OnModuleStopped;

    public string[] CoreModulesKeys => _coreModules.Keys.ToArray();
    public ModuleInfo[] CoreModuleValues => _coreModules.Values.ToArray();

    #region Listeners

    public void HookWindowSwitchEvent(Action<WindowSwitchedArgs> method)
    {
        WindowsWindowApi.Instance.OnWindowSwitched += method;
    }

    internal async Task<bool> DownloadModule(ModulesListViewModel.OnlineModuleListItem onlineModuleListItem)
    {
        string filePath = "";
        string fileName = "";
        try
        {
            using (var webClient = new WebClient())
            {
                //await Task.Run(() =>  webClient.DownloadDataAsync(new Uri(onlineModuleListItem.DownloadLink)));

                var data = webClient.DownloadData(onlineModuleListItem.DownloadLink);

                var cd = new ContentDisposition(webClient.ResponseHeaders["Content-Disposition"]);
                fileName = cd.FileName;
                filePath = Paths.APPDATA_C_DIRECTORY + Path.DirectorySeparatorChar + cd.FileName;

                // await Task.Run(() => webClient.DownloadFileAsync(new Uri(onlineModuleListItem.DownloadLink), filePath));

                webClient.DownloadFile(new Uri(onlineModuleListItem.DownloadLink), filePath);
            }
        }
        catch (Exception e)
        {
            MessageBox.Show("Cannot download module");
            return false;
        }

        try
        {
            //FileInfo file = new FileInfo(DesktopHelper.appdataCFOLDER_PATH + Path.DirectorySeparatorChar + cd.FileName);
            using (var zip = ZipFile.OpenRead(filePath))
            {
                if (zip.Entries.Count < 1)
                    return false;

                if (zip.Entries.Where(x =>
                {
                    return x.Name.ToLower().Contains(".dll");
                }).Select(x => x).Count() < 1)
                {
                    return false;
                }

                zip.ExtractToDirectory(Paths.MODULE_DIRECTORY);
            }
        }
        catch (Exception e)
        {
            MessageBox.Show("Cannot add module");
            return false;
        }

        return true;
    }

    internal void RemoveModule(string name)
    {
        if (_coreModules.ContainsKey(name))
        {
            ModuleInfo outVal = null;
            if (_coreModules.TryGetValue(name, out outVal))
            {
                outVal.Loader.Stop();
            }
            bool alive = outVal.AssemblyLoadContext.IsAlive;
            if (alive)
                ((PluginLoadContext)outVal.AssemblyLoadContext.Target).Unload();

            _coreModules.Remove(name);

            OnModuleStopped?.Invoke(outVal);

            outVal.Loader.Clear();
            outVal.AssemblyLoadContext = null;
            outVal.Loader = null;
            outVal = null;
        }

        GC.Collect();
        GC.WaitForPendingFinalizers();
    }

    public void RemoveModuleCatalog(string name)
    {
        var asss = AssemblyLoadContext.All;

        try
        {
            string pathToMod = Paths.MODULE_DIRECTORY + Path.DirectorySeparatorChar + name;
            string pathTOModDir = pathToMod + Path.DirectorySeparatorChar + "ActiveTimer.dll";
            if (Directory.Exists(pathToMod))
            {
                try
                {
                    using (var f = File.OpenRead(pathTOModDir))
                    {
                        f.Close();
                    }
                }
                catch (Exception es)
                {
                }

                File.Delete(pathToMod + Path.DirectorySeparatorChar + "ActiveTimer.dll");
                Directory.Delete(pathToMod, true);
            }
        }
        catch (UnauthorizedAccessException e) { }
    }

    internal bool IsModulePresent(string name)
    {
        foreach (var item in CoreModulesKeys)
        {
            if (item == name)
                return true;
        }
        return false;
    }

    internal bool IsModuleActive(string name)
    {
        foreach (var item in CoreModulesKeys)
        {
            if (item == name && _coreModules[item].Loader.IsStarted)
                return true;
        }
        return false;
    }

    private void AddModuleFromFile(FileInfo file)
    {
        if (IsModulePackedToZip(file))
        {
            if (!UnpackZipAsAModule(file))
            {
                MessageBox.Show("Cannot unpack module");
                return;
            }
        }
        else
        {
            file.CopyTo(Paths.MODULE_DIRECTORY);
        }
    }

    private bool UnpackZipAsAModule(FileInfo file)
    {
        using (var zip = ZipFile.OpenRead(file.FullName))
        {
            if (zip.Entries.Count < 1)
                return false;

            if (zip.Entries.Where(x =>
             {
                 return x.Name.ToLower().Contains(".dll");
             }).Select(x => x).Count() < 1)
            {
                return false;
            }

            return true;
        }
    }

    private bool IsModulePackedToZip(FileInfo file)
    {
        if (file.FullName.ToLower().Contains("7zip") || file.FullName.ToLower().Contains("zip") || file.FullName.ToLower().Contains("rar") || file.FullName.ToLower().Contains("7z"))
            return true;
        else return false;
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

    public void HookKeyboardReleaseEvent(Action<KeyReleasedArgs> method)
    {
        KeyboardListener.Instance.OnKeyReleased += method;
    }

    public void UnHookKeyboardReleasedEvent(Action<KeyReleasedArgs> method)
    {
        KeyboardListener.Instance.OnKeyReleased -= method;
    }

    #endregion Listeners

    public bool ScanDirectory(string path)
    {
        List<string> SpecificModuleDirectories = new List<string>();

        if (IsModulesDirectory(path))
        {
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
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
            return false;

        bool foundSomeModule = false;

        foreach (var dllDirectory in SpecificModuleDirectories)
        {
            string dllPath = Directory.GetFiles(dllDirectory, GetModuleNameFromPath(dllDirectory) + ".dll").FirstOrDefault();
            if (string.IsNullOrEmpty(dllPath))
                continue;

            try
            {
                RemoteLoader remoteLoader = new RemoteLoader();
                PluginLoadContext loadContext = new PluginLoadContext(dllPath);

                remoteLoader.Init(this, loadContext, dllPath);

                var mi = new ModuleInfo
                {
                    AssemblyLoadContext = new WeakReference(loadContext),
                    Loader = remoteLoader
                };

                _coreModules.Add(remoteLoader.Name, mi);

                OnModuleStarted?.Invoke(mi);
                foundSomeModule = true;
            }
            catch (InvalidCastException ce)
            {
            }
            catch (Exception e)
            {
                Debug.WriteLine("failed loading plugin " + dllDirectory + " " + e.Message);
            }
        }

        if (foundSomeModule) return true;
        else return false;
    }

    private string GetModuleNameFromPath(string path)
    {
        var pat = new DirectoryInfo(path).Name;
        return pat;
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

    public ObservableCollection<UserControl> GetSettingsViews()
    {
        ObservableCollection<UserControl> list = new ObservableCollection<UserControl>();
        foreach (var item in CoreModuleValues)
        {
            var v = item.Loader.Instance.GetSettingsUserControlView();
            if (v != null)
                list.Add(v);
        }

        if (list.Count > 0)
            return list;
        return null;
    }

    public class SavedActiveModules
    {
        public List<string> activatedList;
        public List<string> deactivatedList;
    }

    internal void LoadSavedActiveModules()
    {
        ModuleController.Instance.ScanDirectory(Paths.APP_DIRECTORY + Path.DirectorySeparatorChar + "Modules");

        var load = Saves.Load<SavedActiveModules>(Paths.APP_NAME, "SavedActiveModules");

        var listToActivate = new List<ModuleInfo>();

        foreach (var item in _coreModules)
        {
            listToActivate.Add(item.Value);
        }

        if (load != null)
            if (load.deactivatedList != null)
                if (load.deactivatedList.Count > 0)
                    foreach (var cm in _coreModules)
                    {
                        if (load.deactivatedList.Contains(cm.Key))
                        {
                            listToActivate.Remove(cm.Value);
                        }
                    }

        foreach (var item in listToActivate)
        {
            item.Loader.Start();
        }
    }

    internal ModuleInfo[] GetActiveModules()
    {
        return CoreModuleValues.Where(e => e.Loader.IsStarted).ToArray();

        //List<ModuleInfo> ret = new List<ModuleInfo>();
        //foreach (var item in CoreModuleValues)
        //{
        //    if (item.Loader.IsStarted)
        //        ret.Add(item);
        //}
        //return ret.ToArray();
    }

    internal void SaveActiveModulesNames()
    {
        var sam = new SavedActiveModules();
        sam.activatedList = new List<string>();
        foreach (var item in GetActiveModuleNames())
        {
            sam.activatedList.Add(item);
        }

        sam.deactivatedList = new List<string>();
        foreach (var item in GetInactiveModuleNames())
        {
            sam.deactivatedList.Add(item);
        }

        Saves.Save(Paths.APP_NAME, "SavedActiveModules", sam);
    }

    public void StartCoreModule(string name)
    {
        if (_coreModules.ContainsKey(name))
        {
            var p = _coreModules[name].Loader;
            if (!p.IsStarted)
            {
                p.Start();
                OnModuleStarted?.Invoke(_coreModules[name]);
            }
        }
    }

    public void SendMessage(string ModuleName, string message)
    {
        Debug.WriteLine(ModuleName + " " + message);
        if (_coreModules.ContainsKey(ModuleName))
        {
            var p = _coreModules[ModuleName].Loader;
            if (p.IsStarted)
            {
                p.ReceiveMessage(message);
            }
        }

        SendGlobalMessage(ModuleName + ":" + message);
    }

    public void SendGlobalMessage(string msg)
    {//todo refactor for messages/ listeners instead of simple values
        Share.SetValue(msg);
    }

    public void StopCoreModule(string name)
    {
        if (_coreModules.ContainsKey(name))
        {
            var p = _coreModules[name].Loader;
            if (p.IsStarted)
            {
                p.Stop();
                OnModuleStopped?.Invoke(_coreModules[name]);
            }
        }
    }

    internal void StopAllModules()
    {
        foreach (var item in CoreModulesKeys)
        {
            OnModuleStopped?.Invoke(_coreModules[item]);
            _coreModules[item].Loader.Stop();
        }
    }

    private void OnInteractableEntered()
    {
        foreach (var item in CoreModuleValues)
        {
            if (item.Loader.IsStarted)
                item.Loader.OnInteractableEntered();
        }
    }

    private void OnInteractableExited()
    {
        foreach (var item in CoreModuleValues)
        {
            if (item.Loader.IsStarted)
                item.Loader.OnInteractableExited();
        }
    }

    public void OnMinViewEntered()
    {
        foreach (var item in CoreModuleValues)
        {
            if (item.Loader.IsStarted)
                item.Loader.OnMinViewEntered();
        }
    }

    public void OnFullViewEntered()
    {
        foreach (var item in CoreModuleValues)
        {
            if (item.Loader.IsStarted)
                item.Loader.OnFullViewEntered();
        }
    }

    private List<string> GetInactiveModuleNames()
    {
        List<string> str = new List<string>();
        foreach (var item in _coreModules)
        {
            if (!item.Value.Loader.IsStarted)
            {
                str.Add(item.Key);
            }
        }

        return str;
    }

    public List<string> GetActiveModuleNames()
    {
        List<string> str = new List<string>();
        foreach (var item in _coreModules)
        {
            if (item.Value.Loader.IsStarted)
            {
                str.Add(item.Key);
            }
        }

        return str;
    }

    public bool SaveModuleInformation(string ModuleName, string saveFileName, object objectToSave)
    {
        return Saves.Save(ModuleName, saveFileName, objectToSave);
    }

    public T LoadModuleInformation<T>(string ModuleName, string saveFileName)
    {
        return Saves.Load<T>(ModuleName, saveFileName);
    }

    public void SubscribeToEvent(string ModuleName, string eventName, Action<object> action)
    {
        var suc = _events.TryGetValue(ModuleName, out Dictionary<string, Action<object>> result);
        if (!suc)
        {
            var nd = new Dictionary<string, Action<object>>();
            nd.Add(eventName, action);
            _events.Add(ModuleName, nd);
            return;
        }

        var eventSuc = result.TryGetValue(eventName, out Action<object> actions);
        if (!eventSuc)
        {
            var nd = new Dictionary<string, Action<object>>();
            nd.Add(eventName, action);
            result = nd;
            return;
        }

        actions += action;
    }

    public void UnsubscribeToEvent(string ModuleName, string eventName, Action<object> action)
    {
        var suc = _events.TryGetValue(ModuleName, out Dictionary<string, Action<object>> result);
        if (!suc)
        {
            return;
        }

        var eventSuc = result.TryGetValue(eventName, out Action<object> actions);
        if (!eventSuc)
        {
            _events.Remove(ModuleName);
            return;
        }

        if (actions == null)
        {
            result.Remove(eventName);
        }

        actions -= action;
    }

    public void OnEventTriggered(string ModuleName, string eventName, object data)
    {
        var suc = _events.TryGetValue(ModuleName, out Dictionary<string, Action<object>> result);
        if (!suc) return;

        for (int i = 0; i < result.Keys.Count; i++)
        {
            var eventSuc = result.TryGetValue(eventName, out Action<object> actions);
            if (!eventSuc)
                return;

            actions?.Invoke(data);
        }
    }
}