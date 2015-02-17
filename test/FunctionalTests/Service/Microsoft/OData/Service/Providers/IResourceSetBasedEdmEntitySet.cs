//---------------------------------------------------------------------
// <copyright file="IResourceSetBasedEdmEntitySet.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Service.Providers
{
    using Microsoft.OData.Edm;

    /// <summary>
    /// Extends <see cref="IEdmEntitySet"/> to expose the <see cref="ResourceSetWrapper"/> that the entity set was based on.
    /// </summary>
    internal interface IResourceSetBasedEdmEntitySet : IEdmEntitySet
    {
        /// <summary>
        /// The resource-set wrapper that this entity-set was created from.
        /// </summary>
        ResourceSetWrapper ResourceSet { get; }
    }
}