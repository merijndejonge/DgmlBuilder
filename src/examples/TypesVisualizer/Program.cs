using System.Linq;
using System.Runtime.CompilerServices;
using OpenSoftware.DgmlTools.Reflection;
using OpenSoftware.DgmlTools.Model;

namespace OpenSoftware.DgmlTools
{
    internal static class Program
    {
        private static void Main()
        {
            var assemblyPaths = new[] {@"OpenSoftware.DgmlBuilder.dll"};
            
            var excludeFilters = new IExcludeFilter[]
            {
                new ExcludeByNameSpace(x => x.Contains("OpenSoftware") == false),
                new ExcludeByNameSpace(x => x.EndsWith(".Annotations")),
                new ExcludeByCustomAttribute<CompilerGeneratedAttribute>()
            };
            using var loader = new TypesLoader(assemblyPaths, excludeFilters);
            var types = loader.Load().ToArray();
            var graph = TypesVisualizer.Types2Dgml(types);

            //graph.GraphDirection = GraphDirection.LeftToRight;
            //graph.Layout = Layout.Sugiyama;
            //graph.NeighborhoodDistance = 1;

            graph.WriteToFile(@"../../../class-diagram.dgml");
        }
    }
}
