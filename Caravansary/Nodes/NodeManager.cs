using System.Collections.Generic;

namespace Caravansary
{
    public class NodeManager
    {
        public List<Node> nodes;
        public List<Connection> connections;

        public NodeManager()
        {
            nodes = new List<Node>();
            connections = new List<Connection>();
        }

        public bool CanCreateConnection(NodeOutput from, NodeInput to)
        {
            return to.CanAccept(from);
        }

        public void CreateConnection(NodeOutput from, NodeInput to)
        {
            if (!CanCreateConnection(from, to)) return;
        }

        public List<Connection> GetOutgoingFromId(string id)
        {
            return null;
        }

        public List<Connection> GetIncomingInId(string id)
        {
            return null;
        }
    }
}