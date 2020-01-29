//---------------------------------------------------------------------
// <copyright file="IEdmEntitySet.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm
{
    /// <summary>
    /// Represents an EDM entity set.
    /// </summary>
    public interface IEdmEntitySet : IEdmEntitySetBase, IEdmEntityContainerElement
    {
        /// <summary>
        /// Gets a value indicating whether the entity set is included in the service document.
        /// </summary>
        bool IncludeInServiceDocument { get; }
    }
}
