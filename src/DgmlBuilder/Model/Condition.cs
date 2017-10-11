using System.Xml.Serialization;

namespace OpenSoftware.DgmlTools.Model
{
    public class Condition
    {
        [XmlAttribute]
        public string Expression { get; set; }
    }
}