namespace OpenSoftware.DgmlTools.Model;

public class Category
{
    [XmlAttribute]
    public string Id { get; set; }
    [XmlAttribute]
    public string Label { get; set; }
    [XmlAttribute]
    public string Background { get; set; }
    [XmlAttribute]
    public string Icon { get; set; }
}
