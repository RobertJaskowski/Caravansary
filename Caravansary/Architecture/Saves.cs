using Newtonsoft.Json;
using System;
using System.IO;
using System.Text.Json;

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

    public static bool Save(string ModuleName, string fileName, Object ObjectToSave)
    {

        var savePathDir = GetModuleSaveDirectory(ModuleName);
        CreateFolderStructureIfDoesntExist(savePathDir);

        //var js = JsonSerializer.Serialize(Obj, Obj.GetType());
        var js = JsonConvert.SerializeObject(ObjectToSave);
        File.WriteAllText(GetModuleSaveFilePath(ModuleName, fileName+".json"), js);


        if (File.Exists(GetModuleSaveFilePath(ModuleName, fileName + ".json")))
            return true;
        else return false;


    }
    public static T Load<T>(string ModuleName, string fileName)
    {
        Object rslt;

        if (File.Exists(GetModuleSaveFilePath(ModuleName, fileName + ".json")))
        {

            var js = File.ReadAllText(GetModuleSaveFilePath(ModuleName, fileName + ".json"));
            //rslt = JsonSerializer.Deserialize<T>(js);
            rslt = JsonConvert.DeserializeObject<T>(js);

            return (T)rslt;
        }
        else
        {
            return default(T);
        }
    }



}
