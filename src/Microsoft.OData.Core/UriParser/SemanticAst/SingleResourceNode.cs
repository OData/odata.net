//---------------------------------------------------------------------
// <copyright file="SingleResourceNode.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.OData.Edm;

namespace Microsoft.OData.UriParser
{
    /// <summary>
    /// Node representing a resource, including entity and complex.
    /// </summary>
    public abstract class SingleResourceNode : SingleValueNode
    {
        /// <summary>
        /// Gets the navigation source containing this single entity/complex.
        /// </summary>
        public abstract IEdmNavigationSource NavigationSource { get; }

        /// <summary>
        /// Gets the type of this single entity/complex.
        /// </summary>
        public abstract IEdmStructuredTypeReference StructuredTypeReference { get; }
    }
}
