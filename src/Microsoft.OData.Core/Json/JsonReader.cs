//---------------------------------------------------------------------
// <copyright file="JsonReader.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

#if SPATIAL
namespace Microsoft.Data.Spatial
#else
namespace Microsoft.OData.Core.Json
#endif
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.IO;
#if SPATIAL
    using Microsoft.Spatial;
#endif
    using System.Text;
    #endregion Namespaces

    /// <summary>
    /// Reader for the JSON format. http://www.json.org
    /// </summary>
    [DebuggerDisplay("{NodeType}: {Value}")]
#if ODATALIB_PUBLICJSONREADER
    public
#else
    internal
#endif
    class JsonReader
    {
        /// <summary>
        /// The initial size of the buffer of characters.
        /// </summary>
        /// <remarks>
        /// 4K (page size) divided by the size of a single character 2 and a little less
        /// so that array structures also fit into that page.
        /// The goal is for the entire buffer to fit into one page so that we don't cause
        /// too many L1 cache misses.
        /// </remarks>
        private const int InitialCharacterBufferSize = ((4 * 1024) / 2) - 8;

        /// <summary>
        /// Maximum number of characters to move in the buffer. If the current token size is bigger than this, we will allocate a larger buffer.
        /// </summary>
        /// <remarks>This threshold is copied from the XmlReader implementation.</remarks>
        private const int MaxCharacterCountToMove = 128 / 2;

        /// <summary>
        /// The text reader to read input characters from.
        /// </summary>
        private readonly TextReader reader;

        /// <summary>
        /// If it is IEEE754Compatible, read quoted string for INT64 and decimal;
        /// otherwise read number directly.
        /// </summary>
        private readonly bool isIeee754Compatible;

        /// <summary>
        /// Stack of scopes.
        /// </summary>
        /// <remarks>
        /// At the beginning the Root scope is pushed to the stack and stays there for the entire parsing 
        ///   (so that we don't have to check for empty stack and also to track the number of root-level values)
        /// Each time a new object or array is started the Object or Array scope is pushed to the stack.
        /// If a property inside an Object is found, the Property scope is pushed to the stack.
        /// The Property is popped once we find the value for the property.
        /// The Object and Array scopes are popped when their end is found.
        /// </remarks>
        private readonly Stack<Scope> scopes;

        /// <summary>true if annotations are allowed and thus the reader has to 
        /// accept more characters in property names than we do normally; otherwise false.</summary>
        private readonly bool allowAnnotations;

        /// <summary>
        /// End of input from the reader was already reached.
        /// </summary>
        /// <remarks>This is used to avoid calling Read on the text reader multiple times
        /// even though it already reported the end of input.</remarks>
        private bool endOfInputReached;

        /// <summary>
        /// Buffer of characters from the input.
        /// </summary>
        private char[] characterBuffer;

        /// <summary>
        /// Number of characters available in the input buffer.
        /// </summary>
        /// <remarks>This can have value of 0 to characterBuffer.Length.</remarks>
        private int storedCharacterCount;

        /// <summary>
        /// Index into the characterBuffer which points to the first character
        /// of the token being currently processed (while in the Read method)
        /// or of the next token to be processed (while in the caller code).
        /// </summary>
        /// <remarks>This can have value from 0 to storedCharacterCount.</remarks>
        private int tokenStartIndex;

        /// <summary>
        /// The last reported node type.
        /// </summary>
        private JsonNodeType nodeType;

        /// <summary>
        /// The value of the last reported node.
        /// </summary>
        private object nodeValue;

        /// <summary>
        /// Cached string builder to be used when constructing string values (needed to resolve escape sequences).
        /// </summary>
        /// <remarks>The string builder instance is cached to avoid excessive allocation when many string values with escape sequences
        /// are found in the payload.</remarks>
        private StringBuilder stringValueBuilder;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="reader">The text reader to read input characters from.</param>
        /// <param name="jsonFormat">The specific JSON-based format expected by the reader.</param>
        /// <param name="isIeee754Compatible">If it is isIeee754Compatible</param>
        public JsonReader(TextReader reader, ODataFormat jsonFormat, bool isIeee754Compatible)
        {
            Debug.Assert(reader != null, "reader != null");
            Debug.Assert(jsonFormat == ODataFormat.Json, "Expected a json-based format to create a JsonReader");

            this.nodeType = JsonNodeType.None;
            this.nodeValue = null;
            this.reader = reader;
            this.characterBuffer = new char[InitialCharacterBufferSize];
            this.storedCharacterCount = 0;
            this.tokenStartIndex = 0;
            this.endOfInputReached = false;
            this.isIeee754Compatible = isIeee754Compatible;
            this.allowAnnotations = jsonFormat == ODataFormat.Json;
            this.scopes = new Stack<Scope>();
            this.scopes.Push(new Scope(ScopeType.Root));
        }

        /// <summary>
        /// Various scope types for Json writer.
        /// </summary>
        private enum ScopeType
        {
            /// <summary>
            /// Root scope - the top-level of the JSON content.
            /// </summary>
            /// <remarks>This scope is only once on the stack and that is at the bottom, always.
            /// It's used to track the fact that only one top-level value is allowed.</remarks>
            Root,

            /// <summary>
            /// Array scope - inside an array.
            /// </summary>
            /// <remarks>This scope is pushed when [ is found and is active before the first and between the elements in the array.
            /// Between the elements it's active when the parser is in front of the comma, the parser is never after comma as then
            /// it always immediately processed the next token.</remarks>
            Array,

            /// <summary>
            /// Object scope - inside the object (but not in a property value).
            /// </summary>
            /// <remarks>This scope is pushed when { is found and is active before the first and between the properties in the object.
            /// Between the properties it's active when the parser is in front of the comma, the parser is never after comma as then
            /// it always immediately processed the next token.</remarks>
            Object,

            /// <summary>
            /// Property scope - after the property name and colon and throughout the value.
            /// </summary>
            /// <remarks>This scope is pushed when a property name and colon is found.
            /// The scope remains on the stack while the property value is parsed, but once the property value ends, it's immediately removed
            /// so that it doesn't appear on the stack after the value (ever).</remarks>
            Property,
        }

        /// <summary>
        /// The value of the last reported node.
        /// </summary>
        /// <remarks>This is non-null only if the last node was a PrimitiveValue or Property.
        /// If the last node is a PrimitiveValue this property returns the value:
        /// - null if the null token was found.
        /// - boolean if the true or false token was found.
        /// - string if a string token was found.
        /// - DateTime if a string token formatted as DateTime was found.
        /// - Int32 if a number which fits into the Int32 was found.
        /// - Double if a number which doesn't fit into Int32 was found.
        /// If the last node is a Property this property returns a string which is the name of the property.
        /// </remarks>
        public virtual object Value
        {
            get
            {
                return this.nodeValue;
            }
        }

        /// <summary>
        /// The type of the last node read.
        /// </summary>
        public virtual JsonNodeType NodeType
        {
            get
            {
                return this.nodeType;
            }
        }

        /// <summary>
        /// if it is IEEE754 compatible
        /// </summary>
        public virtual bool IsIeee754Compatible
        {
            get
            {
                return this.isIeee754Compatible;
            }
        }

        /// <summary>
        /// Reads the next node from the input.
        /// </summary>
        /// <returns>true if a new node was found, or false if end of input was reached.</returns>
        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Not really feasible to extract code to methods without introducing unnecessary complexity.")]
        public virtual bool Read()
        {
            // Reset the node value.
            this.nodeValue = null;

#if DEBUG
            // Reset the node type to None - so that we can verify that the Read method actually sets it.
            this.nodeType = JsonNodeType.None;
#endif

            // Skip any whitespace characters.
            // This also makes sure that we have at least one non-whitespace character available.
            if (!this.SkipWhitespaces())
            {
                return this.EndOfInput();
            }

            Debug.Assert(
                this.tokenStartIndex < this.storedCharacterCount && !IsWhitespaceCharacter(this.characterBuffer[this.tokenStartIndex]),
                "The SkipWhitespaces didn't correctly skip all whitespace characters from the input.");

            Scope currentScope = this.scopes.Peek();

            bool commaFound = false;
            if (this.characterBuffer[this.tokenStartIndex] == ',')
            {
                commaFound = true;
                this.tokenStartIndex++;

                // Note that validity of the comma is verified below depending on the current scope.
                // Skip all whitespaces after comma.
                // Note that this causes "Unexpected EOF" error if the comma is the last thing in the input.
                // It might not be the best error message in certain cases, but it's still correct (a JSON payload can never end in comma).
                if (!this.SkipWhitespaces())
                {
                    return this.EndOfInput();
                }

                Debug.Assert(
                    this.tokenStartIndex < this.storedCharacterCount && !IsWhitespaceCharacter(this.characterBuffer[this.tokenStartIndex]),
                    "The SkipWhitespaces didn't correctly skip all whitespace characters from the input.");
            }

            switch (currentScope.Type)
            {
                case ScopeType.Root:
                    if (commaFound)
                    {
                        throw CreateException(Strings.JsonReader_UnexpectedComma(ScopeType.Root));
                    }

                    if (currentScope.ValueCount > 0)
                    {
                        // We already found the top-level value, so fail
                        throw CreateException(Strings.JsonReader_MultipleTopLevelValues);
                    }

                    // We expect a "value" - start array, start object or primitive value
                    this.nodeType = this.ParseValue();
                    break;

                case ScopeType.Array:
                    if (commaFound && currentScope.ValueCount == 0)
                    {
                        throw CreateException(Strings.JsonReader_UnexpectedComma(ScopeType.Array));
                    }

                    // We might see end of array here
                    if (this.characterBuffer[this.tokenStartIndex] == ']')
                    {
                        this.tokenStartIndex++;

                        // End of array is only valid when there was no comma before it.
                        if (commaFound)
                        {
                            throw CreateException(Strings.JsonReader_UnexpectedComma(ScopeType.Array));
                        }

                        this.PopScope();
                        this.nodeType = JsonNodeType.EndArray;
                        break;
                    }

                    if (!commaFound && currentScope.ValueCount > 0)
                    {
                        throw CreateException(Strings.JsonReader_MissingComma(ScopeType.Array));
                    }

                    // We expect element which is a "value" - start array, start object or primitive value
                    this.nodeType = this.ParseValue();
                    break;

                case ScopeType.Object:
                    if (commaFound && currentScope.ValueCount == 0)
                    {
                        throw CreateException(Strings.JsonReader_UnexpectedComma(ScopeType.Object));
                    }

                    // We might see end of object here
                    if (this.characterBuffer[this.tokenStartIndex] == '}')
                    {
                        this.tokenStartIndex++;

                        // End of object is only valid when there was no comma before it.
                        if (commaFound)
                        {
                            throw CreateException(Strings.JsonReader_UnexpectedComma(ScopeType.Object));
                        }

                        this.PopScope();
                        this.nodeType = JsonNodeType.EndObject;
                        break;
                    }
                    else
                    {
                        if (!commaFound && currentScope.ValueCount > 0)
                        {
                            throw CreateException(Strings.JsonReader_MissingComma(ScopeType.Object));
                        }

                        // We expect a property here
                        this.nodeType = this.ParseProperty();
                        break;
                    }

                case ScopeType.Property:
                    if (commaFound)
                    {
                        throw CreateException(Strings.JsonReader_UnexpectedComma(ScopeType.Property));
                    }

                    // We expect the property value, which is a "value" - start array, start object or primitive value
                    this.nodeType = this.ParseValue();
                    break;

                default:
#if SPATIAL
                    throw CreateException(Strings.JsonReader_InternalError);
#else
                    throw CreateException(Strings.General_InternalError(InternalErrorCodes.JsonReader_Read));
#endif
            }

            Debug.Assert(
                this.nodeType != JsonNodeType.None && this.nodeType != JsonNodeType.EndOfInput,
                "Read should never go back to None and EndOfInput should be reported by directly returning.");

            return true;
        }

        /// <summary>
        /// Extracts text before and including the current token for use in an error message.
        /// </summary>
        /// <param name="maxContextLength">The maximum number of characters to include in the parsing context.</param>
        /// <returns>Text from the character buffer.</returns>
        internal string CaptureParsingContext(int maxContextLength = 100)
        {
            int startIndex = Math.Max(0, this.tokenStartIndex - maxContextLength);
            int actualLength = this.tokenStartIndex - startIndex;

            return new String(this.characterBuffer, startIndex, actualLength);
        }

        /// <summary>
        /// Determines if a given character is a whitespace character.
        /// </summary>
        /// <param name="character">The character to test.</param>
        /// <returns>true if the <paramref name="character"/> is a whitespace; false otherwise.</returns>
        /// <remarks>Note that the behavior of this method is different from Char.IsWhitespace, since that method
        /// returns true for all characters defined as whitespace by the Unicode spec (which is a lot of characters),
        /// this one on the other hand recognizes just the whitespaces as defined by the JSON spec.</remarks>
        private static bool IsWhitespaceCharacter(char character)
        {
            // The whitespace characters are 0x20 (space), 0x09 (tab), 0x0A (new line), 0x0D (carriage return)
            // Anything above 0x20 is a non-whitespace character.
            if (character > (char)0x20 || character != (char)0x20 && character != (char)0x09 && character != (char)0x0A && character != (char)0x0D)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// Parses a "value", that is an array, object or primitive value.
        /// </summary>
        /// <returns>The node type to report to the user.</returns>
        private JsonNodeType ParseValue()
        {
            Debug.Assert(
                this.tokenStartIndex < this.storedCharacterCount && !IsWhitespaceCharacter(this.characterBuffer[this.tokenStartIndex]),
                "The SkipWhitespaces wasn't called or it didn't correctly skip all whitespace characters from the input.");
            Debug.Assert(this.scopes.Count >= 1 && this.scopes.Peek().Type != ScopeType.Object, "Value can only occure at the root, in array or as a property value.");

            // Increase the count of values under the current scope.
            this.scopes.Peek().ValueCount++;

            char currentCharacter = this.characterBuffer[this.tokenStartIndex];
            switch (currentCharacter)
            {
                case '{':
                    // Start of object
                    this.PushScope(ScopeType.Object);
                    this.tokenStartIndex++;
                    return JsonNodeType.StartObject;

                case '[':
                    // Start of array
                    this.PushScope(ScopeType.Array);
                    this.tokenStartIndex++;
                    return JsonNodeType.StartArray;

                case '"':
                case '\'':
                    // String primitive value
                    bool hasLeadingBackslash;
                    this.nodeValue = this.ParseStringPrimitiveValue(out hasLeadingBackslash);
                    break;

                case 'n':
                    this.nodeValue = this.ParseNullPrimitiveValue();
                    break;

                case 't':
                case 'f':
                    this.nodeValue = this.ParseBooleanPrimitiveValue();
                    break;

                default:
                    // COMPAT 47: JSON number can start with dot.
                    // The JSON spec doesn't allow numbers to start with ., but WCF DS does. We will follow the WCF DS behavior for compatibility.
                    if (Char.IsDigit(currentCharacter) || (currentCharacter == '-') || (currentCharacter == '.'))
                    {
                        this.nodeValue = this.ParseNumberPrimitiveValue();
                        break;
                    }
                    else
                    {
                        // Unknown token - fail.
                        throw CreateException(Strings.JsonReader_UnrecognizedToken);
                    }
            }

            this.TryPopPropertyScope();
            return JsonNodeType.PrimitiveValue;
        }

        /// <summary>
        /// Parses a property name and the colon after it.
        /// </summary>
        /// <returns>The node type to report to the user.</returns>
        private JsonNodeType ParseProperty()
        {
            // Increase the count of values under the object (the number of properties).
            Debug.Assert(this.scopes.Count >= 1 && this.scopes.Peek().Type == ScopeType.Object, "Property can only occur in an object.");
            this.scopes.Peek().ValueCount++;

            this.PushScope(ScopeType.Property);

            // Parse the name of the property
            this.nodeValue = this.ParseName();

            if (string.IsNullOrEmpty((string)this.nodeValue))
            {
                // The name can't be empty.
                throw CreateException(Strings.JsonReader_InvalidPropertyNameOrUnexpectedComma((string)this.nodeValue));
            }

            if (!this.SkipWhitespaces() || this.characterBuffer[this.tokenStartIndex] != ':')
            {
                // We need the colon character after the property name
                throw CreateException(Strings.JsonReader_MissingColon((string)this.nodeValue));
            }

            // Consume the colon.
            Debug.Assert(this.characterBuffer[this.tokenStartIndex] == ':', "The above should verify that there's a colon.");
            this.tokenStartIndex++;

            return JsonNodeType.Property;
        }

        /// <summary>
        /// Parses a primitive string value.
        /// </summary>
        /// <returns>The value of the string primitive value.</returns>
        /// <remarks>
        /// Assumes that the current token position points to the opening quote.
        /// Note that the string parsing can never end with EndOfInput, since we're already seen the quote.
        /// So it can either return a string succesfully or fail.</remarks>
        private string ParseStringPrimitiveValue()
        {
            bool hasLeadingBackslash;
            return this.ParseStringPrimitiveValue(out hasLeadingBackslash);
        }

        /// <summary>
        /// Parses a primitive string value.
        /// </summary>
        /// <param name="hasLeadingBackslash">Set to true if the first character in the string was a backslash. This is used when parsing DateTime values
        /// since they must start with an escaped slash character (\/).</param>
        /// <returns>The value of the string primitive value.</returns>
        /// <remarks>
        /// Assumes that the current token position points to the opening quote.
        /// Note that the string parsing can never end with EndOfInput, since we're already seen the quote.
        /// So it can either return a string succesfully or fail.</remarks>
        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Splitting the function would make it hard to understand.")]
        private string ParseStringPrimitiveValue(out bool hasLeadingBackslash)
        {
            Debug.Assert(this.tokenStartIndex < this.storedCharacterCount, "At least the quote must be present.");

            hasLeadingBackslash = false;

            // COMPAT 45: We allow both double and single quotes around a primitive string
            // Even though JSON spec only allows double quotes.
            char openingQuoteCharacter = this.characterBuffer[this.tokenStartIndex];
            Debug.Assert(openingQuoteCharacter == '"' || openingQuoteCharacter == '\'', "The quote character must be the current character when this method is called.");

            // Consume the quote character
            this.tokenStartIndex++;

            // String builder to be used if we need to resolve escape sequences.
            StringBuilder valueBuilder = null;

            int currentCharacterTokenRelativeIndex = 0;
            while ((this.tokenStartIndex + currentCharacterTokenRelativeIndex) < this.storedCharacterCount || this.ReadInput())
            {
                Debug.Assert((this.tokenStartIndex + currentCharacterTokenRelativeIndex) < this.storedCharacterCount, "ReadInput didn't read more data but returned true.");

                char character = this.characterBuffer[this.tokenStartIndex + currentCharacterTokenRelativeIndex];
                if (character == '\\')
                {
                    // If we're at the begining of the string
                    // (means that relative token index must be 0 and we must not have consumed anything into our value builder yet)
                    if (currentCharacterTokenRelativeIndex == 0 && valueBuilder == null)
                    {
                        hasLeadingBackslash = true;
                    }

                    // We will need the stringbuilder to resolve the escape sequences.
                    if (valueBuilder == null)
                    {
                        if (this.stringValueBuilder == null)
                        {
                            this.stringValueBuilder = new StringBuilder();
                        }
                        else
                        {
                            this.stringValueBuilder.Length = 0;
                        }

                        valueBuilder = this.stringValueBuilder;
                    }

                    // Append everything up to the \ character to the value.
                    valueBuilder.Append(this.ConsumeTokenToString(currentCharacterTokenRelativeIndex));
                    currentCharacterTokenRelativeIndex = 0;
                    Debug.Assert(this.characterBuffer[this.tokenStartIndex] == '\\', "We should have consumed everything up to the escape character.");

                    // Escape sequence - we need at least two characters, the backslash and the one character after it.
                    if (!this.EnsureAvailableCharacters(2))
                    {
                        throw CreateException(Strings.JsonReader_UnrecognizedEscapeSequence("\\"));
                    }

                    // To simplify the code, consume the character after the \ as well, since that is the start of the escape sequence.
                    character = this.characterBuffer[this.tokenStartIndex + 1];
                    this.tokenStartIndex += 2;

                    switch (character)
                    {
                        case 'b':
                            valueBuilder.Append('\b');
                            break;
                        case 'f':
                            valueBuilder.Append('\f');
                            break;
                        case 'n':
                            valueBuilder.Append('\n');
                            break;
                        case 'r':
                            valueBuilder.Append('\r');
                            break;
                        case 't':
                            valueBuilder.Append('\t');
                            break;
                        case '\\':
                        case '\"':
                        case '\'':
                        case '/':
                            valueBuilder.Append(character);
                            break;
                        case 'u':
                            Debug.Assert(currentCharacterTokenRelativeIndex == 0, "The token should be starting at the first character after the \\u");

                            // We need 4 hex characters
                            if (!this.EnsureAvailableCharacters(4))
                            {
                                throw CreateException(Strings.JsonReader_UnrecognizedEscapeSequence("\\uXXXX"));
                            }

                            string unicodeHexValue = this.ConsumeTokenToString(4);
                            int characterValue;
                            if (!Int32.TryParse(unicodeHexValue, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out characterValue))
                            {
                                throw CreateException(Strings.JsonReader_UnrecognizedEscapeSequence("\\u" + unicodeHexValue));
                            }

                            valueBuilder.Append((char)characterValue);
                            break;
                        default:
                            throw CreateException(Strings.JsonReader_UnrecognizedEscapeSequence("\\" + character));
                    }
                }
                else if (character == openingQuoteCharacter)
                {
                    // Consume everything up to the quote character
                    string result = this.ConsumeTokenToString(currentCharacterTokenRelativeIndex);
                    Debug.Assert(this.characterBuffer[this.tokenStartIndex] == openingQuoteCharacter, "We should have consumed everything up to the quote character.");

                    // Consume the quote character as well.
                    this.tokenStartIndex++;

                    if (valueBuilder != null)
                    {
                        valueBuilder.Append(result);
                        result = valueBuilder.ToString();
                    }

                    return result;
                }
                else
                {
                    // Normal character, just skip over it - it will become part of the value as is.
                    currentCharacterTokenRelativeIndex++;
                }
            }

            throw CreateException(Strings.JsonReader_UnexpectedEndOfString);
        }

        /// <summary>
        /// Parses the null primitive value.
        /// </summary>
        /// <returns>Always returns null if successful. Otherwise throws.</returns>
        /// <remarks>Assumes that the current token position points to the 'n' character.</remarks>
        private object ParseNullPrimitiveValue()
        {
            Debug.Assert(
                this.tokenStartIndex < this.storedCharacterCount && this.characterBuffer[this.tokenStartIndex] == 'n',
                "The method should only be called when the 'n' character is the start of the token.");

            // We can call ParseName since we know the first character is 'n' and thus it won't be quoted.
            string token = this.ParseName();

            if (!string.Equals(token, JsonConstants.JsonNullLiteral))
            {
                throw CreateException(Strings.JsonReader_UnexpectedToken(token));
            }

            return null;
        }

        /// <summary>
        /// Parses the true or false primitive values.
        /// </summary>
        /// <returns>true of false boolean value if successful. Otherwise throws.</returns>
        /// <remarks>Assumes that the current token position points to the 't' or 'f' character.</remarks>
        private object ParseBooleanPrimitiveValue()
        {
            Debug.Assert(
                this.tokenStartIndex < this.storedCharacterCount && (this.characterBuffer[this.tokenStartIndex] == 't' || this.characterBuffer[this.tokenStartIndex] == 'f'),
                "The method should only be called when the 't' or 'f' character is the start of the token.");

            // We can call ParseName since we know the first character is 't' or 'f' and thus it won't be quoted.
            string token = this.ParseName();

            if (string.Equals(token, JsonConstants.JsonFalseLiteral))
            {
                return false;
            }
            
            if (string.Equals(token, JsonConstants.JsonTrueLiteral))
            {
                return true;
            }

            throw CreateException(Strings.JsonReader_UnexpectedToken(token));
        }

        /// <summary>
        /// Parses the number primitive values.
        /// </summary>
        /// <returns>Parse value to Int32, Decimal or Double. Otherwise throws.</returns>
        /// <remarks>Assumes that the current token position points to the first character of the number, so either digit, dot or dash.</remarks>
        private object ParseNumberPrimitiveValue()
        {
            Debug.Assert(
                this.tokenStartIndex < this.storedCharacterCount && (this.characterBuffer[this.tokenStartIndex] == '.' || this.characterBuffer[this.tokenStartIndex] == '-' || Char.IsDigit(this.characterBuffer[this.tokenStartIndex])),
                "The method should only be called when a digit, dash or dot character is the start of the token.");

            // Walk over all characters which might belong to the number
            // Skip the first one since we already verified it belongs to the number.
            int currentCharacterTokenRelativeIndex = 1;
            while ((this.tokenStartIndex + currentCharacterTokenRelativeIndex) < this.storedCharacterCount || this.ReadInput())
            {
                char character = this.characterBuffer[this.tokenStartIndex + currentCharacterTokenRelativeIndex];
                if (Char.IsDigit(character) ||
                    (character == '.') ||
                    (character == 'E') ||
                    (character == 'e') ||
                    (character == '-') ||
                    (character == '+'))
                {
                    currentCharacterTokenRelativeIndex++;
                }
                else
                {
                    break;
                }
            }

            // We now have all the characters which belong to the number, consume it into a string.
            string numberString = this.ConsumeTokenToString(currentCharacterTokenRelativeIndex);
            double doubleValue;
            int intValue;
            decimal decimalValue;

            // We will first try and convert the value to Int32. If it succeeds, use that.
            // And then, we will try Decimal, since it will lose precision while expected type is specified.
            // Otherwise, we will try and convert the value into a double.
            if (Int32.TryParse(numberString, NumberStyles.Integer, NumberFormatInfo.InvariantInfo, out intValue))
            {
                return intValue;
            }

            // if it is not Ieee754Compatible, decimal will be parsed before double to keep precision
            if (!isIeee754Compatible && Decimal.TryParse(numberString, NumberStyles.Number, NumberFormatInfo.InvariantInfo, out decimalValue))
            {
                return decimalValue;
            }
            
            if (Double.TryParse(numberString, NumberStyles.Float, NumberFormatInfo.InvariantInfo, out doubleValue))
            {
                return doubleValue;
            }
            
            throw CreateException(Strings.JsonReader_InvalidNumberFormat(numberString));
        }

        /// <summary>
        /// Parses a name token.
        /// </summary>
        /// <returns>The value of the name token.</returns>
        /// <remarks>Name tokens are (for backward compat reasons) either
        /// - string value quoted with double quotes.
        /// - string value quoted with single quotes.
        /// - sequence of letters, digits, underscores and dollar signs (without quoted and in any order).</remarks>
        private string ParseName()
        {
            Debug.Assert(this.tokenStartIndex < this.storedCharacterCount, "Must have at least one character available.");

            char firstCharacter = this.characterBuffer[this.tokenStartIndex];
            if ((firstCharacter == '"') || (firstCharacter == '\''))
            {
                return this.ParseStringPrimitiveValue();
            }

            int currentCharacterTokenRelativeIndex = 0;
            do
            {
                Debug.Assert(this.tokenStartIndex < this.storedCharacterCount, "Must have at least one character available.");

                char character = this.characterBuffer[this.tokenStartIndex + currentCharacterTokenRelativeIndex];

                // COMPAT 46: JSON property names don't require quotes and they allow any letter, digit, underscore or dollar sign in them.
                if (character == '_' ||
                    Char.IsLetterOrDigit(character) ||
                    character == '$' ||
                    (this.allowAnnotations && (character == '.' || character == '@')))
                {
                    currentCharacterTokenRelativeIndex++;
                }
                else
                {
                    break;
                }
            }
            while ((this.tokenStartIndex + currentCharacterTokenRelativeIndex) < this.storedCharacterCount || this.ReadInput());

            return this.ConsumeTokenToString(currentCharacterTokenRelativeIndex);
        }

        /// <summary>
        /// Called when end of input is reached.
        /// </summary>
        /// <returns>Always returns false, used for easy readability of the callers.</returns>
        private bool EndOfInput()
        {
            // We should be ending the input only with Root in the scope.
            if (this.scopes.Count > 1)
            {
                // Not all open scopes were closed.
                throw CreateException(Strings.JsonReader_EndOfInputWithOpenScope);
            }

            Debug.Assert(
                this.scopes.Count > 0 && this.scopes.Peek().Type == ScopeType.Root && this.scopes.Peek().ValueCount <= 1,
                "The end of input should only occure with root at the top of the stack with zero or one value.");
            Debug.Assert(this.nodeValue == null, "The node value should have been reset to null.");

            this.nodeType = JsonNodeType.EndOfInput;
            return false;
        }

        /// <summary>
        /// Creates a new scope of type <paramref name="newScopeType"/> and pushes the stack.
        /// </summary>
        /// <param name="newScopeType">The scope type to push.</param>
        private void PushScope(ScopeType newScopeType)
        {
            Debug.Assert(this.scopes.Count >= 1, "The root must always be on the stack.");
            Debug.Assert(newScopeType != ScopeType.Root, "We should never try to push root scope.");
            Debug.Assert(newScopeType != ScopeType.Property || this.scopes.Peek().Type == ScopeType.Object, "We should only try to push property onto an object.");
            Debug.Assert(newScopeType == ScopeType.Property || this.scopes.Peek().Type != ScopeType.Object, "We should only try to push property onto an object.");

            this.scopes.Push(new Scope(newScopeType));
        }

        /// <summary>
        /// Pops a scope from the stack.
        /// </summary>
        private void PopScope()
        {
            Debug.Assert(this.scopes.Count > 1, "We can never pop the root.");
            Debug.Assert(this.scopes.Peek().Type != ScopeType.Property, "We should never try to pop property alone.");

            this.scopes.Pop();
            this.TryPopPropertyScope();
        }

        /// <summary>
        /// Pops a property scope if it's present on the stack.
        /// </summary>
        private void TryPopPropertyScope()
        {
            Debug.Assert(this.scopes.Count > 0, "There should always be at least root on the stack.");
            if (this.scopes.Peek().Type == ScopeType.Property)
            {
                Debug.Assert(this.scopes.Count > 2, "If the property is at the top of the stack there must be an object after it and then root.");
                this.scopes.Pop();
                Debug.Assert(this.scopes.Peek().Type == ScopeType.Object, "The parent of a property must be an object.");
            }
        }

        /// <summary>
        /// Skips all whitespace characters in the input.
        /// </summary>
        /// <returns>true if a non-whitespace character was found in which case the tokenStartIndex is pointing at that character.
        /// false if there are no non-whitespace characters left in the input.</returns>
        private bool SkipWhitespaces()
        {
            do
            {
                for (; this.tokenStartIndex < this.storedCharacterCount; this.tokenStartIndex++)
                {
                    if (!IsWhitespaceCharacter(this.characterBuffer[this.tokenStartIndex]))
                    {
                        return true;
                    }
                }
            }
            while (this.ReadInput());

            return false;
        }

        /// <summary>
        /// Ensures that a specified number of characters after the token start is available in the buffer.
        /// </summary>
        /// <param name="characterCountAfterTokenStart">The number of character after the token to make available.</param>
        /// <returns>true if at least the required number of characters is available; false if end of input was reached.</returns>
        private bool EnsureAvailableCharacters(int characterCountAfterTokenStart)
        {
            while (this.tokenStartIndex + characterCountAfterTokenStart > this.storedCharacterCount)
            {
                if (!this.ReadInput())
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Consumes the <paramref name="characterCount"/> characters starting at the start of the token
        /// and returns them as a string.
        /// </summary>
        /// <param name="characterCount">The number of characters after the token start to consume.</param>
        /// <returns>The string value of the consumed token.</returns>
        private string ConsumeTokenToString(int characterCount)
        {
            Debug.Assert(characterCount >= 0, "characterCount >= 0");
            Debug.Assert(this.tokenStartIndex + characterCount <= this.storedCharacterCount, "characterCount specified characters outside of the available range.");

            string result = new string(this.characterBuffer, this.tokenStartIndex, characterCount);
            this.tokenStartIndex += characterCount;

            return result;
        }

        /// <summary>
        /// Reads more characters from the input.
        /// </summary>
        /// <returns>true if more characters are available; false if end of input was reached.</returns>
        /// <remarks>This may move characters in the characterBuffer, so after this is called
        /// all indeces to the characterBuffer are invalid except for tokenStartIndex.</remarks>
        private bool ReadInput()
        {
            Debug.Assert(this.storedCharacterCount <= this.characterBuffer.Length, "We can only store as many characters as fit into our buffer.");
            Debug.Assert(this.tokenStartIndex >= 0 && this.tokenStartIndex <= this.storedCharacterCount, "The token start is out of stored characters range.");

            if (this.endOfInputReached)
            {
                return false;
            }
            
            // We need to make sure we have more room in the buffer to read characters into
            if (this.storedCharacterCount == this.characterBuffer.Length)
            {
                // No more room in the buffer, move or grow the buffer.
                if (this.tokenStartIndex == this.storedCharacterCount)
                {
                    // If the buffer is empty (all characters were consumed from it)
                    // just start over.
                    this.tokenStartIndex = 0;
                    this.storedCharacterCount = 0;
                }
                else if (this.tokenStartIndex > (this.characterBuffer.Length - MaxCharacterCountToMove))
                {
                    // Some characters were consumed, we can just move them in the buffer
                    // to get more room without allocating.
                    Array.Copy(
                        this.characterBuffer,
                        this.tokenStartIndex,
                        this.characterBuffer,
                        0,
                        this.storedCharacterCount - this.tokenStartIndex);
                    this.storedCharacterCount -= this.tokenStartIndex;
                    this.tokenStartIndex = 0;
                }
                else
                {
                    // The entire buffer is full of unconsumed characters
                    // We need to grow the buffer.
                    // Double the size of the buffer.
                    int newBufferSize = this.characterBuffer.Length * 2;
                    char[] newCharacterBuffer = new char[newBufferSize];

                    // Copy the existing characters to the new buffer.
                    Array.Copy(
                        this.characterBuffer,
                        0,  // The tokenStartIndex is 0 - we checked above.
                        newCharacterBuffer,
                        0,
                        this.characterBuffer.Length);  // The storedCharacterCount is the size of the old buffer

                    // And switch the buffers
                    this.characterBuffer = newCharacterBuffer;
                    
                    // Note that the number of characters stored in the buffer remains unchanged
                    // as well as the token start index which is 0.
                }
            }

            Debug.Assert(
                this.storedCharacterCount < this.characterBuffer.Length,
                "We should have more room in the buffer by now.");

            // Read more characters from the input.
            // Use the Read method which returns any character as soon as it's available
            // we don't want to wait for the entire buffer to fill if the input doesn't have
            // the characters ready.
            int readCount = this.reader.Read(
                this.characterBuffer,
                this.storedCharacterCount,
                this.characterBuffer.Length - this.storedCharacterCount);

            if (readCount == 0)
            {
                // No more characters available, end of input.
                this.endOfInputReached = true;
                return false;
            }

            this.storedCharacterCount += readCount;
            return true;
        }

        private ODataException CreateException(string exceptionMessage)
        {
            return JsonReaderExtensions.CreateException(exceptionMessage, this.CaptureParsingContext());
        }

        /// <summary>
        /// Class representing scope information.
        /// </summary>
        private sealed class Scope
        {
            /// <summary>
            /// The type of the scope.
            /// </summary>
            private readonly ScopeType type;

            /// <summary>
            /// Constructor.
            /// </summary>
            /// <param name="type">The type of the scope.</param>
            public Scope(ScopeType type)
            {
                this.type = type;
            }

            /// <summary>
            /// Get/Set the number of values found under the current scope.
            /// </summary>
            public int ValueCount
            {
                get;
                set;
            }

            /// <summary>
            /// Gets the scope type for this scope.
            /// </summary>
            public ScopeType Type
            {
                get
                {
                    return this.type;
                }
            }
        }
    }
}
