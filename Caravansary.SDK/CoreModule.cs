using System.Windows.Controls;


public abstract class CoreModule : BaseViewModel
{
    public abstract string ModuleName { get; }

    public abstract UserControl View { get; set; }
    
}
