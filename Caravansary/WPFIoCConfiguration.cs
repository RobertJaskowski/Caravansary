using Caravansary.Core;
using Caravansary.Views;
using Ninject.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caravansary
{
    public class WPFIoCConfiguration : NinjectModule
    {
        public override void Load()
        {
            IoC.RegisterSelfSingleton<MainWindowPageModel, MainWindow>();
            IoC.RegisterSelfSingleton<SettingsWindowViewModel, SettingsWindow>();
            IoC.RegisterSelfSingleton<ModulesListViewModel, ModulesListWindow>();


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
