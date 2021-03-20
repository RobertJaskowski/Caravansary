using Caravansary.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Xml;
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

    
    private ICommand _getModulesClick;
    public ICommand GetModuleClick
    {
        get
        {
            if (_getModulesClick == null)
                _getModulesClick = new RelayCommand(
                   (object o) =>
                   {

                       if(o is ViewModuleListItem)
                       {

                           var vmli = o as ViewModuleListItem;
                           GetModule(vmli);


                       }

                   },
                   (object o) =>
                   {
                       return true;
                   });

            return _getModulesClick;
        }
    }

    private async void GetModule(ViewModuleListItem vmli)
    {
        vmli.Description += "Downloading";
        await ModuleController.Instance.DownloadModule(new Uri(vmli.DownloadLink));

        vmli.Description.Replace("Downloading", "");


    }

    #endregion

    public ModulesListViewModel()
    {
        ShowListOfModules();
    }

    

    private void ShowListOfModules()
    {
        var res = GetModuleListOfModules();
        if (res == null)
        {
            //var oml = new OnlineModuleList()
            //{

            //    onlineModuleListItems = new List<OnlineModuleListItem>()

            //};
            //oml.onlineModuleListItems.Add(new OnlineModuleListItem()
            //{
            //    ModuleName = "testName",
            //    Description = "testDes"
            //});
            //oml.onlineModuleListItems.Add(new OnlineModuleListItem()
            //{
            //    ModuleName = "testName2",
            //    Description = "testDes2"
            //});

            //SerializeModuleList(oml);
           
            return;
        }

        cachedOnlineModuleList = res;

        ObservableCollection<ViewModuleListItem> ret = new ObservableCollection<ViewModuleListItem>();

        foreach (var item in res.onlineModuleListItems)
        {
            ret.Add(new ViewModuleListItem()
            {
                ModuleName = item.ModuleName,
                Description = item.Description,
                DownloadLink = item.DownloadLink
            }) ;

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




    public class ViewModuleListItem
    {
        public string ModuleName { get; set; }
        public string Description { get; set; }

        public string DownloadLink { get; set; }
        
        public string ImageUrl { get; set; }

    }

    [Serializable]
    public class OnlineModuleListItem
    {
        public string ModuleName { get; set; }
        public string Description { get; set; }
        public string DownloadLink { get; set; }
    }

    [Serializable]
    public class OnlineModuleList
    {
        public List<OnlineModuleListItem> onlineModuleListItems;
    }
}
