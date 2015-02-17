//---------------------------------------------------------------------
// <copyright file="ISpatialEqualityComparer.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts.Spatial
{
    using System.Collections.Generic;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Spatial equality comparer.
    /// </summary>
    [ImplementationSelector("SpatialEqualityComparer", DefaultImplementation = "WellKnownText")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1040:AvoidEmptyInterfaces", Justification = "Need distinct interface for dependency-injection")]
    public interface ISpatialEqualityComparer : IEqualityComparer<object>
    {
    }
}
