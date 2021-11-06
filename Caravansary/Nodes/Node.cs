using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Caravansary
{
    public class Node : ObservableObject, IDragable
    {
        private string id;

        public string Id
        {
            get => id; set
            {
                id = value;
                OnPropertyChanged(nameof(Id));
            }
        }

        private string name;

        public string Name
        {
            get => name; set
            {
                name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        private Position position;

        public Position Position
        {
            get => position;
            set
            {
                position = value;
                OnPropertyChanged(nameof(Position));
            }
        }

        private IShape shape;

        public IShape Shape
        {
            get => shape; set
            {
                shape = value;
                OnPropertyChanged(nameof(Shape));
            }
        }

        private List<NodeInput> nodeInputs;

        public List<NodeInput> NodeInputs
        {
            get => nodeInputs; set
            {
                nodeInputs = value;
                OnPropertyChanged(nameof(NodeInputs));
            }
        }

        private List<NodeOutput> nodeOutputs;

        public List<NodeOutput> NodeOutputs
        {
            get => nodeOutputs; set
            {
                nodeOutputs = value;
                OnPropertyChanged(nameof(NodeOutputs));
            }
        }

        public string DataType => "Node";

        public Node(string name, Position position, IShape shape, List<NodeInput> nodeInputs, List<NodeOutput> nodeOutputs)
        {
            id = System.Guid.NewGuid().ToString();

            Name = name;
            Position = position;
            Shape = shape;
            NodeInputs = nodeInputs;
            NodeOutputs = nodeOutputs;
        }

        public void AddNodeInput(NodeInput nodeInput)
        {
            nodeInputs.Add(nodeInput);
        }

        public void Remove(object i)
        {
            throw new NotImplementedException();
        }
    }
}