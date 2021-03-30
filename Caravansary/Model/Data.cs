using System;
using System.Reflection;


public static class Data
{
    public static Version _version;
    public static Version Version
    {
        get
        {
            if (_version == null)
            {
                _version = Assembly.GetExecutingAssembly().GetName().Version;

            }


            return _version;
        }

    }

    private static MainWindowSettings _mainWindowSettingsSave;

    public static MainWindowSettings MainWindowSettingsSave
    {
        get
        {
            if (_mainWindowSettingsSave == null)
            {
                _mainWindowSettingsSave = Saves.Load<MainWindowSettings>("Caravansary", "MainWindowSettings");
                if (_mainWindowSettingsSave == null)
                {
                    _mainWindowSettingsSave = new MainWindowSettings()
                    {
                        BackgroundTransparency = 1,
                        ShowInTaskbar = false,
                        MainWindowPositionX = 0,
                        MainWindowPositionY = 0
                    };

                }
            }



            return _mainWindowSettingsSave;
        }
        internal set
        {
            _mainWindowSettingsSave = value;

            SaveWindowSettings();
        }
    }


    internal static void SaveWindowSettings()
    {
        if (_mainWindowSettingsSave != null)
        {
            bool ret = Saves.Save(Paths.APP_NAME, "MainWindowSettings", _mainWindowSettingsSave);//todo make async
        }
    }
}
