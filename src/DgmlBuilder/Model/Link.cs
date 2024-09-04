namespace OpenSoftware.DgmlTools.Model;

public partial class Link : ICustomPropertiesProvider
{
    [XmlAttribute]
    public string Source { get; set; }
    [XmlAttribute]
    public string Target { get; set; }
    [XmlAttribute]
    public string Category { get; set; }
    [XmlAttribute]
    public string StrokeThickness { get; set; }
    [XmlAttribute]
    public string Visibility { get; set; }
    [XmlAttribute]
    public string Background { get; set; }
    [XmlAttribute]
    public string Stroke { get; set; }
    [XmlAttribute]
    public string Label { get; set; }
    [XmlAttribute]
    public string Description { get; set; }

    [XmlElement("Category")]
    public List<CategoryRef> CategoryRefs { get; set; } = new List<CategoryRef>();
    
    [XmlIgnore]
    public Dictionary<string, object> Properties { get; } = new Dictionary<string, object>();

    public bool IsContainment { get; set; }
}
