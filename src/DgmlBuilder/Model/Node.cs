using System.Collections.Generic;
using System.Xml.Serialization;

namespace OpenSoftware.DgmlTools.Model
{
    public class Node
    {
        [XmlAttribute]
        public string Id { get; set; }
        [XmlAttribute]
        public string Label { get; set; }
        [XmlAttribute]
        public string Category { get; set; }
        [XmlAttribute]
        public string Group { get; set; }

        [XmlElement("Category")]
        public List<CategoryRef> CategoryRefs { get; set; }
        [XmlAttribute]
        public bool IsReferenced{ get; set; }
        [XmlAttribute]
        public int Count{ get; set; }
        [XmlAttribute]
        public string Reference { get; set; }
    }
}