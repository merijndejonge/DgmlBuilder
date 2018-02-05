using System;
using System.Collections.Generic;

namespace OpenSoftware.DgmlTools.Model
{
    public class GraphAnalysis
    {
        public Action<DirectedGraph> Analysis { get; set; }
        public Func<DirectedGraph,IEnumerable<Property>> GetProperties { get; set; }
        public Func<DirectedGraph, IEnumerable<Style>> GetStyles { get; set; }
    }
}