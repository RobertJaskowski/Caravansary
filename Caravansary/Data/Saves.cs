using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

public static class Saves
{

    public static string savesDirectoryLocation =
        !DesktopHelper.IsApplicationPortable() ?
        DesktopHelper.appdataCFOLDER_PATH + Path.DirectorySeparatorChar + "Saves" :
        DesktopHelper.mainApplicationDirectoryPath + Path.DirectorySeparatorChar + "Saves";


    public static string GetModuleSaveDirectory(string moduleName)
    {
        return savesDirectoryLocation + Path.DirectorySeparatorChar + moduleName;
    }

    public static string GetModuleSaveFilePath(string ModuleName, string fileName)
    {
        return savesDirectoryLocation + Path.DirectorySeparatorChar + ModuleName + Path.DirectorySeparatorChar + fileName;
    }

    public static void CreateFolderStructureIfDoesntExist(string savePathDir)
    {
        if (!Directory.Exists(savePathDir))
        {
            Directory.CreateDirectory(savePathDir);
        }
    }

    public static bool Save(string ModuleName, string fileName, Object Obj)
    {

        var savePathDir = GetModuleSaveDirectory(ModuleName);
        CreateFolderStructureIfDoesntExist(savePathDir);

        var xs = new XmlSerializer(Obj.GetType());

        using (TextWriter sw = new StreamWriter(GetModuleSaveFilePath(ModuleName, fileName), false))
        {
            xs.Serialize(sw, Obj);
        }
        if (File.Exists(GetModuleSaveFilePath(ModuleName, fileName)))
            return true;
        else return false;


    }
    public static T Load<T>(string ModuleName, string fileName)
    {
        Object rslt;

        if (File.Exists(GetModuleSaveFilePath(ModuleName, fileName)))
        {
            var xs = new XmlSerializer(typeof(T));

            using (var sr = new StreamReader(GetModuleSaveFilePath(ModuleName, fileName)))
            {
                rslt = (T)xs.Deserialize(sr);
            }
            return (T)rslt;
        }
        else
        {
            return default(T);
        }
    }



}
