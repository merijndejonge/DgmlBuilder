using System.Collections.Generic;

namespace OpenSoftware.DgmlTools.Builders
{
    internal interface IBuilder<out T>
    {
        IEnumerable<T> Build(object node);
    }
}