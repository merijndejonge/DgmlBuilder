namespace OpenSoftware.DgmlTools.Comparers;

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
