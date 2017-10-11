using System.Collections.Generic;

namespace OpenSoftware.DgmlTools.Builders
{
    public interface IBuilder<out T>
    {
        IEnumerable<T> Build(object node);
    }
}