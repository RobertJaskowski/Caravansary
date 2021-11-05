using Caravansary.SDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caravansary
{
    public class NodePageModel : PageModelBase
    {
        private TrulyObservableCollection<IShape> _shapes;

        public TrulyObservableCollection<IShape> Shapes
        {
            get { return _shapes; }
            set
            {
                _shapes = value;
                OnPropertyChanged(nameof(Shapes));
            }
        }

        public NodePageModel()
        {
            Shapes = new TrulyObservableCollection<IShape>();

            Shapes.Add(new Rectangle { Top = 50, Left = 50, Height = 50, Width = 50 });
            Shapes.Add(new Circle { Top = 100, Left = 100, Radius = 50 });
        }
    }
}