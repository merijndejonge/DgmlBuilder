using System;
using System.Collections.Generic;
using OpenSoftware.DgmlTools.Model;

namespace OpenSoftware.DgmlTools.Builders
{
    public abstract class NodeBuilder : Builder
    {
        protected NodeBuilder(Type type)
            : base(type)
        {
        }
    }

    public class NodesBuilder<T> : NodeBuilder, IBuilder<Node>
    {
        private readonly Func<T, IEnumerable<Node>> _builder;
        private readonly Func<T, bool> _accept;

        public NodesBuilder(Func<T, IEnumerable<Node>> builder, Func<T, bool> accept = null)
            : base(typeof(T))
        {
            _builder = builder;
            _accept = accept;
        }

        public bool Accept(object node)
        {
            return _accept?.Invoke((T) node) ?? true;
        }

        IEnumerable<Node> IBuilder<Node>.Build(object node)
        {
            return _builder((T) node);
        }
    }

    public class NodeBuilder<T> : NodesBuilder<T>
    {
        public NodeBuilder(Func<T, Node> builder, Func<T, bool> accept = null) : base(x => new[] {builder(x)}, accept)
        {
        }
    }
}