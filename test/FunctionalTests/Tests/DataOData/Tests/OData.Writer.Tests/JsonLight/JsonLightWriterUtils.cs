//---------------------------------------------------------------------
// <copyright file="JsonLightWriterUtils.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Writer.Tests.JsonLight
{
    #region Namespaces
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using Microsoft.Test.Taupo.Astoria.Contracts.Json;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.OData.Json;
    using Microsoft.Test.Taupo.OData.JsonLight;
    using Microsoft.Test.Taupo.OData.Writer.Tests.Common;
    using Microsoft.Test.Taupo.OData.Writer.Tests.Json;
    #endregion Namespaces

    /// <summary>
    /// Utility methods for working with JSON Lite
    /// </summary>
    public static class JsonLightWriterUtils
    {
        /// <summary>
        /// Returns the type property for a complex type in JSON Lite
        /// </summary>
        /// <param name="complexTypeName">The name of the complex type.</param>
        /// <param name="indent">The amount of indentation to add before the type property.</param>
        /// <param name="subsequentContentOnSameLine">Content that should go on the same line as the last line in the resulting array.</param>
        /// <returns>The JSON Lite representation of the type property for the complex type.</returns>
        public static string GetTypePropertyForComplexType(string complexTypeName, int indent, string subsequentContentOnSameLine)
        {
            Debug.Assert(subsequentContentOnSameLine != null, "subsequentContentOnSameLine must not be null.");

            return JsonUtils.GetIndentation(indent) + "\"OData.Type\":\"#" + complexTypeName + "\"" + subsequentContentOnSameLine;
        }

        /// <summary>
        /// Combines the <paramref name="jsonLines"/> into a single string payload (with newline markers).
        /// </summary>
        /// <param name="jsonLines">The json payload (as separate lines) to combine.</param>
        /// <returns>The wrapped json payload.</returns>
        public static string CombineLines(params string[] jsonLines)
        {
            return string.Join("$(NL)", jsonLines);
        }

        /// <summary>
        /// Combines the <paramref name="jsonProperties"/> into a single string payload.
        /// </summary>
        /// <param name="jsonProperties">The json property payload (as separate lines) to combine.</param>
        /// <returns>The wrapped json payload.</returns>
        public static string CombineProperties(params string[] jsonProperties)
        {
            return string.Join(",", jsonProperties.Where(p => !string.IsNullOrEmpty(p)).ToArray());
        }

        /// <summary>
        /// Trims the whitespace surrounding a JSON value.
        /// </summary>
        /// <param name="value">The value to trim.</param>
        /// <returns>The trimmed value.</returns>
        public static JsonValue TrimWhitespace(JsonValue value)
        {
            JsonUtils.TrimSurroundingWhitespaces(value);
            return value;
        }

        /// <summary>
        /// Gets the JSON array holding the entries of the given feed.
        /// </summary>
        /// <param name="testConfiguration">The test configuration to consider.</param>
        /// <param name="feed">The JSON value representing the feed.</param>
        /// <returns>A JSON array with the items in a feed.</returns>
        public static JsonArray GetTopLevelFeedItemsArray(WriterTestConfiguration testConfiguration, JsonValue feed)
        {
            feed = feed.Object().PropertyValue("value");
            ExceptionUtilities.CheckObjectNotNull(feed, "The specified JSON Lite payload is not wrapped in the expected \"value\": wrapper.");
            ExceptionUtilities.Assert(feed.JsonType == JsonValueType.JsonArray, "Feed contents must be an array.");
            return (JsonArray)feed;
        }

        /// <summary>
        /// Builds the metadata URL property for top-level entry payloads.
        /// </summary>
        /// <param name="fragment">The URL fragment.</param>
        /// <param name="metadataUrl">The metadata URL.</param>
        /// <returns>String which is the JSON representation of the metadata URL.</returns>
        public static string GetMetadataUrlPropertyForEntry(string fragment, string metadataUrl = null)
        {
            if (metadataUrl == null)
            {
                metadataUrl = JsonLightConstants.DefaultMetadataDocumentUri.OriginalString;
            }

            return "\"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataContextAnnotationName + "\":\"" + metadataUrl + "#" + fragment + "/$entity\"";
        }

        /// <summary>
        /// Builds the metadata URL property for top-level feed payloads.
        /// </summary>
        /// <param name="fragment">The URL fragment.</param>
        /// <param name="metadataUrl">The metadata URL.</param>
        /// <returns>String which is the JSON representation of the metadata URL.</returns>
        public static string GetMetadataUrlPropertyForFeed(string fragment, string metadataUrl = null)
        {
            if (metadataUrl == null)
            {
                metadataUrl = JsonLightConstants.DefaultMetadataDocumentUri.OriginalString;
            }

            return "\"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataContextAnnotationName + "\":\"" + metadataUrl + "#" + fragment + "\"";
        }

        /// <summary>
        /// Builds the metadata URL property for top-level property payloads.
        /// </summary>
        /// <param name="typeName">The name of the owning type.</param>
        /// <param name="propertyName">The name of the property for which to create the context URI.</param>
        /// <param name="metadataUrl">The metadata URL.</param>
        /// <returns>String which is the JSON representation of the metadata URL.</returns>
        public static string GetMetadataUrlPropertyForProperty(string propertyType, string metadataUrl = null)
        {
            if (metadataUrl == null)
            {
                metadataUrl = JsonLightConstants.DefaultMetadataDocumentUri.OriginalString;
            }

            return "\"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataContextAnnotationName + "\":\"" + metadataUrl + "#" + propertyType + "\"";
        }
    }
}