# DgmlBuilder

## Description
DgmlBuilder is a small `DotNet` library for generating `DGML` graphs without having to know all the details of `DGML` (the Microsoft Directed Graph Markup Language). Visual Studio contains a powerful, interactive `DGML` viewer that is used mostly for displaying code structures of your Visual Studio projects. Despite the powerful viewer, the `DGML` format is not so much in use for other purposes. It is also a bit difficult to use. The aim of `DgmlBuilder` is to make it more easy to represent your (graph-) structured data in `DGML` such that you can use the Visual Studio `DGML` viewer to display, analyze, and interact with your data.

## How it works
`GraphBuilder` operates on a collection of objects and turns these into collections of nodes, edges, styles, and categories, based on a given ste of builders.

### Builders
Builders are specific classes that you write that convert objects in your model to to specific graph elements.

The following builders are supported:
* `NodeBuilder` These are builders that construct graph nodes from objects in your model. For instance, if you have a type `Component` in your model with a `Name`,  `Id`, `ComponentType` fields, you can tranform these into corresponding nodes with the following node builder:
    ```csharp
    new NodeBuilder<Component>(
        x => new Node 
            {
                Id = x.Id,
                Label = x.Name,
                Category = x.ComponentType
            });
    ```
    As we'll see shortly, you can defien multiple node builders for different types in your model. For instance, if your model also contains `Interface` as type, you can instantiate a specific node builder for this type.
* `EdgeBuilder` These are builders for constructing edges in your graph. For instance, if there is a `Call` relation in your model to represent methods calls from one component to another, you could instantiate an edge builder as follows:
    ```csharp
    new LinkBuilder<Call>(
        x => new Link
            {
                Source = x.Source,
                Target = x.Target
            });
    ```
* `CategoryBuilder` These are builders for adding containment to your graph. For instance, to put all your components inside there containing module.
* `StyleBuilder` These are builders for applying visual styles to your nodes and edges. For example, to use different background colors for components and interfaces.
## Using GraphBuilder
To use `GraphBuilder`, you create an instance of `GraphBuilder` and instantiate the rights bulders. The basic setup is like this:
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

The `DgmlBuilder` class supports to `Build` methods:
* The first, simply accepts a collection of objects:
    ```csharp
    public DirectedGraph Build(IEnumerable<object> elements)
    ```
* The second supprts multiple collections:
    ```csharp
    public DirectedGraph Build(params IEnumerable<object>[] elements)`
    ```
Both build methods will apply the configured builders to all elements and produces a `DGML` graph as output.

For example, if you have your components, interfaces, and calls contained in separate colletions, could do the following to generate a corresponding `DGML` graph:
```csharp
var graph = builder.Build(components, interfaces, calls);
```

The resulting `DGML` graph is a serializable object. This means that with the standard .Net serializers you can save the corresponding graph to disk. E.g., as follows:
```csharp
using (var writer = new StreamWriter("my-graph.dgml"))
{
    var serializer = new XmlSerializer(typeof(DirectedGraph));
    serializer.Serialize(writer, graph);
}
```
You can now open the file `my-graph.dgml` in the `DGML` viewer of Visual Studio to inspect and analyze the graph.
## Examples
[GitHub](https://github.com/merijndejonge/DgmlBuilder) will soon contain  a couple of examples of how to use `DgmlBuilder` in practice.
## More info
Source code of `DgmlBuilder` is available at [GitHub](https://github.com/merijndejonge/DgmlBuilder). Nuget packages are available at [Nuget.org](https://www.nuget.org/packages/DgmlBuilder).

`DgmlBuilder` is distributed under the [Apache 2.0 License](https://github.com/merijndejonge/OptionParser/blob/master/LICENSE).
