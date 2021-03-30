using System;
using System.IO;
using System.Reflection;

public static class Paths
{
    public static readonly string APP_NAME = "Caravansary";

    public static readonly string APP_EXE = System.Reflection.Assembly.GetExecutingAssembly().Location;
    public static readonly string APP_DIRECTORY = Directory.GetParent(Assembly.GetExecutingAssembly().Location).ToString();
    public static readonly string MODULE_DIRECTORY = Path.Combine(APP_DIRECTORY, "Modules");

    public static readonly string APPDATA_DIRECTORY = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
    public static readonly string APPDATA_C_DIRECTORY = Path.Combine(APPDATA_DIRECTORY, "Caravansary");

    public static readonly string DUMPFILE = Path.Combine(APPDATA_C_DIRECTORY, "dump.txt");

    public static readonly string APPDATA_LAUNCHER_ZIP = APPDATA_C_DIRECTORY + Path.DirectorySeparatorChar + "LauncherCaravansary.zip";
    public static readonly string APPDATA_LAUNCHER_EXE = APPDATA_C_DIRECTORY + Path.DirectorySeparatorChar + "LauncherCaravansary.exe";

    public static bool IsApplicationPortable()
    {
        return false;
    }
}
