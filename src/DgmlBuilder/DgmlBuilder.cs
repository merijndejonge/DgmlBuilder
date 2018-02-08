using System;
using System.Collections.Generic;
using System.Linq;
using OpenSoftware.DgmlTools.Builders;
using OpenSoftware.DgmlTools.Model;

namespace OpenSoftware.DgmlTools
{
    public class DgmlBuilder
    {
        private readonly List<IGraphAnalysis> _graphAnalyses;
        private List<Link> _links = new List<Link>();
        private List<Node> _nodes = new List<Node>();
        private List<Category> _categories = new List<Category>();
        private List<Style> _styles = new List<Style>();

        public IEnumerable<NodeBuilder> NodeBuilders { get; set; }
        public IEnumerable<CategoryBuilder> CategoryBuilders { get; set; }
        public IEnumerable<LinkBuilder> LinkBuilders { get; set; }
        public IEnumerable<StyleBuilder> StyleBuilders { get; set; }

        
        /// <summary>
        /// Creates a new instance of the DgmlBuilder class.
        /// </summary>
        /// <param name="graphAnalyses">optional collection of graph analysis to apply</param>
        public DgmlBuilder(params IGraphAnalysis[] graphAnalyses)
        {
            _graphAnalyses = graphAnalyses.ToList();
        }

        /// <summary>
        /// Buid a DgmlGraph from a single set of elements
        /// </summary>
        /// <param name="elements"></param>
        /// <returns></returns>
        public DirectedGraph Build(IEnumerable<object> elements)
        {
            BuildElements(elements);
            return BuildGraph();
        }
        /// <summary>
        /// Build a DgmlGraph from multiple sets of elements
        /// </summary>
        /// <param name="elements"></param>
        /// <returns></returns>
        public DirectedGraph Build(params IEnumerable<object>[] elements)
        {
            var allElements = elements.SelectMany(x => x).ToArray();
            BuildElements(allElements);
            return BuildGraph();
        }

        private DirectedGraph BuildGraph()
        {
            var graph = new DirectedGraph();

            _nodes.ForEach(graph.Nodes.Add);
            _links.ForEach(graph.Links.Add);
            _categories.ForEach(graph.Categories.Add);
            _styles.ForEach(graph.Styles.Add);

            _graphAnalyses.ForEach(x => ExecuteAnalysis(x, graph));
            
            return graph;
        }

        private static void ExecuteAnalysis(IGraphAnalysis analysis, DirectedGraph graph)
        {
            analysis.Execute(graph);
            analysis.GetStyles(graph)?.ToList().ForEach(graph.Styles.Add);
            analysis.GetProperties(graph)?.ToList().ForEach(graph.Properties.Add);            
        }

        private static IEnumerable<T> Build<T>(object element, IEnumerable<Builder> builderCollection) where T : class
        {
            var builders = FindBuilders(builderCollection, element).OfType<IBuilder<T>> ();
            foreach (var builder in builders)
            {
                if (builder.Accept(element) == false) continue;
                var items = builder.Build(element);
                if (items == null) continue;
                foreach (var item in items.Where(x => x != null))
                {
                    yield return item;
                }
            }
        }

        private void BuildElements(IEnumerable<object> elements)
        {
            _nodes.Clear();
            _links.Clear();
            _categories.Clear();
            _styles.Clear();

            foreach (var element in elements)
            {
                Build<Node>(element, NodeBuilders).ToList().ForEach(_nodes.Add);
                Build<Link>(element, LinkBuilders).ToList().ForEach(_links.Add);
                Build<Category>(element, CategoryBuilders).ToList().ForEach(_categories.Add);
            }
            _nodes = _nodes.Distinct(new NodeComparer()).ToList();
            _links = _links.Distinct(new LinkComparer()).ToList();
            _categories = _categories.Distinct(new CategoryComparer()).ToList();

            BuildContainmentForCategories();

            // Apply styles to links and nodes
            foreach (var link in _links.Where(x => x.Category != "Contains"))
            {
                Build<Style>(link, StyleBuilders).ToList().ForEach(x =>
                {
                    x.Condition = new List<Condition>
                    {
                        new Condition {Expression = "HasCategory('" + x.GroupLabel + "')"}
                    };
                    x.TargetType = "Link";
                    _styles.Add(x);
                });
            }
            foreach (var node in _nodes)
            {
                Build<Style>(node, StyleBuilders).ToList().ForEach(x =>
                {
                    x.Condition = new List<Condition>
                    {
                        new Condition {Expression = "HasCategory('" + x.GroupLabel + "')"}
                    };
                    x.TargetType = "Node";
                    _styles.Add(x);
                });
            }
            _styles = _styles.Distinct(new StyleComparer()).ToList();
            _nodes = _nodes.Distinct(new NodeComparer()).ToList();
            _links = _links.Distinct(new LinkComparer()).ToList();
        }

        /// <summary>
        /// Create categories and nodes for categories
        /// </summary>
        private void BuildContainmentForCategories()
        {

            foreach (var link in _links.Where(x => x.IsContainment))
            {
                var parent = _nodes.SingleOrDefault(x => x.Id == link.Source);
                if (parent == null)
                {
                    parent = new Node
                    {
                        Id = link.Source
                    };
                    _nodes.Add(parent);
                }

                parent.Group = "Expanded";
                link.Category = "Contains";
                link.Visibility = "Hidden";
            }
        }

        private static IEnumerable<Builder> FindBuilders(IEnumerable<Builder> builders, object element)
        {
            if (builders == null) yield break;
            var type = element.GetType();
            var matchingBuilders =builders.Where(x => x.Type.IsAssignableFrom(type));
            foreach (var matchingBuilder in matchingBuilders)
            {
                yield return matchingBuilder;
            }
        }
    }
}
