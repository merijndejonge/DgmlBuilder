using System.Collections.Generic;

namespace OpenSoftware.DgmlTools.Builders
{
    internal interface IBuilder<out T>
    {
        /// <summary>
        /// Indicates of this builder accepts the given node.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        bool Accept(object node);
        /// <summary>
        /// Converts this node into a collection of items of tye T.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        IEnumerable<T> Build(object node);
    }
}