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
