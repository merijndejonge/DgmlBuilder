using System;
using System.Collections.Generic;

namespace OpenSoftware.DgmlTools.Model
{
    public interface IGraphAnalysis
    {
        void Execute(DirectedGraph graph);
        IEnumerable<Property> GetProperties(DirectedGraph graph);
        IEnumerable<Style> GetStyles(DirectedGraph graph);
    }
}