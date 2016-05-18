//---------------------------------------------------------------------
// <copyright file="IEdmType.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm
{
    using System;

    /// <summary>
    /// Defines EDM metatypes.
    /// </summary>
    [Flags]
    public enum EdmTypeKind
    {
        /// <summary>
        /// Represents a type with an unknown or error kind.
        /// </summary>
        None = 0,

        /// <summary>
        /// Represents a type implementing <see cref="IEdmPrimitiveType"/>.
        /// </summary>
        Primitive = 1,

        /// <summary>
        /// Represents a type implementing <see cref="IEdmEntityType"/>.
        /// </summary>
        Entity = 2,

        /// <summary>
        /// Represents a type implementing <see cref="IEdmComplexType"/>.
        /// </summary>
        Complex = 4,

        /// <summary>
        /// Represents a type implementing <see cref="IEdmCollectionType"/>.
        /// </summary>
        Collection = 8,

        /// <summary>
        /// Represents a type implementing <see cref="IEdmEntityReferenceType"/>.
        /// </summary>
        EntityReference = 16,

        /// <summary>
        /// Represents a type implementing <see cref="IEdmEnumType"/>.
        /// </summary>
        Enum = 32,

        /// <summary>
        /// Represents a type implementing <see cref="IEdmTypeDefinition"/>.
        /// </summary>
        TypeDefinition = 64,
    }

    /// <summary>
    /// Represents the definition of an EDM type.
    /// </summary>
    public interface IEdmType : IEdmElement
    {
        /// <summary>
        /// Gets the kind of this type.
        /// </summary>
        EdmTypeKind TypeKind { get; }
    }
}
