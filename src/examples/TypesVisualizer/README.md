# TypesVisualizer

This folder contains the implementation of `TypesVisualizer`. This is a small program that demonstrates most features of `DgmlBuilder`.

# Usage
With this project added to your solution, you can use the tool as follows:
```csharp

    // Create a collection of types. This is a simple way of doing that
    // but you may want to use a more dedicated way.
    var types = assembly.GetTypes();

    // Use the TypesVisualizer class to create a DGML graph from the types collection
    var graph = TypesVisualizer.Types2Dgml(types);

    // This code is to serialize the graph to a file
    using (var writer = new StreamWriter(@"my-class-diagram.dgml"))
    {
        var serializer = new XmlSerializer(typeof(DirectedGraph));
        serializer.Serialize(writer, graph);
    }
```
# Implementation
The `Types2Dgml` method makes use of `DgmlBuilder`. Have a look at the code to learn how `DgmlBuilder` can easily be used to create nice graphs.