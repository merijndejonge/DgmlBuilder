﻿namespace OpenSoftware.DgmlTools.Model;

internal interface ICustomPropertiesProvider
{
    Dictionary<string, object> Properties { get; }
}
