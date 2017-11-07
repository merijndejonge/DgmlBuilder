using System;

namespace OpenSoftware.DgmlTools.Reflection
{
    public interface IExcludeFilter
    {
        bool Exclude(Type type);
    }
}