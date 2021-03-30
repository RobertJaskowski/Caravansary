using System;
using System.Windows.Threading;

namespace Caravansary
{
    public class Notification : INotification
    {
        public string message;
        public bool requiresAction;
        public TimeSpan discardTimer;

        public Notification(string message, bool requiresAction, TimeSpan discardTimer)
        {
            this.message = message;
            this.requiresAction = requiresAction;
            this.discardTimer = discardTimer;
        }

        private DispatcherTimer shownTimer;
        private int currentlyShownFor = 0;
        public void OnNotificationDisplayed()
        {
            currentlyShownFor = 0;
            shownTimer = new DispatcherTimer();
            shownTimer.Interval = TimeSpan.FromSeconds(1);
            shownTimer.Tick += ShownForTimerTick;
            shownTimer.Start();
        }

        private void ShownForTimerTick(object sender, EventArgs e)
        {
            currentlyShownFor++;
            if(currentlyShownFor > discardTimer.TotalSeconds)
            {
                NotificationSystem.Instance.StartHidingNotification();
                shownTimer.Stop();
                shownTimer = null;
            }
        }

        public void OnNotificationDiscarded()
        {
        }

        public void OnNotificationActionPassed()
        {
        }

        public void OnNotificationActionFailed()
        {
        }

        public void OnNotificationHidden()
        {
        }



    }
}
