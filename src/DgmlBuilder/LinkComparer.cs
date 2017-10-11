using System.Collections.Generic;
using OpenSoftware.DgmlTools.Model;

namespace OpenSoftware.DgmlTools
{
    internal class LinkComparer : IEqualityComparer<Link>
    {
        public bool Equals(Link x, Link y)
        {
            return x.Target == y.Target && x.Source == y.Source && x.Label == y.Label;
        }

        public int GetHashCode(Link obj)
        {
            var hashCode = 0;
            if (obj.Target != null) hashCode += obj.Target.GetHashCode();
            if (obj.Source != null) hashCode += obj.Source.GetHashCode();
            if (obj.Label != null) hashCode += obj.Label.GetHashCode();

            return hashCode;
        }
    }
}