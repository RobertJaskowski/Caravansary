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

        public double _backgroundTransparency => Settings.Default.BackgroundTransparency;
        public double BackgroundTransparency
        {
            get => Settings.Default.BackgroundTransparency;
            set
            {
                ((MainWindowViewModel)Application.Current.MainWindow.DataContext).CurrentWindow.GetAssociatedWindow.Background.Opacity = value;

                Settings.Default.BackgroundTransparency = value;
                Settings.Default.Save();
                OnPropertyChanged(nameof(BackgroundTransparency));
            }
        }


    }
}
