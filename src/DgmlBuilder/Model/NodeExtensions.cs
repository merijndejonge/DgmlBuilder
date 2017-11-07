using System.Linq;

namespace OpenSoftware.DgmlTools.Model
{
    public static class NodeExtensions
    {
        public static bool HasCategory(this Node node, string category)
        {
            if (node.Category == category) return true;
            if (node.CategoryRefs == null) return false;
            return node.CategoryRefs.Any(x => x.Ref == category);
        }
    }
}