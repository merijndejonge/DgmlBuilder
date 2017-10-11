using System;
using System.Collections.Generic;
using OpenSoftware.DgmlTools.Model;

namespace OpenSoftware.DgmlTools.Builders
{
    public abstract class StyleBuilder : Builder
    {
        protected StyleBuilder(Type type) : base(type)
        {
        }
        public abstract IEnumerable<Style> Build(object node);
    }
    public class StyleBuilder<T> : StyleBuilder, IBuilder<Style>
    {
        private readonly Func<T, IEnumerable<Style>> _builder;

        public StyleBuilder(Func<T, IEnumerable<Style>> builder)
            : base(typeof(T))
        {
            _builder = builder;
        }

        public StyleBuilder(Func<T, Style> builder)
            : base(typeof(T))
        {
            _builder = x => new[] {builder(x)};
        }

        public override IEnumerable<Style> Build(object element)
        {
            return _builder((T)element);
        }
    }
}