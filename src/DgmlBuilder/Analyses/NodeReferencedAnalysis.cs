using System.Linq;
using System.Collections.Generic;
using OpenSoftware.DgmlTools.Model;

namespace OpenSoftware.DgmlTools.Analyses
{
    /// <summary>
    /// Analysis that adds a red cross icon to nodes which are not referenced
    /// </summary>
    public class NodeReferencedAnalysis : IGraphAnalysis
    {
        private const string CrossIcon =
                @"pack://application:,,,/Microsoft.VisualStudio.Progression.GraphControl;component/Icons/kpi_red_sym2_large.png"
            ;

        public void Execute(DirectedGraph graph)
        {
            foreach (var node in graph.Nodes)
            {
                var isReferenced = graph.Links.Any(x =>
                    x.Target == node.Id && x.Category != "Contains");
                node.Properties.Add("IsReferenced", isReferenced);
            }
        }

        public IEnumerable<Property> GetProperties(DirectedGraph graph)
        {
            yield return new Property
            {
                Id = "IsReferenced",
                DataType = "System.Boolean",
                Label = "IsReferenced",
                Description = "IsReferenced"
            };
        }

        public IEnumerable<Style> GetStyles(DirectedGraph graph)
        {
            yield return new Style
            {
                TargetType = "Node",
                GroupLabel = "Unreferenced",
                ValueLabel = "True",
                Condition =
                    new List<Condition>
                    {
                        new Condition {Expression = "IsReferenced='false'"}
                    },
                Setter =
                    new List<Setter>
                    {
                        new Setter
                        {
                            Property = "Icon",
                            Value = CrossIcon
                        }
                    }
            };
        }
    }
}