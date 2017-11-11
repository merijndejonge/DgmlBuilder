# TypesVisualizer

This folder contains the implementation of `TypesVisualizer`. This is a small program that demonstrates most features of `DgmlBuilder`.
![class diagram](https://raw.githubusercontent.com/merijndejonge/DgmlBuilder/master/src/examples/TypesVisualizer/class-diagram.png)
# Usage
With this project added to your solution, you can use the tool as follows:
```csharp

    // Create a collection of types. This is a simple way of doing that
    // but you may want to have a look at the example files to see how you can
    // also make use of the `TypesLoader` class.
    var types = assembly.GetTypes();

    // Use the TypesVisualizer class to create a DGML graph from the types collection
    var graph = TypesVisualizer.Types2Dgml(types);

    // Write the graph a file
    graph.WriteToFile("my-class-diagram.dgml");
```
# Implementation
The `Types2Dgml` method makes use of `DgmlBuilder`. Have a look at the code to learn how `DgmlBuilder` can easily be used to create nice graphs.