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
    using System.Data.Services.Providers;
    using System.Diagnostics;
    using System.Reflection;
    #endregion Namespaces.

    /// <summary>
    /// Extension methods to make it easier to work with PropertyInfo objects on a resource type.
    /// </summary>
    internal static class PropertyInfoExtensionMethods
    {
        /// <summary>
        /// Gets the property info for the resource property on the specified resource type.
        /// </summary>
        /// <param name="resourceType">The resource type to get the property on.</param>
        /// <param name="resourceProperty">Resource property instance to get the property info for.</param>
        /// <returns>Returns the PropertyInfo object for the specified resource property.</returns>
        /// <remarks>The method searches this type as well as all its base types for the property.</remarks>
        internal static PropertyInfo GetPropertyInfo(this ResourceType resourceType, ResourceProperty resourceProperty)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(resourceType != null, "resourceType != null");
            Debug.Assert(resourceProperty != null, "resourceProperty != null");
            Debug.Assert(resourceProperty.CanReflectOnInstanceTypeProperty, "resourceProperty.CanReflectOnInstanceTypeProperty");
            Debug.Assert(resourceType.Properties.Contains(resourceProperty), "The resourceType does not define the specified resourceProperty.");

            return PropertyInfoResourceTypeAnnotation.GetPropertyInfoResourceTypeAnnotation(resourceType).GetPropertyInfo(resourceType, resourceProperty);
        }
    }
}
