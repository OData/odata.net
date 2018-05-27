//---------------------------------------------------------------------
// <copyright file="ODataItemExtensions.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Client.Materialization
{
    using System.Diagnostics;
    using Microsoft.OData;

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