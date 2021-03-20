using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Net;
using System.Windows.Input;
using System.Xml.Serialization;

public class ModulesListViewModel : BaseViewModel
{
    #region Properties

    private ObservableCollection<ViewModuleListItem> _moduleListItems;
    public ObservableCollection<ViewModuleListItem> ModuleListItems
    {
        get
        {

            return _moduleListItems;
        }
        set
        {
            _moduleListItems = value;

            OnPropertyChanged(nameof(ModuleListItems));
        }
    }

    private OnlineModuleList cachedOnlineModuleList;

    #endregion

    #region Commands


    private ICommand _moduleButtonClicked;
    public ICommand ModuleButtonClicked
    {
        get
        {
            if (_moduleButtonClicked == null)
                _moduleButtonClicked = new RelayCommand(
                   (object o) =>
                   {

                       if (o is ViewModuleListItem)
                       {

                           var vmli = o as ViewModuleListItem;


                           if (vmli.ModuleButtonActionText.ToLower().Contains("get"))
                           {
                               GetModule(vmli);
                           }else if (vmli.ModuleButtonActionText.ToLower().Contains("remove"))
                           {
                               RemoveModule(vmli);
                           }


                       }

                   },
                   (object o) =>
                   {
                       return true;
                   });

            return _moduleButtonClicked;
        }
    }

    

    #endregion

    public ModulesListViewModel()
    {
        ShowListOfModules();

        ModuleController.Instance.OnModuleRemoved += OnModuleRemoved;
    }

    private void OnModuleRemoved(ModuleInfo mod)
    {
        ViewModuleListItem modToRemove = null;
        foreach (var vMod in ModuleListItems)
        {
            if(vMod.Name == mod.Loader.Name)
            {
                modToRemove = vMod;
            }
        }
        if (modToRemove != null)
        {
            ModuleListItems.Remove(modToRemove);
        }
    }

    private void RemoveModule(ViewModuleListItem vmli)
    {
        vmli.ModuleButtonActionText = "Removing... ";
        vmli.ModuleButtonActionEnabled = false;

        ModuleController.Instance.RemoveModule(vmli.Name);


    }

    private async void GetModule(ViewModuleListItem vmli)
    {
        vmli.ModuleButtonActionText = "Downloading... ";
        vmli.ModuleButtonActionEnabled = false;
        var res = await ModuleController.Instance.DownloadModule(new OnlineModuleListItem()
        {
            Name = vmli.Name,
            Description = vmli.Description,
            DownloadLink = vmli.DownloadLink
        });

        if (res)
        {
            ModuleController.Instance.ScanDirectory(DesktopHelper.moduleFolder + Path.DirectorySeparatorChar + vmli.Name);
        }




        vmli.ModuleButtonActionText = "Remove";
        vmli.ModuleButtonActionEnabled = true;
    }

    private void ShowListOfModules()
    {
        var res = GetModuleListOfModules();
        if (res == null)
            return;


        cachedOnlineModuleList = res;

        ObservableCollection<ViewModuleListItem> ret = new ObservableCollection<ViewModuleListItem>();

        foreach (var item in res.onlineModuleListItems)
        {
            ret.Add(new ViewModuleListItem()
            {
                Name = item.Name,
                Description = item.Description,
                DownloadLink = item.DownloadLink,
                ModuleButtonActionEnabled = true,
                ModuleButtonActionText = ModuleController.Instance.IsModuleActive(item.Name) ? "Remove" : "Get"
            });

        }
        if (ret.Count > 0)
            ModuleListItems = ret;

    }

    public OnlineModuleList GetModuleListOfModules()
    {
        WebClient webClient = new WebClient();

        try
        {
            var strXml = webClient.DownloadString("https://raw.githubusercontent.com/RobertJaskowski/Caravansary/master/OnlineModuleList");
            if (strXml == null)
            {
                return null;
            }

            Object rslt;


            var xs = new XmlSerializer(typeof(OnlineModuleList));

            using (TextReader reader = new StringReader(strXml))
            {
                rslt = (OnlineModuleList)xs.Deserialize(reader);
            }
            return (OnlineModuleList)rslt;

        }
        catch
        {
            return null;
        }
        finally
        {
            webClient.Dispose();
        }

    }

    public void SerializeModuleList(OnlineModuleList list)
    {
        Saves.Save("custom", "OnlineModuleList", list);
    }



    public class ViewModuleListItem : ObservableObject
    {
        private string _moduleName;
        public string Name
        {
            get { return _moduleName; }
            set
            {
                _moduleName = value;
                OnPropertyChanged(nameof(Name));
            }
        }
        private string _description;
        public string Description
        {
            get { return _description; }
            set
            {
                _description = value;
                OnPropertyChanged(nameof(Description));
            }
        }
        private string _downloadLink;
        public string DownloadLink
        {
            get { return _downloadLink; }
            set
            {
                _downloadLink = value;
                OnPropertyChanged(nameof(DownloadLink));
            }
        }

        private string _imageUrl;
        public string ImageUrl
        {
            get { return _imageUrl; }
            set
            {
                _imageUrl = value;
                OnPropertyChanged(nameof(ImageUrl));
            }
        }

        private bool _moduleButtonActionEnabled;
        public bool ModuleButtonActionEnabled
        {
            get
            {
                return _moduleButtonActionEnabled;
            }
            set
            {
                _moduleButtonActionEnabled = value;
                OnPropertyChanged(nameof(ModuleButtonActionEnabled));
            }
        }

        private string _moduleButtonActionText;
        public string ModuleButtonActionText
        {
            get
            {
                return _moduleButtonActionText;
            }
            set
            {
                _moduleButtonActionText = value;
                OnPropertyChanged(nameof(ModuleButtonActionText));
            }
        }
    }

    [Serializable]
    public class OnlineModuleListItem
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string DownloadLink { get; set; }
    }

    [Serializable]
    public class OnlineModuleList
    {
        public List<OnlineModuleListItem> onlineModuleListItems;
    }
}
