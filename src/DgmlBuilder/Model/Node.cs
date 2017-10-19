using System.Collections.Generic;
using System.Xml.Serialization;

namespace OpenSoftware.DgmlTools.Model
{
    public partial class Node : ICustomPropertiesProvider
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
        public string Reference { get; set; }

        public Dictionary<string, object> Properties { get; } = new Dictionary<string, object>();
    }
}