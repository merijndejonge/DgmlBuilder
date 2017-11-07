using System.IO;
using System.Xml.Serialization;

namespace OpenSoftware.DgmlTools.Model
{
    public static class DirectedGraphExtensions
    {
        /// <summary>
        /// Writes the graph to file.
        /// </summary>
        /// <param name="graph"></param>
        /// <param name="fileName"></param>
        public static void WriteToFile(this DirectedGraph graph, string fileName)
        {
            using (var writer = new StreamWriter(fileName))
            {
                var serializer = new XmlSerializer(typeof(DirectedGraph));
                serializer.Serialize(writer, graph);
            }
        }
    }
}
