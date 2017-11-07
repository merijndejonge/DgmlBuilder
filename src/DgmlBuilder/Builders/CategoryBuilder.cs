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

    public class CategoriesBuilder<T> : CategoryBuilder, IBuilder<Category>
    {
        private readonly Func<T, IEnumerable<Category>> _builder;
        private readonly Func<T, bool> _accept;

        public CategoriesBuilder(Func<T, IEnumerable<Category>> builder, Func<T, bool> accept = null)
            : base(typeof(T))
        {
            _builder = builder;
            _accept = accept;
        }

        public bool Accept(object node)
        {
            return _accept?.Invoke((T) node) ?? true;
        }

        public override IEnumerable<Category> Build(object node)
        {
            return _builder((T) node);
        }
    }

    public class CategoryBuilder<T> : CategoriesBuilder<T>
    {
        public CategoryBuilder(Func<T, Category> builder, Func<T, bool> accept = null) : base(x => new[] {builder(x)}, accept)
        {
        }
    }
}