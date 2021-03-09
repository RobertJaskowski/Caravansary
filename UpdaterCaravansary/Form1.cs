using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace UpdaterCaravansary
{
    public partial class Form1 : Form
    {


        static string appdataAPPDATA_PATH = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        static string appdataCFOLDER_PATH = Path.Combine(appdataAPPDATA_PATH, "Caravansary");
        static string dumpFileLocation = Path.Combine(appdataCFOLDER_PATH, "dump.txt");



        string updaterApplicationPath = System.Reflection.Assembly.GetExecutingAssembly().Location;
        static string updaterApplicationDirectoryPath = Directory.GetParent(Assembly.GetExecutingAssembly().Location).ToString();
        


        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SendMessage(IntPtr hWnd, UInt32 Msg, IntPtr wParam, IntPtr lParam);

        private const UInt32 WM_CLOSE = 0x0010;

        void CloseWindow(IntPtr hwnd)
        {
            SendMessage(hwnd, WM_CLOSE, IntPtr.Zero, IntPtr.Zero);
        }



        public Form1()
        {
            InitializeComponent();

            Debug.WriteLine(appdataAPPDATA_PATH);
            Debug.WriteLine(updaterApplicationPath);
            Debug.WriteLine(updaterApplicationDirectoryPath);
            var client = new WebClient();

            try
            {
                System.Threading.Thread.Sleep(5000);




                if (File.Exists(dumpFileLocation))
                {
                    string interlophandle = File.ReadAllText(dumpFileLocation);
                    if (!string.IsNullOrEmpty(interlophandle))
                    {
                        if (int.TryParse(interlophandle, out int result))
                        {
                            if (result > 0)
                                CloseWindow((IntPtr)result);
                        }
                    }


                    File.Delete(dumpFileLocation);
                }


                try
                {
                    if (File.Exists(@".\Caravansary.exe"))
                    {

                        File.Delete(@".\Caravansary.exe");
                    }
                }
                catch
                {
                    File.WriteAllText(dumpFileLocation, "Can't delete Caravansary.exe");

                }







                client.DownloadFile("https://github.com/RobertJaskowski/Caravansary/releases/latest/download/x64Caravansary.zip", @"x64Caravansary.zip");
                string zipPath = @".\x64Caravansary.zip";



                string tempExtractingPathToExe = Path.Combine(appdataCFOLDER_PATH, @"Caravansary.exe");
                string tempExtractingPathToUpdaterExe = Path.Combine(appdataCFOLDER_PATH, @"UpdaterCaravansary.exe");

                if (File.Exists(tempExtractingPathToExe))
                {
                    File.Delete(tempExtractingPathToExe);
                    File.Delete(tempExtractingPathToUpdaterExe);
                }

                ZipFile.ExtractToDirectory(zipPath, appdataCFOLDER_PATH);
                //File.Move(tempExtractingPathToExe, applicationPath);
                //File.Copy("Caravansary.exe", applicationPath, true);
                FileInfo fi = new FileInfo(tempExtractingPathToExe);
                fi.CopyTo(Path.Combine(updaterApplicationDirectoryPath, "Caravansary.exe"), true);



                File.Delete(@".\x64Caravansary.zip");
                Process.Start(@".\Caravansary.exe");
                this.Close();
            }
            catch
            {
                File.WriteAllText(dumpFileLocation, "app update failed");
                //Debug.WriteLine("app update fail");
                this.Close();

            }
        }
    }
}
