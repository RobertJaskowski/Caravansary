using DFA;
using Microsoft.Win32;
using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using static DFA.WinApi;
using Application = System.Windows.Application;
using Label = System.Windows.Controls.Label;
using ProgressBar = System.Windows.Controls.ProgressBar;


namespace DFA
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class UselessWindow : Window, IMainWindow,IWindow
    {
        public static UselessWindow Instance;
        public enum MsgType
        {
            WM_INPUT = 0x00FF

        }

        public int DefaultWindowX => (int)((System.Windows.SystemParameters.WorkArea.Width / 2) - (window.Width / 2));// Screen.FromControl(this).WorkingArea.Width / 2;
        public int DefaultWindowY => 0;


        private const int WM_NCHITTEST = 0x84;
        private const int HTCLIENT = 0x1;
        private const int HTCAPTION = 0x2;
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int WM_NCRBUTTONDOWN = 0xA4;
        public const int WM_RBUTTONDOWN = 0x0204;
        int penTrackingResetCounter = 0;
        int penTrackingResetCounterLimitAsOfPenTrackingErrorOffset = 0;


        //   int refreshArtistStateTickTimerInMiliseconds = 6;//, 10 * 1000 = 10 secs , 1000 = ,1 sec

        TimeSpan refreshArtistStateTickTimer = TimeSpan.FromMilliseconds(1000);

        public DateTime startingTime;
        public DateTime lastStopTime;


        public TimeSpan activatedFullTime;

        public bool ShowActiveTimeInSeconds = false;



        public int graphicalProgressBarUpdateInMiliseconds = 20;
        private bool receivedInputFromPenOnLastWndProc = false;

        public bool pausedState = false;



        private static ArtistState _currentArtistState = ArtistState.INACTIVE;
        public static ArtistState ArtistState { get => _currentArtistState; private set => _currentArtistState = value; }

        public static bool ArtistActive
        {
            get
            {
                return ArtistState == ArtistState.ACTIVE;
            }
            set
            {
                ArtistState = value ? ArtistState.ACTIVE : ArtistState.INACTIVE;
            }
        }


        private NotifyIcon trayIcon;
        public IntPtr CurrentHandleWindow { get; set; }

        public Window GetAssociatedWindow => window;


        //public HandleRef HWnd { get; set; }
        const int MYACTION_HOTKEY_ID = 1;

        public UselessWindow()
        {
            Dispatcher.UnhandledException += OnDispatcherUnhandledException;

            //throw new Exception("testtt");
            if (Instance == null)
                Instance = this;
            InitializeComponent();


            LoadSettings();

            CurrentHandleWindow = new WindowInteropHelper(this).Handle;



            ClearLabels();

            CreateTrayMenu();
            LoadWindowPosition();
            LoadDailyGoal();




            startingTime = DateTime.Now;
            lastStopTime = DateTime.Now;

            //  RegisterTabletDevice();

            CreateArtistStateTimers();

            //  CreateGraphicalBarsAndTickTimer();



            CreateMilestoneSystem();
            CreateNotificationSystem();


            // Modifier keys codes: Alt = 1, Ctrl = 2, Shift = 4, Win = 8
            // Compute the addition of each combination of the keys you want to be pressed
            // ALT+CTRL = 1 + 2 = 3 , CTRL+SHIFT = 2 + 4 = 6...
            // RegisterHotKey(CurrentHandleWindow, MYACTION_HOTKEY_ID, 2, (int)Keys.Z);


        }
        private static void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            System.Windows.MessageBox.Show("Unhandled exception occurred: \n" + e.Exception.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);

        }
        private void LoadSettings()
        {
            var key = Registry.CurrentUser.OpenSubKey("DFA", false);
            if (key != null)
            {
                ShowInTaskbar = (int)key.GetValue("ShowInTaskbar", 0) == 1;

                if (!((int)key.GetValue("CheckBoxBottomBar", 1) == 1))
                {
                    progressBarBottomMost.Visibility = Visibility.Collapsed;
                }

                int d = (int)key.GetValue("BackgroundTransparency", 100);
                window.Background.Opacity = (double)d / 100;
                key?.Close();
            }
        }

        private void ClearLabels()
        {

            label1.Content = "";
            label2.Content = "";
            label3.Content = "00:00:00.00";
            label4.Content = "";
            label5.Content = "Set daily goal!";
        }


        private void RegisterTabletDevice()
        {
            RAWINPUTDEVICE rid = new RAWINPUTDEVICE();

            rid.usUsagePage = 0x000D;
            rid.usUsage = 0x0001; //01 for external, 02 for integrated
            rid.dwFlags = 0x00000100;
            rid.hwndTarget = CurrentHandleWindow;

            if (WinApi.RegisterRawInputDevices(rid, 1, Convert.ToUInt32(Marshal.SizeOf(rid))) == false)
            {
                label1.Content = "registration failed";
            }
        }
        public void CreateTrayMenu()
        {

            trayIcon = new NotifyIcon();
            //    var iconHandle  = DFA.Properties.Resources.MyImage.GetHicon();
            var bm = new Bitmap(DFA.Properties.Resources.logo);


            trayIcon.Icon = System.Drawing.Icon.FromHandle(bm.GetHicon());

            DestroyIcon(bm.GetHicon());

            trayIcon.Text = "DFA";
            trayIcon.MouseClick += TrayIconMouseClicked;

            trayIcon.ContextMenuStrip = new ContextMenuStrip();
            trayIcon.ContextMenuStrip.Items.Add("Exit", null, TrayOnExitClicked);
            trayIcon.ContextMenuStrip.Items.Add("Settings", null, TrayOnSettingsClicked);
            trayIcon.ContextMenuStrip.Items.Add(new ToolStripDropDownButton("Quick...", null,
                new ToolStripLabel("Reset position", null, false, TrayResetPosition),
                new ToolStripLabel("Stay On Top", null, false, TrayStayOnTop),
                new ToolStripLabel("Show in taskbar", null, false, TrayShowInTaskBar)
                ));
            trayIcon.Visible = true;
        }

        private void TrayShowInTaskBar(object sender, EventArgs e)
        {
            window.ShowInTaskbar = !window.ShowInTaskbar;

            RegistryKey key;
            key = Registry.CurrentUser.OpenSubKey("DFA", true);
            if (key == null)
                key = Registry.CurrentUser.CreateSubKey("DFA", true);

            key.SetValue("ShowInTaskbar", window.ShowInTaskbar);

            if (key != null)
                key.Close();
        }

        private void TrayStayOnTop(object sender, EventArgs e)
        {
            this.Topmost = !this.Topmost;

            if (Topmost)
                Activate();
        }

        private void TrayOnSettingsClicked(object sender, EventArgs e)
        {
            SettingsWindow dialog = new SettingsWindow(this.window);
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

            if (e.Button == MouseButtons.Left)
            {

                if (Visibility == Visibility.Collapsed)
                {
                    Visibility = Visibility.Visible;
                }
                else
                {
                    Visibility = Visibility.Collapsed;
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
        public void ResetWindowPosition()
        {
            window.WindowStartupLocation = WindowStartupLocation.Manual;
            SetWindowPosition(DefaultWindowX, DefaultWindowY);
        }

        public void SetWindowPosition(int leftX, int topY)
        {
            window.Left = leftX;
            window.Top = topY;
        }


        private void LoadWindowPosition()
        {
            var key = Registry.CurrentUser.OpenSubKey("DFA", true);
            if (key != null)
            {
                int posX = (int)key.GetValue("DFAMainWindowPositionX");
                int posY = (int)key.GetValue("DFAMainWindowPositionY");

                if (posX >= 0 && posY >= 0)
                {
                    window.WindowStartupLocation = WindowStartupLocation.Manual;

                    SetWindowPosition(posX, posY);


                }

                key?.Close();

            }
        }


        private void CreateArtistStateTimers()
        {

            DispatcherTimer timerArtistStateCheck = new DispatcherTimer();
            timerArtistStateCheck.Interval = TimeSpan.FromSeconds(1);
            timerArtistStateCheck.Tick += new EventHandler(TimerArtistStateCheck);
            timerArtistStateCheck.Start();

            DispatcherTimer timerArtistStateTick = new DispatcherTimer();
            timerArtistStateTick.Interval = refreshArtistStateTickTimer;
            timerArtistStateTick.Tick += new EventHandler(TimerArtistStateTick);
            timerArtistStateTick.Start();

            //DispatcherTimer timerHotkey = new DispatcherTimer();
            //timerHotkey.Interval = TimeSpan.FromMilliseconds(500);
            //timerHotkey.Tick += new EventHandler(TimerHotkeyModule);
            //timerHotkey.Start();
        }

        int ctrlZCounter = 0;


        private KeyboardListener _listener;



        private void TimerHotkeyModule(object sender, EventArgs e)
        {
            //label1.Content = ctrlZCounter+"";


            //NativeMessage msg = new NativeMessage();
            //PeekMessage(out msg, IntPtr.Zero, 0x0100, 0, 0x0000);//handle to null
            //label4.Content = msg.msg + " " + msg.wParam;
            //if (msg.msg == 0x0312 && msg.wParam.ToInt32() == MYACTION_HOTKEY_ID)
            //{
            //    ctrlZCounter++;

            //}
        }

        private const int maxSecAfkTime = 5;
        private int currentCheckingAfkTime = 0;

        private void TimerArtistStateCheck(object sender, EventArgs e)
        {
            bool changed = false;



            if (pausedState)
            {
                ArtistState = ArtistState.INACTIVE;
                return;
            }
            else if (!ArtistActive)
            {
                changed = CheckArtistStateChanged();


            }
            else//active
            {
                var lastInputState = TickTimerGetNewArtistStateBasedOnInput();

                if (lastInputState == ArtistState.ACTIVE)
                {
                    currentCheckingAfkTime = 0;
                }
                else
                {
                    currentCheckingAfkTime++;
                    if (currentCheckingAfkTime > maxSecAfkTime)
                    {
                        currentCheckingAfkTime = 0;
                        ArtistState = lastInputState;
                        DisplayArtistStateInUI("INACTIVE");

                        OnArtistStateInactiveActivated();

                    }
                }
            }
        }

        private void DisplayArtistStateInUI(string state)
        {
            label2.Content = state;

        }

        private bool CheckArtistStateChanged()
        {
            ArtistState newState = TickTimerGetNewArtistStateBasedOnInput();
            DisplayArtistStateInUI(newState.ToString());

            bool changed;

            if (ArtistState == newState)
                changed = false;
            else
                changed = true;

            ArtistState = newState;


            if (changed)
                if (ArtistState == ArtistState.ACTIVE)
                    OnArtistStateActiveActivated();
                else
                    OnArtistStateInactiveActivated();




            return changed;
        }

        public System.Windows.Point lastMousePosition;

        uint lastActive = 0;

        private ArtistState TickTimerGetNewArtistStateBasedOnInput()
        {

            LASTINPUTINFO lastInputInfo = new LASTINPUTINFO();
            lastInputInfo.cbSize = (UInt32)Marshal.SizeOf(lastInputInfo);
            lastInputInfo.dwTime = 0;

            if (GetLastInputInfo(ref lastInputInfo))
            {
                if (lastInputInfo.dwTime != lastActive)
                {
                    lastActive = lastInputInfo.dwTime;
                    //label4.Content = "T" + lastInputInfo.dwTime;
                    return ArtistState.ACTIVE;
                }
            }
            //else
            // label4.Content = "F";


            return ArtistState.INACTIVE;


            //keys only
            // if (AnyKeyPressed())
            //      return ArtistState.ACTIVE;



            //work weridly with administrative on csp not working at all
            //if (lastMousePosition == new System.Windows.Point(0, 0))
            //{
            //    lastMousePosition = GetMousePosition();
            //    return ArtistState.ACTIVE;
            //}
            //else
            //{
            //    if (Math.Abs(lastMousePosition.X - GetMousePosition().X) > 1 && Math.Abs(lastMousePosition.Y - GetMousePosition().Y) > 1)
            //    {

            //        lastMousePosition = GetMousePosition();
            //        return ArtistState.ACTIVE;
            //    }
            //}



            //return new testing
            //return ArtistState.INACTIVE;


            //if (!receivedInputFromPenOnLastWndProc)
            //{
            //    if (penTrackingResetCounterLimitAsOfPenTrackingErrorOffset > 0)
            //    {
            //        penTrackingResetCounter++;

            //        if (penTrackingResetCounter > penTrackingResetCounterLimitAsOfPenTrackingErrorOffset)
            //        {

            //            return ArtistState.INACTIVE;
            //        }
            //        else
            //            return ArtistState.ACTIVE;
            //    }
            //    else
            //        return ArtistState.INACTIVE;


            //}


            //if (receivedInputFromPenOnLastWndProc)
            //{
            //    penTrackingResetCounter = 0;
            //    return ArtistState.ACTIVE;
            //}

            //return ArtistState.INACTIVE;

        }


        private void OnArtistStateActiveActivated()
        {
            SolidColorBrush b = new SolidColorBrush(System.Windows.Media.Color.FromRgb(178, 255, 89));
            progressBarTopMost.Background = b;
            progressBarTopMost.BorderBrush = b;

            StartAnimatingBottomBar();


            //   progressBarBottomMost.Maximum = timeSecToFillBotBar;
            //     progressBarBottomMost.SetValueWithAnimation( timeSecToFillBotBar,);

        }

        private void StartAnimatingBottomBar()
        {

            var procentFilledAlready = Utils.ToProcentage(progressBarBottomMost.Value, progressBarBottomMost.Minimum, progressBarBottomMost.Maximum);


            var valueOfTimeFilled = Utils.ProcentToValue(procentFilledAlready, timeSecToFillBotBar);

            var timeLeft = timeSecToFillBotBar - valueOfTimeFilled;

            DoubleAnimation animation = new DoubleAnimation(progressBarBottomMost.Maximum, TimeSpan.FromSeconds(timeLeft));
            animation.Completed += BottomBarCompleted;

            progressBarBottomMost.BeginAnimation(ProgressBar.ValueProperty, animation);
        }

        private void BottomBarCompleted(object sender, EventArgs e)
        {
            progressBarBottomMost.StopAnimation();

            progressBarBottomMost.Value = progressBarBottomMost.Minimum;
            if (ArtistActive)
                StartAnimatingBottomBar();
        }

        private void OnArtistStateInactiveActivated()
        {
            SolidColorBrush b = new SolidColorBrush(System.Windows.Media.Color.FromRgb(221, 44, 0));
            progressBarTopMost.Background = b;
            progressBarTopMost.BorderBrush = b;

            progressBarBottomMost.StopAnimation();
            // progressBarBottomMost.BeginAnimation(ProgressBar.ValueProperty, null);
        }


        private void TimerArtistStateTick(object sender, EventArgs e)
        {
            if (pausedState)
                return;

            if (ArtistActive)
                OnArtistStateActiveTick();
            else
                OnArtistStateInactiveTick();

        }






        private void OnArtistStateActiveTick()
        {
            activatedFullTime += refreshArtistStateTickTimer;


            DisplayTimeActivatedInUI();

            UpdateBottomBar();
            UpdateTopBar();
        }

        private void DisplayTimeActivatedInUI()
        {
            if (ShowActiveTimeInSeconds)
            {
                label3.Content = activatedFullTime.TotalSeconds.ToString().TrimEnd('0', ' ');
            }
            else
            {

                StringBuilder sb = new StringBuilder(activatedFullTime.ToString());

                sb.Truncate(8);


                //  sb.Truncate(11);

                //  if (sb.Length == 8)
                //      sb.Append(".00");
                label3.Content = sb.ToString();

                sb.Clear();
                sb = null;


            }
        }


        private void OnArtistStateInactiveTick()
        {

        }



        private void CreateGraphicalBarsAndTickTimer()
        {

            //   progressBarBottomMost.WithLerp = true;

            DispatcherTimer timerProgressBarsUpdate = new DispatcherTimer();
            timerProgressBarsUpdate.Interval = TimeSpan.FromMilliseconds(graphicalProgressBarUpdateInMiliseconds);
            timerProgressBarsUpdate.Tick += new EventHandler(TimerUpdateProgressBarsGraphically);
            timerProgressBarsUpdate.Start();
        }


        float botCurrentVisualProgressOfLerp = 0;
        float botDesiredBarValue = 0;
        float botPercentFilled = 0;
        public int timeSecToFillBotBar = 5;

        float topCurrentVisualProgressOfLerp = 0;
        float topDesiredBarValue = 0;
        float topPercentFilled = 0;
        public int timeSecToFillTopBar = 0;



        private void TimerUpdateProgressBarsGraphically(object sender, EventArgs e)
        {

            // UpdateTopBar();
            // UpdateBottomBar();




        }

        private void UpdateTopBar()
        {
            if (timeSecToFillTopBar == 0)
                return;
            if (ArtistActive)
            {

                float rest = (float)(activatedFullTime.TotalSeconds % (timeSecToFillTopBar));
                topPercentFilled = Utils.ToProcentage(rest, 0, timeSecToFillTopBar);
                progressBarTopMost.SetValueWithAnimation(topPercentFilled, true);


                //   topDesiredBarValue = (int)Utils.ProcentToProgressBarValue(progressBarTopMost, topPercentFilled);
                //  progressBarTopMost.Value = topDesiredBarValue;

            }




        }

        private void UpdateBottomBar()
        {
            if (ArtistActive)
            {
                //timesectofill , activatedfulltime

                //     float rest = (float)(activatedFullTime.TotalSeconds % (timeSecToFillBotBar));

                //       botPercentFilled = Utils.ToProcentage(rest, 0, timeSecToFillBotBar);

                //       progressBarBottomMost.SetValueWithAnimation(botPercentFilled,true);


                //botDesiredBarValue = Utils.ProcentToProgressBarValue(progressBarBottomMost, botPercentFilled);


                //progressBarBottomMost.Value = botDesiredBarValue;

            }
        }

        //private void UpdateBottomBar()
        //{
        //    if (ArtistActive)
        //    {

        //        float rest = (float)(activatedFullTime.TotalSeconds % (timeSecToFillBotBar));
        //        botPercentFilled = Utils.ToProcentage(rest, 0, timeSecToFillBotBar);
        //        botDesiredBarValue = Utils.ProcentToProgressBarValue(progressBarBottomMost, botPercentFilled);
        //    }



        //    if (botCurrentVisualProgressOfLerp > botDesiredBarValue)
        //    {
        //        botCurrentVisualProgressOfLerp = botDesiredBarValue;
        //        progressBarBottomMost.Value = botDesiredBarValue;
        //    }

        //    float lerpSpeed = botPercentFilled / 100;
        //    botCurrentVisualProgressOfLerp = Utils.Lerp(botCurrentVisualProgressOfLerp, botDesiredBarValue, lerpSpeed);

        //    progressBarBottomMost.Value = botCurrentVisualProgressOfLerp < 0 ? 0 : botCurrentVisualProgressOfLerp;

        //    label1.Text = progressBarBottomMost.Value.ToString();
        //}

        MilestoneSystem milestoneSystem;

        private void CreateMilestoneSystem()
        {
            milestoneSystem = new MilestoneSystem(this);

        }

        NotificationSystem notificationSystem;
        private void CreateNotificationSystem()
        {
            notificationSystem = new NotificationSystem(this);

        }



        public bool isMouseDown = false;
        public bool isLMouseDown = false;
        public bool isRMouseDown = false;

        public int mouseX;
        public int mouseY;
        public int mouseinX;
        public int mouseinY;


        private void Window_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.LeftButton == System.Windows.Input.MouseButtonState.Pressed && e.RightButton == System.Windows.Input.MouseButtonState.Pressed)
                this.DragMove();
        }
        private void Window_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.LeftButton == System.Windows.Input.MouseButtonState.Released && e.RightButton == System.Windows.Input.MouseButtonState.Released)
                SaveWindowPosition();
        }

        //private void FormMouseDown(object sender, MouseEventArgs e)
        //{

        //    isMouseDown = true;

        //    if (e.Button == MouseButtons.Left)
        //        isLMouseDown = true;

        //    if (e.Button == MouseButtons.Right)
        //    {
        //        isRMouseDown = true;

        //   //     mouseinX = MousePosition.X - Bounds.X;
        //    //    mouseinY = MousePosition.Y - Bounds.Y;

        //        //ReleaseCapture();
        //        //SendMessage(Handle, WM_NCLBUTTONDOWN, HTCAPTION, 0);

        //    }

        //}

        //private void FormMouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        //{
        //    if (isRMouseDown)
        //    {


        //     //   mouseX = MousePosition.X - mouseinX;
        //   //     mouseY = MousePosition.Y - mouseinY;

        //  //      this.SetDesktopLocation(mouseX, mouseY);

        //    }
        //}

        private void SaveWindowPosition()
        {
            RegistryKey key;
            key = Registry.CurrentUser.OpenSubKey("DFA", true);
            if (key == null)
                key = Registry.CurrentUser.CreateSubKey("DFA", true);

            int x = (int)this.Left;
            int y = (int)this.Top;
            key.SetValue("DFAMainWindowPositionX", x);
            key.SetValue("DFAMainWindowPositionY", y);


            key?.Close();
        }

        //  private void formmouseup(object sender, system.windows.forms.mouseeventargs e)
        //   {


        //if (control.mousebuttons == mousebuttons.none)
        //    ismousedown = false;

        //if (e.button == mousebuttons.left)
        //    islmousedown = false;

        //if (e.button == mousebuttons.right)
        //{
        //    isrmousedown = false;

        //    savewindowposition();


        //}

        //   }



        public void SetMidLable(string text)
        {
            label3.Content = text;
        }

        public TimeSpan GetActivatedTime()
        {
            return activatedFullTime;
        }


        public Label GetNotificationTextbox()
        {
            return labelNotification;
        }

        private void LoadDailyGoal()
        {
            if (DailyGoalViewModel.GetDailyGoalTimespan(out TimeSpan result))
                SetDailyGoal(result);
        }

        public void SetDailyGoal(TimeSpan time)
        {
            label5.Content = "Daily goal: " + time.ToString();



            timeSecToFillTopBar = (int)time.TotalSeconds;
        }



        public void ShowNotification(Notification notification)
        {
            notificationSystem.ShowNotification(notification);
        }


        protected override void OnClosed(EventArgs e)
        {
            trayIcon.Icon = null;
            trayIcon.Visible = false;
            trayIcon.Dispose();

            base.OnClosed(e);
        }

        ArtistState IMainWindow.GetArtistState()
        {
            return ArtistState;
        }

        public void SetArtistState(ArtistState value)
        {
            ArtistState = value;

        }

        public bool GetArtistActive()
        {
            return ArtistActive;
        }

        public void SetArtistActive(bool value)
        {
            ArtistActive = value;
        }

        internal StackPanel GetNotificationStackPanel()
        {
            return stackNotification;
        }


        private void Label2_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (!pausedState)
            {
                pausedState = true;
                label2.Content = "PAUSED";
                label3.Content = "|| " + label3.Content;

                ArtistState = ArtistState.INACTIVE;
                OnArtistStateInactiveActivated();
            }
            else
            {
                pausedState = false;
            }
        }


        private void Label3_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            //   notificationSystem.ShowNotification(new Notification(Milestone.GetDefaultMilestoneMessage(), false, TimeSpan.FromSeconds(3)));

        }

        private void Label4_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {

        }

        private void Label5_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            //DailyGoalWindow dialog = new DailyGoalWindow();
            //bool? result = dialog.ShowDialog();
            //if (result == true)
            //    SetDailyGoal(dialog.returnTime);
        }

        private void window_Loaded(object sender, RoutedEventArgs e)
        {
            _listener = new KeyboardListener();
            _listener.OnKeyPressed += _listener_OnKeyPressed;
            _listener.OnKeyReleased += _listener_OnKeyReleased;


            _listener.HookKeyboard();
        }

        #region KeyCombination


        private bool ZPressed = false;
        private bool CtrlPressed = false;

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
                ctrlZCounter++;
                label1.Content = ctrlZCounter;
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

        private void window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _listener.UnHookKeyboard();
        }
        #endregion
    }
}
