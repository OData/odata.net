//---------------------------------------------------------------------
// <copyright file="IEdmEntityType.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.Generic;

namespace Microsoft.OData.Edm
{
    /// <summary>
    /// Represents a definition of an EDM entity type.
    /// </summary>
    public interface IEdmEntityType : IEdmStructuredType, IEdmSchemaType, IEdmTerm
    {
        /// <summary>
        /// Gets the structural properties of the entity type that make up the entity key.
        /// </summary>
        IEnumerable<IEdmStructuralProperty> DeclaredKey { get; }

        /// <summary>
        /// Gets the value indicating whether or not this type is a media entity.
        /// </summary>
        bool HasStream { get; }
    }
}
