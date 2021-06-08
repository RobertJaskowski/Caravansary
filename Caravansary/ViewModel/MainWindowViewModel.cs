using Caravansary;
using Caravansary.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Markup;
using System.Windows.Threading;

public class MainWindowViewModel : BaseViewModel
{
    #region Properties

    private bool _interactable;

    public bool Interactable
    {
        get
        {
            return _interactable;
        }
        set
        {
            _interactable = value;
            OnPropertyChanged(nameof(Interactable));
            OnPropertyChanged(nameof(BackgroundTransparency));
        }
    }

    private bool _mouseOnWindow;

    public bool MouseOnWindow
    {
        get { return _mouseOnWindow; }
        set
        {
            _mouseOnWindow = value;
            OnPropertyChanged(nameof(MouseOnWindow));
        }
    }

    private bool _fullView;

    public bool FullView
    {
        get
        {
            return _fullView;
        }
        set
        {
            _fullView = value;
            OnPropertyChanged(nameof(FullView));
            if (value)
                ModuleController.Instance.OnFullViewEntered();
            else
                ModuleController.Instance.OnMinViewEntered();
        }
    }

    public bool ShowInTaskbar
    {
        get
        {
            return Data.MainWindowSettingsSave.ShowInTaskbar;
        }
        set
        {
            Data.MainWindowSettingsSave.ShowInTaskbar = value;
            OnPropertyChanged(nameof(ShowInTaskbar));
            OnPropertyChanged(nameof(Data.MainWindowSettingsSave));
            Data.SaveWindowSettings();
        }
    }

    public double BackgroundTransparency
    {
        get
        {
            if (Interactable)
                return 1;
            else
                return Data.MainWindowSettingsSave.BackgroundTransparency;
        }
        set
        {
            Data.MainWindowSettingsSave.BackgroundTransparency = value;
            OnPropertyChanged(nameof(BackgroundTransparency));
            OnPropertyChanged(nameof(Data.MainWindowSettingsSave));
            Data.SaveWindowSettings();
        }
    }

    public double MainWindowPositionX
    {
        get
        {
            return Data.MainWindowSettingsSave.MainWindowPositionX;
        }
        set
        {
            Data.MainWindowSettingsSave.MainWindowPositionX = value;
            OnPropertyChanged(nameof(MainWindowPositionX));
            OnPropertyChanged(nameof(Data.MainWindowSettingsSave));
            Data.SaveWindowSettings();
        }
    }

    public double MainWindowPositionY
    {
        get
        {
            return Data.MainWindowSettingsSave.MainWindowPositionY;
        }
        set
        {
            Data.MainWindowSettingsSave.MainWindowPositionY = value;
            OnPropertyChanged(nameof(MainWindowPositionY));
            Data.SaveWindowSettings();
        }
    }

    private Visibility _visibility = Visibility.Collapsed;

    public Visibility GetModulesButtonVisibility
    {
        get
        {
            return _visibility;
        }
        set
        {
            _visibility = value;
            OnPropertyChanged(nameof(GetModulesButtonVisibility));
        }
    }

    #endregion Properties

    #region Events

