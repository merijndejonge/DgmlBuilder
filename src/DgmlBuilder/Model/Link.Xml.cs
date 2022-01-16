namespace OpenSoftware.DgmlTools.Model;

public partial class Link : IXmlSerializable
{
    public XmlSchema GetSchema()
    {
        return null;
    }

    public void ReadXml(XmlReader reader)
    {
        throw new System.NotImplementedException();
    }

    public void WriteXml(XmlWriter writer)
    {
        this.ToXml(writer);
    }
}
