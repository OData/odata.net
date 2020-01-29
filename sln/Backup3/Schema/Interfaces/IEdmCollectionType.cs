//---------------------------------------------------------------------
// <copyright file="IEdmCollectionType.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm
{
    /// <summary>
    /// Represents a definition of an EDM collection type.
    /// </summary>
    public interface IEdmCollectionType : IEdmType
    {
        /// <summary>
        /// Gets the element type of this collection.
        /// </summary>
        IEdmTypeReference ElementType { get; }
    }
}
