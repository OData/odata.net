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
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Reflection;
    using Microsoft.Data.Edm;
    using Microsoft.Data.OData.Metadata;
    using ODataErrorStrings = Microsoft.Data.OData.Strings;
    #endregion Namespaces

    /// <summary>
    /// Annotation stored on a type to hold PropertyInfo objects for its properties.
    /// </summary>
    internal sealed class PropertyInfoTypeAnnotation
    {
        /// <summary>
        /// Cache of property info objects already created for properties on the type the annotation is on.
        /// </summary>
        private Dictionary<IEdmProperty, PropertyInfo> propertyInfosDeclaredOnThisType;

        /// <summary>
        /// Gets the property info annotation for the specified type or creates a new one if it doesn't exist.
        /// </summary>
        /// <param name="structuredType">The type to get the annotation for.</param>
        /// <param name="model">The model containing annotations.</param>
        /// <returns>The property info annotation.</returns>
        internal static PropertyInfoTypeAnnotation GetPropertyInfoTypeAnnotation(IEdmStructuredType structuredType, IEdmModel model)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(structuredType != null, "structuredType != null");
            Debug.Assert(model != null, "model != null");

            PropertyInfoTypeAnnotation propertyInfoTypeAnnotation = model.GetAnnotationValue<PropertyInfoTypeAnnotation>(structuredType);
            if (propertyInfoTypeAnnotation == null)
            {
                propertyInfoTypeAnnotation = new PropertyInfoTypeAnnotation();
                model.SetAnnotationValue(structuredType, propertyInfoTypeAnnotation);
            }

            return propertyInfoTypeAnnotation;
        }

        /// <summary>
        /// Gets the property info for the EDM property declared on this type.
        /// </summary>
        /// <param name="structuredType">The structured type to get the property on.</param>
        /// <param name="property">Property instance to get the property info for.</param>
        /// <param name="model">The model containing annotations.</param>
        /// <returns>Returns the PropertyInfo object for the specified EDM property.</returns>
        internal PropertyInfo GetPropertyInfo(IEdmStructuredType structuredType, IEdmProperty property, IEdmModel model)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(structuredType != null, "structuredType != null");
            Debug.Assert(property != null, "property != null");
            Debug.Assert(model != null, "model != null");
            Debug.Assert(property.GetCanReflectOnInstanceTypeProperty(model), "property.CanReflectOnInstanceTypeProperty()");
#if DEBUG
            Debug.Assert(structuredType.ContainsProperty(property), "The structuredType does not define the specified property.");
#endif

            if (this.propertyInfosDeclaredOnThisType == null)
            {
                this.propertyInfosDeclaredOnThisType = new Dictionary<IEdmProperty, PropertyInfo>(ReferenceEqualityComparer<IEdmProperty>.Instance);
            }

            PropertyInfo propertyInfo;
            if (!this.propertyInfosDeclaredOnThisType.TryGetValue(property, out propertyInfo))
            {
                BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Instance;
                propertyInfo = structuredType.GetInstanceType(model).GetProperty(property.Name, bindingFlags);
                if (propertyInfo == null)
                {
                    throw new ODataException(ODataErrorStrings.PropertyInfoTypeAnnotation_CannotFindProperty(structuredType.ODataFullName(), structuredType.GetInstanceType(model), property.Name));
                }

                this.propertyInfosDeclaredOnThisType.Add(property, propertyInfo);
            }

            Debug.Assert(propertyInfo != null, "propertyInfo != null");
            return propertyInfo;
        }
    }
}
