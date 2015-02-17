//---------------------------------------------------------------------
// <copyright file="IGmlSpatialFormatter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Spatial.Contracts
{
    using System.Xml.Linq;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Interface for converting a spatial primitive value to/from the GML microformat
    /// </summary>
    [ImplementationSelector("GmlSpatialFormatter", DefaultImplementation = "Default")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1040:AvoidEmptyInterfaces", Justification = "Need distinct interface for dependency-injection")]
    public interface IGmlSpatialFormatter : ISpatialPrimitiveFormatter<XElement>
    {
    }
}
