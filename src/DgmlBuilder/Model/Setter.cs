using System.Xml.Serialization;

namespace OpenSoftware.DgmlTools.Model
{
    public class Setter
    {
        [XmlAttribute]
        public string Property { get; set; }
        [XmlAttribute]
        public string Value { get; set; }
        [XmlAttribute]
        public string Expression{ get; set; }
    }
}