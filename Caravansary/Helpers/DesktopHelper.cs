using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

public static class DesktopHelper
{
    public static string APP_NAME = "Caravansary";

    public static string mainApplicationPath = System.Reflection.Assembly.GetExecutingAssembly().Location;
    public static string mainApplicationDirectoryPath = Directory.GetParent(Assembly.GetExecutingAssembly().Location).ToString();
    public static string moduleFolder = Path.Combine(mainApplicationDirectoryPath, "Modules");

    public static string appdataAPPDATA_PATH = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
    public static string appdataCFOLDER_PATH = Path.Combine(appdataAPPDATA_PATH, "Caravansary");


    static string dumpFileLocation = Path.Combine(appdataCFOLDER_PATH, "dump.txt");


    public static bool IsApplicationPortable()
    {
        return false;
    }
}
