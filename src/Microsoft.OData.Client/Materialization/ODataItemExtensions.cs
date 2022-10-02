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
        /// <param name="materializerContext">The materializer context.</param>
        /// <returns>The materialized value.</returns>
        public static object GetMaterializedValue(this ODataProperty property, IODataMaterializerContext materializerContext)
        {
            ODataAnnotatable annotatableObject = property.Value as ODataAnnotatable ?? property;
            return GetMaterializedValueCore(annotatableObject, materializerContext);
        }

        /// <summary>
        /// Determines whether a value has been materialized.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <param name="materializerContext">The materializer context.</param>
        /// <returns><c>true</c> if the value has been materialized; otherwise, <c>false</c>.</returns>
        public static bool HasMaterializedValue(this ODataProperty property, IODataMaterializerContext materializerContext)
        {
            ODataAnnotatable annotatableObject = property.Value as ODataAnnotatable ?? property;
            return HasMaterializedValueCore(annotatableObject, materializerContext);
        }

        /// <summary>
        /// Sets the materialized value.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <param name="materializedValue">The materializer value.</param>
        /// <param name="materializerContext">The materializer context.</param>
        public static void SetMaterializedValue(this ODataProperty property, object materializedValue, IODataMaterializerContext materializerContext)
        {
            ODataAnnotatable annotatableObject = property.Value as ODataAnnotatable ?? property;
            SetMaterializedValueCore(annotatableObject, materializedValue, materializerContext);
        }

        /// <summary>
        /// Gets the materialized value.
        /// </summary>
        /// <param name="annotatableObject">The annotatable object.</param>
        /// <param name="materializerContext">The materializer context.</param>
        /// <returns>The materialized value</returns>
        private static object GetMaterializedValueCore(ODataAnnotatable annotatableObject, IODataMaterializerContext materializerContext)
        {
            MaterializerPropertyValue value = materializerContext.MaterializerCache.GetAnnotation<MaterializerPropertyValue>(annotatableObject);
            Debug.Assert(value != null, "MaterializedValue not set");
            return value.Value;
        }

        /// <summary>
        /// Determines whether a value has been materialized.
        /// </summary>
        /// <param name="annotatableObject">The annotatable object.</param>
        /// <param name="materializerContext">The materializer context.</param>
        /// <returns><c>true</c> if the value has been materialized; otherwise, <c>false</c>.</returns>
        private static bool HasMaterializedValueCore(ODataAnnotatable annotatableObject, IODataMaterializerContext materializerContext)
        {
            return materializerContext.GetAnnotation<MaterializerPropertyValue>(annotatableObject) != null;
        }

        /// <summary>
        /// Sets the materialized value.
        /// </summary>
        /// <param name="annotatableObject">The annotatable object.</param>
        /// <param name="materializedValue">The materialized value.</param>
        /// <param name="materializerContext">The materializer context.</param>
        private static void SetMaterializedValueCore(ODataAnnotatable annotatableObject, object materializedValue, IODataMaterializerContext materializerContext)
        {
            MaterializerPropertyValue materializerValue = new MaterializerPropertyValue { Value = materializedValue };
            materializerContext.SetAnnotation(annotatableObject, materializerValue);
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