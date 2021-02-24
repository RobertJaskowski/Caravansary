using Microsoft.Win32;
using System;
using System.Windows;

namespace DFA.Windows
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        public SettingsWindow()
        {
            InitializeComponent();
        }

        RegistryKey key;

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            OpenRegistryKey();


            LoadSettingsFromRegistry();
        }



        private void OpenRegistryKey()
        {
            key = Registry.CurrentUser.OpenSubKey("DFA", true);
            if (key == null)
                key = Registry.CurrentUser.CreateSubKey("DFA", true);
        }
        private void LoadSettingsFromRegistry()
        {
            checkbox_bottombar.IsChecked = (int)key.GetValue("CheckBoxBottomBar", 1) == 1;
            slider_transparency.Value = (int)key.GetValue("BackgroundTransparency", 100);

        }
        private void CheckBox_BottomBar_Checked(object sender, RoutedEventArgs e)
        {

            key.SetValue("CheckBoxBottomBar", 1);
            MainWindow.Instance.progressBarBottomMost.Visibility = Visibility.Visible;

        }
        private void CheckBox_BottomBar_Unchecked(object sender, RoutedEventArgs e)
        {
            key.SetValue("CheckBoxBottomBar", 0);
            MainWindow.Instance.progressBarBottomMost.Visibility = Visibility.Collapsed;

        }

        private void Slider_Transparency_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (key != null)
                key.SetValue("BackgroundTransparency", (int)e.NewValue);
            MainWindow.Instance.window.Background.Opacity = e.NewValue/100;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            key.Close();
        }
    }
}
