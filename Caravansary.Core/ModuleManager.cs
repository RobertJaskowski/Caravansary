using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Linq;

namespace Caravansary.Core
{
    public class ModuleManager
    {
        private static ModuleManager _instance;
        public static ModuleManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ModuleManager();


                    CoreModules = new List<CoreModule>();





                    //CoreModules.Add(new MainBarViewModel());

                    //CoreModules.Add(new KeyCounterViewModel());
                    //CoreModules.Add(new ActiveTimerViewModel());
                    //CoreModules.Add(new DailyGoalViewModel());


                    //CoreModules.Add(new FillerViewModel());


                    //CoreModules.Add(new RoadmapViewModel());


                }
                return _instance;
            }
        }

        string mainApplicationPath = System.Reflection.Assembly.GetExecutingAssembly().Location;
        static string mainApplicationDirectoryPath = Directory.GetParent(Assembly.GetExecutingAssembly().Location).ToString();
        static string moduleFolder = Path.Combine(mainApplicationDirectoryPath, "Modules");


        private static List<CoreModule> CoreModules;
        public void InitializeModules()
        {
            #region old try
            //if (!Directory.Exists(moduleFolder))
            //{
            //    Directory.CreateDirectory(moduleFolder);
            //    return;
            //}
            //string[] directoryNames = Directory.GetDirectories(moduleFolder);




            ////string moduleName = modDirectory.Split('\\').Last();

            //string dllPath =  mainApplicationDirectoryPath + @"\Modules\ActiveTimer\ActiveTimerModule.dll";
            //var assemblyName = AssemblyName.GetAssemblyName(dllPath);

            //var assembly = Assembly.Load(assemblyName);
            //Type type=null;
            //try
            //{

            // type = assembly.GetType("ActiveTimerModule.TestClass");
            //}
            //catch(FileNotFoundException e)
            //{
            //        Debug.WriteLine(e.Message);
            //}


            //var obj = Activator.CreateInstance(type);

            //var method = type.GetMethod("Test");
            //var result = method.Invoke(obj, null);

            //Debug.WriteLine(result);








            //foreach (var modDirectory in directoryNames)
            //{


            //    if (CheckIfDirectoryIsAValidModule(modDirectory))
            //    {
            //        //string moduleName = modDirectory.Split('\\').Last();

            //        //string dllPath = modDirectory + @"\" + moduleName + ".dll";
            //        //var assemblyName = AssemblyName.GetAssemblyName(dllPath);

            //        //var assembly = Assembly.Load(dllPath);
            //        ////var type = assembly.GetType("ActiveTimer.ViewModel.ActiveTimerViewModel");
            //        //var type = assembly.GetType("ActiveTimer.TestClass");

            //        //var obj = Activator.CreateInstance(type);

            //        //var method = type.GetMethod("Test");
            //        //var result = method.Invoke(obj, null);

            //        //Debug.WriteLine(result);
            //    }

            //}
            #endregion
        }
       
        private bool CheckIfDirectoryIsAValidModule(string dirName)
        {
            var files = Directory.GetFiles(dirName);

            string moduleName = dirName.Split('\\').Last();

            foreach (var fn in files)
            {
                if (fn.ToLower().Contains(moduleName.ToLower() + ".dll"))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

            return false;
        }


        public CoreModule GetCoreModule(string moduleName)
        {
            foreach (CoreModule module in CoreModules)
            {
                if (module.ModuleName == moduleName)
                    return module;

            }
            return null;

        }

        public void CloseModules()
        {
            CoreModules.ForEach(e => e.CloseModule());
        }


    }
}
