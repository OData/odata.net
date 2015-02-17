//---------------------------------------------------------------------
// <copyright file="JsonTextPreservingParser.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Json
{
    #region Namespaces
    using System.IO;
    using Microsoft.Test.Taupo.Astoria.Contracts.Json;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.OData.Json.TextAnnotations;
    using Microsoft.Test.Taupo.Astoria.Json;
    #endregion Namespaces

    /// <summary>
    /// Parses Json
    /// </summary>
    public class JsonTextPreservingParser
    {
        private JsonTokenizer tokenizer;

        /// <summary>
        /// Initializes a new instance of the JsonParser class
        /// </summary>
        /// <param name="tokenizer">An instance of <see cref="JsonTokenizer"/></param>
        public JsonTextPreservingParser(JsonTokenizer tokenizer)
        {
            ExceptionUtilities.CheckArgumentNotNull(tokenizer, "tokenizer");
            this.tokenizer = tokenizer;
        }

        /// <summary>
        /// Parses the specified stream into a <see cref="JsonValue"/>
        /// </summary>
        /// <param name="stream">The stream to read.</param>
        /// <returns>The <see cref="JsonValue"/> read from the stream.</returns>
        public static JsonValue ParseValue(Stream stream)
        {
            return ParseValue(new StreamReader(stream));
        }

        /// <summary>
        /// Parses the specified text reader content into a <see cref="JsonValue"/>
        /// </summary>
        /// <param name="reader">The reader to read.</param>
        /// <returns>The <see cref="JsonValue"/> read from the reader.</returns>
        public static JsonValue ParseValue(TextReader reader)
        {
            var tokenizer = new JsonTokenizer(reader);
            var parser = new JsonTextPreservingParser(tokenizer);
            return parser.ParseValueOrProperty();
        }

        /// <summary>
        /// Parses Json value or property.
        /// </summary>
        /// <returns>Parsed value.</returns>
        public JsonValue ParseValueOrProperty()
        {
            JsonValue value = this.ParseValue();
            JsonPrimitiveValue stringValue = value as JsonPrimitiveValue;
            if (stringValue != null && stringValue.Value != null && stringValue.Value is string)
            {
                if (this.tokenizer.HasMoreTokens())
                {
                    JsonPrimitiveValueTextAnnotation textAnnotation = stringValue.GetAnnotation<JsonPrimitiveValueTextAnnotation>();
                    JsonPropertyNameTextAnnotation propertyNameTextAnnotation = null;
                    if (textAnnotation != null)
                    {
                        propertyNameTextAnnotation = new JsonPropertyNameTextAnnotation() { Text = textAnnotation.Text };
                    }

                    return this.ParsePropertyWithName((string)stringValue.Value, propertyNameTextAnnotation);
                }
                else
                {
                    return stringValue;
                }
            }
            else
            {
                return value;
            }
        }

        /// <summary>
        /// Parses Json token values
        /// </summary>
        /// <returns>Parsed value</returns>
        private JsonValue ParseValue()
        {
            if (this.tokenizer.TokenType == JsonTokenType.Integer || this.tokenizer.TokenType == JsonTokenType.Float)
            {
                return this.ParseNumber();
            }
            else if (this.tokenizer.TokenType == JsonTokenType.String)
            {
                return this.ParseString();
            }
            else if (this.tokenizer.TokenType == JsonTokenType.LeftCurly)
            {
                return this.ParseObject();
            }
            else if (this.tokenizer.TokenType == JsonTokenType.LeftSquareBracket)
            {
                return this.ParseArray();
            }
            else if (this.tokenizer.TokenType == JsonTokenType.True || this.tokenizer.TokenType == JsonTokenType.False)
            {
                return this.ParseBool();
            }
            else
            {
                ExceptionUtilities.Assert(this.tokenizer.TokenType == JsonTokenType.Null, "Invalid Token");
                return this.ParseNull();
            }
        }

        /// <summary>
        /// Parses Objects starting with left curly braces
        /// </summary>
        /// <returns>parsed object</returns>
        private JsonObject ParseObject()
        {
            // An object begins with '{' (left brace)
            ExceptionUtilities.Assert(this.tokenizer.TokenType == JsonTokenType.LeftCurly, "Invalid Token");

            // The stream cannot terminate without '}' (right brace)
            ExceptionUtilities.Assert(this.tokenizer.HasMoreTokens(), "Invalid End Of Stream");

            var startObjectTextAnnotation = new JsonStartObjectTextAnnotation() { Text = this.tokenizer.TokenText };
            this.tokenizer.GetNextToken();

            // An object is an unordered set of name/value pairs. So next token cannot be anything other than name or '}'
            ExceptionUtilities.Assert(this.tokenizer.TokenType == JsonTokenType.RightCurly || this.tokenizer.TokenType == JsonTokenType.String, "Invalid Token");

            var result = new JsonObject();
            result.SetAnnotation(startObjectTextAnnotation);
            JsonPropertySeparatorTextAnnotation propertySeparatorTextAnnotation = null;
            while (this.tokenizer.HasMoreTokens())
            {
                ExceptionUtilities.Assert(this.tokenizer.TokenType == JsonTokenType.String || this.tokenizer.TokenType == JsonTokenType.Comma || this.tokenizer.TokenType == JsonTokenType.RightCurly, "Invalid Token");

                // End of Object
                if (this.tokenizer.TokenType == JsonTokenType.RightCurly)
                {
                    break;
                }
                else if (this.tokenizer.TokenType == JsonTokenType.String)
                {
                    JsonPropertyNameTextAnnotation nameTextAnnotation;
                    string name = this.ParseName(out nameTextAnnotation);

                    JsonProperty property = this.ParsePropertyWithName(name, nameTextAnnotation);
                    if (propertySeparatorTextAnnotation != null)
                    {
                        property.SetAnnotation(propertySeparatorTextAnnotation);
                    }

                    propertySeparatorTextAnnotation = null;
                    result.Add(property);

                    // name/value pairs are separated by , (comma) or value can be followed by '}' (right brace)
                    ExceptionUtilities.Assert(this.tokenizer.TokenType == JsonTokenType.Comma || this.tokenizer.TokenType == JsonTokenType.RightCurly, "Invalid Token");
                }
                else if (this.tokenizer.TokenType == JsonTokenType.Comma)
                {
                    propertySeparatorTextAnnotation = new JsonPropertySeparatorTextAnnotation() { Text = this.tokenizer.TokenText };
                    this.tokenizer.GetNextToken();

                    // Last property in the object should not be followed by a comma.
                    // Hence asserting that next token is yet another name.
                    ExceptionUtilities.Assert(this.tokenizer.TokenType == JsonTokenType.String, "Invalid Token");
                }
            }

            if (this.tokenizer.TokenType == JsonTokenType.RightCurly)
            {
                result.SetAnnotation(new JsonEndObjectTextAnnotation() { Text = this.tokenizer.TokenText });

                // Parser should always be promoted to next token when leaving the loop
                if (this.tokenizer.HasMoreTokens())
                {
                    this.tokenizer.GetNextToken();
                }
            }

            return result;
        }

        /// <summary>
        /// Parses a property starting with the colon after the property name.
        /// </summary>
        /// <param name="propertyName">The name of the property.</param>
        /// <param name="propertyNameTextAnnotation">The name text annotation for the property.</param>
        /// <returns>parsed object</returns>
        private JsonProperty ParsePropertyWithName(string propertyName, JsonPropertyNameTextAnnotation propertyNameTextAnnotation)
        {
            // Each name is followed by : (colon)
            ExceptionUtilities.Assert(this.tokenizer.TokenType == JsonTokenType.Colon, "Invalid Token");

            var nameValueSeparatorTextAnnotation = new JsonPropertyNameValueSeparatorTextAnnotation() { Text = this.tokenizer.TokenText };
            this.tokenizer.GetNextToken();

            // Colon is followed by a value
            ExceptionUtilities.Assert(this.IsValueType(this.tokenizer.TokenType), "Invalid Token");

            JsonValue value = this.ParseValue();
            var property = new JsonProperty(propertyName, value);
            property.SetAnnotation(propertyNameTextAnnotation);
            property.SetAnnotation(nameValueSeparatorTextAnnotation);

            return property;
        }

        /// <summary>
        /// Parses the token of type number
        /// </summary>
        /// <returns>Parsed number</returns>
        private JsonPrimitiveValue ParseNumber()
        {
            var result = this.tokenizer.Value;
            var textAnnotation = new JsonPrimitiveValueTextAnnotation() { Text = this.tokenizer.TokenText };
            this.tokenizer.GetNextToken();
            var primitiveValue = new JsonPrimitiveValue(result);
            primitiveValue.SetAnnotation(textAnnotation);
            return primitiveValue;
        }

        /// <summary>
        /// Parses the token of type string
        /// </summary>
        /// <returns>Parsed string</returns>
        private JsonPrimitiveValue ParseString()
        {
            string result = (string)this.tokenizer.Value;
            var textAnnotation = new JsonPrimitiveValueTextAnnotation() { Text = this.tokenizer.TokenText };
            this.tokenizer.GetNextToken();
            var primitiveValue = new JsonPrimitiveValue(result);
            primitiveValue.SetAnnotation(textAnnotation);
            return primitiveValue;
        }

        /// <summary>
        /// Parses the token of type null
        /// </summary>
        /// <returns>Parsed NullValue</returns>
        private JsonPrimitiveValue ParseNull()
        {
            var result = this.tokenizer.Value;
            var textAnnotation = new JsonPrimitiveValueTextAnnotation() { Text = this.tokenizer.TokenText };
            this.tokenizer.GetNextToken();
            var primitiveValue = new JsonPrimitiveValue(result);
            primitiveValue.SetAnnotation(textAnnotation);
            return primitiveValue;
        }

        /// <summary>
        /// Parses the token of type bool
        /// </summary>
        /// <returns>Parsed Bool Value</returns>
        private JsonPrimitiveValue ParseBool()
        {
            var result = this.tokenizer.Value;
            var textAnnotation = new JsonPrimitiveValueTextAnnotation() { Text = this.tokenizer.TokenText };
            this.tokenizer.GetNextToken();
            var primitiveValue = new JsonPrimitiveValue(result);
            primitiveValue.SetAnnotation(textAnnotation);
            return primitiveValue;
        }

        /// <summary>
        /// Parses the property name as string
        /// </summary>
        /// <returns>Parsed name.</returns>
        private string ParseName(out JsonPropertyNameTextAnnotation nameTextAnnotation)
        {
            string result = (string)this.tokenizer.Value;
            nameTextAnnotation = new JsonPropertyNameTextAnnotation() { Text = this.tokenizer.TokenText };
            this.tokenizer.GetNextToken();
            return result;
        }

        /// <summary>
        /// Parses the token of type array
        /// </summary>
        /// <returns>Parsed array</returns>
        private JsonArray ParseArray()
        {
            // Array can start only with '['
            ExceptionUtilities.Assert(this.tokenizer.TokenType == JsonTokenType.LeftSquareBracket, "Invalid Token");

            // Should not end before we get ']'
            ExceptionUtilities.Assert(this.tokenizer.HasMoreTokens(), "Invalid End Of Stream");
            var startArrayTextAnnotation = new JsonStartArrayTextAnnotation() { Text = this.tokenizer.TokenText };
            this.tokenizer.GetNextToken();

            // Array is an ordered collection of values
            ExceptionUtilities.Assert(this.IsValueType(this.tokenizer.TokenType) || this.tokenizer.TokenType == JsonTokenType.RightSquareBracket, "InvalidToken");
            var result = new JsonArray();
            result.SetAnnotation(startArrayTextAnnotation);
            JsonArrayElementSeparatorTextAnnotation arrayElementSeparatorTextAnnotation = null;
            while (this.tokenizer.HasMoreTokens())
            {
                if (this.IsValueType(this.tokenizer.TokenType))
                {
                    JsonValue v = this.ParseValue();
                    result.Add(v);
                    if (arrayElementSeparatorTextAnnotation != null)
                    {
                        v.SetAnnotation(arrayElementSeparatorTextAnnotation);
                        arrayElementSeparatorTextAnnotation = null;
                    }

                    // Values are separated by , (comma).
                    ExceptionUtilities.Assert(this.tokenizer.TokenType == JsonTokenType.Comma || this.tokenizer.TokenType == JsonTokenType.RightSquareBracket, "Invalid Token");
                }
                else if (this.tokenizer.TokenType == JsonTokenType.RightSquareBracket)
                {
                    break;
                }
                else if (this.tokenizer.TokenType == JsonTokenType.Comma)
                {
                    arrayElementSeparatorTextAnnotation = new JsonArrayElementSeparatorTextAnnotation() { Text = this.tokenizer.TokenText };
                    this.tokenizer.GetNextToken();

                    // Last element of the array cannot be followed by a comma.
                    ExceptionUtilities.Assert(this.IsValueType(this.tokenizer.TokenType) || this.tokenizer.TokenType == JsonTokenType.RightSquareBracket, "InvalidToken");
                }
            }

            if (this.tokenizer.TokenType == JsonTokenType.RightSquareBracket)
            {
                result.SetAnnotation(new JsonEndArrayTextAnnotation() { Text = this.tokenizer.TokenText });

                if (this.tokenizer.HasMoreTokens())
                {
                    this.tokenizer.GetNextToken();
                }
            }

            return result;
        }

        private bool IsValueType(JsonTokenType token)
        {
            switch (token)
            {
                case JsonTokenType.String:
                case JsonTokenType.Integer:
                case JsonTokenType.Float:
                case JsonTokenType.LeftCurly:
                case JsonTokenType.LeftSquareBracket:
                case JsonTokenType.True:
                case JsonTokenType.False:
                case JsonTokenType.Null:
                    return true;
                default:
                    return false;
            }
        }
    }
}
