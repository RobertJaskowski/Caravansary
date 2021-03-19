using Caravansary.Core;
using Caravansary.Properties;
using System;
using System.Windows;

namespace Caravansary
{
    class TrayIcon
    {
        private System.Windows.Forms.NotifyIcon trayIcon;

        private Window mainWindow;

        private MainWindowSettings _mainWindowSettings;
        public TrayIcon(MainWindowSettings mainWindowSettings)
        {
            _mainWindowSettings = mainWindowSettings;

            CreateTrayMenu();
        }
        public void CreateTrayMenu()
        {
            mainWindow = Application.Current.MainWindow;
            trayIcon = new System.Windows.Forms.NotifyIcon();
            //    var iconHandle  = Caravansary.Properties.Resources.MyImage.GetHicon();
            var bm = new System.Drawing.Bitmap(Resources.logo);


            trayIcon.Icon = System.Drawing.Icon.FromHandle(bm.GetHicon());

            WinApi.DestroyIcon(bm.GetHicon());

            trayIcon.Text = "Caravansary";
            trayIcon.MouseClick += TrayIconMouseClicked;

            trayIcon.ContextMenuStrip = new System.Windows.Forms.ContextMenuStrip();
            trayIcon.ContextMenuStrip.Items.Add("Exit", null, TrayExitClicked);
            trayIcon.ContextMenuStrip.Items.Add("Settings", null, TraySettingsClicked);
            trayIcon.ContextMenuStrip.Items.Add(new System.Windows.Forms.ToolStripDropDownButton("Quick...", null,
                new System.Windows.Forms.ToolStripLabel("Reset position", null, false, TrayResetPositionClicked),
                new System.Windows.Forms.ToolStripLabel("Stay On Top", null, false, TrayStayOnTopClicked),
                new System.Windows.Forms.ToolStripLabel("Show in taskbar", null, false, TrayShowInTaskBarClicked)
                ));
            trayIcon.Visible = true;
        }

        private void TrayShowInTaskBarClicked(object sender, EventArgs e)
        {
            mainWindow.ShowInTaskbar = !mainWindow.ShowInTaskbar;

            _mainWindowSettings.ShowInTaskbar = mainWindow.ShowInTaskbar; //todo test
        }

        private void TrayStayOnTopClicked(object sender, EventArgs e)
        {
            mainWindow.Topmost = !mainWindow.Topmost;

            if (mainWindow.Topmost)
                mainWindow.Activate();
        }

        private void TraySettingsClicked(object sender, EventArgs e)
        {
            SettingsWindow dialog = new SettingsWindow();
            dialog.DataContext = new SettingsWindowViewModel();

            bool? result = dialog.ShowDialog();
            // if (result == true)

        }

        private void TrayResetPositionClicked(object sender, EventArgs e)
        {
           WindowHelper.ResetWindowPosition(mainWindow);
        }

        private void TrayExitClicked(object sender, EventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void TrayIconMouseClicked(object sender, System.Windows.Forms.MouseEventArgs e)
        {

            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {

                if (mainWindow.Visibility == Visibility.Collapsed)
                {
                    mainWindow.Visibility = Visibility.Visible;
                }
                else
                {
                    mainWindow.Visibility = Visibility.Collapsed;
                }



            }
        }
    }
}
