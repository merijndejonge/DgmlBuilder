using System;
using System.Collections.Generic;
using OpenSoftware.DgmlTools.Model;

namespace OpenSoftware.DgmlTools.Builders
{
    public abstract class LinkBuilder : Builder
    {
        protected LinkBuilder(Type type) : base(type)
        {
        }
        public abstract IEnumerable<Link> Build(object node);
    }
    public class LinkBuilder<T> : LinkBuilder, IBuilder<Link>
    {
        private readonly Func<T, IEnumerable<Link>> _builder;

        public LinkBuilder(Func<T, IEnumerable<Link>> builder)
            : base(typeof(T))
        {
            _builder = builder;
        }

        public LinkBuilder(Func<T, Link> builder)
            : base(typeof(T))
        {
            _builder = x => new[] {builder(x)};
        }

        public override IEnumerable<Link> Build(object node)
        {
            return _builder((T)node);
        }
    }
}