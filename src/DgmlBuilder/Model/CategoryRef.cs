using System.Xml.Serialization;

namespace OpenSoftware.DgmlTools.Model
{
    public class CategoryRef
    {
        [XmlAttribute]
        public string Ref { get; set; }
    }
}