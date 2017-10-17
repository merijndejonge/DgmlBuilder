using System.Xml.Serialization;

namespace OpenSoftware.DgmlTools.Model
{
    public class Link
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
    }
}