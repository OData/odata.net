//   OData .NET Libraries ver. 5.6.3
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 
//   MIT License
//   Permission is hereby granted, free of charge, to any person obtaining a copy of
//   this software and associated documentation files (the "Software"), to deal in
//   the Software without restriction, including without limitation the rights to use,
//   copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the
//   Software, and to permit persons to whom the Software is furnished to do so,
//   subject to the following conditions:

//   The above copyright notice and this permission notice shall be included in all
//   copies or substantial portions of the Software.

//   THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
//   FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
//   COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
//   IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
//   CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

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
#if WINRT
                propertyInfo = structuredType.GetInstanceType(model).GetProperty(
                    property.Name,
                    isPublic: true,
                    isStatic: false);
#else
                BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Instance;
                propertyInfo = structuredType.GetInstanceType(model).GetProperty(property.Name, bindingFlags);
#endif
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
