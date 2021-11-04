using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Caravansary
{
    public interface INavigation
    {
        Task<bool> NavigateToAsync<TPageModel>(object navigationData = null)
            where TPageModel : PageModelBase;
    }
}