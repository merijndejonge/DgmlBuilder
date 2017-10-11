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
        public abstract IEnumerable<Node> Build(object node);
    }
    public class NodeBuilder<T> : NodeBuilder, IBuilder<Node>
    {
        private readonly Func<T, IEnumerable<Node>> _builder;

        public NodeBuilder(Func<T, IEnumerable<Node>> builder)
            : base(typeof(T))
        {
            _builder = builder;
        }
        public NodeBuilder(Func<T, Node> builder)
            : base(typeof(T))
        {
            _builder = x => new[] {builder(x)};
        }

        public override IEnumerable<Node> Build(object node)
        {
            return _builder((T)node);
        }
    }
}