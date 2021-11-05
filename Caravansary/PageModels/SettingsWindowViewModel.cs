using Caravansary;
using Caravansary.Core;
using Caravansary.SDK;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

class SettingsWindowViewModel : BasePupupWindowPageModel
{

    #region Properties
    public string _appVersion;
    public string AppVersion
    {
        get => string.IsNullOrEmpty(_appVersion) ? Data.Version.ToString() : _appVersion;
        set
        {
            _appVersion = value;
            OnPropertyChanged(nameof(AppVersion));
        }
    }

    public double BackgroundTransparency
    {
        get => Data.MainWindowSettingsSave.BackgroundTransparency;
        set
        {
            mainWindow.Background.Opacity = value;

            Data.MainWindowSettingsSave.BackgroundTransparency = value;
            Data.SaveWindowSettings();
            OnPropertyChanged(nameof(BackgroundTransparency));
        }
    }


    private TrulyObservableCollection<ViewModuleSettings> _moduleSettingsControls;
    private readonly MainWindow mainWindow;
    private readonly ModuleController moduleController;

    public TrulyObservableCollection<ViewModuleSettings> ModuleSettingsControls
    {
        get
        {
            if (_moduleSettingsControls == null)
            {
                _moduleSettingsControls = new TrulyObservableCollection<ViewModuleSettings>();

                foreach (var item in moduleController.CoreModuleValues)
                {

                    var v = item.Loader.Instance.GetSettingsUserControlView();
                    if (v != null)
                    {

                        _moduleSettingsControls.Add(new ViewModuleSettings
                        {
                            ModuleName = item.Loader.Instance.GetModuleName(),
                            View = item.Loader.Instance.GetSettingsUserControlView()
                        });
                    }
                }

            }

            return _moduleSettingsControls;
        }
        set
        {
            _moduleSettingsControls = value;

            OnPropertyChanged(nameof(ModuleSettingsControls));
        }
    }

    public class ViewModuleSettings : ObservableObject
    {
        private UserControl _view;
        public UserControl View
        {
            get { return _view; }
            set
            {
                _view = value;
                OnPropertyChanged(nameof(View));
            }
        }

        private string _moduleName;
        public string ModuleName
        {
            get { return _moduleName; }
            set
            {
                _moduleName = value;
                OnPropertyChanged(nameof(ModuleName));
            }
            
        }
    }




    #endregion



    public SettingsWindowViewModel(MainWindow mainWindow, IModuleController moduleController)
    {
        this.mainWindow = mainWindow;
        this.moduleController = (ModuleController)moduleController;
    }

    private void InjectSettingsFromModules()
    {
        _moduleSettingsControls = new TrulyObservableCollection<ViewModuleSettings>();

        foreach (var item in moduleController.CoreModuleValues)
        {

            var v = item.Loader.Instance.GetSettingsUserControlView();
            if (v != null)
            {

                _moduleSettingsControls.Add(new ViewModuleSettings
                {
                    ModuleName = item.Loader.Instance.GetModuleName(),
                    View = item.Loader.Instance.GetSettingsUserControlView()
                });
            }
        }
    }

    public override Task<bool> InitializeAsync(object navigationdata = null)
    {
        InjectSettingsFromModules();

        return base.InitializeAsync(navigationdata);
    }
}
