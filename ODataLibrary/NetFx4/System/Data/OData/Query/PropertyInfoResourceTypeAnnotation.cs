//   Copyright 2011 Microsoft Corporation
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

namespace System.Data.OData.Query
{
    #region Namespaces.
    using System.Collections.Generic;
    using System.Data.Services.Providers;
    using System.Diagnostics;
    using System.Reflection;
    #endregion Namespaces.

    /// <summary>
    /// Annotation stored on a resource type to hold PropertyInfo objects for its properties.
    /// </summary>
    internal sealed class PropertyInfoResourceTypeAnnotation
    {
        /// <summary>
        /// Cache of property info objects already created for properties on the type the annotation is on.
        /// </summary>
        private Dictionary<ResourceProperty, PropertyInfo> propertyInfosDeclaredOnThisType;

        /// <summary>
        /// Gets the property info annotation for the specified resource type or creates a new one if it doesn't exist.
        /// </summary>
        /// <param name="resourceType">The resource type to get the annotation for.</param>
        /// <returns>The property info annotation.</returns>
        internal static PropertyInfoResourceTypeAnnotation GetPropertyInfoResourceTypeAnnotation(ResourceType resourceType)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(resourceType != null, "resourceType != null");

            PropertyInfoResourceTypeAnnotation propertyInfoResourceTypeAnnotation = resourceType.GetAnnotation<PropertyInfoResourceTypeAnnotation>();
            if (propertyInfoResourceTypeAnnotation == null)
            {
                propertyInfoResourceTypeAnnotation = new PropertyInfoResourceTypeAnnotation();
                resourceType.SetAnnotation(propertyInfoResourceTypeAnnotation);
            }

            return propertyInfoResourceTypeAnnotation;
        }

        /// <summary>
        /// Gets the property info for the resource property declared on this type.
        /// </summary>
        /// <param name="resourceType">The resource type to get the property on.</param>
        /// <param name="resourceProperty">Resource property instance to get the property info for.</param>
        /// <returns>Returns the PropertyInfo object for the specified resource property.</returns>
        internal PropertyInfo GetPropertyInfo(ResourceType resourceType, ResourceProperty resourceProperty)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(resourceType != null, "resourceType != null");
            Debug.Assert(resourceProperty != null, "resourceProperty != null");
            Debug.Assert(resourceProperty.CanReflectOnInstanceTypeProperty, "resourceProperty.CanReflectOnInstanceTypeProperty");
            Debug.Assert(resourceType.Properties.Contains(resourceProperty), "The resourceType does not define the specified resourceProperty.");

            if (this.propertyInfosDeclaredOnThisType == null)
            {
                this.propertyInfosDeclaredOnThisType = new Dictionary<ResourceProperty, PropertyInfo>(ReferenceEqualityComparer<ResourceProperty>.Instance);
            }

            PropertyInfo propertyInfo;
            if (!this.propertyInfosDeclaredOnThisType.TryGetValue(resourceProperty, out propertyInfo))
            {
                BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Instance;
                propertyInfo = resourceType.InstanceType.GetProperty(resourceProperty.Name, bindingFlags);
                if (propertyInfo == null)
                {
                    throw new ODataException(Strings.PropertyInfoResourceTypeAnnotation_CannotFindProperty(resourceType.FullName, resourceType.InstanceType, resourceProperty.Name));
                }

                this.propertyInfosDeclaredOnThisType.Add(resourceProperty, propertyInfo);
            }

            Debug.Assert(propertyInfo != null, "propertyInfo != null");
            return propertyInfo;
        }
    }
}
