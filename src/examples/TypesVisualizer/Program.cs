using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;
using OpenSoftware.DgmlTools.Model;
using OpenSoftware.DgmlTools.Reflection;

namespace OpenSoftware.DgmlTools
{
    internal class Program
    {
        private static void Main()
        {
            var assemblyPaths = new[] {@".\OpenSoftware.DgmlBuilder.dll"};
            var excludeFilters = new IExcludeFilter[]
            {
                new ExcludeByNameSpace(x => x.Contains("OpenSoftware") == false),
                new ExcludeByNameSpace(x => x.EndsWith(".Annotations")),
                new ExcludeByCustomAttribute<CompilerGeneratedAttribute>()
            };
            using (var loader = new TypesLoader(assemblyPaths, excludeFilters))
            {
                var types = loader.Load().ToArray();
                var graph = TypesVisualizer.Types2Dgml(types);

                using (var writer = new StreamWriter(@"../../class-diagram.dgml"))
                {
                    var serializer = new XmlSerializer(typeof(DirectedGraph));
                    serializer.Serialize(writer, graph);
                }
            }
        }
    }
}
