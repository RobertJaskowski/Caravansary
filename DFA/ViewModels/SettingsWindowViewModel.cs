using DFA.Properties;
using System.Windows;

namespace DFA
{
    class SettingsWindowViewModel : BaseViewModel
    {

        public bool _checkbox_bottombar => Settings.Default.CheckBoxBottomBar;
        public bool CheckboxBottomBar
        {
            get => Settings.Default.CheckBoxBottomBar;
            set
            {
                Settings.Default.CheckBoxBottomBar = value;
                Settings.Default.Save();
                ((MainWindowViewModel)Application.Current.MainWindow.DataContext).BotBarEnabled = value;

                OnPropertyChanged(nameof(CheckboxBottomBar));
            }
        }

        public int _backgroundTransparency => Settings.Default.BackgroundTransparency;
        public int BackgroundTransparency
        {
            get => Settings.Default.BackgroundTransparency;
            set
            {
                ((MainWindowViewModel)Application.Current.MainWindow.DataContext).Window.GetAssociatedWindow.Background.Opacity = value / 100;

                Settings.Default.BackgroundTransparency = value;
                Settings.Default.Save();
                OnPropertyChanged(nameof(BackgroundTransparency));
            }
        }


        //private void Window_Loaded(object sender, RoutedEventArgs e)
        //{


        //    LoadSettings();


        //}



        //private void LoadSettings()
        //{
        //    //checkbox_bottombar.IsChecked = Settings.Default.CheckBoxBottomBar;
        //    //slider_transparency.Value = Settings.Default.BackgroundTransparency;

        //}

        //private void CheckBox_BottomBar_Checked(object sender, RoutedEventArgs e)
        //{
        //    Settings.Default.CheckBoxBottomBar = true;
        //    Settings.Default.Save();
        //    ((MainWindowViewModel)Application.Current.MainWindow.DataContext).BotBarEnabled = true;

        //}
        //private void CheckBox_BottomBar_Unchecked(object sender, RoutedEventArgs e)
        //{
        //    Settings.Default.CheckBoxBottomBar = false;
        //    Settings.Default.Save();
        //    ((MainWindowViewModel)Application.Current.MainWindow.DataContext).BotBarEnabled = false;

        //}

        //private void Slider_Transparency_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        //{
        //   // if (!IsLoaded)
        //     //   return;
        //    Settings.Default.BackgroundTransparency = (int)e.NewValue;
        //    ((MainWindowViewModel)Application.Current.MainWindow.DataContext).Window.GetAssociatedWindow.Background.Opacity = e.NewValue / 100;

        //    Settings.Default.Save();
        //}

    }
}
