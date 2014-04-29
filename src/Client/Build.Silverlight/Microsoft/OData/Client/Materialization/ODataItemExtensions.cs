//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

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
