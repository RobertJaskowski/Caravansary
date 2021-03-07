namespace DFA
{
    public interface INotification
    {
        public void OnNotificationDisplayed();
        public void OnNotificationDiscarded();
        public void OnNotificationActionPassed();
        public void OnNotificationActionFailed();
        public void OnNotificationHidden();
    }
}