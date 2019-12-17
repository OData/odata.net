//---------------------------------------------------------------------
// <copyright file="ISpatial.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Spatial
{
    /// <summary>Represents the spatial interface.</summary>
    public interface ISpatial
    {
        /// <summary>Gets the coordinate system.</summary>
        /// <returns>The coordinate system.</returns>
        CoordinateSystem CoordinateSystem { get; }

        /// <summary>Gets a value that indicates whether the spatial type is empty.</summary>
        /// <returns>true if the spatial type is empty; otherwise, false.</returns>
        bool IsEmpty { get; }
    }
}