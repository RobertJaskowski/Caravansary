﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;


public static class Data
{
    public static Version _version;
    public static Version Version
    {
        get
        {
            if (_version == null)
            {
                var v = Assembly.GetExecutingAssembly().GetName().Version.ToString();
                _version = new Version(v);

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
            bool ret = Saves.Save(DesktopHelper.APP_NAME, "MainWindowSettings", _mainWindowSettingsSave);//todo make async
        }
    }
}
