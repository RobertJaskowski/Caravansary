using DFA.ViewModels;
using DFA.Views;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Threading;

namespace DFA
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private IntPtr CurrentHandleWindow { get; set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            TopWindow wnd = new TopWindow();
            wnd.InitializeComponent();
            Application.Current.MainWindow = wnd;
            CurrentHandleWindow = new WindowInteropHelper(wnd).Handle;
            var vm =  new MainWindowViewModel(CurrentHandleWindow, wnd);

            wnd.DataContext = vm;
            
            wnd.Loaded += vm.OnWindowLoaded ;
            wnd.Closing += vm.OnWindowClosing ;
            wnd.MouseUp += vm.MouseUp;
            wnd.MouseDown += vm.MouseDown;
            wnd.Show();
        }
		
	}
}
