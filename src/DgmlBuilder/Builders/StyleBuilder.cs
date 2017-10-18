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
    }

    public class StylesBuilder<T> : StyleBuilder, IBuilder<Style>
    {
        private readonly Func<T, IEnumerable<Style>> _builder;

        public StylesBuilder(Func<T, IEnumerable<Style>> builder)
            : base(typeof(T))
        {
            _builder = builder;
        }

        IEnumerable<Style> IBuilder<Style>.Build(object element)
        {
            return _builder((T) element);
        }
    }

    public class StyleBuilder<T> : StylesBuilder<T>
    {
        public StyleBuilder(Func<T, Style> builder) : base(x => new[] {builder(x)})
        {
        }
    }
}