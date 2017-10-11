using System;

namespace OpenSoftware.DgmlTools.Builders
{
    public abstract class Builder
    {
        protected Builder(Type type)
        {
            Type = type;
        }

        public Type Type { get; }
    }
}