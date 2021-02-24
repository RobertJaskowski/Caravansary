using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DFA
{
    public interface IView
    {
        Window window { get;  }
        double Top { get; set; }
        double Left { get; set; }
        double Height { get; set; }
        double Width { get; set; }
    }
}
