//---------------------------------------------------------------------
// <copyright file="JsonAnnotationExtensions.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Json.TextAnnotations
{
    #region Namespaces
    using System.Linq;
    using Microsoft.Test.Taupo.Astoria.Contracts.Json;
    using Microsoft.Test.Taupo.Contracts;
    #endregion Namespaces

    /// <summary>
    /// Helper extension methods for easier usage of JSON OM annotations
    /// </summary>
    public static class JsonAnnotationExtensions
    {
        /// <summary>
        /// Gets annotation from a JsonAnnotation annotatable.
        /// </summary>
        /// <typeparam name="T">The type of the annotation to get.</typeparam>
        /// <param name="annotatable">The annotatable to get the annotation from.</param>
        /// <returns>The annotation or null if there's no such annotation.</returns>
        public static T GetAnnotation<T>(this IAnnotatable<JsonAnnotation> annotatable) where T : JsonAnnotation
        {
            return (T)annotatable.GetAnnotation(typeof(T));
        }

        /// <summary>
        /// Sets annotation of a JsonAnnotation annotatable.
        /// </summary>
        /// <typeparam name="T">The type of the annotation to set.</typeparam>
        /// <param name="annotatable">The annotatable to set the annotation on.</param>
        /// <param name="annotation">The annotation to set.</param>
        public static void SetAnnotation<T>(this IAnnotatable<JsonAnnotation> annotatable, T annotation) where T : JsonAnnotation
        {
            annotatable.SetAnnotation<T, JsonAnnotation>(annotation);
        }

        /// <summary>
        /// Removes all annotations from the specified JsonAnnotation annotatable of the specified type.
        /// </summary>
        /// <typeparam name="T">The type of the annotation to remove.</typeparam>
        /// <param name="annotatable">The annotatable to remove the annotations from.</param>
        /// <remarks>Note that this only affects the annotatable itself, it does not remove all text annotations in the JSON subtree.</remarks>
        public static void RemoveAnnotationsOfType<T>(this IAnnotatable<JsonAnnotation> annotatable) where T : JsonAnnotation
        {
            foreach (var annotation in annotatable.Annotations.OfType<T>().ToList())
            {
                annotatable.Annotations.Remove(annotation);
            }
        }

        /// <summary>
        /// Removes all annotations from the specified JsonValue; optionally recurses to remove deeply remove all annotations.
        /// </summary>
        /// <param name="jsonValue">The value to remove the annotations from.</param>
        /// <param name="deep">Flag indicating whether annotations should be removed recursively or not.</param>
        public static JsonValue RemoveAllAnnotations(this JsonValue jsonValue, bool deep)
        {
            jsonValue.Annotations.Clear();

            if (deep)
            {
                switch (jsonValue.JsonType)
                {
                    case JsonValueType.JsonArray:
                        foreach (JsonValue e in ((JsonArray)jsonValue).Elements)
                        {
                            RemoveAllAnnotations(e, deep);
                        }
                        break;
                    case JsonValueType.JsonObject:
                        foreach (JsonProperty p in ((JsonObject)jsonValue).Properties)
                        {
                            RemoveAllAnnotations(p, deep);
                        }
                        break;
                    case JsonValueType.JsonPrimitiveValue:
                        break;
                    case JsonValueType.JsonProperty:
                        RemoveAllAnnotations(((JsonProperty)jsonValue).Value, deep);
                        break;
                    default:
                        throw new System.NotSupportedException();
                }
            }

            return jsonValue;
        }
    }
}
