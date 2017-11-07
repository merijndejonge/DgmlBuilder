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
        private readonly Func<T, bool> _accept;

        public LinksBuilder(Func<T, IEnumerable<Link>> builder, Func<T, bool> accept = null)
            : base(typeof(T))
        {
            _builder = builder;
            _accept = accept;
        }

        public bool Accept(object node)
        {
            return _accept?.Invoke((T) node) ?? true;
        }

        IEnumerable<Link> IBuilder<Link>.Build(object node)
        {
            return _builder((T) node);
        }
    }

    public class LinkBuilder<T> : LinksBuilder<T>
    {
        public LinkBuilder(Func<T, Link> builder, Func<T, bool> accept = null) : base(x => new[] {builder(x)}, accept)
        {
        }
    }
}