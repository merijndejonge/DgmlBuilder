using System;
using System.Collections.Generic;
using OpenSoftware.DgmlTools.Model;

namespace OpenSoftware.DgmlTools.Builders
{
    public abstract class CategoryBuilder : Builder
    {
        protected CategoryBuilder(Type type)
            : base(type)
        {
        }
        public abstract IEnumerable<Category> Build(object node);
    }
    public class CategoryBuilder<T> : CategoryBuilder, IBuilder<Category>
    {
        private readonly Func<T, IEnumerable<Category>> _builder;

        public CategoryBuilder(Func<T, IEnumerable<Category>> builder)
            : base(typeof(T))
        {
            _builder = builder;
        }

        public CategoryBuilder(Func<T, Category> builder)
            : base(typeof(T))
        {
            _builder = x => new[] {builder(x)};
        }

        public override IEnumerable<Category> Build(object node)
        {
            return _builder((T)node);
        }
    }
}