using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caravansary.CoreModules.Roadmap
{
    [SettingsSerializeAs(SettingsSerializeAs.Xml)]
    public class RoadmapItem : INotifyPropertyChanged
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

        public RoadmapItem()
        { }

        public RoadmapItem(string rule)
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
