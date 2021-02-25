
namespace DFA
{
    using System.ComponentModel;

    class BaseViewModel : INotifyPropertyChanged
    {





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
