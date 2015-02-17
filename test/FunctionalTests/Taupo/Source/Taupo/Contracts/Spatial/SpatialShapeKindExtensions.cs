//---------------------------------------------------------------------
// <copyright file="SpatialShapeKindExtensions.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts.Spatial
{
    using System;

    /// <summary>
    /// Extension methods for the SpatialShapeKind enum.
    /// </summary>
    public static class SpatialShapeKindExtensions
    {
        /// <summary>
        /// Determines whether the specified kind has the given facet.
        /// </summary>
        /// <param name="kind">The shape kind.</param>
        /// <param name="facet">The facet to look for.</param>
        /// <returns>
        ///   <c>true</c> if the specified kind has the facet; otherwise, <c>false</c>.
        /// </returns>
        public static bool HasFacet(this SpatialShapeKind kind, SpatialShapeFacets facet)
        {
            return 0 != ((int)kind & (int)facet);
        }
    }
}