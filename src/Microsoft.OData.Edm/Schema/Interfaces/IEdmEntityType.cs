//---------------------------------------------------------------------
// <copyright file="IEdmEntityType.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;

namespace Microsoft.OData.Edm
{
    /// <summary>
    /// Represents a definition of an EDM entity type.
    /// </summary>
    public interface IEdmEntityType : IEdmStructuredType, IEdmSchemaType
    {
        /// <summary>
        /// Gets the key ref of the entity type that make up the entity key.
        /// </summary>
        IEnumerable<IEdmPropertyRef> DeclaredKeyRef { get; }

        /// <summary>
        /// Gets the structural properties of the entity type that make up the entity key.
        /// </summary>
        IEnumerable<IEdmStructuralProperty> DeclaredKey { get => DeclaredKeyRef?.Select(x => x.ReferencedProperty); }

        /// <summary>
        /// Gets the value indicating whether or not this type is a media entity.
        /// </summary>
        bool HasStream { get; }
    }
}
