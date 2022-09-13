﻿//---------------------------------------------------------------------
// <copyright file="IGeometryProvider.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Spatial
{
    using System;

    /// <summary>Provides access to the geometry objects that this object constructs.</summary>
    public interface IGeometryProvider
    {
        /// <summary>Fires when the provider constructs a geometry object.</summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1003:Use generic event handler instances", Justification = "<Pending>")]
        event Action<Geometry> ProduceGeometry;

        /// <summary>Gets the geometry object that was constructed most recently.</summary>
        /// <returns>The geometry object that was constructed.</returns>
        Geometry ConstructedGeometry { get; }
    }
}