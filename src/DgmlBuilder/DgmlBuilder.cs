using System.Collections.Generic;
using System.Linq;
using OpenSoftware.DgmlTools.Builders;
using OpenSoftware.DgmlTools.Model;

namespace OpenSoftware.DgmlTools
{
    public class DgmlBuilder
    {
        private List<Link> _links = new List<Link>();
        private List<Node> _nodes = new List<Node>();
        private List<Category> _categories = new List<Category>();
        private List<Style> _styles = new List<Style>();

        public IEnumerable<NodeBuilder> NodeBuilders { get; set; }

        public IEnumerable<CategoryBuilder> CategoryBuilders { get; set; }
        public IEnumerable<LinkBuilder> LinkBuilders { get; set; }

        public IEnumerable<StyleBuilder> StyleBuilders { get; set; }

        /// <summary>
        /// Property to access global DGML styles
        /// </summary>
        public List<Style> Styles { get; } = new List<Style>();
        /// <summary>
        /// Property to access global DGML properties.
        /// </summary>
        public List<Property> Properties { get; } = new List<Property>();
        /// <summary>
        /// Creates a new instance of the DgmlBuilder class.
        /// </summary>
        /// <param name="withSizedNodes">Indicates whether the size of a node should reflect the number of connected edges (default true).</param>
        public DgmlBuilder(bool withSizedNodes = true)
        {
            if (withSizedNodes)
            {
                AddDefaultStyles();
                AddDefaultProperties();
            }
        }

        private void AddDefaultProperties()
        {
            // Node count
            Properties.Add(new Property
            {
                Id = "Count",
                DataType = "System.Int32",
                Label = "Count",
                Description = "Count"
            });
        }

        private void AddDefaultStyles()
        {
            Styles.Add(new Style
            {
                TargetType = "Node",
                GroupLabel = "Count",
                ValueLabel = "Function",
                Setter = new List<Setter>
                {
                    new Setter {Property = "FontSize", Expression = "14+(Count*2)",},
                }
            });
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
            _nodes.ForEach(x => graph.Nodes.Add(x));
            _links.ForEach(x => graph.Links.Add(x));
            _categories.ForEach(x => graph.Categories.Add(x));
            _styles.ForEach(x => graph.Styles.Add(x));


            //// Unreferenced nodes
            //graph.Styles.Add(new Style
            //{
            //    TargetType = "Node",
            //    GroupLabel = "Unreferenced",
            //    ValueLabel = "True",
            //    Condition =
            //        new List<Condition>
            //        {
            //            new Condition {Expression = "IsReferenced='false'"},
            //            new Condition {Expression = "HasCategory('Signals')"}
            //        },
            //    Setter =
            //        new List<Setter>
            //        {
            //            new Setter
            //            {
            //                Property = "Icon",
            //                Value =
            //                    "pack://application:,,,/Microsoft.VisualStudio.Progression.GraphControl;component/Icons/kpi_red_sym2_large.png"
            //            }
            //        }
            //});
            graph.Styles = Styles;
            graph.Properties = Properties;

            Annotategraph(graph);
            return graph;
        }

        /// <summary>
        /// Add node counting and is-referenced info to nodes in the graph
        /// </summary>
        /// <param name="graph"></param>
        private void Annotategraph(DirectedGraph graph)
        {
            foreach (var node in graph.Nodes)
            {
                var count = _links.Count(x => (x.Source == node.Id || x.Target == node.Id) && x.Category != "Contains");
                if (count > 0)
                {
                    node.IsReferenced = true;
                    node.Count = count;
                }
                else
                {
                    if (graph.Categories.Any(x => x.Id == node.Id))
                    {
                        node.IsReferenced = true;
                    }
                }
            }
        }

        private static IEnumerable<T> Build<T>(object element, IEnumerable<Builder> builders) where T : class
        {
            var builder = (IBuilder<T>) FindBuilder(builders, element);
            var items = builder?.Build(element);
            if (items == null) yield break;

            foreach (var item in items)
            {
                yield return item;
            }
        }

        private void BuildElements(IEnumerable<object> elements)
        {
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
            foreach (var link in _nodes)
            {
                Build<Style>(link, StyleBuilders).ToList().ForEach(x =>
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
                foreach (var node in _nodes.Where(c => c.Category == category.Id))
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
