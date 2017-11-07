using System;
using System.Linq;
using System.Reflection;

namespace OpenSoftware.DgmlTools.Reflection
{
    public class ExcludeByCustomAttribute<TAttribute> : IExcludeFilter where TAttribute : Attribute
    {
        public bool Exclude(Type type)
        {
            return CustomAttributeData.GetCustomAttributes(type).Any(a => a.AttributeType == typeof(TAttribute));
        }
    }
}