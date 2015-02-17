//---------------------------------------------------------------------
// <copyright file="IResourceTypeBasedEdmType.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Service.Providers
{
    using Microsoft.OData.Edm;

    /// <summary>
    /// Extends <see cref="IEdmType"/> to expose the <see cref="ResourceType"/> that the type was based on.
    /// </summary>
    internal interface IResourceTypeBasedEdmType : IEdmType
    {
        /// <summary>
        /// The resource-type that this type was created from.
        /// </summary>
        ResourceType ResourceType { get; }
    }
}