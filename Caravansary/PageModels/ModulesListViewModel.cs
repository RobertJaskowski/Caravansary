using Caravansary.SDK;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Windows.Input;
using System.Xml.Serialization;

public class ModulesListViewModel : BasePupupWindowPageModel
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

    private ICommand _removeModuleButton;
    public ICommand RemoveModuleButton
    {
        get
        {
            if (_removeModuleButton == null)
                _removeModuleButton = new RelayCommand(
                   (object o) =>
                   {
                       Process.Start("explorer.exe", Paths.MODULE_DIRECTORY);

                   },
                   (object o) =>
                   {
                       return Directory.Exists(Paths.MODULE_DIRECTORY);
                   });

            return _removeModuleButton;
        }
    }

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
                           }
                           else if (vmli.ModuleButtonActionText.ToLower().Contains("deactivate"))
                           {
                               DeactivateModule(vmli);
                           }
                           else if (vmli.ModuleButtonActionText.ToLower().Contains("activate"))
                           {
                               ActivateModule(vmli);
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

    private void ActivateModule(ViewModuleListItem vmli)
    {
        vmli.ModuleButtonActionText = "Activating... ";
        vmli.ModuleButtonActionEnabled = false;

        ModuleController.Instance.StartCoreModule(vmli.Name);
        ModuleController.Instance.SaveActiveModulesNames();


        vmli.ModuleButtonActionText = "Deactivate";
        vmli.ModuleButtonActionEnabled = true;
    }

    private void DeactivateModule(ViewModuleListItem vmli)
    {
        vmli.ModuleButtonActionText = "Deactivating... ";
        vmli.ModuleButtonActionEnabled = false;


        ModuleController.Instance.StopCoreModule(vmli.Name);
        ModuleController.Instance.SaveActiveModulesNames();


        vmli.ModuleButtonActionText = "Activate";
        vmli.ModuleButtonActionEnabled = true;
    }



    #endregion

    public ModulesListViewModel()
    {
        ShowListOfModules();

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
            ModuleController.Instance.ScanDirectory(Paths.MODULE_DIRECTORY + Path.DirectorySeparatorChar + vmli.Name);
        }




        vmli.ModuleButtonActionText = "Deactivate";
        vmli.ModuleButtonActionEnabled = true;
    }

    private void ShowListOfModules()
    {



        var res = GetOnlineListOfModules();
        if (res != null)
            cachedOnlineModuleList = res;

        ObservableCollection<ViewModuleListItem> showedModules = new ObservableCollection<ViewModuleListItem>();

        foreach (var item in res.onlineModuleListItems)
        {
            showedModules.Add(new ViewModuleListItem()
            {
                Name = item.Name,
                Description = item.Description,
                DownloadLink = item.DownloadLink,
                ModuleButtonActionEnabled = true,
                ModuleButtonActionText = ModuleController.Instance.IsModulePresent(item.Name) ? (ModuleController.Instance.IsModuleActive(item.Name) ? "Deactivate" : "Activate") : "Get"
            });

        }

        foreach (var item in ModuleController.Instance.CoreModuleValues)
        {
            bool found = false;
            for (int i = 0; i < showedModules.Count; i++)
            {
                ViewModuleListItem retI = showedModules[i];
                if (retI.Name == item.Loader.Name)
                {
                    found = true;
                    break;
                }
            }

            if (!found)
            {
                showedModules.Add(new ViewModuleListItem()
                {
                    Name = item.Loader.Name,
                    Description = "No description",
                    DownloadLink = "",
                    ModuleButtonActionEnabled = true,
                    ModuleButtonActionText = (ModuleController.Instance.IsModuleActive(item.Loader.Name) ? "Deactivate" : "Activate")
                });
            }
        } 




        if (showedModules.Count > 0)
            ModuleListItems = showedModules;

    }

    public OnlineModuleList GetOnlineListOfModules()
    {
        WebClient webClient = new WebClient();

        try
        {
            var strJson = webClient.DownloadString("https://raw.githubusercontent.com/RobertJaskowski/Caravansary/master/OnlineModuleList.json");
            if (strJson == null)
            {
                return null;
            }

            Object rslt = JsonConvert.DeserializeObject<OnlineModuleList>(strJson);

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
