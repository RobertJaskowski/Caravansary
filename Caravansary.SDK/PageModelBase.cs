
using System.ComponentModel;
using System.Threading.Tasks;

public class PageModelBase : ObservableObject
{

    public virtual async Task<bool> InitializeAsync(object navigationdata = null)
    {
        return true;
    }
   
   
}
