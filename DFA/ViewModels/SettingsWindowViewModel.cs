using DFA.Properties;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace DFA
{
    class SettingsWindowViewModel : BaseViewModel
    {

        #region Properties
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

        public bool CheckBoxBlacklistEnabled
        {
            get => Settings.Default.CheckBoxBlacklistEnabled;
            set
            {
                Settings.Default.CheckBoxBlacklistEnabled = value;
                Settings.Default.Save();


                OnPropertyChanged(nameof(CheckBoxBlacklistEnabled));
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



       

        private ObservableCollection<BlacklistItem> _blacklistItems;
        public ObservableCollection<BlacklistItem> BlacklistItems
        {
            get
            {
                if (_blacklistItems == null)
                {
                    _blacklistItems = new ObservableCollection<BlacklistItem>(Settings.Default.BlacklistItems);

                }

                return _blacklistItems;
            }
            set
            {
                _blacklistItems = value;

                OnPropertyChanged(nameof(BlacklistItems));
            }
        }


        #endregion

        #region Commands

        private ICommand _createNewBlacklistItem;
        public ICommand CreateNewBlacklistItem
        {
            get
            {
                if (_createNewBlacklistItem == null)
                    _createNewBlacklistItem = new RelayCommand(
                       (object o) =>
                       {
                           var k = new BlacklistItem("empty");
                           _blacklistItems.Add(k);

                           GlobalSettings.SettingsBlackListItems.Add(k);
                           Settings.Default.Save();

                       },
                       (object o) =>
                       {
                           return true;

                       });

                return _createNewBlacklistItem;

            }
        }


        private RelayCommand _removeBlacklistItem;
        public RelayCommand RemoveBlacklistItem
        {
            get
            {
                if (_removeBlacklistItem == null)
                    _removeBlacklistItem = new RelayCommand(
                       (object o) =>
                       {
                           if (o is BlacklistItem)
                           {
                               BlacklistItems.Remove(o as BlacklistItem);

                               GlobalSettings.SettingsBlackListItems.Remove(o as BlacklistItem);
                               Settings.Default.Save();
                               
                           }

                       },
                       (object o) =>
                       {
                           return true;

                       });

                return _removeBlacklistItem;

            }
        }

        #endregion


    }
}
