
namespace DFA
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Windows;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Threading;
    using Settings = Properties.Settings;

    class MainWindowViewModel : BaseViewModel
    {
        #region properties

        private ArtistModel _artistModel;
        public ArtistModel Artist
        {
            get
            {
                return _artistModel;
            }
            set
            {
                _artistModel = value;
            }

        }

        private string _artistTimeString;
        public String ArtistTimeString
        {
            get
            {
                return _artistTimeString;
            }
            set
            {
                switch (Artist.ArtistState)
                {
                    case ArtistState.ACTIVE:
                        _artistTimeString = value;
                        break;
                    case ArtistState.INACTIVE:
                        _artistTimeString = value;
                        break;
                    case ArtistState.PAUSED:
                        _artistTimeString = "|| " + value;
                        break;
                }
                OnPropertyChanged(nameof(ArtistTimeString));
            }
        }

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

        private Color _topBarStateColor;
        public Color TopBarStateColor
        {
            get
            {

                return _topBarStateColor;
            }
            set
            {
                _topBarStateColor = value;
                OnPropertyChanged("TopBarStateColor");
            }
        }

        private double _progressTopBar;
        public double ProgressTopBar
        {
            get
            {
                return _progressTopBar;
            }
            set
            {
                _progressTopBar = value;
                OnPropertyChanged(nameof(ProgressTopBar));
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


        private ICommand _activeTimeUpdate;
        public ICommand ActiveTimeUpdate1Sec
        {
            get
            {
                if (_activeTimeUpdate == null)
                    _activeTimeUpdate = new RelayCommand(
                       (object o) =>
                       {
                           Artist.ActiveTime += TimeSpan.FromSeconds(1);

                           ArtistTimeString = Artist.ActiveTime.ToString();
                       },
                       (object o) =>
                       {
                           return true;
                       });

                return _activeTimeUpdate;

            }
        }

        private ICommand _activeTimeClicked;
        public ICommand ActiveTimeClicked
        {
            get
            {
                if (_activeTimeClicked == null)
                    _activeTimeClicked = new RelayCommand(
                       (object o) =>
                       {
                           if (Artist.ArtistState == ArtistState.PAUSED)
                           {
                               ArtistActivate.Execute(null);
                           }
                           else
                               ArtistPause.Execute(null);

                       },
                       (object o) =>
                       {
                           return true;
                       });

                return _activeTimeClicked;

            }
        }

        private ICommand _artistActivate;
        public ICommand ArtistActivate
        {
            get
            {
                if (_artistActivate == null)
                    _artistActivate = new RelayCommand(
                       (object o) =>
                       {
                           Artist.ArtistState = ArtistState.ACTIVE;

                           Color c = Color.FromArgb(255, 178, 255, 89);
                           TopBarStateColor = c;

                           //  StartAnimatingBottomBar();


                           //timeSecToFillTopBar = 20;
                       },
                       (object o) =>
                       {
                           return true;
                       });

                return _artistActivate;

            }
        }

        private ICommand _artistDeactivate;
        public ICommand ArtistDeactivate
        {
            get
            {
                if (_artistDeactivate == null)
                    _artistDeactivate = new RelayCommand(
                       (object o) =>
                       {

                           Artist.ArtistState = ArtistState.INACTIVE;
                           ArtistTimeString = Artist.ActiveTime.ToString();


                           Color c = Color.FromArgb(255, 221, 44, 0);

                           TopBarStateColor = c;


                           //   progressBarBottomMost.StopAnimation();

                       },
                       (object o) =>
                       {
                           return true;
                       });

                return _artistDeactivate;

            }
        }
        private ICommand _artistPause;
        public ICommand ArtistPause
        {
            get
            {
                if (_artistPause == null)
                    _artistPause = new RelayCommand(
                       (object o) =>
                       {

                           Artist.ArtistState = ArtistState.PAUSED;
                           ArtistTimeString = Artist.ActiveTime.ToString();


                           Color c = Color.FromArgb(255, 221, 44, 0);

                           TopBarStateColor = c;


                           //   progressBarBottomMost.StopAnimation();

                       },
                       (object o) =>
                       {
                           return true;
                       });

                return _artistPause;

            }
        }

        #endregion


        private IntPtr CurrentHandleWindow { get; set; }




        public IWindow CurrentWindow;
        TrayIcon trayIcon;

        ArtistTimeController artistTimeController;
        public MainWindowViewModel(IntPtr handle, IWindow window)
        {
            Application.Current.Dispatcher.UnhandledException += OnDispatcherUnhandledException;
            this.CurrentWindow = window;
            CurrentHandleWindow = handle;
            _artistModel = new ArtistModel(TimeSpan.FromSeconds(0));

            trayIcon = new TrayIcon();
            trayIcon.CreateTrayMenu();



            LoadSettings();

            artistTimeController = new ArtistTimeController(this);



            CreateMilestoneSystem();
            CreateNotificationSystem();


        }


        private void LoadSettings()
        {

            Application.Current.MainWindow.ShowInTaskbar = Settings.Default.ShowInTaskbar;

            BackgroundTransparency = Settings.Default.BackgroundTransparency;

            CurrentWindow.GetAssociatedWindow.WindowStartupLocation = WindowStartupLocation.Manual;
            double posX = Settings.Default.MainWindowPositionX;
            double posY = Settings.Default.MainWindowPositionY;


            if (posX != 0 || posY != 0)
            {

                WindowHelper.SetWindowPosition(CurrentWindow.GetAssociatedWindow,posX, posY);

            }
            else
            {
                WindowHelper.ResetWindowPosition(CurrentWindow.GetAssociatedWindow);
            }


            LoadDailyGoal();
        }

        
        

        #region daily goal
        private string _dailyGoalText;
        public string DailyGoalText
        {
            get
            {

                return _dailyGoalText;
            }
            set
            {

                _dailyGoalText = value;
                OnPropertyChanged(nameof(DailyGoalText));
            }
        }
        public void SetDailyGoal(TimeSpan time)
        {
            //label5.Content = "Daily goal: " + time.ToString();

            DailyGoalText = "Daily goal: " + time.ToString();

            timeSecToFillTopBar = (int)time.TotalSeconds;
        }
        private ICommand _dailyGoalClicked;
        public ICommand DailyGoalClicked
        {
            get
            {
                if (_dailyGoalClicked == null)
                    _dailyGoalClicked = new RelayCommand(
                       (object o) =>
                       {

                           DailyGoalWindow dialog = new DailyGoalWindow();
                           dialog.DataContext = new DailyGoalViewModel();
                           dialog.ShowDialog();
                           DailyGoalViewModel.GetDailyGoalTimespan(out TimeSpan result);
                           SetDailyGoal(result);

                       },
                       (object o) =>
                       {
                           return true;
                       });

                return _dailyGoalClicked;

            }
        }

        private void LoadDailyGoal()
        {

            if (DailyGoalViewModel.GetDailyGoalTimespan(out TimeSpan result))
                SetDailyGoal(result);
            else
                DailyGoalText = "Set daily goal! ";

        }

        #endregion

        float topPercentFilled = 0;
        public int timeSecToFillTopBar = 0;



        public void UpdateTopBar()
        {
            if (timeSecToFillTopBar == 0)
                return;
            if (Artist.ArtistActive)
            {

                float rest = (float)(Artist.ActiveTime.TotalSeconds % (timeSecToFillTopBar));
                topPercentFilled = Utils.ToProcentage(rest, 0, timeSecToFillTopBar);

                ProgressTopBar = topPercentFilled;

                //progressBarTopMost.SetValueWithAnimation(topPercentFilled, true);


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
