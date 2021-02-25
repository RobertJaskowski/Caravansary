
namespace DFA
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using System.Windows;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Threading;
    using DFA.Properties;
    using static DFA.WinApi;
    using Settings = Properties.Settings;

    class MainWindowViewModel : BaseViewModel
    {

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
        internal void MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && e.RightButton == MouseButtonState.Pressed)
                Window.GetAssociatedWindow.DragMove();
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

        private int _backgroundTransparency;
        public int BackgroundTransparency
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

        private IntPtr CurrentHandleWindow { get; set; }


        private const int maxSecAfkTime = 2;
        private int currentCheckingAfkTime = 0;





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

        public IWindow Window;

        private static void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            System.Windows.MessageBox.Show("Unhandled exception occurred: \n" + e.Exception.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);

        }

        public MainWindowViewModel(IntPtr handle, IWindow window)
        {
            Application.Current.Dispatcher.UnhandledException += OnDispatcherUnhandledException;
            this.Window = window;
            CurrentHandleWindow = handle;
            _artistModel = new ArtistModel(TimeSpan.FromSeconds(0));

            CreateTrayMenu();
            LoadSettings();

            CreateArtistStateTimers();



            CreateMilestoneSystem();
            CreateNotificationSystem();

        }


        private void LoadSettings()
        {

            Application.Current.MainWindow.ShowInTaskbar = Settings.Default.ShowInTaskbar;

            BackgroundTransparency = Settings.Default.BackgroundTransparency / 100;

            Window.GetAssociatedWindow.WindowStartupLocation = WindowStartupLocation.Manual;
            int posX = Settings.Default.MainWindowPositionX;
            int posY = Settings.Default.MainWindowPositionY;


            if (posX != 0 || posY != 0)
            {

                SetWindowPosition(posX, posY);
                //window.WindowStartupLocation = WindowStartupLocation.Manual;



            }
            else
            {
                ResetWindowPosition();
            }


            LoadDailyGoal();
        }
        private System.Windows.Forms.NotifyIcon trayIcon;

        public void CreateTrayMenu()
        {

            trayIcon = new System.Windows.Forms.NotifyIcon();
            //    var iconHandle  = DFA.Properties.Resources.MyImage.GetHicon();
            var bm = new System.Drawing.Bitmap(Resources.logo);


            trayIcon.Icon = System.Drawing.Icon.FromHandle(bm.GetHicon());

            DestroyIcon(bm.GetHicon());

            trayIcon.Text = "DFA";
            trayIcon.MouseClick += TrayIconMouseClicked;

            trayIcon.ContextMenuStrip = new System.Windows.Forms.ContextMenuStrip();
            trayIcon.ContextMenuStrip.Items.Add("Exit", null, TrayOnExitClicked);
            trayIcon.ContextMenuStrip.Items.Add("Settings", null, TrayOnSettingsClicked);
            trayIcon.ContextMenuStrip.Items.Add(new System.Windows.Forms.ToolStripDropDownButton("Quick...", null,
                new System.Windows.Forms.ToolStripLabel("Reset position", null, false, TrayResetPosition),
                new System.Windows.Forms.ToolStripLabel("Stay On Top", null, false, TrayStayOnTop),
                new System.Windows.Forms.ToolStripLabel("Show in taskbar", null, false, TrayShowInTaskBar)
                ));
            trayIcon.Visible = true;
        }

        private void TrayShowInTaskBar(object sender, EventArgs e)
        {
            Window.GetAssociatedWindow.ShowInTaskbar = !Window.GetAssociatedWindow.ShowInTaskbar;

            Settings.Default.ShowInTaskbar = Window.GetAssociatedWindow.ShowInTaskbar;
            Settings.Default.Save();
        }

        private void TrayStayOnTop(object sender, EventArgs e)
        {
            Window.GetAssociatedWindow.Topmost = !Window.GetAssociatedWindow.Topmost;

            if (Window.GetAssociatedWindow.Topmost)
                Window.GetAssociatedWindow.Activate();
        }

        private void TrayOnSettingsClicked(object sender, EventArgs e)
        {
            SettingsWindow dialog = new SettingsWindow(Window);
            dialog.DataContext = new SettingsWindowViewModel();

            bool? result = dialog.ShowDialog();
            // if (result == true)

        }

        private void TrayResetPosition(object sender, EventArgs e)
        {
            ResetWindowPosition();
        }

        private void TrayOnExitClicked(object sender, EventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void TrayIconMouseClicked(object sender, System.Windows.Forms.MouseEventArgs e)
        {

            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {

                if (Window.GetAssociatedWindow.Visibility == Visibility.Collapsed)
                {
                    Window.GetAssociatedWindow.Visibility = Visibility.Visible;
                }
                else
                {
                    Window.GetAssociatedWindow.Visibility = Visibility.Collapsed;
                }

                //if(window.WindowState == WindowState.Minimized)
                //{
                //    window.WindowState = WindowState.Normal;
                //}
                //else
                //{
                //    window.WindowState = WindowState.Minimized;
                //}


            }
        }
        public int DefaultWindowX => (int)((System.Windows.SystemParameters.WorkArea.Width / 2) - (Window.GetAssociatedWindow.Width / 2));// Screen.FromControl(this).WorkingArea.Width / 2;
        public int DefaultWindowY => 0;
        public void ResetWindowPosition()
        {
            Window.GetAssociatedWindow.WindowStartupLocation = WindowStartupLocation.Manual;
            SetWindowPosition(DefaultWindowX, DefaultWindowY);
        }
        public void SetWindowPosition(int leftX, int topY)
        {
            Window.GetAssociatedWindow.Left = leftX;
            Window.GetAssociatedWindow.Top = topY;
        }

        private void CreateArtistStateTimers()
        {

            DispatcherTimer timerArtistStateCheck = new DispatcherTimer();
            timerArtistStateCheck.Interval = TimeSpan.FromSeconds(1);
            timerArtistStateCheck.Tick += new EventHandler(TimerTick);
            timerArtistStateCheck.Start();

        }

        private void TimerTick(object sender, EventArgs e)
        {
            // ActiveTimeUpdate.Execute(null);

            CheckStateChanges();

            OnTick();
        }



        private void CheckStateChanges()
        {


            if (Artist.ArtistState == ArtistState.PAUSED)
            {
                return;
            }
            else if (!Artist.ArtistActive)
            {
                var r = CheckIfAnyInputReceived();

                if (r)
                {

                    ArtistActivate.Execute(null);
                }
            }
            else//active
            {
                var lastInputState = CheckIfAnyInputReceived();

                if (lastInputState)
                {
                    currentCheckingAfkTime = 0;
                }
                else
                {
                    currentCheckingAfkTime++;
                    if (currentCheckingAfkTime > maxSecAfkTime)
                    {
                        currentCheckingAfkTime = 0;
                        ArtistDeactivate.Execute(null);

                    }
                }
            }
        }



        uint lastActive = 0;
        private bool CheckIfAnyInputReceived()
        {

            LASTINPUTINFO lastInputInfo = new LASTINPUTINFO();
            lastInputInfo.cbSize = (UInt32)Marshal.SizeOf(lastInputInfo);
            lastInputInfo.dwTime = 0;

            if (GetLastInputInfo(ref lastInputInfo))
            {
                if (lastInputInfo.dwTime != lastActive)
                {
                    lastActive = lastInputInfo.dwTime;
                    return true;
                }
            }


            return false;
        }


        private void OnTick()
        {
            switch (Artist.ArtistState)
            {
                case ArtistState.ACTIVE:
                    OnArtistStateActiveTick();
                    break;
                case ArtistState.INACTIVE:
                    OnArtistStateInactiveTick();
                    break;
                case ArtistState.PAUSED:
                    break;
            }
        }

        private void OnArtistStateActiveTick()
        {
            ActiveTimeUpdate1Sec.Execute(null);



            UpdateTopBar();
            //UpdateBottomBar();
            //StartAnimatingBottomBar();
        }

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

        float topPercentFilled = 0;
        public int timeSecToFillTopBar = 0;



        private void UpdateTopBar()
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


        private void OnArtistStateInactiveTick()
        {

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

            int x = (int)Window.GetAssociatedWindow.Left;
            int y = (int)Window.GetAssociatedWindow.Top;
            Settings.Default.MainWindowPositionX = x;
            Settings.Default.MainWindowPositionY = y;
            Settings.Default.Save();
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
                           bool? result = dialog.ShowDialog();
                           if (result == true)
                               SetDailyGoal(dialog.returnTime);

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

            if (DailyGoalWindow.GetDailyGoalTimespan(out TimeSpan result))
                SetDailyGoal(result);
            else
                DailyGoalText = "Set daily goal! ";

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
