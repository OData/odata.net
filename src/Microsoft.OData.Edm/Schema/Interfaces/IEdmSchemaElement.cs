//---------------------------------------------------------------------
// <copyright file="IEdmSchemaElement.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.OData.Edm.Vocabularies;

namespace Microsoft.OData.Edm
{
    /// <summary>
    /// Defines EDM schema element types.
    /// </summary>
    public enum EdmSchemaElementKind
    {
        /// <summary>
        /// Represents a schema element with unknown or error kind.
        /// </summary>
        None = 0,

        /// <summary>
        /// Represents a schema element implementing <see cref="IEdmSchemaType"/>.
        /// </summary>
        TypeDefinition,

        /// <summary>
        /// Represents a schema element implementing <see cref="IEdmTerm"/>.
        /// </summary>
        Term,


        /// <summary>
        /// Represents a schema element implementing <see cref="IEdmAction"/>.
        /// </summary>
        Action,

        /// <summary>
        /// Represents a schema element implementing <see cref="IEdmEntityContainer"/>
        /// </summary>
        EntityContainer,

        /// <summary>
        /// Represents a schema element implementing <see cref="IEdmAction"/>.
        /// </summary>
        Function,
    }

    /// <summary>
    /// Common base interface for all named children of EDM schema.
    /// </summary>
    public interface IEdmSchemaElement : IEdmNamedElement, IEdmVocabularyAnnotatable
    {
        /// <summary>
        /// Gets the kind of this schema element.
        /// </summary>
        EdmSchemaElementKind SchemaElementKind { get; }

        /// <summary>
        /// Gets the namespace this schema element belongs to.
        /// </summary>
        string Namespace { get; }
    }
}
