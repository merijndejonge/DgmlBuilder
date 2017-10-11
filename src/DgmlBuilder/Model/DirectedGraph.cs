using System.Collections.Generic;
using System.Xml.Serialization;

namespace OpenSoftware.DgmlTools.Model
{
    [XmlRoot("DirectedGraph", Namespace = "http://schemas.microsoft.com/vs/2009/dgml")]
    public class DirectedGraph
    {
        public DirectedGraph()
        {
            Nodes = new List<Node>();
            Links = new List<Link>();
            Styles = new List<Style>();
            Categories=new List<Category>();
            Properties=new List<Property>();
        }

        [XmlAttribute]
        public bool DataVirtualized { get; set; }

        public List<Node> Nodes { get; set; }
        public List<Link> Links { get; set; }
        public List<Category> Categories { get; set; }
        public List<Style> Styles { get; set; }
        public List<Property> Properties { get; set; }
    }
}