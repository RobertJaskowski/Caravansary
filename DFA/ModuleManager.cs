using DFA.CoreModules.ActiveTimer.ViewModel;
using DFA.CoreModules.MainBar.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DFA
{
    public class ModuleManager 
    {
        private static ModuleManager _instance;
        public static ModuleManager Instance {
            get
            {
                if (_instance == null)
                {
                    _instance = new ModuleManager();


                    CoreModules = new List<CoreModule>();

                    CoreModules.Add(new MainBarViewModel());
                    CoreModules.Add(new ActiveTimerViewModel());
                }
                return _instance;
            }
        }


        private static List<CoreModule> CoreModules;
        

        public CoreModule GetCoreModule(string moduleName)
        {
            foreach (CoreModule module in CoreModules)
            {
                if (module.ModuleName == moduleName)
                    return module;

            }
            return null;

        }



    }
}
