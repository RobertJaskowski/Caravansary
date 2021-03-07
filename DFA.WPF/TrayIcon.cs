using DFA.Properties;
using System;
using System.Windows;

namespace DFA
{
    class TrayIcon
    {
        private System.Windows.Forms.NotifyIcon trayIcon;

        private Window mainWindow;
        public void CreateTrayMenu()
        {
            mainWindow = Application.Current.MainWindow;
            trayIcon = new System.Windows.Forms.NotifyIcon();
            //    var iconHandle  = DFA.Properties.Resources.MyImage.GetHicon();
            var bm = new System.Drawing.Bitmap(Resources.logo);


            trayIcon.Icon = System.Drawing.Icon.FromHandle(bm.GetHicon());

            WinApi.DestroyIcon(bm.GetHicon());

            trayIcon.Text = "DFA";
            trayIcon.MouseClick += TrayIconMouseClicked;

            trayIcon.ContextMenuStrip = new System.Windows.Forms.ContextMenuStrip();
            trayIcon.ContextMenuStrip.Items.Add("Exit", null, TrayOnExitClicked);
            trayIcon.ContextMenuStrip.Items.Add("Settings", null, TrayOnSettingsClicked);
            trayIcon.ContextMenuStrip.Items.Add(new System.Windows.Forms.ToolStripDropDownButton("Quick...", null,
                new System.Windows.Forms.ToolStripLabel("Reset position", null, false, TrayResetPosition),
                new System.Windows.Forms.ToolStripLabel("Stay On Top", null, false, TrayStayOnTop),
                new System.Windows.Forms.ToolStripLabel("Show in taskbar", null, false, TrayShowInTaskBar)
                ));
            trayIcon.Visible = true;
        }

        private void TrayShowInTaskBar(object sender, EventArgs e)
        {
            mainWindow.ShowInTaskbar = !mainWindow.ShowInTaskbar;

            Settings.Default.ShowInTaskbar = mainWindow.ShowInTaskbar;
            Settings.Default.Save();
        }

        private void TrayStayOnTop(object sender, EventArgs e)
        {
            mainWindow.Topmost = !mainWindow.Topmost;

            if (mainWindow.Topmost)
                mainWindow.Activate();
        }

        private void TrayOnSettingsClicked(object sender, EventArgs e)
        {
            SettingsWindow dialog = new SettingsWindow();
            dialog.DataContext = new SettingsWindowViewModel();

            bool? result = dialog.ShowDialog();
            // if (result == true)

        }

        private void TrayResetPosition(object sender, EventArgs e)
        {
           WindowHelper.ResetWindowPosition(mainWindow);
        }

        private void TrayOnExitClicked(object sender, EventArgs e)
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
