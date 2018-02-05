using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using OpenSoftware.DgmlTools.Model;

namespace OpenSoftware.DgmlTools.Analyses
{
    /// <summary>
    /// Analysis that gives a specific color to each node of a particular category.
    /// Specify overrides for categories for which color does not satisfy.
    /// </summary>
    public class CategoryColorAnalysis : IGraphAnalysis
    {
        private readonly (string, Color)[] _colorOverrides;

        /// <summary>
        /// Constructor. Specify color overrides as tuple ("category", color). E.g., ("myCategory", Color.Red).
        /// </summary>
        /// <param name="colorOverrides"></param>
        public CategoryColorAnalysis(params (string, Color)[] colorOverrides)
        {
            _colorOverrides = colorOverrides;
        }

        public void Execute(DirectedGraph graph)
        {
        }

        public IEnumerable<Property> GetProperties(DirectedGraph graph)
        {
            return null;
        }

        public IEnumerable<Style> GetStyles(DirectedGraph graph)
        {
            var colorNames = GetColorNames();
            var categoryColorMappings = GetCategoryColorMappings(graph, colorNames, _colorOverrides);

            foreach (var category in categoryColorMappings.Keys)
            {
                yield return new Style
                {
                    TargetType = "Node",
                    GroupLabel = category.Label,
                    ValueLabel = "True",
                    Condition = new List<Condition>
                    {
                        new Condition
                        {
                            Expression = $"HasCategory('{category.Id}')"

                        }
                    },
                    Setter = new List<Setter>
                    {
                        new Setter {Property = "Background", Value = categoryColorMappings[category]}
                    }
                };
            }
        }

        private static Dictionary<Category, string> GetCategoryColorMappings(DirectedGraph graph,
            IReadOnlyList<string> colorNames, IEnumerable<(string, Color)> colorOverrides)
        {
            var categories = graph.Categories.ToArray();
            var categoryColorMappings = new Dictionary<Category, string>();

            for (var i = 0; i < categories.Length; i++)
            {
                categoryColorMappings.Add(categories[i], colorNames[i % colorNames.Count]);
            }
            foreach (var colorOverride in colorOverrides)
            {
                var existingCategory = categories.SingleOrDefault(x => x.Id == colorOverride.Item1);

                if (existingCategory != null)
                {
                    categoryColorMappings[existingCategory] = colorOverride.Item2.Name;
                }
            }
            return categoryColorMappings;
        }


        private static string[] GetColorNames()
        {
            var colorType = typeof(Color);

            const BindingFlags bindingFlags = BindingFlags.Static | BindingFlags.Public | BindingFlags.GetField;
            return colorType.GetProperties(bindingFlags)
                .Select(x => x.GetValue(null))
                .OfType<Color>()
                .Select(x => x.Name)
                .Skip(1) // skip transparant color
                .ToArray();
        }
    }
}
