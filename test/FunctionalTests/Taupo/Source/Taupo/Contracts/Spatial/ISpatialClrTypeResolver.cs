//---------------------------------------------------------------------
// <copyright file="ISpatialClrTypeResolver.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts.Spatial
{
    using System;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts.Types;

    /// <summary>
    /// Spatial Clr type resolver.
    /// </summary>
    [ImplementationSelector("SpatialClrTypeResolver", DefaultImplementation = "Default")]
    public interface ISpatialClrTypeResolver
    {
        /// <summary>
        /// Finds and returns the clr type for given spatial type.
        /// </summary>
        /// <returns>the clr type for given spatial type.</returns>
        /// <param name="spatialType">the spatial type to get clr type for.</param>
        Type GetClrType(SpatialDataType spatialType);

        /// <summary>
        /// Returns true if the specified clr type is spatial, false otherwise.
        /// </summary>
        /// <param name="clrType">The clr type to check.</param>
        /// <returns>Value indicating whether the specified clr type is spatial.</returns>
        bool IsSpatial(Type clrType);
    }
}
