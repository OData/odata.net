//---------------------------------------------------------------------
// <copyright file="IGeographyProvider.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Spatial
{
    using System;

    /// <summary>Provides access to the geography objects that this object constructs.</summary>
    public interface IGeographyProvider
    {
        /// <summary>Fires when the provider constructs a geography object.</summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1003:Use generic event handler instances", Justification = "<Pending>")]
        event Action<Geography> ProduceGeography;

        /// <summary>Gets the geography object that was constructed most recently.</summary>
        /// <returns>The geography object that was constructed.</returns>
        Geography ConstructedGeography { get; }
    }
}