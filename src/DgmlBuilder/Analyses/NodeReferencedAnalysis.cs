using System.Linq;
using System.Collections.Generic;
using OpenSoftware.DgmlTools.Model;

namespace OpenSoftware.DgmlTools.Analyses
{
    /// <summary>
    /// Analysis that adds a red cross icon to nodes which are not referenced
    /// </summary>
    public class NodeReferencedAnalysis : GraphAnalysis
    {
        private const string CrossIcon =
                @"pack://application:,,,/Microsoft.VisualStudio.Progression.GraphControl;component/Icons/kpi_red_sym2_large.png"
            ;
            
        public NodeReferencedAnalysis()
        {
            Analysis = PerformNodeReferencedAnalysis;
            Properties = new[]
            {
                new Property
                {
                    Id = "IsReferenced",
                    DataType = "System.Boolean",
                    Label = "IsReferenced",
                    Description = "IsReferenced"
                }
            };
            Styles = new[]
            {
                new Style
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
                }
            };
        }

        private static void PerformNodeReferencedAnalysis(DirectedGraph graph)
        {
            foreach (var node in graph.Nodes)
            {
                var count = graph.Links.Count(x =>
                    x.Target == node.Id && x.Category != "Contains");
                node.Properties.Add("IsReferenced", count > 0);
            }
        }
    }
}