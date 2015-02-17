//---------------------------------------------------------------------
// <copyright file="IEdmStructuralProperty.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm
{
    /// <summary>
    /// Enumerates the EDM property concurrency modes.
    /// </summary>
    public enum EdmConcurrencyMode
    {
        /// <summary>
        /// Denotes a property that should not be used for optimistic concurrency checks.
        /// </summary>
        None = 0,

        /// <summary>
        /// Denotes a property that should be used for optimistic concurrency checks.
        /// </summary>
        Fixed
    }

    /// <summary>
    /// Represents an EDM structural (i.e. non-navigation) property.
    /// </summary>
    public interface IEdmStructuralProperty : IEdmProperty
    {
        /// <summary>
        /// Gets the default value of this property.
        /// </summary>
        string DefaultValueString { get; }

        /// <summary>
        /// Gets the concurrency mode of this property.
        /// </summary>
        EdmConcurrencyMode ConcurrencyMode { get; }
    }
}
