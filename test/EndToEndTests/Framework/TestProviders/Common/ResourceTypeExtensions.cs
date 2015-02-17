//---------------------------------------------------------------------
// <copyright file="ResourceTypeExtensions.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Framework.TestProviders.Common
{
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.OData.Service.Providers;

    /// <summary>
    /// Extension methods for the products metadata APIs
    /// </summary>
    public static class ResourceTypeExtensions
    {
        /// <summary>
        /// Returns all properties on the type without explicitly loading them if the type is lazy
        /// </summary>
        /// <param name="resourceType">The resource type to get all the properties for</param>
        /// <returns>All the properties for the type, including those on any base type(s)</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Taupo.DataServices", "TDS0001:ResourceTypePropertiesAccessRule", 
            Justification = "This is the helper method to access the properties safely")]
        public static IEnumerable<ResourceProperty> GetAllPropertiesLazily(this ResourceType resourceType)
        {
            ExceptionUtilities.CheckArgumentNotNull(resourceType, "resourceType");

            var lazyType = resourceType as LazyResourceType;
            if (lazyType != null)
            {
                IEnumerable<ResourceProperty> properties = lazyType.LazyProperties;
                if (lazyType.BaseType != null)
                {
                    properties = lazyType.BaseType.GetAllPropertiesLazily().Concat(properties);
                }

                return properties;
            }

            return resourceType.Properties;
        }

        /// <summary>
        /// Returns only the properties locally defined on the given type without explicitly loading them if the type is lazy
        /// </summary>
        /// <param name="resourceType">The resource type to get properties for</param>
        /// <returns>Only the locally defined properties for the type</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Taupo.DataServices", "TDS0001:ResourceTypePropertiesAccessRule",
            Justification = "This is the helper method to access the properties safely")]
        public static IEnumerable<ResourceProperty> GetLocalPropertiesLazily(this ResourceType resourceType)
        {
            ExceptionUtilities.CheckArgumentNotNull(resourceType, "resourceType");

            var lazyType = resourceType as LazyResourceType;
            if (lazyType != null)
            {
                return lazyType.LazyProperties;
            }

            return resourceType.PropertiesDeclaredOnThisType;
        }
    }
}