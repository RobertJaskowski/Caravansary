using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Interop;

namespace Caravansary
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private IntPtr CurrentHandleWindow { get; set; }
        static string appdataAPPDATA_PATH = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        static string appdataCFOLDER_PATH = Path.Combine(appdataAPPDATA_PATH, "Caravansary");
        static string dumpFileLocation = Path.Combine(appdataCFOLDER_PATH, "dump.txt");


        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            MainWindow wnd = new MainWindow();
            wnd.InitializeComponent();
            Application.Current.MainWindow = wnd;
            CurrentHandleWindow = new WindowInteropHelper(wnd).Handle;
            var vm = new MainWindowViewModel(CurrentHandleWindow, wnd);

            wnd.DataContext = vm;

            wnd.Loaded += vm.OnWindowLoaded;
            wnd.Closing += vm.OnWindowClosing;
            wnd.MouseUp += vm.MouseUp;
            wnd.MouseDown += vm.MouseDown;
            wnd.Show();

            WebClient webClient = new WebClient();

            try
            {
                var ver = new Version(webClient.DownloadString("https://raw.githubusercontent.com/RobertJaskowski/Caravansary/master/version.txt"));
                if (GlobalSettings.Version.IsLower(ver))
                {
                    string handlestr = CurrentHandleWindow.ToString();
                    File.WriteAllText(dumpFileLocation, handlestr) ;

                    Process.Start("UpdaterCaravansary.exe");



                    Application.Current.MainWindow.Close();
                }

            }
            catch
            {

            }
        }

    }
}
