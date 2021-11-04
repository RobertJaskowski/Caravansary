using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Windows.Threading;

public static class Saves
{

    public static string savesDirectoryLocation =
        !Paths.IsApplicationPortable() ?
        Paths.APPDATA_C_DIRECTORY + Path.DirectorySeparatorChar + "Saves" :
        Paths.APP_DIRECTORY + Path.DirectorySeparatorChar + "Saves";


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

    private static readonly int saveCooldownTime = 10;
    private static int currectSaveCooldownTime = 10;
    public static bool IsSavingInProgress => SaveCooldownTimer.IsEnabled;
    private static DispatcherTimer SaveCooldownTimer;

    private class CachedSave
    {
        public string moduleName;
        public string pathOfFile;
        public Object objectToSave;

        public CachedSave(string moduleName, string pathOfFile, object objectToSave)
        {
            this.moduleName = moduleName;
            this.pathOfFile = pathOfFile;
            this.objectToSave = objectToSave;
        }
    }

    private static Dictionary<string, CachedSave> cachedSaves = new Dictionary<string, CachedSave>();

    private static void OnSaveCooldownTimerTick(object sender, EventArgs e)
    {
        currectSaveCooldownTime--;
        if(currectSaveCooldownTime <= 0)
        {
            SaveCachedObjectsToDisc();
            currectSaveCooldownTime = saveCooldownTime;
            SaveCooldownTimer.Stop();
        }
    }

    private static bool SaveCachedObjectsToDisc()
    {

        try
        {
            foreach (var csv in cachedSaves.Values)
            {


                var savePathDir = GetModuleSaveDirectory(csv.moduleName);
                CreateFolderStructureIfDoesntExist(savePathDir);

                var js = JsonConvert.SerializeObject(csv.objectToSave);
                File.WriteAllText(csv.pathOfFile, js);

            }
        }
        catch
        {
            return false;
        }
        

        //if (File.Exists(GetModuleSaveFilePath(ModuleName, fileName + ".json")))
        //    return true;
        //else return false;

        return true;
    }

    public static bool Save(string ModuleName, string fileName, Object ObjectToSave)
    {
        if (SaveCooldownTimer == null)
        {
            SaveCooldownTimer = new DispatcherTimer();
            SaveCooldownTimer.Interval = TimeSpan.FromSeconds(1);
            SaveCooldownTimer.Tick += OnSaveCooldownTimerTick;
            SaveCooldownTimer.Start();
        }
        else if (!SaveCooldownTimer.IsEnabled)
        {
            currectSaveCooldownTime = saveCooldownTime;
            SaveCooldownTimer.Start();
        }
        else
        {
            currectSaveCooldownTime = saveCooldownTime;
        }

        string pathOfFile = GetModuleSaveFilePath(ModuleName, fileName + ".json");

        if (cachedSaves.ContainsKey(pathOfFile))
        {
            cachedSaves[pathOfFile] = new CachedSave(ModuleName, pathOfFile, ObjectToSave);
        }
        else
        {
            cachedSaves.Add(pathOfFile, new CachedSave(ModuleName, pathOfFile, ObjectToSave));
        }

        return true;
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
