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

namespace Microsoft.Data.OData.Query
{
    #region Namespaces
    using System.Diagnostics;
    using System.Reflection;
    using Microsoft.Data.Edm;
    using Microsoft.Data.OData.Metadata;
    #endregion Namespaces

    /// <summary>
    /// Extension methods to make it easier to work with PropertyInfo objects on a type.
    /// </summary>
    internal static class PropertyInfoExtensionMethods
    {
        /// <summary>
        /// Gets the property info for the EDM property on the specified type.
        /// </summary>
        /// <param name="typeReference">The type to get the property on.</param>
        /// <param name="property">Property instance to get the property info for.</param>
        /// <param name="model">Model containing annotations.</param>
        /// <returns>Returns the PropertyInfo object for the specified property.</returns>
        /// <remarks>The method searches this type as well as all its base types for the property.</remarks>
        internal static PropertyInfo GetPropertyInfo(this IEdmStructuredTypeReference typeReference, IEdmProperty property, IEdmModel model)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(typeReference != null, "typeReference != null");
            Debug.Assert(property != null, "property != null");
            Debug.Assert(model != null, "model != null");
            Debug.Assert(property.GetCanReflectOnInstanceTypeProperty(model), "property.CanReflectOnInstanceTypeProperty()");
#if DEBUG
            Debug.Assert(typeReference.ContainsProperty(property), "The typeReference does not define the specified property.");
#endif

            IEdmStructuredType structuredType = typeReference.StructuredDefinition();
            return PropertyInfoTypeAnnotation.GetPropertyInfoTypeAnnotation(structuredType, model).GetPropertyInfo(structuredType, property, model);
        }
    }
}
