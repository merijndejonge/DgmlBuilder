# DgmlBuilder

## Description
DgmlBuilder is a small `DotNet` library for generating `DGML` graphs without having to know all the details of `DGML` (the Microsoft Directed Graph Markup Language). Visual Studio contains a powerful, interactive `DGML` viewer that is used mostly for displaying code structures of your Visual Studio projects. Despite the powerful viewer, the `DGML` format is not so much in use for other purposes. It is also a bit difficult to use. The aim of `DgmlBuilder` is to make it more easy to represent your (graph-) structured data in `DGML` such that you can use the Visual Studio `DGML` viewer to display, analyze, and interact with your data.

Below are a few example pictures.
### DgmlBuilder for [creating class diagrams](https://github.com/merijndejonge/DgmlBuilder/tree/master/src/examples/TypesVisualizer)
![DgmlBuilder for creating class diagrams](https://raw.githubusercontent.com/merijndejonge/DgmlBuilder/master/src/examples/TypesVisualizer/class-diagram.png)
### DgmlBuilder for [visualizing Structurizr C4 models](https://github.com/merijndejonge/Structurizr.Dgml)
![DgmlBuilder for visualizing Structurizr C4 models](https://raw.githubusercontent.com/merijndejonge/Structurizr.Dgml/master/images/ContosoUniversity.png)

## How it works
`DgmlBuilder` operates on a collection of objects and turns these into collections of nodes, edges, styles, and categories, based on a given set of builders.

### Builders
Builders are specific classes that you write to convert objects in your model to specific graph elements.

The following builders are supported:
* `NodeBuilder` These are builders that construct graph nodes from objects in your model. For instance, if you have a type `Component` in your model with `Name`,  `Id`, and `ComponentType` properties, you can transform these into corresponding nodes with the following node builder:
    ```csharp
    new NodeBuilder<Component>(
        x => new Node 
            {
                Id = x.Id,
                Label = x.Name,
                Category = x.ComponentType
            });
    ```
    As we'll see shortly, you can define multiple node builders. For instance, if your model also contains `Interface` as type, you can instantiate a specific node builder for this type too.
* `LinkBuilder` These are builders for constructing links in your graph. For instance, if there is a `Call` relation in your model to represent method calls from one component to another, you could instantiate an link builder as follows:
```csharp
    new LinkBuilder<Call>(
        x => new Link
            {
                Source = x.Source,
                Target = x.Target
            });
```
If a link denotes a containment relation, set the attribute `IsContainment` to true. `DgmlBuilder` will use this information to place the target node as child of the source node.
* `CategoryBuilder` These are builders for grouping nodes into categories. Categories make it easy to style all nodes of a category consistently.
* `StyleBuilder` These are builders for applying visual styles to your nodes and edges. For example, to use different background colors for components and interfaces.

While each of the above builder types generates a single node, there are also methods available to generate multiple nodes at once. E.g., you can create an instance of `NodesBuilder` for elements in your model for which you want multiple nodes in the resulting graph. Likewise, to generate multiple links at once, you can instantiate the `LinksBuilder` class.

Each builder has an optional `accept` parameter that can be used to specify when the builder should be applied to an element. For example, in case of the aforementioned node builder, we could add an `accept` parameter as follows:
```csharp
new NodeBuilder<Component>(
    x => new Node 
        {
            Id = x.Id,
            Label = x.Name,
            Category = x.ComponentType
        },
    x => x.IsPublic);
```
In this example, the property `IsPublic` of a component is used to specify that the builder should only be applied to public components.
## Using DgmlBuilder
To use `DgmlBuilder`, you instantiate `DgmlBuilder` with the builders you need. The basic setup is like this:
```csharp
var builder = new DgmlBuilder
{
    NodeBuilders = new NodeBuilder[]
    {
        <your node builders>
    },
    LinkBuilders = new LinkBuilder[]
    {
        <your link builders>
    },
    CategoryBuilders = new CategoryBuilder[]
    {
        <your category builders>
    },
    StyleBuilders = new StyleBuilder[]
    {
        <your style builders>
    }
};
```
As you can see, all builder properties of `DgmlBuilder` are collections, allowing you to specify multiple builders for each.

The `DgmlBuilder` class supports two `Build` methods:
* The first, simply accepts a collection of objects:
    ```csharp
    public DirectedGraph Build(IEnumerable<object> elements)
    ```
* The second supports multiple collections:
    ```csharp
    public DirectedGraph Build(params IEnumerable<object>[] elements)`
    ```
Both build methods will apply the configured builders to all elements and produces a `DGML` graph as output.

For example, if you have your components, interfaces, and calls contained in separate collections, you can generate the corresponding `DGML` graph as follows:
```csharp
var graph = builder.Build(components, interfaces, calls);
```

The resulting `DGML` graph is a serializable object. This means that with the standard .NET serializers you can save the corresponding graph to disk. E.g., as follows:
```csharp
using (var writer = new StreamWriter("my-graph.dgml"))
{
    var serializer = new XmlSerializer(typeof(DirectedGraph));
    serializer.Serialize(writer, graph);
}
```
There is also a convenient method available that does exactly this:
```csharp
graph.WriteToFile("my-graph.dgml");
```
You can now open the file `my-graph.dgml` in the `DGML` viewer of Visual Studio to inspect and analyze the graph.
## Graph analyses
The graphs produced by `DgmlBuilder` so far are pretty straight-forward graphs with nodes and edges. However, more powerful graphs can be generated by having `DgmlBuilder` execute graph analyses. These graph analyses can be 
passed as constructor arguments to `DgmlBuilder`. The `DgmlBuilder` library contains three pre-defined analyses: 
* `HubNodeAnalysis` resizes each node depending on the number of edges connected to that node. This gives a clear visual representation of important nodes in your graphs. 
* `NodeReferencedAnalysis` marks unreferenced nodes in the graph with a special color and icon.
* `CategoryColorAnalysis` automatically colors all the nodes of a category. Colors can be overridden by specifying specific color mappings to the constructor of the analysis.

To use any of these or your own analyses, pass them to the constructor of `DgmlBuilder`. E.g.,
```csharp
var builder = new DgmlBuilder(new HubNodeAnalysis(), new NodeReferencedAnalysis(), new CategoryColorAnalysis());
``` 
## Create custom analyses
To create your own analysis, create a class that implements `IGraphAnalysis`. This interface has the following three methods:
```csharp
void Execute(DirectedGraph graph);
IEnumerable<Property> GetProperties(DirectedGraph graph);
IEnumerable<Style> GetStyles(DirectedGraph graph);
```
Below we discuss these methods in more detail.
### Specifying your analysis action 
The first method defines your analysis action:
```csharp
void Execute(DirectedGraph graph);
```
It receives a `DGML` graph as input. The analysis method typically decorates this graph with additional attributes. The `DGML` model as defined in `DgmlBuilder` supports adding your custom properties to edges and nodes. These will be converted to XML attributes during serialization. To add a custom property with name `foo` and value `bar` to a node `n` you can use the following code:
```csharp
n.Properties.Add("foo", "bar");
```
To use such newly created properties in DGML (e.g., by referencing them in styles), you first have to register the property in `DGML` (see below). As an example, the analysis action of `NodeReferencedAnalysis` is defined as follows:
```csharp
foreach (var node in graph.Nodes)
{
    var isReferenced = graph.Links.Any(x =>
        x.Target == node.Id && x.Category != "Contains");
    node.Properties.Add("IsReferenced", isReferenced);
}
```
It iterates over each node in the graph and adds the property `IsReferenced`. Note that containment links are ignored.

The `Execute` method is calle before the other two methods. This is to enable that list of properties and styles can be dependent on the result of the analysis.
### Defining custom properties
Your analysis typically decorates the graph with properties which are not present in `DGML`. With `Properties`, you can declare such additional properties to `DGML`:
```csharp
IEnumerable<Property> GetProperties(DirectedGraph graph);
```
For example, `NodeReferencedAnalysis` adds a boolean to mark a node as referenced. To that end, the following property is declared:
```csharp
new Property
{
    Id = "IsReferenced",
    DataType = "System.Boolean",
    Label = "IsReferenced",
    Description = "IsReferenced"
}
```
This declares the property `IsReferenced` of type `System.Boolean`. Since `GetProperties` is called after `Execute`, the generated list of properties can be dependent on the outcome of the analysis action.
### Adding styles for your analysis
Your analysis will typically not only decorate the graph with computed properties, but also add styles to the graph to give some visual appearance of the analysis. With the `GetStyles` method any additional styles can be added:
```csharp
IEnumerable<Style> GetStyles(DirectedGraph graph)
```
Styles are defined as `DGML` styles. E.g., `NodeReferencedAnalysis` registers the following conditional style:
```csharp
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
                Value =  @"pack://application:,,,/Microsoft.VisualStudio.Progression.GraphControl;component/Icons/kpi_red_sym2_large.png"
            }
        }
}
```
This style adds an icon to nodes for which the property `IsReferenced` equals `false`. Since `GetStyles` is called after `Execute`, the generated list of properties can be dependent on the outcome of the analysis action.

More information about creating custom (conditional) styles can be found at https://docs.microsoft.com/en-us/visualstudio/modeling/customize-code-maps-by-editing-the-dgml-files.
## Examples
The
[GitHub](https://github.com/merijndejonge/DgmlBuilder/tree/master/src/examples/TypesVisualizer) repository of `DgmlBuilder` contains [`TypesVisualizer`](https://github.com/merijndejonge/DgmlBuilder/tree/master/src/examples/TypesVisualizer), which is a small tool to capture the structure and relationships of collections of types in a DGML graph. This tool demonstrates most features of DgmlBuilder.

## More info
* Source code of `DgmlBuilder` is available at [GitHub](https://github.com/merijndejonge/DgmlBuilder). Nuget packages are available at [Nuget.org](https://www.nuget.org/packages/OpenSoftware.DgmlBuilder/).
* `DgmlBuilder` is distributed under the [Apache 2.0 License](https://github.com/merijndejonge/DgmlBuilder/blob/master/LICENSE).
* [Directed Graph Markup Language (DGML) reference](https://docs.microsoft.com/en-us/visualstudio/modeling/directed-graph-markup-language-dgml-reference).
