using System.Collections.Generic;
using System.Xml.Serialization;

namespace OpenSoftware.DgmlTools.Model
{
    public class Style
    {
        [XmlAttribute]
        public string TargetType { get; set; }
        [XmlAttribute]
        public string GroupLabel { get; set; }
        [XmlAttribute]
        public string ValueLabel { get; set; }
        [XmlElement]
        public List<Condition> Condition { get; set; }
        [XmlElement]
        public List<Setter> Setter { get; set; }
    }
}