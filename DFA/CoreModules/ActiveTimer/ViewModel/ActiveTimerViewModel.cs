using DFA.Properties;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;
using static DFA.WinApi;

namespace DFA.CoreModules.ActiveTimer.ViewModel
{
    class ActiveTimerViewModel : CoreModule
    {

        #region Properties 

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

                           //Color c = Color.FromArgb(255, 178, 255, 89);
                           //TopBarStateColor = c;

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


                           //Color c = Color.FromArgb(255, 221, 44, 0);

                           //TopBarStateColor = c;


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


                           //Color c = Color.FromArgb(255, 221, 44, 0); 

                           //TopBarStateColor = c;


                           //   progressBarBottomMost.StopAnimation(); old

                       },
                       (object o) =>
                       {
                           return true;
                       });

                return _artistPause;

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


        private void WindowSwitched(object sender, WindowsWindowApi.WindowSwitchedArgs e)
        {


            if (!Settings.Default.CheckBoxBlacklistEnabled) return;
            if (!(GlobalSettings.SettingsBlackListItems.Count > 0)) return;



            //Debug.WriteLine(windowTitle + "\r\n");



            //if (IsChrome(windowTitle))
            //{
            for (int i = 0; i < GlobalSettings.SettingsBlackListItems.Count; i++)
            {

                BlacklistItem item = GlobalSettings.SettingsBlackListItems[i];
                if (WindowsWindowApi.IsChrome(e.Title))//called alot in debug
                {
                    if (WindowsWindowApi.IsChromeTab(item.Rule.ToLower(), out string settingsTabUrl))
                    {

                        var ActiveTabUrl = WindowsWindowApi.GetChromeActiveTabUrl();
                        if (ActiveTabUrl.Contains(settingsTabUrl))
                        {
                            ArtistDeactivate.Execute(null);
                            Debug.WriteLine("stopping");
                            return;
                        }
                    }
                }
                else
                {
                    if (e.Title.Contains(item.Rule.ToLower()))
                    {
                        ArtistDeactivate.Execute(null);
                        return;
                    }
                }

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


            // ActiveTimeUpdate.Execute(null);

            CheckStateChanges();

            OnTick();
        }


        private void OnArtistStateInactiveTick()
        {

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



            //mainWindow.UpdateTopBar();


            //old
            //UpdateBottomBar();
            //StartAnimatingBottomBar();
        }
    }
}
