//---------------------------------------------------------------------
// <copyright file="IEdmTerm.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm
{
    /// <summary>
    /// Defines EDM term kinds.
    /// </summary>
    public enum EdmTermKind
    {
        /// <summary>
        /// Represents a term with unknown or error kind.
        /// </summary>
        None = 0,

        /// <summary>
        /// Represents a term implementing <see cref="IEdmStructuredType"/> and <see cref="IEdmSchemaType"/>.
        /// </summary>
        Type,

        /// <summary>
        /// Represents a term implementing <see cref="IEdmValueTerm"/>.
        /// </summary>
        Value
    }

    /// <summary>
    /// Term to which an annotation can bind.
    /// </summary>
    public interface IEdmTerm : IEdmSchemaElement
    {
        /// <summary>
        /// Gets the kind of a term.
        /// </summary>
        EdmTermKind TermKind { get; }
    }
}
