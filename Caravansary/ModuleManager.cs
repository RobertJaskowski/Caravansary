using Caravansary.CoreModules.ActiveTimer.ViewModel;
using Caravansary.CoreModules.DailyGoal.ViewModel;
using Caravansary.CoreModules.Filler.ViewModel;
using Caravansary.CoreModules.KeyCounter.ViewModel;
using Caravansary.CoreModules.MainBar.ViewModel;
using Caravansary.CoreModules.Roadmap.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caravansary
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


                    CoreModules.Add(new RoadmapViewModel());


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
