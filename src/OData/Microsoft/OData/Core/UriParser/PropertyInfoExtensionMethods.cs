//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Core.UriParser
{
    #region Namespaces
    using System.Diagnostics;
    using System.Reflection;
    using Microsoft.OData.Core.Metadata;
    using Microsoft.OData.Edm;

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
