//---------------------------------------------------------------------
// <copyright file="ResourceTypeKind.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Service.Providers
{
    /// <summary>
    /// Enumeration for the kind of resource types
    /// </summary>
    public enum ResourceTypeKind
    {
        /// <summary>Resource type with keys.</summary>
        EntityType,

        /// <summary>Resource type without keys.</summary>
        ComplexType,

        /// <summary>A resource type without keys and with no properties.</summary>
        Primitive,

        /// <summary>Resource type representing a collection property of primitive or complex types.</summary>
        Collection,

        /// <summary>Resource type representing a collection of entities.</summary>
        EntityCollection,
    }
}
