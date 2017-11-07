namespace OpenSoftware.DgmlTools.Model
{
    public static class LinkExtensions
    {
        public static bool HasCategory(this Link link, string category)
        {
            return link.Category == category;
        }
    }
}
