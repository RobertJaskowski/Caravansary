using Caravansary.Core;
using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Caravansary
{
    public static class IoC
    {
        public static StandardKernel _kernel;

        private static Dictionary<Type, Type> registrationModelWindow = new();

        public static void RegisterSelfSingleton<TPageModel, TWindow>()
        {
            if (!registrationModelWindow.TryGetValue(typeof(TWindow), out Type val))
            {
                registrationModelWindow.Add(typeof(TPageModel), typeof(TWindow));

                _kernel.Bind<TPageModel>().ToSelf().InSingletonScope();
                _kernel.Bind<TWindow>().ToSelf().InSingletonScope();
            }
        }

        public static Window CreateWindowFor<TPageModelType>() where TPageModelType : PageModelBase
        {
            Type pageModelType = typeof(TPageModelType);
            var pageType = registrationModelWindow[pageModelType];
            var page = (Window)Activator.CreateInstance(pageType);
            var pageModel = Resolve<TPageModelType>();
            page.DataContext = pageModel;
            return page;
        }

        public static T Resolve<T>() where T : class
        {
            try
            {
                return _kernel.Get<T>();
            }
            catch (Exception e)
            {
            }
            return default(T);
        }

        public static T Get<T>()
        {
            return _kernel.Get<T>();
        }

        public static void Setup()
        {
            if (_kernel == null)
                _kernel = new StandardKernel(new IocConfiguration());
        }
    }
}