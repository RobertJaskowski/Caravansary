﻿using Caravansary.Core;
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

namespace Caravansary
{
    partial class MainWindowViewModel : BaseViewModel
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

        //private bool _showInTaskbar;
        public bool ShowInTaskbar
        {
            get
            {
                return MainWindowSettingsSave.ShowInTaskbar;
            }
            set
            {
                MainWindowSettingsSave.ShowInTaskbar = value;
                OnPropertyChanged(nameof(ShowInTaskbar));
                OnPropertyChanged(nameof(MainWindowSettingsSave));
                SaveWindowSettings();

            }
        }

        //private double _backgroundTransparency;
        public double BackgroundTransparency
        {
            get
            {
                return MainWindowSettingsSave.BackgroundTransparency;
            }
            set
            {
                MainWindowSettingsSave.BackgroundTransparency = value;
                OnPropertyChanged(nameof(BackgroundTransparency));
                OnPropertyChanged(nameof(MainWindowSettingsSave));
                SaveWindowSettings();

            }
        }

        //private double _mainWindowPositionX;
        public double MainWindowPositionX
        {
            get
            {
                return MainWindowSettingsSave.MainWindowPositionX;
            }
            set
            {
                MainWindowSettingsSave.MainWindowPositionX = value;
                OnPropertyChanged(nameof(MainWindowPositionX));
                OnPropertyChanged(nameof(MainWindowSettingsSave));
                SaveWindowSettings();

            }
        }

        //private double _mainWindowPositionY;
        public double MainWindowPositionY
        {
            get
            {
                return MainWindowSettingsSave.MainWindowPositionY;
            }
            set
            {
                MainWindowSettingsSave.MainWindowPositionY = value;
                OnPropertyChanged(nameof(MainWindowPositionY));
                SaveWindowSettings();

            }
        }


        //private bool _botBarEnabled;
        //public bool BotBarEnabled
        //{
        //    get
        //    {
        //        return _botBarEnabled;
        //    }
        //    set
        //    {
        //        _botBarEnabled = value;
        //    }

        //}


        //private double _progressBotBar;
        //public double ProgressBotBar
        //{
        //    get
        //    {
        //        return _progressBotBar;
        //    }
        //    set
        //    {
        //        _progressBotBar = value;
        //        OnPropertyChanged(x =>ProgressBotBar);
        //    }
        //}

        //private double _backgroundTransparency;
        //public double BackgroundTransparency
        //{
        //    get
        //    {
        //        return _backgroundTransparency;
        //    }
        //    set
        //    {
        //        _backgroundTransparency = value;
        //        OnPropertyChanged(x =>BackgroundTransparency);
        //    }
        //}


        private MainWindowSettings _mainWindowSettingsSave;
        public MainWindowSettings MainWindowSettingsSave
        {
            get
            {
                if (_mainWindowSettingsSave == null)
                {
                    _mainWindowSettingsSave = Saves.Load<MainWindowSettings>("Caravansary", "MainWindowSettings");
                    if (_mainWindowSettingsSave == null)
                    {
                        _mainWindowSettingsSave = new MainWindowSettings()
                        {
                            BackgroundTransparency = 1,
                            ShowInTaskbar = false,
                            MainWindowPositionX = 0,
                            MainWindowPositionY = 0
                        };

                    }
                }



                return _mainWindowSettingsSave;
            }
            set
            {
                _mainWindowSettingsSave = value;

                SaveWindowSettings();
                OnPropertyChanged(nameof(MainWindowSettingsSave));
            }
        }

        private void SaveWindowSettings()
        {
            if (_mainWindowSettingsSave != null)
            {
                bool ret = Saves.Save(DesktopHelper.APP_NAME, "MainWindowSettings", _mainWindowSettingsSave);//todo make async
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

            moduleController.StopAllModules();
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
            trayIcon = new TrayIcon(MainWindowSettingsSave);



            LoadWindowSettings();




            //LoadDailyGoal();






        }

        static ModuleController moduleController;
        public void InitModuleController()
        {
            moduleController = new ModuleController();
            moduleController.ScanDirectory(DesktopHelper.mainApplicationDirectoryPath + Path.DirectorySeparatorChar + "Modules");

            foreach (var modInfo in moduleController.CoreModuleValues)
            {

                modInfo.Loader.Start();

                switch (modInfo.Loader.Instance.GetModulePosition())
                {
                    case ModulePosition.TOP:
                        TopModules.Add(modInfo.Loader.View);
                        break;
                    case ModulePosition.MID:
                        ViewCoreModules.Add(modInfo.Loader.View);
                        break;
                    case ModulePosition.BOT:
                        BotModules.Add(modInfo.Loader.View);
                        break;
                }
            }

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

            //BackgroundTransparency = MainWindowSettingsSave.BackgroundTransparency;

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
}
