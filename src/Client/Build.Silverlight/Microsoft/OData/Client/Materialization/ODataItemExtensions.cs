//   OData .NET Libraries ver. 6.8.1
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

namespace Microsoft.OData.Client.Materialization
{
    using System.Diagnostics;
    using Microsoft.OData.Core;

    /// <summary>
    /// Extension methods for ODataItems
    /// </summary>
    internal static class ODataItemExtensions
    {
        /// <summary>
        /// Gets the materialized value.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <returns>The materialized value.</returns>
        public static object GetMaterializedValue(this ODataProperty property)
        {
            ODataAnnotatable annotatableObject = property.Value as ODataAnnotatable ?? property;
            return GetMaterializedValueCore(annotatableObject);
        }

        /// <summary>
        /// Determines whether a value has been materialized.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <returns><c>true</c> if the value has been materialized; otherwise, <c>false</c>.</returns>
        public static bool HasMaterializedValue(this ODataProperty property)
        {
            ODataAnnotatable annotatableObject = property.Value as ODataAnnotatable ?? property;
            return HasMaterializedValueCore(annotatableObject);
        }

        /// <summary>
        /// Sets the materialized value.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <param name="materializedValue">The materialized value.</param>
        public static void SetMaterializedValue(this ODataProperty property, object materializedValue)
        {
            ODataAnnotatable annotatableObject = property.Value as ODataAnnotatable ?? property;
            SetMaterializedValueCore(annotatableObject, materializedValue);
        }

        /// <summary>
        /// Gets the materialized value.
        /// </summary>
        /// <param name="complexValue">The complex value.</param>
        /// <returns>The materialized value.</returns>
        public static object GetMaterializedValue(this ODataComplexValue complexValue)
        {
            return GetMaterializedValueCore(complexValue);
        }

        /// <summary>
        /// Determines whether a value has been materialized.
        /// </summary>
        /// <param name="complexValue">The complex value.</param>
        /// <returns><c>true</c> if the value has been materialized; otherwise, <c>false</c>.</returns>
        public static bool HasMaterializedValue(this ODataComplexValue complexValue)
        {
            return HasMaterializedValueCore(complexValue);
        }

        /// <summary>
        /// Sets the materialized value.
        /// </summary>
        /// <param name="complexValue">The complex value.</param>
        /// <param name="materializedValue">The materialized value.</param>
        public static void SetMaterializedValue(this ODataComplexValue complexValue, object materializedValue)
        {
            SetMaterializedValueCore(complexValue, materializedValue);
        }

        /// <summary>
        /// Gets the materialized value.
        /// </summary>
        /// <param name="annotatableObject">The annotatable object.</param>
        /// <returns>The materialized value</returns>
        private static object GetMaterializedValueCore(ODataAnnotatable annotatableObject)
        {
            MaterializerPropertyValue value = annotatableObject.GetAnnotation<MaterializerPropertyValue>();
            Debug.Assert(value != null, "MaterializedValue not set");
            return value.Value;
        }

        /// <summary>
        /// Determines whether a value has been materialized.
        /// </summary>
        /// <param name="annotatableObject">The annotatable object.</param>
        /// <returns><c>true</c> if the value has been materialized; otherwise, <c>false</c>.</returns>
        private static bool HasMaterializedValueCore(ODataAnnotatable annotatableObject)
        {
            return annotatableObject.GetAnnotation<MaterializerPropertyValue>() != null;
        }

        /// <summary>
        /// Sets the materialized value.
        /// </summary>
        /// <param name="annotatableObject">The annotatable object.</param>
        /// <param name="materializedValue">The materialized value.</param>
        private static void SetMaterializedValueCore(ODataAnnotatable annotatableObject, object materializedValue)
        {
            MaterializerPropertyValue materializerValue = new MaterializerPropertyValue { Value = materializedValue };
            annotatableObject.SetAnnotation(materializerValue);
        }

        /// <summary>
        /// Annotation class for the materialized value
        /// </summary>
        private class MaterializerPropertyValue
        {
            /// <summary>
            /// Gets or sets the value.
            /// </summary>
            /// <value>
            /// The value.
            /// </value>
            public object Value
            {
                get;
                set;
            }
        }
    }
}
