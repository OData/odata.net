//---------------------------------------------------------------------
// <copyright file="JsonUtils.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Writer.Tests.Json
{
    #region Namespaces
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Text;
    using Microsoft.OData;
    using Microsoft.Test.Taupo.Astoria.Contracts.Json;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.OData.Common;
    using Microsoft.Test.Taupo.OData.Json;
    using Microsoft.Test.Taupo.OData.Json.TextAnnotations;
    using Microsoft.Test.Taupo.OData.Writer.Tests.Common;
    #endregion Namespaces

    /// <summary>
    /// Utility methods for working with JSON
    /// </summary>
    public static class JsonUtils
    {
        private const string indentString = "$(Indent)";

        private static readonly string[] indentationStrings = new string[]
        {
            string.Empty,
            "$(Indent)",
            "$(Indent)$(Indent)",
            "$(Indent)$(Indent)$(Indent)",
            "$(Indent)$(Indent)$(Indent)$(Indent)",
            "$(Indent)$(Indent)$(Indent)$(Indent)$(Indent)",
        };

        /// <summary>
        /// Depending on the passed in <paramref name="testConfiguration"/> wraps the specified <paramref name="json"/>
        /// in the "d" wrapper.
        /// </summary>
        /// <param name="testConfiguration">The test configuration to consider.</param>
        /// <param name="jsonLines">The json payload (as separate lines) to wrap.</param>
        /// <returns>The wrapped json payload.</returns>
        public static string WrapTopLevelValue(WriterTestConfiguration testConfiguration, params string[] jsonLines)
        {
            if (!testConfiguration.IsRequest)
            {
                string[] wrappedJsonLines = new string[jsonLines.Length + 2];
                wrappedJsonLines[0] = "{";

                for (int i = 0; i < jsonLines.Length; ++i)
                {
                    if (i == 0)
                    {
                        wrappedJsonLines[i + 1] = indentString + "\"d\":" + RemoveIndent(jsonLines[i]);
                    }
                    else
                    {
                        wrappedJsonLines[i + 1] = indentString + jsonLines[i];
                    }
                }

                wrappedJsonLines[wrappedJsonLines.Length - 1] = "}";
                jsonLines = wrappedJsonLines;
            }

            return string.Join("$(NL)", jsonLines);
        }

        /// <summary>
        /// Splits the given json payload into multiple lines and adds the indentation marker to it.
        /// </summary>
        /// <param name="jsonString">Json Payload.</param>
        /// <param name="indentDepth">Indentation depth to start with.</param>
        /// <returns>all the lines after indenting the json payload.</returns>
        public static string[] GetJsonLines(string jsonString, int indentDepth = 0)
        {
            string indentVariable = "$(Indent)";
            int originalIndentDepth = indentDepth;
            List<string> jsonLines = new List<string>();
            StringBuilder stringBuilder = new StringBuilder();

            for (int i = 0; i < jsonString.Length; i++)
            {
                if (jsonString[i] == '{' || jsonString[i] == '[')
                {
                    stringBuilder.Append(jsonString[i]);
                    jsonLines.Add(stringBuilder.ToString());
                    stringBuilder.Clear();
                    indentDepth++;
                    for (int j = 0; j < indentDepth; j++)
                    {
                        stringBuilder.Append(indentVariable);
                    }
                }
                else if (jsonString[i] == '}' || jsonString[i] == ']')
                {
                    jsonLines.Add(stringBuilder.ToString());
                    stringBuilder.Clear();
                    indentDepth--;
                    for (int j = 0; j < indentDepth; j++)
                    {
                        stringBuilder.Append(indentVariable);
                    }

                    stringBuilder.Append(jsonString[i]);
                }
                else
                {
                    stringBuilder.Append(jsonString[i]);
                }
            }

            jsonLines.Add(stringBuilder.ToString());
            ExceptionUtilities.Assert(originalIndentDepth == indentDepth, "indentDepth must be zero");
            return jsonLines.ToArray();
        }


        /// <summary>
        /// Depending on the passed in <paramref name="testConfiguration"/> wraps the specified <paramref name="json"/>
        /// in the "d" wrapper.
        /// </summary>
        /// <param name="testConfiguration">The test configuration to consider.</param>
        /// <param name="jsonLines">The json payload (as separate lines) to wrap.</param>
        /// <returns>The wrapped json payload.</returns>
        public static string[] WrapTopLevelObject(string[] jsonLines)
        {
            string[] wrappedJsonLines = new string[jsonLines.Length + 2];
            wrappedJsonLines[0] = "{";

            for (int i = 0; i < jsonLines.Length; ++i)
            {
                wrappedJsonLines[i + 1] = indentString + jsonLines[i];
            }

            wrappedJsonLines[wrappedJsonLines.Length - 1] = "}";
            return wrappedJsonLines;
        }

        /// <summary>
        /// Depending on the passed in <paramref name="testConfiguration"/> wraps the specified <paramref name="json"/>
        /// in the start of the "d" wrapper but does not add the closing '}' for the "d" wrapper; used for error payloads.
        /// </summary>
        /// <param name="testConfiguration">The test configuration to consider.</param>
        /// <param name="jsonLines">The json payload (as separate lines) to (partially) wrap.</param>
        /// <returns>The partially wrapped json payload.</returns>
        public static string WrapTopLevelValuePartial(WriterTestConfiguration testConfiguration, params string[] jsonLines)
        {
            if (!testConfiguration.IsRequest)
            {
                string[] wrappedJsonLines = new string[jsonLines.Length + 1];
                wrappedJsonLines[0] = "{";

                for (int i = 0; i < jsonLines.Length; ++i)
                {
                    if (i == 0)
                    {
                        wrappedJsonLines[i + 1] = indentString + "\"d\":" + RemoveIndent(jsonLines[i]);
                    }
                    else
                    {
                        wrappedJsonLines[i + 1] = indentString + jsonLines[i];
                    }
                }

                jsonLines = wrappedJsonLines;
            }

            return string.Join("$(NL)", jsonLines);
        }

        /// <summary>
        /// Depending on the passed in <paramref name="testConfiguration"/> wraps the specified <paramref name="json"/>
        /// in either the "d" or "results" wrapper or both.
        /// </summary>
        /// <param name="testConfiguration">The test configuration to consider.</param>
        /// <param name="jsonLines">The json payload to wrap.</param>
        /// <returns>The wrapped json payload.</returns>
        public static string WrapTopLevelResults(WriterTestConfiguration testConfiguration, string[] jsonLines)
        {
            return WrapTopLevelResults(testConfiguration, null, null, jsonLines);
        }

        /// <summary>
        /// Depending on the passed in <paramref name="testConfiguration"/> wraps the specified <paramref name="json"/>
        /// in either the "d" or "results" wrapper or both.
        /// </summary>
        /// <param name="testConfiguration">The test configuration to consider.</param>
        /// <param name="countString">An optional string that represents the inline count.</param>
        /// <param name="nextLinkString">An optional string that represents the next page link.</param>
        /// <param name="jsonLines">The json payload to wrap.</param>
        /// <returns>The wrapped json payload.</returns>
        public static string WrapTopLevelResults(WriterTestConfiguration testConfiguration, string countString, string nextLinkString, string[] jsonLines)
        {
            if (!testConfiguration.IsRequest)
            {
                List<string> wrappedJsonLines = new List<string>();
                wrappedJsonLines.Add("{");
                
                for (int i = 0; i < jsonLines.Length; ++i)
                {
                    // remove the $(Indent) from the first line (if it exists) and add the 'results' property
                    if (i == 0)
                    {
                        if (countString != null)
                        {
                            wrappedJsonLines.Add(indentString + countString + ",\"results\":" + RemoveIndent(jsonLines[i]));
                        }
                        else
                        {
                            wrappedJsonLines.Add(indentString + "\"results\":" + RemoveIndent(jsonLines[i]));
                        }
                    }
                    else
                    {
                        wrappedJsonLines.Add(indentString + jsonLines[i]);
                    }
                }

                if (nextLinkString != null)
                {
                    int lastIx = wrappedJsonLines.Count - 1;
                    wrappedJsonLines[lastIx] = wrappedJsonLines[lastIx] + "," + nextLinkString;
                }

                wrappedJsonLines.Add("}");
                jsonLines = wrappedJsonLines.ToArray();
            }

            return WrapTopLevelValue(testConfiguration, jsonLines);
        }

        /// <summary>
        /// Returns the __metadata property for a complex type in JSON
        /// </summary>
        /// <param name="complexTypeName">The name of the complex type.</param>
        /// <param name="indent">The amount of indentation to add for the metadata property.</param>
        /// <param name="subsequentContentOnSameLine">Content that should go on the same line as the last line in the resulting array.</param>
        /// <returns>The JSON representation of the __metadata property for the complex type.</returns>
        public static string[] GetMetadataPropertyForComplexType(string complexTypeName, int indent, string subsequentContentOnSameLine)
        {
            Debug.Assert(subsequentContentOnSameLine != null, "subsequentContentOnSameLine must not be null.");

            return new string[]
            {
                GetIndentation(indent) + "\"__metadata\":{",
                GetIndentation(indent + 1) + "\"type\":\"" + complexTypeName + "\"",
                GetIndentation(indent) + "}" + subsequentContentOnSameLine
            };
        }

        /// <summary>
        /// Returns the __metadata property for a collection type in JSON
        /// </summary>
        /// <param name="complexTypeName">The name of the collection type.</param>
        /// <param name="indent">The amount of indentation to add for the metadata property.</param>
        /// <param name="subsequentContentOnSameLine">Content that should go on the same line as the last line in the resulting array.</param>
        /// <returns>The JSON representation of the __metadata property for the collection type.</returns>
        public static string[] GetMetadataPropertyForCollectionType(string collectionTypeName, int indent, string subsequentContentOnSameLine)
        {
            Debug.Assert(subsequentContentOnSameLine != null, "subsequentContentOnSameLine must not be null.");

            return new string[]
            {
                GetIndentation(indent) + "\"__metadata\":{",
                GetIndentation(indent + 1) + "\"type\":\"" + collectionTypeName + "\"",
                GetIndentation(indent) + "}" + subsequentContentOnSameLine
            };
        }

        /// <summary>
        /// Trims the surrounding whitespaces from the text representation of the JSON object.
        /// </summary>
        /// <param name="jsonValue">The JSON object to process.</param>
        public static void TrimSurroundingWhitespaces(JsonValue jsonValue)
        {
            if (jsonValue is JsonObject)
            {
                TrimSurroundingWhitespaces<JsonStartObjectTextAnnotation, JsonEndObjectTextAnnotation>(jsonValue);
            }
            else if (jsonValue is JsonArray)
            {
                TrimSurroundingWhitespaces<JsonStartArrayTextAnnotation, JsonEndArrayTextAnnotation>(jsonValue);
            }
        }

        /// <summary>
        /// Unwraps top-level JSON payload from the "d" wrapper as appropriate depending on <paramref name="testConfiguration"/>.
        /// </summary>
        /// <param name="testConfiguration">The test configuration to consider.</param>
        /// <param name="value">The value to unwrap.</param>
        /// <returns>Unwrapped value.</returns>
        public static JsonValue UnwrapTopLevelValue(WriterTestConfiguration testConfiguration, JsonValue value)
        {
            if (!testConfiguration.IsRequest)
            {
                value = value.Object().PropertyValue("d");
                ExceptionUtilities.CheckObjectNotNull(value, "The specified JSON payload is not wrapped in the expected \"d\":{{}} wrapper.");
            }

            TrimSurroundingWhitespaces(value);

            return value;
        }

        /// <summary>
        /// Unwraps top-level multiple JSON results from the "d" and/or "results" wrappers as appropriate depending on <paramref name="testConfiguration"/>.
        /// </summary>
        /// <param name="testConfiguration">The test configuration to consider.</param>
        /// <param name="value">The value to unwrap.</param>
        /// <returns>Unwrapped value.</returns>
        public static JsonValue UnwrapTopLevelResults(WriterTestConfiguration testConfiguration, JsonValue value)
        {
            value = UnwrapTopLevelValue(testConfiguration, value);
            value = GetFeedItemsArray(testConfiguration, value);

            TrimSurroundingWhitespaces(value);

            return value;
        }

        /// <summary>
        /// Gets the JSON array holding the entries of the given feed.
        /// </summary>
        /// <param name="testConfiguration">The test configuration to consider.</param>
        /// <param name="feed">The JSON value representing the feed.</param>
        /// <returns>A JSON array with the items in a feed.</returns>
        public static JsonArray GetFeedItemsArray(WriterTestConfiguration testConfiguration, JsonValue feed)
        {
            if (!testConfiguration.IsRequest)
            {
                feed = feed.Object().PropertyValue("results");
                ExceptionUtilities.CheckObjectNotNull(feed, "The specified JSON payload is not wrapped in the expected \"results\": wrapper.");
            }

            ExceptionUtilities.Assert(feed.JsonType == JsonValueType.JsonArray, "Feed contents must be an array.");
            return (JsonArray)feed;
        }

        /// <summary>
        /// Returns indentation string for the specified amount of indentation.
        /// </summary>
        /// <param name="indent">The amount of indentation.</param>
        /// <returns>The indentation string to use.</returns>
        public static string GetIndentation(int indent)
        {
            if (indent < 0)
            {
                return string.Empty;
            }
            else if (indent < indentationStrings.Length)
            {
                return indentationStrings[indent];
            }
            else
            {
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < indent; ++i)
                {
                    builder.Append(indentString);
                }
                return builder.ToString();
            }
        }

        /// <summary>
        /// Adds a collection of properties to a JSON object.
        /// </summary>
        /// <param name="jsonObject">The JSON object to add properties to.</param>
        /// <param name="properties">The properties to add.</param>
        /// <returns>The JSON object with added properties.</returns>
        public static JsonObject AddProperties(this JsonObject jsonObject, IEnumerable<JsonProperty> properties)
        {
            foreach (var prop in properties)
            {
                jsonObject.Add(prop);
            }

            return jsonObject;
        }

        /// <summary>
        /// Expands a Json template by resolving all variables.
        /// </summary>
        /// <param name="xmlTemplate">The Json template to expand.</param>
        /// <param name="variables">The set of variables to resolve.</param>
        /// <returns>The string representation of the <paramref name="jsonTemplate"/> with resolved variables.</returns>
        internal static string GetComparableJsonString(string jsonTemplate, Dictionary<string, string> variables)
        {
            // NOTE: we have to trim the resulting string here since the fragment extractor potentially extracts a small
            //       portion of the JSON payload and converting that fragment to a string does not preserve leading/trailing
            //       whitespace. This means our comparison is not 100% (modulo the leading/trailing whitespace) but should be
            //       good enough for the purposes of the unit tests.
            return StringUtils.ResolveVariables(jsonTemplate, variables).Trim();
        }

        /// <summary>
        /// Trims the surrounding whitespaces from the text representation of the JSON value.
        /// </summary>
        /// <typeparam name="TStartTextAnnotation">The type of the start text annotation.</typeparam>
        /// <typeparam name="TEndTextAnnotation">The type of the end text annotation.</typeparam>
        /// <param name="jsonValue">The JSON value to process.</param>
        private static void TrimSurroundingWhitespaces<TStartTextAnnotation, TEndTextAnnotation>(JsonValue jsonValue) 
            where TStartTextAnnotation : JsonTextAnnotation
            where TEndTextAnnotation : JsonTextAnnotation
        {
            var startTextAnnotation = jsonValue.GetAnnotation<TStartTextAnnotation>();
            if (startTextAnnotation != null)
            {
                startTextAnnotation.Text = startTextAnnotation.Text.TrimStart(null);
            }

            var endTextAnnotation = jsonValue.GetAnnotation<TEndTextAnnotation>();
            if (endTextAnnotation != null)
            {
                endTextAnnotation.Text = endTextAnnotation.Text.TrimEnd(null);
            }
        }

        private static string RemoveIndent(string line)
        {
            // figure out whether the first line starts with the indent variable; if so remove it
            if (line.StartsWith(indentString))
            {
                return line.Substring(indentString.Length);
            }

            return line;
        }
    }
}