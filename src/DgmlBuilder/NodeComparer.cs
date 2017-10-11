using System.Collections.Generic;
using OpenSoftware.DgmlTools.Model;

namespace OpenSoftware.DgmlTools
{
    internal class NodeComparer : IEqualityComparer<Node>
    {
        public bool Equals(Node x, Node y)
        {
            return x.Id == y.Id;
        }

        public int GetHashCode(Node obj)
        {
            return obj.Id.GetHashCode();
        }
    }
}