    internal void MouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.LeftButton == MouseButtonState.Pressed && e.RightButton == MouseButtonState.Pressed)
            CurrentWindow.GetAssociatedWindow.DragMove();
    }

    internal void MouseUp(object sender, MouseButtonEventArgs e)
    {
        if (e.LeftButton == MouseButtonState.Released && e.RightButton == MouseButtonState.Released)
            SaveWindowPosition();
    }

    internal void OnWindowLoaded(object sender, RoutedEventArgs e)
    {
        KeyboardListener.Instance.OnKeyPressed += OnKeyPressed;
        KeyboardListener.Instance.OnKeyReleased += OnKeyReleased;
    }

    internal void MouseEnter(object sender, MouseEventArgs e)
    {
        Debug.WriteLine("mouse enter");
        MouseOnWindow = true;
    }

    internal void MouseLeave(object sender, MouseEventArgs e)
    {
        Debug.WriteLine("mouse leave");
        MouseOnWindow = false;
    }

    private void OnKeyReleased(KeyReleasedArgs obj)
    {
    }

    private void OnKeyPressed(KeyPressedArgs obj)
    {
        if (obj.KeyPressed == Key.LeftCtrl)
        {
            Debug.WriteLine("interactible key pressed");
            if (!Interactable)
            {
                currentSecInteractible = 0;

                MakeWindowNonClickThrough();
                Interactable = true;
            }
        }

        if (obj.KeyPressed == Key.Space)
        {
            if (KeyboardListener.Instance.IsKeyPressed(Key.LeftCtrl))
            {
                FullView = !FullView;
            }
        }
    }

    internal void OnWindowClosing(object sender, CancelEventArgs e)
    {
        ModuleController.Instance.StopAllModules();
        KeyboardListener.Instance.UnHookKeyboard();
    }

    private static void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
    {
        System.Windows.MessageBox.Show("Unhandled exception occurred: \n" + e.Exception.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
    }

    #endregion Events

    #region Commands

    private ICommand _remDir;

    public ICommand RemoveDir
    {
        get
        {
            if (_remDir == null)
                _remDir = new RelayCommand(
                   (object o) =>
                   {
                       ModuleController.Instance.RemoveModuleCatalog("ActiveTimer");
                   },
                   (object o) =>
                   {
                       return true;
                   });

            return _remDir;
        }
    }

    private ICommand _getModulesClick;

    public ICommand GetModulesClick
    {
        get
        {
            if (_getModulesClick == null)
                _getModulesClick = new RelayCommand(
                   (object o) =>
                   {
                       ShowWindow();
                   },
                   (object o) =>
                   {
                       return true;
                   });

            return _getModulesClick;
        }
    }

    private void ShowWindow()
    {
        ModulesListWindow dialog = new();
        dialog.DataContext = new ModulesListViewModel();

        dialog.Show();
    }

    private ICommand _showSettings;

    public ICommand ShowSettings
    {
        get
        {
            if (_showSettings == null)
                _showSettings = new RelayCommand(
                   (object o) =>
                   {
                       SettingsWindow dialog = new SettingsWindow();
                       dialog.DataContext = new SettingsWindowViewModel();

                       bool? result = dialog.ShowDialog();
                   },
                   (object o) =>
                   {
                       return true;
                   });

            return _showSettings;
        }
    }

    private ICommand _quitApp;

    public ICommand QuitApp
    {
        get
        {
            if (_quitApp == null)
                _quitApp = new RelayCommand(
                   (object o) =>
                       {
                           Application.Current.Shutdown();
                       },
                   (object o) =>
                   {
                       return true;
                   });

            return _quitApp;
        }
    }

    #endregion Commands

    private IntPtr CurrentHandleWindow { get; set; }

    public IWindow CurrentWindow;
    private TrayIcon trayIcon;
    public int secInteractibleAfk = 3;
    public int currentSecInteractible = 0;

    #region mods

    private ObservableCollection<ViewModule> _topModules;

    public ObservableCollection<ViewModule> TopModules
    {
        get
        {
            if (_topModules == null)
            {
                _topModules = new ObservableCollection<ViewModule>();
            }

            return _topModules;
        }
        set
        {
            _topModules = value;

            OnPropertyChanged(nameof(TopModules));
        }
    }

    private ObservableCollection<ViewModule> _viewCoreModules;

    public ObservableCollection<ViewModule> ViewCoreModules
    {
        get
        {
            if (_viewCoreModules == null)
            {
                _viewCoreModules = new ObservableCollection<ViewModule>();
            }

            return _viewCoreModules;
        }
        set
        {
            _viewCoreModules = value;

            OnPropertyChanged(nameof(ViewCoreModules));
        }
    }

    private ObservableCollection<ViewModule> _botModules;

    public ObservableCollection<ViewModule> BotModules
    {
        get
        {
            if (_botModules == null)
            {
                _botModules = new ObservableCollection<ViewModule>();
            }

            return _botModules;
        }
        set
        {
            _botModules = value;

            OnPropertyChanged(nameof(BotModules));
        }
    }

    //private ObservableCollection<UserControl> _topModules;
    //public ObservableCollection<UserControl> TopModules
    //{
    //    get
    //    {
    //        if (_topModules == null)
    //        {
    //            _topModules = new ObservableCollection<UserControl>();

    //        }

    //        return _topModules;
    //    }
    //    set
    //    {
    //        _topModules = value;

    //        OnPropertyChanged(nameof(TopModules));
    //    }
    //}

    //private ObservableCollection<UserControl> _viewCoreModules;
    //public ObservableCollection<UserControl> ViewCoreModules
    //{
    //    get
    //    {
    //        if (_viewCoreModules == null)
    //        {
    //            _viewCoreModules = new ObservableCollection<UserControl>();

    //        }

    //        return _viewCoreModules;
    //    }
    //    set
    //    {
    //        _viewCoreModules = value;

    //        OnPropertyChanged(nameof(ViewCoreModules));
    //    }
    //}

    //private ObservableCollection<UserControl> _botModules;
    //public ObservableCollection<UserControl> BotModules
    //{
    //    get
    //    {
    //        if (_botModules == null)
    //        {
    //            _botModules = new ObservableCollection<UserControl>();

    //        }

    //        return _botModules;
    //    }
    //    set
    //    {
    //        _botModules = value;

    //        OnPropertyChanged(nameof(BotModules));
    //    }
    //}

    public class ViewModule : ObservableObject
    {
        private string _name;

        public string Name
        {
            get { return _name; }
            set { _name = value; OnPropertyChanged(nameof(Name)); }
        }

        private UserControl _userControlView;

        public UserControl UserControlView
        {
            get { return _userControlView; }
            set { _userControlView = value; OnPropertyChanged(nameof(UserControlView)); }
        }
    }

    #endregion mods

    public MainWindowViewModel(IntPtr handle, IWindow window)
    {
        Application.Current.ShutdownMode = ShutdownMode.OnExplicitShutdown;
        this.CurrentWindow = window;
        CurrentHandleWindow = handle;
        trayIcon = new TrayIcon(Data.MainWindowSettingsSave);

        Share.Start();

        Debug.WriteLine("finished setting server");

        LoadWindowSettings();

        Interactable = true;
        FullView = false;

        DispatcherTimer interactibleTimer = new DispatcherTimer();
        interactibleTimer.Tick += InteractableTick;
        interactibleTimer.Interval = TimeSpan.FromSeconds(1);
        interactibleTimer.Start();

        ModuleController.Instance.OnMinViewEntered();
    }

    private void InteractableTick(object sender, EventArgs e)
    {
        if (MouseOnWindow || FullView)
        {
            currentSecInteractible = 0;
            return;
        }
        currentSecInteractible++;

        if (currentSecInteractible < secInteractibleAfk)
            return;

        if (Interactable)
        {
            MakeWindowClickThrough();
            Interactable = false;
        }
    }

    private void MakeWindowNonClickThrough()
    {
        //var buttonHwndSource = (HwndSource)HwndSource.FromVisual(btn);
        //var buttonHwnd = buttonHwndSource.Handle;
        var windowHwnd = new WindowInteropHelper(CurrentWindow.GetAssociatedWindow).Handle;
        WinApi.SetWindowExNotTransparent(windowHwnd);
    }

    private void MakeWindowClickThrough()
    {
        var windowHwnd = new WindowInteropHelper(CurrentWindow.GetAssociatedWindow).Handle;

        WinApi.SetWindowExTransparent(windowHwnd);
    }

    public void InitModuleController()
    {
        ModuleController.Instance.LoadSavedActiveModules();

        HandleGetModulesButtonVisibility();

        InjectModule(ModuleController.Instance.GetActiveModules());

        ModuleController.Instance.OnModuleStarted += OnModAdded;
        ModuleController.Instance.OnModuleStopped += OnModuleRemoved;
    }

    private void OnModuleRemoved(ModuleInfo mod)
    {
        RemoveViewModFromCollection(TopModules, mod);
        RemoveViewModFromCollection(ViewCoreModules, mod);
        RemoveViewModFromCollection(BotModules, mod);
    }

    private void RemoveViewModFromCollection(ObservableCollection<ViewModule> collection, ModuleInfo mod)
    {
        List<ViewModule> toRemove = new List<ViewModule>();

        foreach (var item in collection)
        {
            if (item.Name == mod.Loader.Name)
            {
                toRemove.Add(item);
            }
        }
        if (toRemove.Count > 0)
            for (int i = 0; i < toRemove.Count; i++)
            {
                collection.Remove(toRemove[i]);
            }
    }

    private void OnModAdded(ModuleInfo mod)
    {
        InjectModule(mod);

        HandleGetModulesButtonVisibility();
    }

    private void InjectModule(ModuleInfo mod)
    {
        mod.Loader.Start();

        switch (mod.Loader.Instance.GetModulePosition())
        {
            case ModulePosition.TOP:
                AddViewToViewCollection(TopModules, mod);
                break;

            case ModulePosition.MID:
                AddViewToViewCollection(ViewCoreModules, mod);
                break;

            case ModulePosition.BOT:
                AddViewToViewCollection(BotModules, mod);
                break;
        }

        InitViewModeMinFull(mod);
    }

    private void AddViewToViewCollection(ObservableCollection<ViewModule> collection, ModuleInfo mod)
    {
        collection.Add(
                    new ViewModule()
                    {
                        Name = mod.Loader.Name,
                        UserControlView = mod.Loader.Instance.GetModuleUserControlView()
                    }
                    );
    }

    private void InjectModule(ModuleInfo[] coreModuleValues)
    {
        foreach (var mod in coreModuleValues)
        {
            switch (mod.Loader.Instance.GetModulePosition())
            {
                case ModulePosition.TOP:
                    AddViewToViewCollection(TopModules, mod);
                    //TopModules.Add(mod.Loader.Instance.GetModuleUserControlView());

                    break;

                case ModulePosition.MID:
                    //ViewCoreModules.Add(mod.Loader.Instance.GetModuleUserControlView());

                    AddViewToViewCollection(ViewCoreModules, mod);

                    break;

                case ModulePosition.BOT:
                    //BotModules.Add(mod.Loader.Instance.GetModuleUserControlView());
                    AddViewToViewCollection(BotModules, mod);

                    break;
            }
            InitViewModeMinFull(mod);
        }
    }

    private void InitViewModeMinFull(ModuleInfo mod)
    {
        if (!FullView)
            mod.Loader.OnMinViewEntered();
        else
            mod.Loader.OnFullViewEntered();
    }

    private void HandleGetModulesButtonVisibility()
    {
        GetModulesButtonVisibility = ModuleController.Instance.CoreModulesKeys.Count() < 1 ? Visibility.Visible : Visibility.Collapsed;
    }

    private DataTemplate CreateTemplate(Type viewModelType, Type viewType)
    {
        const string xamlTemplate = "<DataTemplate DataType=\"{{x:Type vm:{0}}}\"><v:{1} /></DataTemplate>";
        var xaml = String.Format(xamlTemplate, viewModelType.Name, viewType.Name, viewModelType.Namespace, viewType.Namespace);

        var context = new ParserContext();

        context.XamlTypeMapper = new XamlTypeMapper(new string[0]);
        context.XamlTypeMapper.AddMappingProcessingInstruction("vm", viewModelType.Namespace, viewModelType.Assembly.FullName);
        context.XamlTypeMapper.AddMappingProcessingInstruction("v", viewType.Namespace, viewType.Assembly.FullName);

        context.XmlnsDictionary.Add("", "http://schemas.microsoft.com/winfx/2006/xaml/presentation");
        context.XmlnsDictionary.Add("x", "http://schemas.microsoft.com/winfx/2006/xaml");
        context.XmlnsDictionary.Add("vm", "vm");
        context.XmlnsDictionary.Add("v", "v");

        var template = (DataTemplate)XamlReader.Parse(xaml, context);
        return template;
    }

    private void LoadWindowSettings()
    {
        Application.Current.MainWindow.ShowInTaskbar = ShowInTaskbar;

        CurrentWindow.GetAssociatedWindow.WindowStartupLocation = WindowStartupLocation.Manual;
        double posX = MainWindowPositionX;
        double posY = MainWindowPositionY;

        if (posX != 0 || posY != 0)
        {
            WindowHelper.SetWindowPosition(CurrentWindow.GetAssociatedWindow, posX, posY);
        }
        else
        {
            WindowHelper.ResetWindowPosition(CurrentWindow.GetAssociatedWindow);
        }
    }

    private void SaveWindowPosition()
    {
        int x = (int)CurrentWindow.GetAssociatedWindow.Left;
        int y = (int)CurrentWindow.GetAssociatedWindow.Top;
        MainWindowPositionX = x;
        MainWindowPositionY = y;
    }

    //private void StartAnimatingBottomBar()
    //{
    //    var procentFilledAlready = Utils.ToProcentage(progressBarBottomMost.Value, progressBarBottomMost.Minimum, progressBarBottomMost.Maximum);

    //    var valueOfTimeFilled = Utils.ProcentToValue(procentFilledAlready, timeSecToFillBotBar);

    //    var timeLeft = timeSecToFillBotBar - valueOfTimeFilled;

    //    DoubleAnimation animation = new DoubleAnimation(progressBarBottomMost.Maximum, TimeSpan.FromSeconds(timeLeft));
    //    animation.Completed += BottomBarCompleted;

    //    progressBarBottomMost.BeginAnimation(ProgressBar.ValueProperty, animation);
    //}
}