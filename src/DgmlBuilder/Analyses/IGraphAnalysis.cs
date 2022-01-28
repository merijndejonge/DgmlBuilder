namespace OpenSoftware.DgmlTools.Analyses;

public interface IGraphAnalysis
{
    void Execute(DirectedGraph graph);
    IEnumerable<Property> GetProperties(DirectedGraph graph);
    IEnumerable<Style> GetStyles(DirectedGraph graph);
}
