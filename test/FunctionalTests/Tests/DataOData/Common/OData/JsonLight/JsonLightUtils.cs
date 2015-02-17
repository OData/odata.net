//---------------------------------------------------------------------
// <copyright file="JsonLightUtils.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.JsonLight
{
    #region Namespaces
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Test.Taupo.Astoria.Contracts.Json;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.OData.Json;
    #endregion Namespaces

    /// <summary>
    /// Utility methods for dealing with the JSON Lite format.
    /// </summary>
    public static class JsonLightUtils
    {
        /// <summary>
        /// Returns all property annotations for a given property name. It doesn't return the property itself.
        /// </summary>
        /// <param name="jsonObject">The JSON object to get the property annotations from.</param>
        /// <param name="propertyName">The name of the property to get the annotations for.</param>
        /// <returns>List of all property annotations for the specified property.</returns>
        public static IEnumerable<JsonProperty> GetPropertyAnnotations(this JsonObject jsonObject, string propertyName)
        {
            ExceptionUtilities.CheckObjectNotNull(jsonObject, "jsonObject can't be null.");
            return jsonObject.Properties.Where(p => p.IsPropertyAnnotation(propertyName));
        }

        /// <summary>
        /// Returns all property annotations for a given property name and the property itself.
        /// </summary>
        /// <param name="jsonObject">The JSON object to get the property annotations from.</param>
        /// <param name="propertyName">The name of the property to get the annotations for.</param>
        /// <returns>List of all property annotations for the specified property.</returns>
        public static IEnumerable<JsonProperty> GetPropertyAnnotationsAndProperty(this JsonObject jsonObject, string propertyName)
        {
            ExceptionUtilities.CheckObjectNotNull(jsonObject, "jsonObject can't be null.");
            IEnumerable<JsonProperty> result = jsonObject.Properties.Where(p => p.IsPropertyAnnotation(propertyName));
            JsonProperty property = jsonObject.Property(propertyName);
            return property == null ? result : result.Concat(new[] { property });
        }

        /// <summary>
        /// Returns true if the specified JSON property is a property annotation for the specified property.
        /// </summary>
        /// <param name="jsonProperty">The JSON property to test.</param>
        /// <param name="propertyName">The property name for which annotation to test for.</param>
        /// <returns>true if <paramref name="jsonProperty"/> is a property annotation for a property with name <paramref name="propertyName"/>.</returns>
        public static bool IsPropertyAnnotation(this JsonProperty jsonProperty, string propertyName)
        {
            ExceptionUtilities.CheckObjectNotNull(jsonProperty, "jsonProperty can't be null.");
            ExceptionUtilities.CheckStringNotNullOrEmpty(propertyName, "Property name must be a valid EDM name.");

            return jsonProperty.Name.StartsWith(propertyName + JsonLightConstants.ODataPropertyAnnotationSeparator);
        }

        /// <summary>
        /// Determine if a property is an annotation property with the specified name.
        /// </summary>
        /// <param name="jsonProperty">The JSON property to inspect.</param>
        /// <param name="annotationName">The name of the annotation to look for.</param>
        /// <returns>true if the property is an annotation of the specified name, or it's a property annotation
        /// with the specified annotation name.</returns>
        public static bool IsAnnotationWithName(this JsonProperty jsonProperty, string annotationName)
        {
            ExceptionUtilities.CheckObjectNotNull(jsonProperty, "jsonProperty can't be null.");
            ExceptionUtilities.CheckStringNotNullOrEmpty(annotationName, "Annotation name must be a valid name.");

            string propertyName = jsonProperty.Name;
            if (propertyName == annotationName)
            {
                // It's an object level annotation with the specified name.
                return true;
            }

            int propertyAnnotationNameIndex = propertyName.IndexOf(JsonLightConstants.ODataPropertyAnnotationSeparator);
            if (propertyAnnotationNameIndex >= 0)
            {
                string propertyAnnotationName = propertyName.Substring(propertyAnnotationNameIndex + 1);
                if (propertyAnnotationName == annotationName)
                {
                    // It's a property annotation with the specified name.
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Gets the name of the property annotation property.
        /// </summary>
        /// <param name="propertyName">The name of the property to annotate.</param>
        /// <param name="annotationName">The name of the annotation.</param>
        /// <returns>The property name for the annotation property.</returns>
        public static string GetPropertyAnnotationName(string propertyName, string annotationName)
        {
            return propertyName + JsonLightConstants.ODataPropertyAnnotationSeparator + annotationName;
        }

        /// <summary>
        /// Gets all annotations with the specified name on the specified JSON object.
        /// </summary>
        /// <param name="jsonObject">The JSON object to inspect.</param>
        /// <param name="annotationName">The name of the annotation to look for.</param>
        /// <returns>The property which is the annotation of the specified name,
        /// as well as all property annotation which have the specified annotation name.</returns>
        public static IEnumerable<JsonProperty> GetAnnotationsWithName(this JsonObject jsonObject, string annotationName)
        {
            ExceptionUtilities.CheckObjectNotNull(jsonObject, "jsonObject can't be null.");

            return jsonObject.Properties.Where(p => p.IsAnnotationWithName(annotationName));
        }
    }
}
