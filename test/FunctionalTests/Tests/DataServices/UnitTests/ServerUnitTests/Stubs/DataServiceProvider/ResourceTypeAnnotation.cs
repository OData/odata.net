//---------------------------------------------------------------------
// <copyright file="ResourceTypeAnnotation.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.Stubs.DataServiceProvider
{
    using Microsoft.OData.Service.Providers;

    /// <summary>Helper class for extension methods on the <see cref="ResourceType"/>.</summary>
    internal static class ResourceTypeExtensions
    {
        /// <summary>Helper method to get annotation from the specified resource type.</summary>
        /// <param name="resourceType">The resource type to get annotation for.</param>
        /// <returns>The annotation for the resource type or null if the resource type doesn't have annotation.</returns>
        /// <remarks>We store the annotation in the <see cref="ResourceType.CustomState"/>, so this is just a simple helper
        /// which allows strongly typed access.</remarks>
        internal static ResourceTypeAnnotation GetAnnotation(this ResourceType resourceType)
        {
            return resourceType.CustomState as ResourceTypeAnnotation;
        }

        /// <summary>
        /// Checks if the given type is assignable to this type. In other words, if this type
        /// is a subtype of the given type or not.
        /// </summary>
        /// <param name="subType">resource type to check.</param>
        /// <returns>true, if the given type is assignable to this type. Otherwise returns false.</returns>
        internal static bool IsAssignableFrom(this ResourceType thisType, ResourceType subType)
        {
            CollectionResourceType thisCollectionType = thisType as CollectionResourceType;
            CollectionResourceType subCollectionType = subType as CollectionResourceType;
            if (thisCollectionType != null && subCollectionType != null)
            {
                return thisCollectionType.ItemType.IsAssignableFrom(subCollectionType.ItemType);
            }

            EntityCollectionResourceType thisEntityCollectionType = thisType as EntityCollectionResourceType;
            EntityCollectionResourceType subEntityCollectionType = subType as EntityCollectionResourceType;
            if (thisEntityCollectionType != null && subEntityCollectionType != null)
            {
                return thisEntityCollectionType.ItemType.IsAssignableFrom(subEntityCollectionType.ItemType);
            }

            while (subType != null)
            {
                if (subType == thisType)
                {
                    return true;
                }

                subType = subType.BaseType;
            }

            return false;
        }
    }

    /// <summary>Class used to annotate <see cref="ResourceType"/> instances with DSP specific data.</summary>
    internal class ResourceTypeAnnotation
    {
        /// <summary>The resource into which this resource type belongs.</summary>
        /// <remarks>We don't support multiple sets with the same resource type
        ///   So there's a simple mapping between the resource type and the resource set it belongs to</remarks>
        public ResourceSet ResourceSet { get; set; }
    }
}
