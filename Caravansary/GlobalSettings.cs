using Caravansary.Properties;

namespace Caravansary
{
    public static class GlobalSettings
    {
        public static BlacklistItems _settingsBlacklistItems;
        public static BlacklistItems SettingsBlackListItems
        {
            get
            {
                return Settings.Default.BlacklistItems;
            }

            set
            {
                Settings.Default.BlacklistItems = value;
                Settings.Default.Save();
            }
        }
    }
}
