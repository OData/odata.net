//---------------------------------------------------------------------
// <copyright file="IEdmEntityReferenceType.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm
{
    /// <summary>
    /// Represents a definition of an EDM entity reference type.
    /// </summary>
    public interface IEdmEntityReferenceType : IEdmType
    {
        /// <summary>
        /// Gets the entity type pointed to by this entity reference.
        /// </summary>
        IEdmEntityType EntityType { get; }
    }
}
