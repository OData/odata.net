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
    public enum EdmTypeKind
    {
        /// <summary>
        /// Represents a type with an unknown or error kind.
        /// </summary>
        None,

        /// <summary>
        /// Represents a type implementing <see cref="IEdmPrimitiveType"/>.
        /// </summary>
        Primitive,

        /// <summary>
        /// Represents a type implementing <see cref="IEdmEntityType"/>.
        /// </summary>
        Entity,

        /// <summary>
        /// Represents a type implementing <see cref="IEdmComplexType"/>.
        /// </summary>
        Complex,

        /// <summary>
        /// Represents a type implementing <see cref="IEdmCollectionType"/>.
        /// </summary>
        Collection,

        /// <summary>
        /// Represents a type implementing <see cref="IEdmEntityReferenceType"/>.
        /// </summary>
        EntityReference,

        /// <summary>
        /// Represents a type implementing <see cref="IEdmEnumType"/>.
        /// </summary>
        Enum,

        /// <summary>
        /// Represents a type implementing <see cref="IEdmTypeDefinition"/>.
        /// </summary>
        TypeDefinition,

        /// <summary>
        /// Represents a type implementing <see cref="IEdmUntypedType"/>.
        /// </summary>
        Untyped
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
