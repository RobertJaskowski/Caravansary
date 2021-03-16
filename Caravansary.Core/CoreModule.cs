namespace Caravansary.Core
{
    public abstract class CoreModule : BaseViewModel
    {
        public abstract string ModuleName { get; }


        public abstract void CloseModule();
    }
}
