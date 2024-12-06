//---------------------------------------------------------------------
// <copyright file="IEdmEntityType.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
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
        /// Gets the structural properties of the entity type that make up the entity key.
        /// </summary>
        IEnumerable<IEdmStructuralProperty> DeclaredKey { get; }

        /// <summary>
        /// Gets the value indicating whether or not this type is a media entity.
        /// </summary>
        bool HasStream { get; }

        /// <summary>
        /// Gets the property refs of the entity type that make up the entity type.
        /// In the next major release, should use this to replace the DeclaredKey.
        /// </summary>
        IEnumerable<IEdmPropertyRef> DeclaredKeyRef { get => DeclaredKey.Select(x => new EdmPropertyRef(x)); }
    }
}
