using System;

namespace Caravansary
{
    public class NodeInput : NodeInterface, IDropable
    {
        string IDropable.DataType => Type;

        public void Trigger()
        {
        }

        public bool CanAccept(NodeOutput to)
        {
            return true;
        }

        public void Remove(object i)
        {
            throw new System.NotImplementedException();
        }

        public void Drop(object data, int index = -1)
        {
            IoC.Get<NodeManager>().CreateConnection(data as NodeOutput, this);
        }
    }
}