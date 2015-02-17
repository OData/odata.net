//---------------------------------------------------------------------
// <copyright file="SpatialTypeKind.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Spatial.Contracts
{
    /// <summary>
    /// Used to indicate whether a spatial type is geographic or geometric
    /// Using adjective names because they answer the question 'what kind of spatial type is it?'
    /// </summary>
    public enum SpatialTypeKind
    {
        /// <summary>
        /// Represents the Geographic spatial type hierarchy
        /// </summary>
        Geography = 0,

        /// <summary>
        /// Represents the geometric spatial type hierarchy
        /// </summary>
        Geometry = 1,
    }
}