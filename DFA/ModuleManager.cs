using DFA.CoreModules.ActiveTimer.ViewModel;
using DFA.CoreModules.DailyGoal.ViewModel;
using DFA.CoreModules.Filler.ViewModel;
using DFA.CoreModules.KeyCounter.ViewModel;
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

                    CoreModules.Add(new KeyCounterViewModel());
                    CoreModules.Add(new ActiveTimerViewModel());
                    CoreModules.Add(new DailyGoalViewModel());


                    CoreModules.Add(new FillerViewModel());


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

        internal void CloseModules()
        {
            CoreModules.ForEach(e => e.CloseModule());
        }
    }
}
