//---------------------------------------------------------------------
// <copyright file="IEdmProperty.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.OData.Edm.Vocabularies;

namespace Microsoft.OData.Edm
{
    /// <summary>
    /// Defines EDM property types.
    /// </summary>
    public enum EdmPropertyKind
    {
        /// <summary>
        /// Represents a property with an unknown or error kind.
        /// </summary>
        None = 0,

        /// <summary>
        /// Represents a property implementing <see cref="IEdmStructuralProperty"/>.
        /// </summary>
        Structural,

        /// <summary>
        /// Represents a property implementing <see cref="IEdmNavigationProperty"/>.
        /// </summary>
        Navigation,
    }

    /// <summary>
    /// Represents an EDM property.
    /// </summary>
    public interface IEdmProperty : IEdmNamedElement, IEdmVocabularyAnnotatable
    {
        /// <summary>
        /// Gets the kind of this property.
        /// </summary>
        EdmPropertyKind PropertyKind { get; }

        /// <summary>
        /// Gets the type of this property.
        /// </summary>
        IEdmTypeReference Type { get; }

        /// <summary>
        /// Gets the type that this property belongs to.
        /// </summary>
        IEdmStructuredType DeclaringType { get; }
    }
}
