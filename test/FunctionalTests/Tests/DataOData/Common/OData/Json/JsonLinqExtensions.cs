//---------------------------------------------------------------------
// <copyright file="JsonLinqExtensions.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Json
{
    #region Namespaces
    using System.IO;
    using System.Linq;
    using Microsoft.Test.Taupo.Astoria.Contracts.Json;
    using Microsoft.Test.Taupo.Common;
    #endregion Namespaces

    /// <summary>
    /// Helper extension methods to provide LINQ over JSON
    /// </summary>
    public static class JsonLinqExtensions
    {
        /// <summary>
        /// Saves the JSON value as a text into a writer.
        /// </summary>
        /// <param name="writer">The writer to write to.</param>
        /// <param name="writingJsonLight">true if we are writing JSON Light, false if we're writing Verbose JSON.</param>
        public static void SaveAsText(this JsonValue value, TextWriter writer, bool writingJsonLight)
        {
            JsonTextPreservingWriter jsonWriter = new JsonTextPreservingWriter(writer, writingJsonLight);
            jsonWriter.WriteValue(value);
        }

        /// <summary>
        /// Returns the JSON value as a text.
        /// </summary>
        /// <param name="value">The value to return as a text.</param>
        /// <param name="writingJsonLight">true if we are writing JSON Light, false if we're writing Verbose JSON.</param>
        /// <returns>The text representation of the JSON value, the serialized JSON.</returns>
        public static string ToText(this JsonValue value, bool writingJsonLight)
        {
            using (StringWriter writer = new StringWriter())
            {
                writer.NewLine = string.Empty;
                value.SaveAsText(writer, writingJsonLight);
                return writer.ToString();
            }
        }

        /// <summary>
        /// Returns the value as JsonObject.
        /// </summary>
        /// <param name="value">The JSON value to process.</param>
        /// <returns>The value as JsonObject or fails otherwise.</returns>
        public static JsonObject Object(this JsonValue value)
        {
            JsonObject jsonObject = value as JsonObject;
            ExceptionUtilities.CheckObjectNotNull(jsonObject, "A JSON object was expected, but was not found: '{0}", value.ToString());
            return jsonObject;
        }

        /// <summary>
        /// Returns the value as JsonArray.
        /// </summary>
        /// <param name="value">The JSON value to process.</param>
        /// <returns>The value as JsonArray or fails otherwise.</returns>
        public static JsonArray Array(this JsonValue value)
        {
            JsonArray jsonArray = value as JsonArray;
            ExceptionUtilities.CheckObjectNotNull(jsonArray, "A JSON array was expected, but was not found: '{0}", value.ToString());
            return jsonArray;
        }

        /// <summary>
        /// Returns a property by its name.
        /// </summary>
        /// <param name="jsonObject">The JSON object to get the property from.</param>
        /// <param name="propertyName">The name of the property to return.</param>
        /// <returns>The property or null if none was found.</returns>
        public static JsonProperty Property(this JsonObject jsonObject, string propertyName)
        {
            ExceptionUtilities.CheckArgumentNotNull(jsonObject, "jsonObject");
            ExceptionUtilities.CheckArgumentNotNull(propertyName, "propertyName");
            return jsonObject.Properties.SingleOrDefault(property => property.Name == propertyName);
        }

        /// <summary>
        /// Returns a property value by its name.
        /// </summary>
        /// <param name="jsonObject">The JSON object to get the property from.</param>
        /// <param name="propertyName">The name of the property to return value for.</param>
        /// <returns>The value of the property or null if none was found.</returns>
        public static JsonValue PropertyValue(this JsonObject jsonObject, string propertyName)
        {
            ExceptionUtilities.CheckArgumentNotNull(jsonObject, "jsonObject");
            ExceptionUtilities.CheckArgumentNotNull(propertyName, "propertyName");
            JsonProperty property = jsonObject.Property(propertyName);
            return property == null ? null : property.Value;
        }

        /// <summary>
        /// Returns a property value as an object by its name.
        /// </summary>
        /// <param name="jsonObject">The JSON object to get the property from.</param>
        /// <param name="propertyName">The name of the property to return value for.</param>
        /// <returns>The value of the property is it's an object otherwise null.</returns>
        public static JsonObject PropertyObject(this JsonObject jsonObject, string propertyName)
        {
            ExceptionUtilities.CheckArgumentNotNull(jsonObject, "jsonObject");
            ExceptionUtilities.CheckArgumentNotNull(propertyName, "propertyName");
            JsonValue propertyValue = jsonObject.PropertyValue(propertyName);
            ExceptionUtilities.CheckObjectNotNull(propertyValue, "A JSON object '{0}', doesn't have the requested property '{1}'.", jsonObject.ToString(), propertyName);
            return jsonObject.PropertyValue(propertyName).Object();
        }
    }
}
