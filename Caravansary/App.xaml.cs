using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
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
        
        public bool UpdateAwaitingToDownload = false;

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
            wnd.MouseEnter += vm.MouseEnter;
            wnd.MouseLeave += vm.MouseLeave;
            wnd.Show();


            vm.InitModuleController();


            Update.Init();

            
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            if (Update.Status == UpdateStatus.UPDATEAVAILABLE)
            {
                List<string> args = new List<string>()
                {
                    "RequstedUpdate",
                    Paths.APP_EXE
                };

                Process.Start(Paths.APPDATA_LAUNCHER_EXE, args);
                Application.Current.MainWindow?.Close();
                Application.Current?.Shutdown();
            }
        }
    }
}
