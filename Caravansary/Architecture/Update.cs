using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;


public class Update
{
    public static UpdateStatus Status;
    public static void Init()
    {
        HandleUpdates();

    }


    private static async Task HandleUpdates()
    {
        if (!InternetAvailability.IsInternetAvailable()) return;

        if (IsMainAppUpdateAvailable())
        {
            Status = UpdateStatus.UPDATEAVAILABLE;
            await HandleLauncherUpdate();
        }

        HandleMainAppUpdate();


    }

    private static bool IsMainAppUpdateAvailable()
    {

        try
        {
            using (WebClient client = new WebClient())
            {
                var success = Version.TryParse(client.DownloadString("https://raw.githubusercontent.com/RobertJaskowski/Caravansary/master/version.txt"), out Version result);
                if (!success)
                    return false;

                if (Data.Version < result)
                {

                    return true;
                }
                else return false;
            }
        }
        catch
        {
            return false;
        }


    }

    private static async Task HandleLauncherUpdate()
    {

        if (await IsLauncherUpdateAvailable())
        {
            await UpdateLauncher();
        }




    }


    private static async Task<bool> IsLauncherUpdateAvailable()
    {
        try
        {

            Version existingLauncherVersion = null;

            if (File.Exists(Paths.APPDATA_LAUNCHER_EXE))
            {
                existingLauncherVersion = AssemblyName.GetAssemblyName(Paths.APPDATA_LAUNCHER_EXE).Version;

            }

            if (existingLauncherVersion == null)
                existingLauncherVersion = new Version(0, 0, 0);

            //var ver = new Version(client.DownloadString("https://raw.githubusercontent.com/RobertJaskowski/LauncherCaravansary/master/version.txt"));

            using (WebClient client = new WebClient())
            {
                var success = Version.TryParse(await Task.Run(() => client.DownloadString("https://raw.githubusercontent.com/RobertJaskowski/LauncherCaravansary/master/version.txt")), out Version onlineLauncherVersion);
                if (!success)
                    return false;

                if (onlineLauncherVersion > existingLauncherVersion)
                    return true;
                else
                    return false;
            }



        }
        catch
        {
            return false;
        }
    }

    private static async Task<bool> UpdateLauncher()
    {
        try
        {

            if (File.Exists(Paths.APPDATA_LAUNCHER_ZIP))
                File.Delete(Paths.APPDATA_LAUNCHER_ZIP);
            if (File.Exists(Paths.APPDATA_LAUNCHER_EXE))
                File.Delete(Paths.APPDATA_LAUNCHER_EXE);


            using (WebClient client = new WebClient())
            {
                await Task.Run(() => client.DownloadFile(new Uri("https://github.com/RobertJaskowski/LauncherCaravansary/releases/latest/download/LauncherCaravansary.zip"), Paths.APPDATA_LAUNCHER_ZIP));
            }

            ZipFile.ExtractToDirectory(Paths.APPDATA_LAUNCHER_ZIP, Paths.APPDATA_C_DIRECTORY, true);

            return true;
        }
        catch
        {
            return false;
        }
    }

    private static async Task HandleMainAppUpdate()
    {

    }




}

public enum UpdateStatus
{
    UPTODATE,
    UPDATEAVAILABLE,
    UPDATEFAILED
}
