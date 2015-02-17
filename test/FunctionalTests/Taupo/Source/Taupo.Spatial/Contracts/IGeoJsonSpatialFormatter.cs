//---------------------------------------------------------------------
// <copyright file="IGeoJsonSpatialFormatter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Spatial.Contracts
{
    using System.Collections.Generic;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Interface for converting a spatial primitive value to/from a GeoJSON dictionary
    /// </summary>
    [ImplementationSelector("GeoJsonSpatialFormatter", DefaultImplementation = "Default")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1040:AvoidEmptyInterfaces", Justification = "Need distinct interface for dependency-injection")]
    public interface IGeoJsonSpatialFormatter : ISpatialPrimitiveFormatter<IDictionary<string, object>>
    {
    }
}