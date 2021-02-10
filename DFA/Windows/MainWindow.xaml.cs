using DFA.Windows;
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
    public partial class MainWindow : Window, IMainWindow
    {

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


        //public HandleRef HWnd { get; set; }

        public MainWindow()
        {
            InitializeComponent();

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
        }




        private void ClearLabels()
        {

            label1.Content = "";
            label2.Content = "";
            label3.Content = "00:00:00.00";
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
            trayIcon.ContextMenuStrip.Items.Add(new ToolStripDropDownButton("Settings...", null,
                new ToolStripLabel("Reset position", null, false, TrayResetPosition)
                ));
            trayIcon.Visible = true;
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
                this.Topmost = !this.Topmost;

                if (Topmost)
                    Activate();
                //todo change this method

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


        }
        private const int maxSecAfkTime = 5;
        private int currentCheckingAfkTime = 0;



        private void TimerArtistStateCheck(object sender, EventArgs e)
        {
            bool changed = false;




            if (!ArtistActive)
                changed = CheckArtistStateChanged();
            else
            {
                currentCheckingAfkTime++;
                if (currentCheckingAfkTime > maxSecAfkTime)
                {
                    changed = CheckArtistStateChanged();

                    if (changed)
                        currentCheckingAfkTime = 0;
                }

            }


        }

        private bool CheckArtistStateChanged()
        {
            ArtistState newState = TickTimerGetNewArtistStateBasedOnInput();

            label2.Content = newState.ToString();

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
                    label4.Content = "T" + lastInputInfo.dwTime;
                    return ArtistState.ACTIVE;
                }
            }
            else
                label4.Content = "F";


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

            key.Close();
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
            if (DailyGoalWindow.GetDailyGoalTimespan(out TimeSpan result))
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





        private void Label3_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            //   notificationSystem.ShowNotification(new Notification(Milestone.GetDefaultMilestoneMessage(), false, TimeSpan.FromSeconds(3)));

        }

        private void Label4_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {

        }

        private void Label5_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            DailyGoalWindow dialog = new DailyGoalWindow();
            bool? result = dialog.ShowDialog();
            if (result == true)
                SetDailyGoal(dialog.returnTime);
        }
    }
}
