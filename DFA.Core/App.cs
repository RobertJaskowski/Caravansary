using MvvmCross.IoC;
using MvvmCross.ViewModels;
using DFA.Core.ViewModels;

namespace DFA.Core
{
    public class App : MvxApplication
    {
        public override void Initialize()
        {
            CreatableTypes()
                .EndingWith("Service")
                .AsInterfaces()
                .RegisterAsLazySingleton();
            RegisterAppStart<TipViewModel>();
        }
    }
}