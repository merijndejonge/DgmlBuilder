using System;
using System.Collections.Generic;

namespace OpenSoftware.DgmlTools.Model
{
    public class GraphAnalysis
    {
        public Action<DirectedGraph> Analysis { get; set; }
        public IEnumerable<Property> Properties { get; set; } = new List<Property>();
        public IEnumerable<Style> Styles { get; set; } = new List<Style>();
    }
}