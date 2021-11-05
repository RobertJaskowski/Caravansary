using Caravansary.Pages;
using Caravansary.SDK;
using Caravansary.Views;
using Ninject.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caravansary
{
    internal class WPFConfiguration : NinjectModule
    {
        public override void Load()
        {
            Bind<INavigation>().To<NavigationService>().InSingletonScope();
            Bind<IModuleController>().To<ModuleController>().InSingletonScope();

            IoC.RegisterSelfSingleton<MainWindowPageModel, MainWindow>();
            IoC.RegisterSelfSingleton<SettingsWindowViewModel, SettingsWindow>();
            IoC.RegisterSelfSingleton<ModulesListViewModel, ModulesListWindow>();
            IoC.RegisterSelfSingleton<NodePageModel, NodeWindow>();

            //Bind<MainWindow>().ToSelf().InSingletonScope();
            //Bind<MainWindowPageModel>().ToSelf().InSingletonScope();

            //Bind<SettingsWindow>().ToSelf().InSingletonScope();
            //Bind<SettingsWindowViewModel>().ToSelf().InSingletonScope();

            //Bind<ModulesListWindow>().ToSelf().InSingletonScope();
            //Bind<ModulesListViewModel>().ToSelf().InSingletonScope();

            Bind<TrayIcon>().ToSelf().InSingletonScope();
        }
    }
}