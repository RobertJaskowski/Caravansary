using System;

namespace DFA
{
    internal interface IMainWindow
    {
        TimeSpan GetActivatedTime();


        ArtistState GetArtistState();
        void SetArtistState(ArtistState value);


        bool GetArtistActive();
        void SetArtistActive(bool value);

        public void ShowNotification(Notification notification);
        public void SetMidLable(string text);
        
    }
}