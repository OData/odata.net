//---------------------------------------------------------------------
// <copyright file="IWellKnownTextSpatialFormatter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Spatial.Contracts
{
    using System;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts.Types;

    /// <summary>
    /// Interface for converting a spatial primitive value to/from well-known-text
    /// </summary>
    [ImplementationSelector("WellKnownTextSpatialFormatter", DefaultImplementation = "Default")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1040:AvoidEmptyInterfaces", Justification = "Need distinct interface for dependency-injection")]
    public interface IWellKnownTextSpatialFormatter : ISpatialPrimitiveFormatter<string>
    {
    }
}
