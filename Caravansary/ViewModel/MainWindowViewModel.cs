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
using Caravansary.SDK.Contracts;
using System.Resources;
using System.Collections;
using System.Windows.Markup;
using System.Collections.ObjectModel;

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
        private ObservableCollection<CoreModule> _topModules;
        public ObservableCollection<CoreModule> TopModules
        {
            get
            {
                if (_topModules == null)
                {
                    _topModules = new ObservableCollection<CoreModule>();

                }

                return _topModules;
            }
            set
            {
                _topModules = value;

                OnPropertyChanged(nameof(TopModules));
            }
        }


        private ObservableCollection<CoreModule> _coreModules;
        public ObservableCollection<CoreModule> CoreModules
        {
            get
            {
                if (_coreModules == null)
                {
                    _coreModules = new ObservableCollection<CoreModule>();

                }

                return _coreModules;
            }
            set
            {
                _coreModules = value;

                OnPropertyChanged(nameof(CoreModules));
            }
        }

        private ObservableCollection<CoreModule> _botModules;
        public ObservableCollection<CoreModule> BotModules
        {
            get
            {
                if (_botModules == null)
                {
                    _botModules = new ObservableCollection<CoreModule>();

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

        //MilestoneSystem milestoneSystem;
        //NotificationSystem notificationSystem;

        public MainWindowViewModel(IntPtr handle, IWindow window)
        {
            Application.Current.Dispatcher.UnhandledException += OnDispatcherUnhandledException;
            Application.Current.ShutdownMode = ShutdownMode.OnExplicitShutdown;
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




            LoadWindowSettings();




            //LoadDailyGoal();



            //  milestoneSystem = new MilestoneSystem(this);
            //   notificationSystem = new NotificationSystem(this);


        }

        static string mainApplicationDirectoryPath = Directory.GetParent(Assembly.GetExecutingAssembly().Location).ToString();

        public void LoadModules()
        {
            try
            {



                var connectors = Directory.GetDirectories(mainApplicationDirectoryPath + Path.DirectorySeparatorChar + "Modules");
                foreach (var connect in connectors)
                {
                    string dllPath = GetDll(connect);
                    if (string.IsNullOrEmpty(dllPath))
                        continue;
                    Assembly assembly = Assembly.LoadFile(dllPath);
                    var types = assembly.GetTypes()?.ToList();
                    var type = types?.Find(a => typeof(CoreModule).IsAssignableFrom(a));
                    var currentCoreModule = (CoreModule)Activator.CreateInstance(type);







                    var stream = assembly.GetManifestResourceStream(assembly.GetName().Name + ".g.resources");

                    //var stream = assembly.GetManifestResourceStream(assembly.GetName().Name + "ActiveTimerView.xaml");




                    var resourceReader = new ResourceReader(stream);


                    foreach (DictionaryEntry resource in resourceReader)
                    {
                        if (new FileInfo(resource.Key.ToString()).Extension.Equals(".baml"))
                        {

                            Uri uri = new Uri("/" + assembly.GetName().Name + ";component/" + resource.Key.ToString().Replace(".baml", ".xaml"), UriKind.Relative);

                            Debug.WriteLine(resourceReader.ToString());
                            var currentUserControl = Application.LoadComponent(uri) as UserControl;

                            currentUserControl.DataContext = currentCoreModule;
                            currentCoreModule.View = currentUserControl;


                            if (resource.Key.ToString().ToLower().Contains("bar"))
                                TopModules.Add(currentCoreModule);
                            else
                                CoreModules.Add(currentCoreModule);


                            // Mod1 = currentUserControl;


                            // _cmMod0 = currentCoreModule;

                            //ResourceDictionary skin = Application.LoadComponent(uri) as ResourceDictionary;

                            //this.Resources.MergedDictionaries.Add(skin);
                            //Mod1 = resource as UserControl;

                            break;
                        }
                    }


                    //var xaml = XamlReader.Load(stream);

                    //var fe = xaml as UserControl;



                    //Mod1 = fe;

                    //fe.DataContext = win;

                    //Mod1 = win.GetCaravansary.SDK();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Internal Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }



            //Application.Current.Resources.Add(template.DataTemplateKey;, template);

        }

        private string GetDll(string moduleDirectory)
        {
            var files = Directory.GetFiles(moduleDirectory, "*.dll");
            foreach (var file in files)
            {
                if (!FileVersionInfo.GetVersionInfo(file).ProductName.StartsWith("Calci"))
                    return file;
            }
            return string.Empty;
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
