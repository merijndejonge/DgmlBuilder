using System;

namespace OpenSoftware.DgmlTools.Reflection
{
    public class ExcludeByNameSpace : IExcludeFilter
    {
        private readonly Func<string, bool> _namespaceSelector;

        public ExcludeByNameSpace(Func<string, bool> namespaceSelector)
        {
            _namespaceSelector = namespaceSelector;
        }
        public bool Exclude(Type type)
        {
            return type.Namespace == null || _namespaceSelector(type.Namespace);
        }
    }
}