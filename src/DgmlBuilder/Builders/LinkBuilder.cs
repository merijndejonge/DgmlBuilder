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
    }

    public class LinksBuilder<T> : LinkBuilder, IBuilder<Link>
    {
        private readonly Func<T, IEnumerable<Link>> _builder;

        public LinksBuilder(Func<T, IEnumerable<Link>> builder)
            : base(typeof(T))
        {
            _builder = builder;
        }

        IEnumerable<Link> IBuilder<Link>.Build(object node)
        {
            return _builder((T) node);
        }
    }

    public class LinkBuilder<T> : LinksBuilder<T>
    {
        public LinkBuilder(Func<T, Link> builder) : base(x => new[] {builder(x)})
        {
        }
    }
}