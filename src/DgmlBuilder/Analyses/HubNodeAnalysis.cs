using System.Collections.Generic;
using OpenSoftware.DgmlTools.Model;
using System.Linq;

namespace OpenSoftware.DgmlTools.Analyses
{
    /// <summary>
    /// Class that sizes each node in the graph relative to the number of edges of the node.
    /// </summary>
    public class HubNodeAnalysis : IGraphAnalysis
    {
        private readonly string _fontGrowExpression;

        public HubNodeAnalysis(string fontGrowExpression = "14+(Count*2)")
        {
            _fontGrowExpression = fontGrowExpression;
        }


        public void Execute(DirectedGraph graph)
        {
            foreach (var node in graph.Nodes)
            {
                var count = graph.Links.Count(x => (x.Source == node.Id || x.Target == node.Id) && x.Category != "Contains");
                if (count > 0)
                {
                    node.Properties.Add("Count", count);
                }
            }
        }

        public IEnumerable<Property> GetProperties(DirectedGraph graph)
        {
            yield return new Property
            {
                Id = "Count",
                DataType = "System.Int32",
                Label = "Count",
                Description = "Count"
            };
        }

        public IEnumerable<Style> GetStyles(DirectedGraph graph)
        {
            yield return new Style
            {
                TargetType = "Node",
                GroupLabel = "Count",
                ValueLabel = "Function",
                Setter = new List<Setter>
                {
                    new Setter {Property = "FontSize", Expression = _fontGrowExpression}
                }
            };
        }
    }
}