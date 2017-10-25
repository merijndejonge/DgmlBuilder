using System.Collections.Generic;
using System.Linq;
using OpenSoftware.DgmlTools.Builders;
using OpenSoftware.DgmlTools.Model;

namespace OpenSoftware.DgmlTools
{
    public class DgmlBuilder
    {
        private readonly List<GraphAnalysis> _graphAnalyses;
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
        public DgmlBuilder(params GraphAnalysis[] graphAnalyses)
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

        private static void ExecuteAnalysis(GraphAnalysis analysis, DirectedGraph graph)
        {
            analysis.Styles.ToList().ForEach(s => graph.Styles.Add(s));
            analysis.Properties.ToList().ForEach(p => graph.Properties.Add(p));
            analysis.Analysis(graph);
        }

        private static IEnumerable<T> Build<T>(object element, IEnumerable<Builder> builders) where T : class
        {
            var builder = (IBuilder<T>) FindBuilder(builders, element);
            var items = builder?.Build(element);
            if (items == null) yield break;

            foreach (var item in items.Where(x => x != null))
            {
                yield return item;
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
            foreach (var category in _categories)
            {
                // Construct categories for this category
                var categoryCategories = Build<Category>(category, CategoryBuilders).ToList();
                _nodes.Add(new Node
                {
                    Id = category.Id,
                    Group = "Expanded",
                    Category = categoryCategories.Any() ?  categoryCategories.First().Id : null,
                    CategoryRefs = categoryCategories.Any()?  categoryCategories.Skip(1).Select(x => new CategoryRef { Ref = x.Id }).ToList() : null
                });

                categoryCategories.ForEach(x =>
                {
                    _nodes.Add(new Node { Id = x.Id, Group = "Expanded" });
                    _categories.Add(x);
                });
            }
            // create links betwen nodes and categories
            foreach (var category in _categories)
            {
                foreach (var node in _nodes.Where(c => c.Category == category.Id || c.CategoryRefs != null && c.CategoryRefs.Any(x=>x.Ref == category.Id)))
                {
                    _links.Add(new Link
                    {
                        Source = category.Id,
                        Target = node.Id,
                        Category = "Contains",
                        Visibility = "Hidden"
                    });
                }
            }
        }

        private static Builder FindBuilder(IEnumerable<Builder> builders, object element)
        {
            if (builders == null) return null;
            var type = element.GetType();
            return builders.FirstOrDefault(x => x.Type.IsAssignableFrom(type));
        }
    }
}
