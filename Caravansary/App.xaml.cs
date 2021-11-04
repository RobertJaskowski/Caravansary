using Caravansary.Core;
using Ninject;
using Ninject.Modules;
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



    public partial class App : Application
    {
        private IntPtr CurrentHandleWindow { get; set; }

        public bool UpdateAwaitingToDownload = false;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            Data.Version = Assembly.GetExecutingAssembly().GetName().Version;

            IoC.Setup();
            IoC._kernel.Load(new WPFIoCConfiguration());



            var wnd = IoC.Get<MainWindow>();


            var pm = IoC.Get<MainWindowPageModel>();


            wnd.InitializeComponent();
            //Application.Current.MainWindow = wnd;
            //CurrentHandleWindow = new WindowInteropHelper(wnd).Handle;



            wnd.DataContext = pm;

            wnd.Show();


            //pm.InitModuleController();


            Update.CheckForUpdates();


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
