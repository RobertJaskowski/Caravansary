using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Caravansary
{
    public class NavigationService : INavigation
    {
        public async Task<bool> NavigateToAsync<TPageModel>(object navigationData = null) where TPageModel : PageModelBase
        {
            //1 no wraper
            //2 navigate to basepagemodel and ioc has a lookup table, with GetModel GetWindow
            //3 register both as singleton, do weird name of if here

            var wd = IoC.CreateWindowFor<TPageModel>();

            var ret = await IoC.Get<TPageModel>().InitializeAsync(navigationData);

            wd.Show();

            return ret;
        }
    }
}