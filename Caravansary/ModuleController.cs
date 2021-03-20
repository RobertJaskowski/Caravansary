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
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

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



    internal async Task<bool> DownloadModule(ModulesListViewModel.OnlineModuleListItem onlineModuleListItem)
    {


        string filePath = "";
        string fileName = "";
        try
        {
            using (var webClient = new WebClient())
            {


                // Try to extract the filename from the Content-Disposition header

                //await Task.Run(() =>  webClient.DownloadDataAsync(new Uri(onlineModuleListItem.DownloadLink)));

                var data = webClient.DownloadData(onlineModuleListItem.DownloadLink);


                var cd = new ContentDisposition(webClient.ResponseHeaders["Content-Disposition"]);
                fileName = cd.FileName;
                filePath = DesktopHelper.appdataCFOLDER_PATH + Path.DirectorySeparatorChar + cd.FileName;


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


                zip.ExtractToDirectory(DesktopHelper.moduleFolder);

            }

        }
        catch (Exception e)
        {
            MessageBox.Show("Cannot add module");
            return false;
        }




        return true;

    }

    internal bool IsModuleActive(string name)
    {
        foreach (var item in CoreModulesKeys)
        {
            if (item == name)
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
            file.CopyTo(DesktopHelper.moduleFolder);
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

    public void HookKeyboardReleaseEvent(Action<KeyPressedArgs> method)
    {
        KeyboardListener.Instance.OnKeyPressed += method;
    }

    public void UnHookKeyboardReleasedEvent(Action<KeyPressedArgs> method)
    {
        KeyboardListener.Instance.OnKeyPressed -= method;
    }

    #endregion

    public bool ScanDirectory(string path)
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
            return false;

        bool foundSomeModule = false;


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

                var mi = new ModuleInfo
                {
                    AssemblyLoadContext = loadContext,
                    Loader = remoteLoader
                };

                _coreModules.Add(remoteLoader.Name, mi);

                OnModuleAdded?.Invoke(mi);

            }
            catch (Exception e)
            {
                Debug.WriteLine("failed loading plugin " + dllDirectory + " " + e.Message);
            }




        }

        if (foundSomeModule) return true;
        else return false;

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

    public void InjectView(UserControl userControlView)
    {

    }
}
