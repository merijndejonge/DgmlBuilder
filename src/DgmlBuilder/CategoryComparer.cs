namespace OpenSoftware.DgmlTools;

internal class CategoryComparer : IEqualityComparer<Category>
{
    public bool Equals(Category x, Category y)
    {
        return x.Id == y.Id;
    }

    public int GetHashCode(Category obj)
    {
        return obj.Id.GetHashCode();
    }
}
