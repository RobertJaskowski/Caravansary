namespace DFA.Models
{
    using System;
    using System.ComponentModel;

    class ArtistModel : INotifyPropertyChanged
    {

        ArtistState _currentArtistState;
        public ArtistState ArtistState { get => _currentArtistState; set => _currentArtistState = value; }
        public bool ArtistActive
        {
            get
            {
                return ArtistState == ArtistState.ACTIVE;
            }
            set
            {
                ArtistState = value ? ArtistState.ACTIVE : ArtistState.INACTIVE;
                OnPropertyChanged(nameof(ArtistState));
            }
        }


        TimeSpan _activeTime;
        public TimeSpan ActiveTime
        {
            get
            {
                return _activeTime;
            }
            set
            {
                _activeTime = value;
                OnPropertyChanged(nameof(ActiveTime));
            }
        }

        public ArtistModel(TimeSpan timespan)
        {
            ActiveTime = timespan;
            ArtistState = ArtistState.INACTIVE;
        }




        #region INotifyPropertyChanged Members;
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
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
