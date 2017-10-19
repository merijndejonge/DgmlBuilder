using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace OpenSoftware.DgmlTools.Model
{
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
}