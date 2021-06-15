using Caravansary.Core.Services;
using Ninject.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caravansary.Core
{
    public class IocConfiguration : NinjectModule
    {
        public override void Load()
        {



            Bind<INavigation>().To<NavigationService>().InSingletonScope();
            

        }
    }
}
