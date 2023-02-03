//---------------------------------------------------------------------
// <copyright file="IEdmKeyPropertyRef.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.Generic;

namespace Microsoft.OData.Edm
{
    /// <summary>
    /// Represents a definition of key references of an entity type.
    /// In the next major release, we should remove this interface and move the members into IEdmEntityType.
    /// </summary>
    public interface IEdmKeyPropertyRef
    {
        /// <summary>
        /// Gets the declared key references.
        /// </summary>
        IEnumerable<IEdmPropertyRef> DeclaredKeyRef { get; }
    }
}
