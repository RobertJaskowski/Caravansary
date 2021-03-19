using Caravansary.Core;
using System;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using System.Windows.Controls;
using System.Resources;
using System.Collections;
using System.Windows.Markup;
using System.Collections.ObjectModel;
using Caravansary;

public class MainWindowViewModel : BaseViewModel
{

    #region Properties

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








    #endregion

    #region events
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

    private void OnKeyReleased(KeyReleasedArgs obj)
    {

    }

    private void OnKeyPressed(KeyPressedArgs obj)
    {

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

    #endregion


    #region Commands


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


    #endregion


    private IntPtr CurrentHandleWindow { get; set; }




    public IWindow CurrentWindow;
    TrayIcon trayIcon;



    #region mods
    private ObservableCollection<UserControl> _topModules;
    public ObservableCollection<UserControl> TopModules
    {
        get
        {
            if (_topModules == null)
            {
                _topModules = new ObservableCollection<UserControl>();

            }

            return _topModules;
        }
        set
        {
            _topModules = value;

            OnPropertyChanged(nameof(TopModules));
        }
    }


    private ObservableCollection<UserControl> _viewCoreModules;
    public ObservableCollection<UserControl> ViewCoreModules
    {
        get
        {
            if (_viewCoreModules == null)
            {
                _viewCoreModules = new ObservableCollection<UserControl>();

            }

            return _viewCoreModules;
        }
        set
        {
            _viewCoreModules = value;

            OnPropertyChanged(nameof(ViewCoreModules));
        }
    }

    private ObservableCollection<UserControl> _botModules;
    public ObservableCollection<UserControl> BotModules
    {
        get
        {
            if (_botModules == null)
            {
                _botModules = new ObservableCollection<UserControl>();

            }

            return _botModules;
        }
        set
        {
            _botModules = value;

            OnPropertyChanged(nameof(BotModules));
        }
    }

    #endregion


    public MainWindowViewModel(IntPtr handle, IWindow window)
    {

        Application.Current.ShutdownMode = ShutdownMode.OnExplicitShutdown;
        this.CurrentWindow = window;
        CurrentHandleWindow = handle;
        trayIcon = new TrayIcon(Data.MainWindowSettingsSave);



        LoadWindowSettings();




        //LoadDailyGoal();






    }

    public void InitModuleController()
    {
        ModuleController.Instance.ScanDirectory(DesktopHelper.mainApplicationDirectoryPath + Path.DirectorySeparatorChar + "Modules");

        foreach (var mod in ModuleController.Instance.CoreModuleValues)
        {
            mod.Loader.Start();

            switch (mod.Loader.Instance.GetModulePosition())
            {
                case ModulePosition.TOP:
                    TopModules.Add(mod.Loader.Instance.GetModuleUserControlView());

                    break;
                case ModulePosition.MID:
                    ViewCoreModules.Add(mod.Loader.Instance.GetModuleUserControlView());


                    break;
                case ModulePosition.BOT:
                    BotModules.Add(mod.Loader.Instance.GetModuleUserControlView());

                    break;
            }
        }

        //foreach (var modInfo in ModuleController.Instance.CoreModuleValues)
        //{

        //    modInfo.Loader.Start();

        //    switch (modInfo.Loader.Instance.GetModulePosition())
        //    {
        //        case ModulePosition.TOP:
        //            TopModules.Add(modInfo.Loader.View);
        //            break;
        //        case ModulePosition.MID:
        //            ViewCoreModules.Add(modInfo.Loader.View);
        //            break;
        //        case ModulePosition.BOT:
        //            BotModules.Add(modInfo.Loader.View);
        //            break;
        //    }
        //}

    }


    DataTemplate CreateTemplate(Type viewModelType, Type viewType)
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

