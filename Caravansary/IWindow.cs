using System.Windows;

namespace Caravansary
{
    public interface IWindow
    {
        Window GetAssociatedWindow { get; }
        double Top { get; set; }
        double Left { get; set; }
        double Height { get; set; }
        double Width { get; set; }
    }
}
