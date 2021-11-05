using System.ComponentModel;

namespace Caravansary
{
    public interface IShape : INotifyPropertyChanged
    {
        int Top { get; set; }
        int Left { get; set; }
    }
}