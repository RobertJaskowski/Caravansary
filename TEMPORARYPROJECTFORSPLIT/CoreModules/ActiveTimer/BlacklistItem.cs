using System;
using System.ComponentModel;
using System.Configuration;

namespace Caravansary
{

    [SettingsSerializeAs(SettingsSerializeAs.Xml)]
    public class BlacklistItem : INotifyPropertyChanged
    {
        private string _rule;

        public string Rule
        {
            get
            {
                return _rule;
            }
            set
            {
                _rule = value;
                OnPropertyChanged(nameof(Rule));
            }
        }

        public BlacklistItem()
        { }

        public BlacklistItem(string rule)
        {
            Rule = rule;
        }

        #region INotifyPropertyChanged Members;
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;

            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion
    }



}
