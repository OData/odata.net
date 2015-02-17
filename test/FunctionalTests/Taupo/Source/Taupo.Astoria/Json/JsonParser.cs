//---------------------------------------------------------------------
// <copyright file="JsonParser.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Json
{
    using Microsoft.Test.Taupo.Astoria.Contracts.Json;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Parses Json
    /// </summary>
    public class JsonParser
    {
        private JsonTokenizer tokenizer;

        /// <summary>
        /// Initializes a new instance of the JsonParser class
        /// </summary>
        /// <param name="tokenizer">An instance of <see cref="JsonTokenizer"/></param>
        public JsonParser(JsonTokenizer tokenizer)
        {
            ExceptionUtilities.CheckArgumentNotNull(tokenizer, "tokenizer");
            this.tokenizer = tokenizer;
        }

        /// <summary>
        ///  Parses Json token values
        /// </summary>
        /// <returns>Parsed value</returns>
        public JsonValue ParseValue()
        {
            JsonValue value;
            var originalPosition = this.tokenizer.CreateMarkerForCurrentPosition();

            if (this.tokenizer.TokenType == JsonTokenType.Integer || this.tokenizer.TokenType == JsonTokenType.Float)
            {
                value = this.ParseNumber();
            }
            else if (this.tokenizer.TokenType == JsonTokenType.String)
            {
                value = this.ParseString();
            }
            else if (this.tokenizer.TokenType == JsonTokenType.LeftCurly)
            {
                value = this.ParseObject();
            }
            else if (this.tokenizer.TokenType == JsonTokenType.LeftSquareBracket)
            {
                value = this.ParseArray();
            }
            else if (this.tokenizer.TokenType == JsonTokenType.True || this.tokenizer.TokenType == JsonTokenType.False)
            {
                value = this.ParseBool();
            }
            else
            {
                ExceptionUtilities.Assert(this.tokenizer.TokenType == JsonTokenType.Null, "Invalid Token");
                value = this.ParseNull();
            }

            var originalText = this.tokenizer.GetTextSinceMarker(originalPosition);
            value.Annotations.Add(new JsonOriginalTextAnnotation() { Text = originalText });
            return value;
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
            
            this.tokenizer.GetNextToken();

            // An object is an unordered set of name/value pairs. So next token cannot be anything other than name or '}'
            ExceptionUtilities.Assert(this.tokenizer.TokenType == JsonTokenType.RightCurly || this.tokenizer.TokenType == JsonTokenType.String, "Invalid Token");

            var result = new JsonObject();
            while (this.tokenizer.HasMoreTokens())
            {
                ExceptionUtilities.Assert(this.tokenizer.TokenType == JsonTokenType.String || this.tokenizer.TokenType == JsonTokenType.Comma || this.tokenizer.TokenType == JsonTokenType.RightCurly, "Invalid Token");

                // End of Object
                if (this.tokenizer.TokenType == JsonTokenType.RightCurly)
                {
                    // Parser should always be promoted to next token when leaving the loop
                    if (this.tokenizer.HasMoreTokens())
                    {
                        this.tokenizer.GetNextToken();
                    }

                    return result;
                }
                else if (this.tokenizer.TokenType == JsonTokenType.String)
                {
                    string name = this.ParseName();

                    // Each name is followed by : (colon)
                    ExceptionUtilities.Assert(this.tokenizer.TokenType == JsonTokenType.Colon, "Invalid Token");
                    
                    this.tokenizer.GetNextToken();

                    // Colon is followed by a value
                    ExceptionUtilities.Assert(this.IsValueType(this.tokenizer.TokenType), "Invalid Token");
                    
                    JsonValue value = this.ParseValue();
                    result.Add(new JsonProperty(name, value));

                    // name/value pairs are separated by , (comma) or value can be followed by '}' (right brace)
                    ExceptionUtilities.Assert(this.tokenizer.TokenType == JsonTokenType.Comma || this.tokenizer.TokenType == JsonTokenType.RightCurly, "Invalid Token");
                }
                else if (this.tokenizer.TokenType == JsonTokenType.Comma)
                {
                    this.tokenizer.GetNextToken();

                    // Last property in the object should not be followed by a comma.
                    // Hence asserting that next token is yet another name.
                    ExceptionUtilities.Assert(this.tokenizer.TokenType == JsonTokenType.String, "Invalid Token");
                }
            }

            return result;
        }

        /// <summary>
        /// Parses the token of type number
        /// </summary>
        /// <returns>Parsed number</returns>
        private JsonPrimitiveValue ParseNumber()
        {
            var result = this.tokenizer.Value;
            this.tokenizer.GetNextToken();
            return new JsonPrimitiveValue(result);
        }

        /// <summary>
        /// Parses the token of type string
        /// </summary>
        /// <returns>Parsed string</returns>
        private JsonPrimitiveValue ParseString()
        {
            string result = (string)this.tokenizer.Value;
            this.tokenizer.GetNextToken();
            return new JsonPrimitiveValue(result);
        }

        /// <summary>
        /// Parses the token of type null
        /// </summary>
        /// <returns>Parsed NullValue</returns>
        private JsonPrimitiveValue ParseNull()
        {
            var result = (object)this.tokenizer.Value;
            this.tokenizer.GetNextToken();
            return new JsonPrimitiveValue(result);
        }

        /// <summary>
        /// Parses the token of type bool
        /// </summary>
        /// <returns>Parsed Bool Value</returns>
        private JsonPrimitiveValue ParseBool()
        {
            var result = (bool)this.tokenizer.Value;
            this.tokenizer.GetNextToken();
            return new JsonPrimitiveValue(result);
        }

        /// <summary>
        /// Parses the property name as string
        /// </summary>
        /// <returns>Parsed name.</returns>
        private string ParseName()
        {
            string result = (string)this.tokenizer.Value;
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
            this.tokenizer.GetNextToken();

            // Array is an ordered collection of values
            ExceptionUtilities.Assert(this.IsValueType(this.tokenizer.TokenType) || this.tokenizer.TokenType == JsonTokenType.RightSquareBracket, "InvalidToken");
            var result = new JsonArray();
            while (this.tokenizer.HasMoreTokens())
            {
                if (this.IsValueType(this.tokenizer.TokenType))
                {
                    JsonValue v = this.ParseValue();
                    result.Add(v);

                    // Values are separated by , (comma).
                    ExceptionUtilities.Assert(this.tokenizer.TokenType == JsonTokenType.Comma || this.tokenizer.TokenType == JsonTokenType.RightSquareBracket, "Invalid Token");
                }
                else if (this.tokenizer.TokenType == JsonTokenType.RightSquareBracket)
                {
                    if (this.tokenizer.HasMoreTokens())
                    {
                        this.tokenizer.GetNextToken();
                    }

                    return result;
                }
                else if (this.tokenizer.TokenType == JsonTokenType.Comma)
                {
                    this.tokenizer.GetNextToken();

                    // Last element of the array cannot be followed by a comma.
                    ExceptionUtilities.Assert(this.IsValueType(this.tokenizer.TokenType) || this.tokenizer.TokenType == JsonTokenType.RightSquareBracket, "InvalidToken");
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
