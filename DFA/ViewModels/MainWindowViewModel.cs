
namespace DFA
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Windows;
    using System.Windows.Input;
    using System.Windows.Threading;
    using Settings = Properties.Settings;

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
            _listener = new KeyboardListener();
            _listener.OnKeyPressed += _listener_OnKeyPressed;
            _listener.OnKeyReleased += _listener_OnKeyReleased;


            _listener.HookKeyboard();


        }

        internal void OnWindowClosing(object sender, CancelEventArgs e)
        {
            _listener.UnHookKeyboard();


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

        private CoreModule _mod1;

        public CoreModule Mod1
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



        public MainWindowViewModel(IntPtr handle, IWindow window)
        {
            Application.Current.Dispatcher.UnhandledException += OnDispatcherUnhandledException;
            this.CurrentWindow = window;
            CurrentHandleWindow = handle;
            trayIcon = new TrayIcon();

            Mod0 = ModuleManager.Instance.GetCoreModule("MainBar");
            Mod3 = ModuleManager.Instance.GetCoreModule("ActiveTimer");



            LoadWindowSettings();



            //LoadDailyGoal();




            //CreateMilestoneSystem();
            //CreateNotificationSystem();


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


        MilestoneSystem milestoneSystem;

        private void CreateMilestoneSystem()
        {
            //  milestoneSystem = new MilestoneSystem(this);

        }

        NotificationSystem notificationSystem;
        private void CreateNotificationSystem()
        {
            //   notificationSystem = new NotificationSystem(this);

        }



        public bool isMouseDown = false;
        public bool isLMouseDown = false;
        public bool isRMouseDown = false;

        public int mouseX;
        public int mouseY;
        public int mouseinX;
        public int mouseinY;



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




        #region KeyCombination


        private KeyboardListener _listener;

        private int _ctrlZCounterString = 0;
        public int CtrlZCounter
        {
            get
            {

                return _ctrlZCounterString;
            }
            set
            {

                _ctrlZCounterString = value;
                OnPropertyChanged(nameof(CtrlZCounter));
            }
        }

        private bool ZPressed = false;
        private bool CtrlPressed = false;

        private ICommand _ctrlZClicked;
        public ICommand CtrlZClicked
        {
            get
            {
                if (_ctrlZClicked == null)
                    _ctrlZClicked = new RelayCommand(
                       (object o) =>
                       {

                           Debug.WriteLine("ctrlz clicked");

                       },
                       (object o) =>
                       {
                           return true;
                       });

                return _ctrlZClicked;

            }
        }
        void _listener_OnKeyReleased(object sender, KeyReleasedArgs e)
        {
            if (e.KeyReleased == System.Windows.Input.Key.LeftCtrl)
            {
                CtrlPressed = false;

            }
            else if (e.KeyReleased == System.Windows.Input.Key.Z)
            {
                ZPressed = false;

            }


        }

        private void CheckForCtrlZCombination()
        {
            if (CtrlPressed && ZPressed)
            {
                CtrlZCounter++;
                Debug.WriteLine(CtrlZCounter);
                //label1.Content = ctrlZCounter;
            }
        }

        void _listener_OnKeyPressed(object sender, KeyPressedArgs e)
        {
            if (e.KeyPressed == System.Windows.Input.Key.LeftCtrl)
            {
                CtrlPressed = true;
                CheckForCtrlZCombination();
            }
            else if (e.KeyPressed == System.Windows.Input.Key.Z)
            {
                ZPressed = true;
                CheckForCtrlZCombination();
            }


        }

        #endregion



    }
}
