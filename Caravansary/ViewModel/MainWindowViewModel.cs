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
using Settings = Caravansary.Properties.Settings;
using System.Windows.Controls;
using Plugger.Contracts;

namespace Caravansary
{
   
    class MainWindowViewModel : BaseViewModel
    {
        #region Properties


        private bool _botBarEnabled;
        public bool BotBarEnabled
        {
            get
            {
                return _botBarEnabled;
            }
            set
            {
                _botBarEnabled = value;
            }

        }


        private double _progressBotBar;
        public double ProgressBotBar
        {
            get
            {
                return _progressBotBar;
            }
            set
            {
                _progressBotBar = value;
                OnPropertyChanged(nameof(ProgressBotBar));
            }
        }

        private double _backgroundTransparency;
        public double BackgroundTransparency
        {
            get
            {
                return _backgroundTransparency;
            }
            set
            {
                _backgroundTransparency = value;
                OnPropertyChanged(nameof(BackgroundTransparency));
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


        }

        internal void OnWindowClosing(object sender, CancelEventArgs e)
        {

            ModuleManager.Instance.CloseModules();

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


        //private ObservableCollection<CoreModule> _myCoreModules;

        //public ObservableCollection<CoreModule> CoreModules
        //{
        //    get { return _myCoreModules; }
        //    set
        //    {
        //        _myCoreModules = value;
        //        OnPropertyChanged(nameof(CoreModule));
        //    }
        //}

        private CoreModule _mod0;

        public CoreModule Mod0
        {
            get { return _mod0; }
            set
            {
                _mod0 = value;
                OnPropertyChanged(nameof(Mod0));
            }
        }

        private UserControl _mod1;
        public UserControl Mod1
        {
            get { return _mod1; }
            set
            {
                _mod1 = value;
                OnPropertyChanged(nameof(Mod1));
            }
        }


        private CoreModule _mod2;
        public CoreModule Mod2
        {
            get { return _mod2; }
            set
            {
                _mod2 = value;
                OnPropertyChanged(nameof(Mod2));
            }
        }


        private CoreModule _mod3;
        public CoreModule Mod3
        {
            get { return _mod3; }
            set
            {
                _mod3 = value;
                OnPropertyChanged(nameof(Mod3));
            }
        }


        private CoreModule _mod4;
        public CoreModule Mod4
        {
            get { return _mod4; }
            set
            {
                _mod4 = value;
                OnPropertyChanged(nameof(Mod4));
            }
        }


        private CoreModule _mod5;
        public CoreModule Mod5
        {
            get { return _mod5; }
            set
            {
                _mod5 = value;
                OnPropertyChanged(nameof(Mod5));
            }
        }


        private CoreModule _mod10;
        public CoreModule Mod10
        {
            get { return _mod10; }
            set
            {
                _mod10 = value;
                OnPropertyChanged(nameof(Mod10));
            }
        }
        #endregion

        //MilestoneSystem milestoneSystem;
        //NotificationSystem notificationSystem;

        public MainWindowViewModel(IntPtr handle, IWindow window)
        {
            Application.Current.Dispatcher.UnhandledException += OnDispatcherUnhandledException;
            this.CurrentWindow = window;
            CurrentHandleWindow = handle;
            trayIcon = new TrayIcon();

            //top
            //Mod0 = ModuleManager.Instance.GetCoreModule("MainBar");

            //Mod1 = ModuleManager.Instance.GetCoreModule("KeyCounter");
            //Mod2 = ModuleManager.Instance.GetCoreModule("Filler");
            //Mod3 = ModuleManager.Instance.GetCoreModule("ActiveTimer");
            //Mod4 = ModuleManager.Instance.GetCoreModule("Filler");
            //Mod5 = ModuleManager.Instance.GetCoreModule("DailyGoal");


            //Mod10 = ModuleManager.Instance.GetCoreModule("Roadmap");

            ModuleManager.Instance.InitializeModules();


            LoadView();


            LoadWindowSettings();




            //LoadDailyGoal();



            //  milestoneSystem = new MilestoneSystem(this);
            //   notificationSystem = new NotificationSystem(this);


        }

        static string mainApplicationDirectoryPath = Directory.GetParent(Assembly.GetExecutingAssembly().Location).ToString();

        public void LoadView()
        {
            try
            {
                //Configure path of PlugBoard folder to access all calculate libraries   
                string plugName = ConfigurationManager.AppSettings["Plugs"].ToString(); // ConfigurationSettings.AppSettings["Plugs"].ToString();


                var connectors = Directory.GetDirectories(mainApplicationDirectoryPath);
                foreach (var connect in connectors)
                {
                    string dllPath = GetPluggerDll(connect);
                    Assembly _Assembly = Assembly.LoadFile(dllPath);
                    var types = _Assembly.GetTypes()?.ToList();
                    var type = types?.Find(a => typeof(IPlugger).IsAssignableFrom(a));
                    var win = (IPlugger)Activator.CreateInstance(type);

                    Mod1 = win.GetPlugger();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Internal Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        private string GetPluggerDll(string connect)
        {
            var files = Directory.GetFiles(connect, "*.dll");
            foreach (var file in files)
            {
                if (FileVersionInfo.GetVersionInfo(file).ProductName.StartsWith("Calci"))
                    return file;
            }
            return string.Empty;
        }

        private void LoadWindowSettings()
        {

            Application.Current.MainWindow.ShowInTaskbar = Settings.Default.ShowInTaskbar;

            BackgroundTransparency = Settings.Default.BackgroundTransparency;

            CurrentWindow.GetAssociatedWindow.WindowStartupLocation = WindowStartupLocation.Manual;
            double posX = Settings.Default.MainWindowPositionX;
            double posY = Settings.Default.MainWindowPositionY;


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
            Settings.Default.MainWindowPositionX = x;
            Settings.Default.MainWindowPositionY = y;
            Settings.Default.Save();
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
}
