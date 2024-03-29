namespace OpenSoftware.DgmlTools.Model;

public class Property
{
    [XmlAttribute]
    public string Id { get; set; }
    [XmlAttribute]
    public string DataType { get; set; }
    [XmlAttribute]
    public string Label { get; set; }
    [XmlAttribute]
    public string Description { get; set; }
}
