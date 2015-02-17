//---------------------------------------------------------------------
// <copyright file="IResourcePropertyBasedEdmProperty.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Service.Providers
{
    using Microsoft.OData.Edm;

    /// <summary>
    /// Extends <see cref="IEdmProperty"/> to expose the <see cref="ResourceProperty"/> that the property was based on.
    /// </summary>
    internal interface IResourcePropertyBasedEdmProperty : IEdmProperty
    {
        /// <summary>
        /// The <see cref="ResourceProperty"/> this edm property was created from.
        /// </summary>
        ResourceProperty ResourceProperty { get; }
    }
}