using System.Collections.Generic;
using OpenSoftware.DgmlTools.Model;

namespace OpenSoftware.DgmlTools
{
    internal class StyleComparer : IEqualityComparer<Style>
    {
        public bool Equals(Style x, Style y)
        {
            return x.GroupLabel == y.GroupLabel && x.TargetType == y.TargetType && x.ValueLabel == y.ValueLabel;
        }

        public int GetHashCode(Style obj)
        {
            var hashCode = 0;
            if (obj.GroupLabel != null) hashCode += obj.GroupLabel.GetHashCode();
            if (obj.TargetType != null) hashCode += obj.TargetType.GetHashCode();
            if (obj.ValueLabel != null) hashCode += obj.ValueLabel.GetHashCode();

            return hashCode;
        }
    }
}