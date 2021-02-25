using System;
using System.Windows;
using System.Windows.Interop;

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
            MainWindow wnd = new MainWindow();
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
