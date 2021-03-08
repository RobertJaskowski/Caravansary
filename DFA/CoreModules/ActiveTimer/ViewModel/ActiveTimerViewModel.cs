using DFA.CoreModules.MainBar.ViewModel;
using DFA.Properties;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using static DFA.WinApi;

namespace DFA.CoreModules.ActiveTimer.ViewModel
{
    class ActiveTimerViewModel : CoreModule
    {

        #region Properties 
        public override string ModuleName => "ActiveTimer";

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
                OnPropertyChanged(nameof(Artist));
            }

        }

        private string _timeReason;
        public String TimeReason
        {
            get
            {
                return _timeReason;
            }
            set
            {
                _timeReason = value;
                OnPropertyChanged(nameof(TimeReason));
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
                        _artistTimeString = "|| " + value + (!string.IsNullOrEmpty(TimeReason) ? " by " + TimeReason : "");
                        break;
                    case ArtistState.RESUMED:
                        _artistTimeString = "|> " + value + (!string.IsNullOrEmpty(TimeReason) ? " by " + TimeReason : "");//TODO change to fontawesome, hangle font awesome in runtime because its not working
                        break;
                }
                OnPropertyChanged(nameof(ArtistTimeString));
            }
        }
        private MainBarViewModel _mainBarModule;
        private MainBarViewModel MainBarModule
        {
            get
            {
                if (_mainBarModule == null)
                    _mainBarModule = (MainBarViewModel)ModuleManager.Instance.GetCoreModule("MainBar");
                return _mainBarModule;
            }
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
                           TimeReason = "user";

                           if (Artist.ArtistState == ArtistState.ACTIVE || Artist.ArtistState == ArtistState.INACTIVE)
                           {
                               ArtistPause.Execute(null);
                           }
                           else if (Artist.ArtistState == ArtistState.PAUSED)
                           {
                               ArtistResume.Execute(null);
                           }

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
                           MainBarModule.SetBarColor(c);

                           //  StartAnimatingBottomBar(); old 
                           //timeSecToFillTopBar = 20; old 
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
                           MainBarModule.SetBarColor(c);

                           //   progressBarBottomMost.StopAnimation(); old

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
                           MainBarModule.SetBarColor(c);
                           //   progressBarBottomMost.StopAnimation(); old

                       },
                       (object o) =>
                       {
                           return true;
                       });

                return _artistPause;

            }
        }

        private ICommand _artistResume;
        public ICommand ArtistResume
        {
            get
            {
                if (_artistResume == null)
                    _artistResume = new RelayCommand(
                       (object o) =>
                       {

                           Artist.ArtistState = ArtistState.RESUMED;

                           ArtistTimeString = Artist.ActiveTime.ToString();

                           Color c = Color.FromArgb(255, 221, 44, 0);
                           MainBarModule.SetBarColor(c);

                       },
                       (object o) =>
                       {
                           return true;
                       });

                return _artistResume;

            }
        }

        #endregion




        private const int maxSecAfkTime = 2;
        private int currentCheckingAfkTime = 0;

        WindowsWindowApi windowApi;
        public ActiveTimerViewModel()
        {



            _artistModel = new ArtistModel(TimeSpan.FromSeconds(0));

            CreateArtistStateTimers();


            windowApi = new WindowsWindowApi();

            WindowsWindowApi.OnWindowSwitched += WindowSwitched;




        }


     


        private string lastWindow;
        private void WindowSwitched(object sender, WindowsWindowApi.WindowSwitchedArgs e)
        {
            string tit = e.Title.ToLower().Trim();


            if (TimeReason != null)
                if (TimeReason.ToLower().Contains("user"))
                    return;


                    Debug.WriteLine("window switched callback " + tit);

            if (tit.Contains("visual") || tit.Contains("dfa"))
            {
                Debug.WriteLine("is in design");

                return;
            }

            if (lastWindow == null)
                lastWindow = tit;
            else
            {
                if (lastWindow.Length == tit.Length)
                {
                    Debug.WriteLine("is same window");
                    return;
                }
                lastWindow = tit;


            }


            if (!Settings.Default.CheckBoxBlacklistEnabled) return;
            if (!(GlobalSettings.SettingsBlackListItems.Count > 0)) return;


            List<BlacklistItem> chrome = new List<BlacklistItem>();
            List<BlacklistItem> windows = new List<BlacklistItem>();

            foreach (var item in GlobalSettings.SettingsBlackListItems)
            {
                if (WindowsWindowApi.IsChrome(item.Rule))//called alot in debug
                {
                    if (WindowsWindowApi.IsChromeTab(item.Rule.ToLower(), out string settingsTabUrl))
                    {
                        chrome.Add(item);
                    }
                    else
                    {
                        windows.Add(item);
                    }
                }
                else
                {
                    windows.Add(item);
                }
            }


            foreach (var item in windows)
            {
                if (tit.Contains(item.Rule.ToLower()))
                {
                    TimeReason = item.Rule;
                    ArtistPause.Execute(null);
                    Debug.WriteLine(TimeReason);
                    return;
                }
            }


            if (chrome.Count > 0)
            {
                
                foreach (var item in chrome)
                {



                    WindowsWindowApi.SplitProcessAndParameter(item.Rule, out string process, out string settingsTabUrl);
                    if (tit.Contains(settingsTabUrl.ToLower()))
                    {
                        TimeReason = settingsTabUrl;
                        ArtistPause.Execute(null);


                        Debug.WriteLine(TimeReason);

                        return;
                    }
                }
                //chrome tabs
                //string ActiveTabUrl =  WindowsWindowApi.GetChromeActiveTabUrl();
                //foreach (var item in chrome)
                //{
                //    if (ActiveTabUrl == null) break;



                //    WindowsWindowApi.SplitProcessAndParameter(item.Rule, out string process, out string settingsTabUrl);
                //    if (ActiveTabUrl.Contains(settingsTabUrl.ToLower()))
                //    {
                //        TimeReason = settingsTabUrl;
                //        ArtistPause.Execute(null);


                //        Debug.WriteLine(TimeReason);

                //        Debug.WriteLine("stopping");
                //        return;
                //    }
                //}
            }

            if (TimeReason != null)
                if (!TimeReason.ToLower().Contains("user"))
                {
                    TimeReason = "window";
                    ArtistResume.Execute(null);
                }

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



            CheckStateChanges();

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
                case ArtistState.RESUMED:
                    OnArtistStateResumeTick();
                    break;
            }
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


        private void OnArtistStateActiveTick()
        {
            ActiveTimeUpdate1Sec.Execute(null);

            MainBarModule.UpdateTopBar(Artist.ActiveTime.TotalSeconds);


            //old
            //UpdateBottomBar();
            //StartAnimatingBottomBar();
        }

        private void OnArtistStateInactiveTick()
        {

        }

        private void OnArtistStateResumeTick()
        {
            ActiveTimeUpdate1Sec.Execute(null);

            MainBarModule.UpdateTopBar(Artist.ActiveTime.TotalSeconds);

            TimeReason = "";
        }


    }
}
