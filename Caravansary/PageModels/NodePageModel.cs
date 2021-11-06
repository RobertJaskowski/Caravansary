using Caravansary.SDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caravansary
{
    public class NodePageModel : PageModelBase, IDropable
    {
        private TrulyObservableCollection<Node> _nodes;

        public TrulyObservableCollection<Node> Nodes
        {
            get { return _nodes; }
            set
            {
                _nodes = value;
                OnPropertyChanged(nameof(Nodes));
            }
        }

        public string DataType => "Node";

        public NodePageModel()
        {
            Nodes = new TrulyObservableCollection<Node>();

            Nodes.Add(new Node("test1", new Position(50, 50), new Rectangle { Height = 50, Width = 50 }, null, null));
            Nodes.Add(new Node("test2", new Position(0, 0), new Rectangle { Height = 50, Width = 50 }, null, null));
            OnPropertyChanged(nameof(Nodes));
        }

        public void Drop(object data, int index = -1)
        {
        }
    }
}