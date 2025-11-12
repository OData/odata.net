//---------------------------------------------------------------------
// <copyright file="JsonReader.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Json
{
    #region Namespaces
    using System;
    using System.Buffers;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.IO;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.OData.Buffers;
    using Microsoft.OData.Core;
    #endregion Namespaces

    /// <summary>
    /// Reader for the JSON format. http://www.json.org
    /// </summary>
    // NOTE: Do NOT call GetValue()/GetValueAsync() from DebuggerDisplay. The methods mutate internal state (like canStream), obscuring real values while debugging.
    [DebuggerDisplay("{NodeType}")]
    internal class JsonReader : IJsonReader, IDisposable, IAsyncDisposable
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
        /// Whether the user is currently reading a string property as a stream.
        /// </summary>
        /// <remarks>This is used to avoid calling Read on the text reader multiple times
        /// even though it already reported the end of input.</remarks>
        private bool readingStream = false;

        /// <summary>
        /// Whether or not the current value can be streamed
        /// </summary>
        /// <remarks>True if we are positioned on a string or null value, otherwise false</remarks>
        private bool canStream = false;

        /// <summary>
        /// The opening character read when reading a stream value.
        /// </summary>
        private char streamOpeningQuoteCharacter = '"';

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
        /// Number of times an opening character (for example, '{') has been read
        /// greater than the number of times the corresponding closing character
        /// (for example '}') has been read.
        /// </summary>
        private int balancedQuoteCount;

        /// <summary>
        /// The last reported node type.
        /// </summary>
        private JsonNodeType nodeType;

        /// <summary>
        /// The value of the last reported node.
        /// </summary>
        private object nodeValue;

        private static readonly SearchValues<char> jsonWhitespaceSearchValues = SearchValues.Create(" \t\r\n");

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
        /// <param name="isIeee754Compatible">If it is isIeee754Compatible</param>
        public JsonReader(TextReader reader, bool isIeee754Compatible)
        {
            Debug.Assert(reader != null, "reader != null");

            this.nodeType = JsonNodeType.None;
            this.nodeValue = null;
            this.reader = reader;
            this.storedCharacterCount = 0;
            this.tokenStartIndex = 0;
            this.endOfInputReached = false;
            this.isIeee754Compatible = isIeee754Compatible;
            this.allowAnnotations = true;
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
        /// Get/sets the character buffer pool.
        /// </summary>
        public ICharArrayPool ArrayPool { get; set; }

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
        public virtual object GetValue()
        {
            if (this.readingStream)
            {
                throw JsonReaderExtensions.CreateException(SRResources.JsonReader_CannotAccessValueInStreamState);
            }

            if (this.canStream)
            {
                if (this.nodeType != JsonNodeType.Property)
                {
                    this.canStream = false;
                }

                if (this.nodeType == JsonNodeType.PrimitiveValue)
                {
                    if (this.characterBuffer[this.tokenStartIndex] == 'n')
                    {
                        this.nodeValue = this.ParseNullPrimitiveValue();
                    }
                    else
                    {
                        SetNodeValue(this, this.ParseStringPrimitiveValue(out _));
                    }
                }
            }

            return this.nodeValue;
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
        /// Whether the reader can stream the current value.
        /// </summary>
        /// <returns>
        /// True if the current value can be streamed, otherwise false</returns>
        /// <remarks>If the property is a string (or null) it can be streamed</remarks>
        public bool CanStream()
        {
            return this.canStream;
        }

        /// <summary>
        /// Reads the next node from the input.
        /// </summary>
        /// <returns>true if a new node was found, or false if end of input was reached.</returns>
        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Not really feasible to extract code to methods without introducing unnecessary complexity.")]
        public virtual bool Read()
        {
            if (this.readingStream)
            {
                throw JsonReaderExtensions.CreateException(SRResources.JsonReader_CannotCallReadInStreamState);
            }

            if (this.canStream)
            {
                this.canStream = false;
                if (this.nodeType == JsonNodeType.PrimitiveValue)
                {
                    // caller is positioned on a string value that they haven't read, so skip it
                    if (this.characterBuffer[this.tokenStartIndex] == 'n')
                    {
                        this.ParseNullPrimitiveValue();
                    }
                    else
                    {
                        this.ParseStringPrimitiveValue();
                    }
                }
            }

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
                        throw JsonReaderExtensions.CreateException(Error.Format(SRResources.JsonReader_UnexpectedComma, ScopeType.Root));
                    }

                    if (currentScope.ValueCount > 0)
                    {
                        // We already found the top-level value, so fail
                        throw JsonReaderExtensions.CreateException(SRResources.JsonReader_MultipleTopLevelValues);
                    }

                    // We expect a "value" - start array, start object or primitive value
                    this.nodeType = this.ParseValue();
                    break;

                case ScopeType.Array:
                    if (commaFound && currentScope.ValueCount == 0)
                    {
                        throw JsonReaderExtensions.CreateException(Error.Format(SRResources.JsonReader_UnexpectedComma, ScopeType.Array));
                    }

                    // We might see end of array here
                    if (this.characterBuffer[this.tokenStartIndex] == ']')
                    {
                        this.tokenStartIndex++;

                        // End of array is only valid when there was no comma before it.
                        if (commaFound)
                        {
                            throw JsonReaderExtensions.CreateException(Error.Format(SRResources.JsonReader_UnexpectedComma, ScopeType.Array));
                        }

                        this.PopScope();
                        this.nodeType = JsonNodeType.EndArray;
                        break;
                    }

                    if (!commaFound && currentScope.ValueCount > 0)
                    {
                        throw JsonReaderExtensions.CreateException(Error.Format(SRResources.JsonReader_MissingComma, ScopeType.Array));
                    }

                    // We expect element which is a "value" - start array, start object or primitive value
                    this.nodeType = this.ParseValue();
                    break;

                case ScopeType.Object:
                    if (commaFound && currentScope.ValueCount == 0)
                    {
                        throw JsonReaderExtensions.CreateException(Error.Format(SRResources.JsonReader_UnexpectedComma, ScopeType.Object));
                    }

                    // We might see end of object here
                    if (this.characterBuffer[this.tokenStartIndex] == '}')
                    {
                        this.tokenStartIndex++;

                        // End of object is only valid when there was no comma before it.
                        if (commaFound)
                        {
                            throw JsonReaderExtensions.CreateException(Error.Format(SRResources.JsonReader_UnexpectedComma, ScopeType.Object));
                        }

                        this.PopScope();
                        this.nodeType = JsonNodeType.EndObject;
                        break;
                    }
                    else
                    {
                        if (!commaFound && currentScope.ValueCount > 0)
                        {
                            throw JsonReaderExtensions.CreateException(Error.Format(SRResources.JsonReader_MissingComma, ScopeType.Object));
                        }

                        // We expect a property here
                        this.nodeType = this.ParseProperty();
                        break;
                    }

                case ScopeType.Property:
                    if (commaFound)
                    {
                        throw JsonReaderExtensions.CreateException(Error.Format(SRResources.JsonReader_UnexpectedComma, ScopeType.Property));
                    }

                    // We expect the property value, which is a "value" - start array, start object or primitive value
                    this.nodeType = this.ParseValue();
                    break;

                default:
                    throw JsonReaderExtensions.CreateException(Error.Format(SRResources.General_InternalError, InternalErrorCodes.JsonReader_Read));
            }

            Debug.Assert(
                this.nodeType != JsonNodeType.None && this.nodeType != JsonNodeType.EndOfInput,
                "Read should never go back to None and EndOfInput should be reported by directly returning.");

            return true;
        }

        /// <summary>
        /// Creates a stream for reading a base64 binary value.
        /// </summary>
        /// <returns>A stream for reading a base64 URL encoded binary value.</returns>
        public Stream CreateReadStream()
        {
            if (!this.canStream)
            {
                throw JsonReaderExtensions.CreateException(SRResources.JsonReader_CannotCreateReadStream);
            }

            this.canStream = false;
            if ((this.streamOpeningQuoteCharacter = characterBuffer[this.tokenStartIndex]) == 'n')
            {
                this.ParseNullPrimitiveValue();
                this.scopes.Peek().ValueCount++;
                this.Read();
                return new ODataBinaryStreamReader((a, b, c) => { return 0; });
            }

            this.tokenStartIndex++;
            this.readingStream = true;
            return new ODataBinaryStreamReader(new StreamReaderDelegate(this.ReadChars));
        }

        /// <summary>
        /// Creates a TextReader for reading text values.
        /// </summary>
        /// <returns>A TextReader for reading a text value.</returns>
        public TextReader CreateTextReader()
        {
            if (!this.canStream)
            {
                throw JsonReaderExtensions.CreateException(SRResources.JsonReader_CannotCreateTextReader);
            }

            this.canStream = false;
            this.SkipWhitespaces();

            Debug.Assert(this.NodeType == JsonNodeType.PrimitiveValue || this.NodeType == JsonNodeType.Property, "Streaming in an unknown state");
            if ((this.streamOpeningQuoteCharacter = characterBuffer[this.tokenStartIndex]) == 'n')
            {
                // value is null
                this.ParseNullPrimitiveValue();
                this.scopes.Peek().ValueCount++;
                this.Read();
                return new ODataTextStreamReader((a, b, c) => { return 0; });
            }

            // skip over the opening quote character for a string value
            if (this.NodeType == JsonNodeType.PrimitiveValue)
            {
                this.tokenStartIndex++;
            }

            this.readingStream = true;
            return new ODataTextStreamReader(new StreamReaderDelegate(this.ReadChars));
        }

        /// <summary>
        /// Asynchronously returns the value of the last reported node.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.
        /// The value of the TResult parameter contains the value of the last reported node.</returns>
        /// <remarks>A non-null is returned only if the last node was a PrimitiveValue or Property.
        /// If the last node was a PrimitiveValue this property returns the value:
        /// - null if the null token was found.
        /// - boolean if the true or false token was found.
        /// - string if a string token was found.
        /// - DateTime if a string token formatted as DateTime was found.
        /// - Int32 if a number which fits into the Int32 was found.
        /// - Double if a number which doesn't fit into Int32 was found.
        /// If the last node is a Property this property returns a string which is the name of the property.
        /// </remarks>
        public virtual Task<object> GetValueAsync()
        {
            if (this.readingStream)
            {
                throw JsonReaderExtensions.CreateException(SRResources.JsonReader_CannotAccessValueInStreamState);
            }

            // Fast path: if we are not in a streamable state just return the cached value (already materialized)
            if (!this.canStream)
            {
                return Task.FromResult(this.nodeValue);
            }

            // If we are on a property name we leave canStream true (the value will be parsed later)
            if (this.nodeType != JsonNodeType.Property)
            {
                this.canStream = false;
            }

            // Only primitive values require deferred materialization
            if (this.nodeType != JsonNodeType.PrimitiveValue)
            {
                return Task.FromResult(this.nodeValue);
            }

            // We are on a primitive value whose raw characters are still in the buffer.
            // Decide between 'null' literal and string literal.
            char firstChar = this.characterBuffer[this.tokenStartIndex];

            if (firstChar == 'n')
            {
                // Null literal; usually buffer already contains 'null' so the task completes synchronously.
                ValueTask<object> parseNullTask = this.ParseNullPrimitiveValueAsync();
                if (parseNullTask.IsCompletedSuccessfully)
                {
                    this.nodeValue = parseNullTask.Result; // always null
                    return Task.FromResult(this.nodeValue);
                }

                return AwaitNullValueAsync(this, parseNullTask);
            }
            else
            {
                // Quoted string (or single‑quoted for compat); fast-path if already completed.
                ValueTask<(ReadOnlyMemory<char> Value, bool HasLeadingBackslash)> parseStringTask = this.ParseStringPrimitiveValueAsync();
                if (parseStringTask.IsCompletedSuccessfully)
                {
                    SetNodeValue(this, parseStringTask.Result.Value);
                    return Task.FromResult(this.nodeValue);
                }

                return AwaitStringValueAsync(this, parseStringTask);
            }

            // Local static helpers so no closure allocations when slow path triggers.
            static async Task<object> AwaitNullValueAsync(JsonReader thisParam, ValueTask<object> pendingParseNullTask)
            {
                thisParam.nodeValue = await pendingParseNullTask.ConfigureAwait(false); // will be null
                return thisParam.nodeValue;
            }

            static async Task<object> AwaitStringValueAsync(JsonReader thisParam, ValueTask<(ReadOnlyMemory<char> Value, bool HasLeadingBackslash)> pendingParseStringTask)
            {
                (ReadOnlyMemory<char> Value, bool HasLeadingBackslash) result = await pendingParseStringTask.ConfigureAwait(false);
                SetNodeValue(thisParam, result.Value);
                return thisParam.nodeValue;
            }
        }

        /// <summary>
        /// Asynchronously checks whether or not the current value can be streamed.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.
        /// true if the current value can be streamed; otherwise false.</returns>
        public Task<bool> CanStreamAsync()
        {
            return Task.FromResult(this.canStream);
        }

        /// <summary>
        /// Asynchronously reads the next node from the input.
        /// </summary>
        /// <returns>A task that represents the asynchronous read operation.
        /// The value of the TResult parameter contains true if a new node was found,
        /// or false if end of input was reached.</returns>
        public virtual Task<bool> ReadAsync()
        {
            if (this.readingStream)
            {
                return Task.FromException<bool>(
                    JsonReaderExtensions.CreateException(SRResources.JsonReader_CannotCallReadInStreamState));
            }

            // Consume a pending (unmaterialized) primitive if present
            ValueTask consumePendingPrimitiveTask = this.ConsumePendingPrimitiveAsync();
            if (!consumePendingPrimitiveTask.IsCompletedSuccessfully)
            {
                return AwaitConsumePendingPrimitiveAsync(this, consumePendingPrimitiveTask);
            }

            return ContinueAfterConsumePendingPrimitiveAsync(this);

            static Task<bool> ContinueAfterConsumePendingPrimitiveAsync(JsonReader thisParam)
            {
                // Reset the node value.
                thisParam.nodeValue = null;

#if DEBUG
                // Reset the node type to None - so that we can verify that the ReadAsync method actually sets it.
                thisParam.nodeType = JsonNodeType.None;
#endif

                ValueTask<(bool CommaFound, bool EndOfInput)> prepareForNextTokenTask = thisParam.PrepareForNextTokenAsync();
                if (!prepareForNextTokenTask.IsCompletedSuccessfully)
                {
                    return AwaitPrepareForNextTokenAsync(thisParam, prepareForNextTokenTask);
                }

                return ContinueAfterPrepareForNextTokenAsync(thisParam, prepareForNextTokenTask.Result);
            }

            static Task<bool> ContinueAfterPrepareForNextTokenAsync(
                JsonReader thisParam,
                (bool CommaFound, bool EndOfInput) prepareForNextTokenResult)
            {
                (bool commaFound, bool endOfInput) = prepareForNextTokenResult;

                if (endOfInput)
                {
                    // EndOfInput always returns false
                    bool innerResult;
                    try
                    {
                        innerResult = thisParam.EndOfInput();
                    }
                    catch (ODataException ex)
                    {
                        return Task.FromException<bool>(ex);
                    }

                    Debug.Assert(innerResult == false, "EndOfInput should always return false.");

                    return Task.FromResult(false);
                }

                ValueTask<JsonNodeType> readInScopeTask = thisParam.ReadInScopeAsync(commaFound);
                if (!readInScopeTask.IsCompletedSuccessfully)
                {
                    return AwaitReadInScopeAsync(thisParam, readInScopeTask);
                }

                thisParam.nodeType = readInScopeTask.Result;

                Debug.Assert(
                    thisParam.nodeType != JsonNodeType.None && thisParam.nodeType != JsonNodeType.EndOfInput,
                    "Read should never go back to None and EndOfInput should be reported by directly returning.");

                return Task.FromResult(true);
            }

            static async Task<bool> AwaitConsumePendingPrimitiveAsync(
                JsonReader thisParam,
                ValueTask pendingConsumePendingPrimitiveTask)
            {
                await pendingConsumePendingPrimitiveTask.ConfigureAwait(false);

                return await ContinueAfterConsumePendingPrimitiveAsync(thisParam).ConfigureAwait(false);
            }

            static async Task<bool> AwaitPrepareForNextTokenAsync(
                JsonReader thisParam,
                ValueTask<(bool CommaFound, bool EndOfInput)> pendingPrepareForNextTokenTask)
            {
                (bool commaFound, bool endOfInput) = await pendingPrepareForNextTokenTask.ConfigureAwait(false);
                
                return await ContinueAfterPrepareForNextTokenAsync(
                    thisParam,
                    (commaFound, endOfInput)).ConfigureAwait(false);
            }

            static async Task<bool> AwaitReadInScopeAsync(
                JsonReader thisParam,
                ValueTask<JsonNodeType> pendingReadInScopeTask)
            {
                thisParam.nodeType = await pendingReadInScopeTask.ConfigureAwait(false);

                Debug.Assert(
                    thisParam.nodeType != JsonNodeType.None && thisParam.nodeType != JsonNodeType.EndOfInput,
                    "Read should never go back to None and EndOfInput should be reported by directly returning.");

                return true;
            }
        }

        /// <summary>
        /// Asynchronously creates a stream for reading a base64 binary value.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.
        /// The value of the TResult parameter contains a <see cref="Stream"/> for reading a base64 URL encoded binary value.</returns>
        public async Task<Stream> CreateReadStreamAsync()
        {
            if (!this.canStream)
            {
                throw JsonReaderExtensions.CreateException(SRResources.JsonReader_CannotCreateReadStream);
            }

            this.canStream = false;
            if ((this.streamOpeningQuoteCharacter = characterBuffer[this.tokenStartIndex]) == 'n')
            {
                await this.ParseNullPrimitiveValueAsync()
                    .ConfigureAwait(false);
                this.scopes.Peek().ValueCount++;
                await this.ReadAsync()
                    .ConfigureAwait(false);
                return new ODataBinaryStreamReader((a, b, c) => { return Task.FromResult(0); });
            }

            this.tokenStartIndex++;
            this.readingStream = true;
            return new ODataBinaryStreamReader(new AsyncStreamReaderDelegate(this.ReadCharsAsync));
        }

        /// <summary>
        /// Asynchronously creates a TextReader for reading text values.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.
        /// The value of the TResult parameter contains a <see cref="TextReader"/> for reading a text value.</returns>
        public async Task<TextReader> CreateTextReaderAsync()
        {
            if (!this.canStream)
            {
                throw JsonReaderExtensions.CreateException(SRResources.JsonReader_CannotCreateTextReader);
            }

            this.canStream = false;
            await this.SkipWhitespacesAsync().ConfigureAwait(false);

            Debug.Assert(this.NodeType == JsonNodeType.PrimitiveValue || this.NodeType == JsonNodeType.Property, "Streaming in an unknown state");
            if ((this.streamOpeningQuoteCharacter = characterBuffer[this.tokenStartIndex]) == 'n')
            {
                // value is null
                await this.ParseNullPrimitiveValueAsync()
                    .ConfigureAwait(false);
                this.scopes.Peek().ValueCount++;
                await this.ReadAsync()
                    .ConfigureAwait(false);
                return new ODataTextStreamReader((a, b, c) => { return Task.FromResult(0); });
            }

            // skip over the opening quote character for a string value
            if (this.NodeType == JsonNodeType.PrimitiveValue)
            {
                this.tokenStartIndex++;
            }

            this.readingStream = true;
            return new ODataTextStreamReader(new AsyncStreamReaderDelegate(this.ReadCharsAsync));
        }

        /// <summary>
        /// Dispose the reader
        /// </summary>
        public void Dispose()
        {
            if (this.ArrayPool != null && this.characterBuffer != null)
            {
                BufferUtils.ReturnToBuffer(this.ArrayPool, this.characterBuffer);
                this.characterBuffer = null;
            }
        }

        public ValueTask DisposeAsync()
        {
            Dispose();
            return ValueTask.CompletedTask;
        }

        /// <summary>
        /// Consumes (skips) a deferred primitive value (string or null) if the reader is positioned
        /// on an unmaterialized streamable primitive. No-op if streaming is not possible or the
        /// current node is not a primitive value.
        /// </summary>
        /// <returns>
        /// A completed ValueTask if the skip finished synchronously; otherwise a ValueTask that
        /// completes when the primitive has been fully skipped.
        /// </returns>
        private ValueTask ConsumePendingPrimitiveAsync()
        {
            if (!this.canStream)
            {
                return ValueTask.CompletedTask;
            }

            this.canStream = false;

            if (this.nodeType != JsonNodeType.PrimitiveValue)
            {
                return ValueTask.CompletedTask;
            }

            // caller is positioned on a string value that they haven't read, so skip it
            if (this.characterBuffer[this.tokenStartIndex] == 'n')
            {
                ValueTask<object> parseNullTask = this.ParseNullPrimitiveValueAsync();
                if (parseNullTask.IsCompletedSuccessfully)
                {
                    return ValueTask.CompletedTask;
                }

                return AwaitParseNullAsync(parseNullTask);
            }

            ValueTask<(ReadOnlyMemory<char>, bool)> parseStringTask = this.ParseStringPrimitiveValueAsync();
            if (parseStringTask.IsCompletedSuccessfully)
            {
                return ValueTask.CompletedTask;
            }

            return AwaitParseStringAsync(parseStringTask);

            static async ValueTask AwaitParseNullAsync(ValueTask<object> pendingParseNullTask)
            {
                await pendingParseNullTask.ConfigureAwait(false);
            }

            static async ValueTask AwaitParseStringAsync(ValueTask<(ReadOnlyMemory<char>, bool)> pendingParseStringTask)
            {
                await pendingParseStringTask.ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Skips leading whitespace and an optional comma plus following whitespace in the input.
        /// </summary>
        /// <returns>
        /// A ValueTask whose result tuple indicates whether a comma was found and whether end of input
        /// was reached.
        /// </returns>
        private ValueTask<(bool CommaFound, bool EndOfInput)> PrepareForNextTokenAsync()
        {
            // Skip any whitespace characters.
            // This also makes sure that we have at least one non-whitespace character available.
            ValueTask<bool> leadingWhitespaceTask = this.SkipWhitespacesAsync();
            if (!leadingWhitespaceTask.IsCompletedSuccessfully)
            {
                return AwaitLeadingWhitespaceAsync(this, leadingWhitespaceTask);
            }

            if (!leadingWhitespaceTask.Result)
            {
                // All remaining input was whitespace -> EOF
                return ValueTask.FromResult((false, true));
            }

            return ContinueAfterLeadingWhitespaceAsync(this);

            static ValueTask<(bool CommaFound, bool EndOfInput)> ContinueAfterLeadingWhitespaceAsync(JsonReader thisParam)
            {
                Debug.Assert(
                    thisParam.tokenStartIndex < thisParam.storedCharacterCount && !IsWhitespaceCharacter(thisParam.characterBuffer[thisParam.tokenStartIndex]),
                    "The SkipWhitespacesAsync didn't correctly skip all whitespace characters from the input.");

                bool commaFound = false;

                if (thisParam.characterBuffer[thisParam.tokenStartIndex] == ',')
                {
                    commaFound = true;
                    thisParam.tokenStartIndex++;

                    // NOTE: The validity of the comma is verified later depending on the current scope.
                    // Skip all whitespaces after comma.
                    ValueTask<bool> postCommaWhitespaceTask = thisParam.SkipWhitespacesAsync();
                    if (!postCommaWhitespaceTask.IsCompletedSuccessfully)
                    {
                        return AwaitPostCommaWhitespaceAsync(thisParam, commaFound, postCommaWhitespaceTask);
                    }

                    if (!postCommaWhitespaceTask.Result)
                    {
                        // Comma followed only by trailing whitespace -> EOF
                        return ValueTask.FromResult((true, true));
                    }

                    Debug.Assert(
                        thisParam.tokenStartIndex < thisParam.storedCharacterCount && !IsWhitespaceCharacter(thisParam.characterBuffer[thisParam.tokenStartIndex]),
                        "The SkipWhitespacesAsync didn't correctly skip all whitespace characters from the input.");
                }

                return ValueTask.FromResult((commaFound, false));
            }

            static async ValueTask<(bool CommaFound, bool EndOfInput)> AwaitLeadingWhitespaceAsync(
                JsonReader thisParam,
                ValueTask<bool> pendingLeadingWhitespaceTask)
            {
                if (!await GetOrAwait(pendingLeadingWhitespaceTask).ConfigureAwait(false))
                {
                    return (false, true);
                }

                return await ContinueAfterLeadingWhitespaceAsync(thisParam).ConfigureAwait(false);
            }

            static async ValueTask<(bool CommaFound, bool EndOfInput)> AwaitPostCommaWhitespaceAsync(
                JsonReader thisParam,
                bool commaFound,
                ValueTask<bool> pendingPostCommaWhitespaceTask)
            {
                if (!await GetOrAwait(pendingPostCommaWhitespaceTask).ConfigureAwait(false))
                {
                    // Comma followed only by trailing whitespace -> EOF
                    return (commaFound, true);
                }

                Debug.Assert(
                    thisParam.tokenStartIndex < thisParam.storedCharacterCount && !IsWhitespaceCharacter(thisParam.characterBuffer[thisParam.tokenStartIndex]),
                    "The SkipWhitespacesAsync didn't correctly skip all whitespace characters from the input.");

                return (commaFound, false);
            }
        }

        /// <summary>
        /// Dispatches reading logic for the current scope type, validating comma usage and returning the next node type.
        /// </summary>
        /// <param name="commaFound"><c>true</c> if a comma was detected before this scope’s next token.</param>
        /// <returns>A ValueTask producing the next <see cref="JsonNodeType"/>; otherwise faulted task.</returns>
        private ValueTask<JsonNodeType> ReadInScopeAsync(bool commaFound)
        {
            Scope currentScope = this.scopes.Peek();

            switch (currentScope.Type)
            {
                case ScopeType.Root:
                    return this.ReadInRootScopeAsync(commaFound, currentScope);

                case ScopeType.Array:
                    return this.ReadInArrayScopeAsync(commaFound, currentScope);

                case ScopeType.Object:
                    return this.ReadInObjectScopeAsync(commaFound, currentScope);

                case ScopeType.Property:
                    return this.ReadInPropertyScopeAsync(commaFound);

                default:
                    // In reality unreachable
                    return ValueTask.FromException<JsonNodeType>(
                        JsonReaderExtensions.CreateException(
                            Error.Format(SRResources.General_InternalError, InternalErrorCodes.JsonReader_Read)));
            }
        }

        /// <summary>
        /// Processes the next token while in the root scope.
        /// Validates that only a single top-level value appears and no leading comma exists.
        /// </summary>
        /// <param name="commaFound"><c>true</c> if a comma preceded this position - invalid for root scope.</param>
        /// <param name="currentScope">The current root scope instance.</param>
        /// <returns>A ValueTask producing the next <see cref="JsonNodeType"/>; otherwise faulted task.</returns>
        private ValueTask<JsonNodeType> ReadInRootScopeAsync(bool commaFound, Scope currentScope)
        {
            if (commaFound)
            {
                return ValueTask.FromException<JsonNodeType>(
                    JsonReaderExtensions.CreateException(
                        Error.Format(SRResources.JsonReader_UnexpectedComma, ScopeType.Root)));
            }

            if (currentScope.ValueCount > 0)
            {
                // We already found the top-level value, so fail
                return ValueTask.FromException<JsonNodeType>(
                    JsonReaderExtensions.CreateException(SRResources.JsonReader_MultipleTopLevelValues));
            }

            // We expect a "value" - start array, start object or primitive value
            ValueTask<JsonNodeType> parseValueTask = this.ParseValueAsync();
            if (parseValueTask.IsCompletedSuccessfully)
            {
                return ValueTask.FromResult(parseValueTask.Result);
            }

            return AwaitParseValueAsync(parseValueTask);

            static async ValueTask<JsonNodeType> AwaitParseValueAsync(ValueTask<JsonNodeType> pendingParseValueTask)
            {
                return await pendingParseValueTask.ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Processes the next token while in an array scope.
        /// Handles end-of-array, comma rules, and element parsing.
        /// </summary>
        /// <param name="commaFound"><c>true</c> if a comma preceded this position.</param>
        /// <param name="currentScope">The active array scope.</param>
        /// <returns>A ValueTask producing the next <see cref="JsonNodeType"/>; otherwise faulted task.</returns>
        private ValueTask<JsonNodeType> ReadInArrayScopeAsync(bool commaFound, Scope currentScope)
        {
            if (commaFound && currentScope.ValueCount == 0)
            {
                return ValueTask.FromException<JsonNodeType>(
                    JsonReaderExtensions.CreateException(
                        Error.Format(SRResources.JsonReader_UnexpectedComma, ScopeType.Array)));
            }

            // We might see end of array here
            if (this.characterBuffer[this.tokenStartIndex] == ']')
            {
                this.tokenStartIndex++;

                // End of array is only valid when there was no comma before it.
                if (commaFound)
                {
                    return ValueTask.FromException<JsonNodeType>(
                        JsonReaderExtensions.CreateException(
                            Error.Format(SRResources.JsonReader_UnexpectedComma, ScopeType.Array)));
                }

                this.PopScope();
                return ValueTask.FromResult(JsonNodeType.EndArray);
            }

            if (!commaFound && currentScope.ValueCount > 0)
            {
                return ValueTask.FromException<JsonNodeType>(
                    JsonReaderExtensions.CreateException(
                        Error.Format(SRResources.JsonReader_MissingComma, ScopeType.Array)));
            }

            // We expect element which is a "value" - start array, start object or primitive value
            ValueTask<JsonNodeType> parseValueTask = this.ParseValueAsync();
            if (parseValueTask.IsCompletedSuccessfully)
            {
                return ValueTask.FromResult(parseValueTask.Result);
            }

            return AwaitParseValueAsync(parseValueTask);

            static async ValueTask<JsonNodeType> AwaitParseValueAsync(ValueTask<JsonNodeType> pendingParseValueTask)
            {
                return await pendingParseValueTask.ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Processes the next token while in an object scope.
        /// Handles end-of-object, comma validation, and property parsing.
        /// </summary>
        /// <param name="commaFound"><c>true</c> if a comma preceded this position.</param>
        /// <param name="currentScope">The active object scope.</param>
        /// <returns>A ValueTask producing the next <see cref="JsonNodeType"/>; otherwise faulted task.</returns>
        private ValueTask<JsonNodeType> ReadInObjectScopeAsync(bool commaFound, Scope currentScope)
        {
            if (commaFound && currentScope.ValueCount == 0)
            {
                return ValueTask.FromException<JsonNodeType>(
                    JsonReaderExtensions.CreateException(
                        Error.Format(SRResources.JsonReader_UnexpectedComma, ScopeType.Object)));
            }

            // We might see end of object here
            if (this.characterBuffer[this.tokenStartIndex] == '}')
            {
                this.tokenStartIndex++;

                // End of object is only valid when there was no comma before it.
                if (commaFound)
                {
                    return ValueTask.FromException<JsonNodeType>(
                        JsonReaderExtensions.CreateException(
                            Error.Format(SRResources.JsonReader_UnexpectedComma, ScopeType.Object)));
                }

                this.PopScope();
                return ValueTask.FromResult(JsonNodeType.EndObject);
            }

            if (!commaFound && currentScope.ValueCount > 0)
            {
                return ValueTask.FromException<JsonNodeType>(
                    JsonReaderExtensions.CreateException(
                        Error.Format(SRResources.JsonReader_MissingComma, ScopeType.Object)));
            }

            // We expect a property here
            ValueTask<JsonNodeType> parsePropertyTask = this.ParsePropertyAsync();
            if (parsePropertyTask.IsCompletedSuccessfully)
            {
                return ValueTask.FromResult(parsePropertyTask.Result);
            }

            return AwaitParsePropertyAsync(parsePropertyTask);

            static async ValueTask<JsonNodeType> AwaitParsePropertyAsync(ValueTask<JsonNodeType> pendingParsePropertyTask)
            {
                return await pendingParsePropertyTask.ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Processes the next token while in an property scope; validates that no comma precedes the value.
        /// </summary>
        /// <param name="commaFound"><c>true</c> if a comma preceded this position - invalid in property scope.</param>
        /// <returns>A ValueTask producing the next <see cref="JsonNodeType"/>; otherwise faulted task.</returns>
        private ValueTask<JsonNodeType> ReadInPropertyScopeAsync(bool commaFound)
        {
            if (commaFound)
            {
                return ValueTask.FromException<JsonNodeType>(JsonReaderExtensions.CreateException(
                    Error.Format(SRResources.JsonReader_UnexpectedComma, ScopeType.Property)));
            }

            // We expect the property value, which is a "value" - start array, start object or primitive value
            ValueTask<JsonNodeType> parseValueTask = this.ParseValueAsync();
            if (parseValueTask.IsCompletedSuccessfully)
            {
                return ValueTask.FromResult(parseValueTask.Result);
            }

            return AwaitParseValueAsync(parseValueTask);

            static async ValueTask<JsonNodeType> AwaitParseValueAsync(ValueTask<JsonNodeType> pendingParseValueTask)
            {
                return await pendingParseValueTask.ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Determines if a given character is a whitespace character.
        /// </summary>
        /// <param name="character">The character to test.</param>
        /// <returns>true if the <paramref name="character"/> is a whitespace; false otherwise.</returns>
        /// <remarks>Note that the behavior of this method is different from Char.IsWhitespace, since that method
        /// returns true for all characters defined as whitespace by the Unicode spec (which is a lot of characters),
        /// this one on the other hand recognizes just the whitespaces as defined by the JSON spec.</remarks>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
            Debug.Assert(this.scopes.Count >= 1 && this.scopes.Peek().Type != ScopeType.Object, "Value can only occur at the root, in array or as a property value.");

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
                    this.SkipWhitespaces();
                    this.canStream =
                        this.characterBuffer[this.tokenStartIndex] == '"' ||
                        this.characterBuffer[this.tokenStartIndex] == '\'' ||
                        this.characterBuffer[this.tokenStartIndex] == 'n';
                    return JsonNodeType.StartArray;

                case '"':
                case '\'':
                    // String primitive value
                    // Don't parse yet, as it may be a stream. Defer parsing until .Value is called.
                    this.canStream = true;
                    break;

                case 'n':
                    // Null value
                    // Don't parse yet, as user may be streaming a stream. Defer parsing until .Value is called.
                    this.canStream = true;
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
                        throw JsonReaderExtensions.CreateException(SRResources.JsonReader_UnrecognizedToken);
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
            ReadOnlyMemory<char> token = this.ParseName();

            if (token.IsEmpty)
            {
                this.nodeValue = string.Empty;

                // The name can't be empty.
                throw JsonReaderExtensions.CreateException(Error.Format(SRResources.JsonReader_InvalidPropertyNameOrUnexpectedComma, this.nodeValue));
            }

            SetNodeValue(this, token);

            if (!this.SkipWhitespaces() || this.characterBuffer[this.tokenStartIndex] != ':')
            {
                // We need the colon character after the property name
                throw JsonReaderExtensions.CreateException(Error.Format(SRResources.JsonReader_MissingColon, this.nodeValue));
            }

            // Consume the colon.
            Debug.Assert(this.characterBuffer[this.tokenStartIndex] == ':', "The above should verify that there's a colon.");
            this.tokenStartIndex++;
            this.SkipWhitespaces();

            // if the content is nested json, we can stream
            this.canStream = this.characterBuffer[this.tokenStartIndex] == '{' || this.characterBuffer[this.tokenStartIndex] == '[';
            return JsonNodeType.Property;
        }

        /// <summary>
        /// Parses a primitive string value.
        /// </summary>
        /// <returns>The value of the string primitive value.</returns>
        /// <remarks>
        /// Assumes that the current token position points to the opening quote.
        /// Note that the string parsing can never end with EndOfInput, since we're already seen the quote.
        /// So it can either return a string successfully or fail.</remarks>
        private ReadOnlyMemory<char> ParseStringPrimitiveValue()
        {
            return this.ParseStringPrimitiveValue(out _);
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
        /// So it can either return a string successfully or fail.</remarks>
        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Splitting the function would make it hard to understand.")]
        private ReadOnlyMemory<char> ParseStringPrimitiveValue(out bool hasLeadingBackslash)
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
                    // If we're at the beginning of the string
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
                            this.stringValueBuilder.Clear();
                        }

                        valueBuilder = this.stringValueBuilder;
                    }

                    // Append everything up to the \ character to the value.
                    this.ConsumeTokenAppendToBuilder(valueBuilder, currentCharacterTokenRelativeIndex);
                    currentCharacterTokenRelativeIndex = 0;
                    Debug.Assert(this.characterBuffer[this.tokenStartIndex] == '\\', "We should have consumed everything up to the escape character.");

                    // Escape sequence - we need at least two characters, the backslash and the one character after it.
                    if (!this.EnsureAvailableCharacters(2))
                    {
                        throw JsonReaderExtensions.CreateException(Error.Format(SRResources.JsonReader_UnrecognizedEscapeSequence, "\\"));
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
                                throw JsonReaderExtensions.CreateException(Error.Format(SRResources.JsonReader_UnrecognizedEscapeSequence, "\\uXXXX"));
                            }

                            valueBuilder.Append((char)this.ParseUnicodeHexValue());
                            break;
                        default:
                            throw JsonReaderExtensions.CreateException(Error.Format(SRResources.JsonReader_UnrecognizedEscapeSequence, "\\" + character));
                    }
                }
                else if (character == openingQuoteCharacter)
                {
                    // Consume everything up to the quote character
                    ReadOnlyMemory<char> result;
                    if (valueBuilder != null)
                    {
                        this.ConsumeTokenAppendToBuilder(valueBuilder, currentCharacterTokenRelativeIndex);
                        result = valueBuilder.ToString().AsMemory();
                    }
                    else
                    {
                        result = this.ConsumeTokenToMemory(currentCharacterTokenRelativeIndex);
                    }

                    Debug.Assert(this.characterBuffer[this.tokenStartIndex] == openingQuoteCharacter, "We should have consumed everything up to the quote character.");

                    // Consume the quote character as well.
                    this.tokenStartIndex++;
                    return result;
                }
                else
                {
                    // Normal character, just skip over it - it will become part of the value as is.
                    currentCharacterTokenRelativeIndex++;
                }
            }

            throw JsonReaderExtensions.CreateException(SRResources.JsonReader_UnexpectedEndOfString);
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
            ReadOnlyMemory<char> token = this.ParseName();

            if (!token.Span.SequenceEqual(JsonConstants.JsonNullLiteral.AsSpan()))
            {
                throw JsonReaderExtensions.CreateException(Error.Format(SRResources.JsonReader_UnexpectedToken, token.ToString()));
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
            ReadOnlyMemory<char> token = this.ParseName();

            if (token.Span.SequenceEqual(JsonConstants.JsonFalseLiteral.AsSpan()))
            {
                return false;
            }

            if (token.Span.SequenceEqual(JsonConstants.JsonTrueLiteral.AsSpan()))
            {
                return true;
            }

            throw JsonReaderExtensions.CreateException(Error.Format(SRResources.JsonReader_UnexpectedToken, token.ToString()));
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
                if (IsNumberChar(character))
                {
                    currentCharacterTokenRelativeIndex++;
                }
                else
                {
                    break;
                }
            }

            // We now have all the characters which belong to the number, consume it into a string.
            ReadOnlySpan<char> numberSpan = this.ConsumeTokenToSpan(currentCharacterTokenRelativeIndex);
            
            return ParseNumericToken(numberSpan);
        }

        /// <summary>
        /// Parses a name token.
        /// </summary>
        /// <returns>The value <see cref="ReadOnlyMemory<char>"/> of the name token.</returns>
        /// <remarks>Name tokens are (for backward compat reasons) either
        /// - string value quoted with double quotes.
        /// - string value quoted with single quotes.
        /// - sequence of letters, digits, underscores and dollar signs (without quoted and in any order).</remarks>
        private ReadOnlyMemory<char> ParseName()
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
                if (IsCharacterAllowedInPropertyName(character))
                {
                    currentCharacterTokenRelativeIndex++;
                }
                else
                {
                    break;
                }
            }
            while ((this.tokenStartIndex + currentCharacterTokenRelativeIndex) < this.storedCharacterCount || this.ReadInput());

            return this.ConsumeTokenToMemory(currentCharacterTokenRelativeIndex);
        }

        /// <summary>
        /// Reads bytes from the current string value.
        /// </summary>
        /// <param name="chars">The character buffer to populate</param>
        /// <param name="offset">The number of characters offset into the buffer to read</param>
        /// <param name="maxLength">The maximum number of characters to read into the buffer</param>
        /// <returns>The number of characters read.</returns>
        /// <remarks>
        /// Assumes that the current token position points to the opening quote.
        /// Note that the string parsing can never end with EndOfInput, since we're already seen the quote.
        /// So it can either return a string successfully or fail.</remarks>
        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Splitting the function would make it hard to understand.")]
        private int ReadChars(char[] chars, int offset, int maxLength)
        {
            if (!readingStream)
            {
                return 0;
            }

            int charsRead = 0;

            while (charsRead < maxLength && (this.tokenStartIndex < this.storedCharacterCount || this.ReadInput()))
            {
                char character = this.characterBuffer[this.tokenStartIndex];
                bool advance = true;

                if (character == GetClosingQuoteCharacter(this.streamOpeningQuoteCharacter) && --this.balancedQuoteCount < 1)
                {
                    if (character != '"')
                    {
                        // The character is part of the JSON stream; copy it to the output buffer
                        chars[charsRead + offset] = character;
                        charsRead++;
                        this.scopes.Pop();
                    }

                    // Consume the closing quote character.
                    this.tokenStartIndex++;
                    readingStream = false;
                    this.scopes.Peek().ValueCount++;

                    // move to next node
                    this.Read();
                    return charsRead;
                }

                if (character == this.streamOpeningQuoteCharacter)
                {
                    this.balancedQuoteCount++;
                }

                if (character == '\\')
                {
                    // Consume the escape
                    this.tokenStartIndex++;
                    if (!this.EnsureAvailableCharacters(1))
                    {
                        throw JsonReaderExtensions.CreateException(Error.Format(SRResources.JsonReader_UnrecognizedEscapeSequence, "\\uXXXX"));
                    }

                    character = this.characterBuffer[this.tokenStartIndex];

                    switch (character)
                    {
                        case 'b':
                            character = '\b';
                            break;
                        case 'f':
                            character = '\f';
                            break;
                        case 'n':
                            character = '\n';
                            break;
                        case 'r':
                            character = '\r';
                            break;
                        case 't':
                            character = '\t';
                            break;
                        case '\\':
                        case '\"':
                        case '\'':
                        case '/':
                            // Use the (now unescaped) character from reader;
                            break;
                        case 'u':

                            // Consume the "u"
                            tokenStartIndex++;

                            // We need 4 hex characters
                            if (!this.EnsureAvailableCharacters(4))
                            {
                                throw JsonReaderExtensions.CreateException(Error.Format(SRResources.JsonReader_UnrecognizedEscapeSequence, "\\uXXXX"));
                            }

                            character = (char)this.ParseUnicodeHexValue();

                            // We are already positioned on the next character, so don't advance at the end
                            advance = false;
                            break;
                        default:
                            throw JsonReaderExtensions.CreateException(Error.Format(SRResources.JsonReader_UnrecognizedEscapeSequence, "\\" + character));
                    }
                }

                chars[charsRead + offset] = character;
                charsRead++;

                if (advance)
                {
                    this.tokenStartIndex++;
                }
            }

            // we reached the end of the file without finding a closing quote character
            if (charsRead < maxLength)
            {
                throw JsonReaderExtensions.CreateException(SRResources.JsonReader_UnexpectedEndOfString);
            }

            return charsRead;
        }

        private static char GetClosingQuoteCharacter(char openingCharacter)
        {
            switch (openingCharacter)
            {
                case '{':
                    return '}';
                case '[':
                    return ']';
                default:
                    return openingCharacter;
            }
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
                throw JsonReaderExtensions.CreateException(SRResources.JsonReader_EndOfInputWithOpenScope);
            }

            Debug.Assert(
                this.scopes.Count > 0 && this.scopes.Peek().Type == ScopeType.Root && this.scopes.Peek().ValueCount <= 1,
                "The end of input should only occur with root at the top of the stack with zero or one value.");
            Debug.Assert(this.nodeValue == null, "The node value should have been reset to null.");

            this.nodeType = JsonNodeType.EndOfInput;

            if (this.ArrayPool != null)
            {
                BufferUtils.ReturnToBuffer(this.ArrayPool, this.characterBuffer);
                this.characterBuffer = null;
            }

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
                int nonWhitespaceIndex = FindFirstNonWhitespace(this.characterBuffer, this.tokenStartIndex, this.storedCharacterCount);

                if (nonWhitespaceIndex >= 0)
                {
                    this.tokenStartIndex += nonWhitespaceIndex;
                    return true;
                }

                // All remaining characters are whitespace, advance to end
                this.tokenStartIndex = this.storedCharacterCount;
            }
            while (this.ReadInput());

            return false;
        }

        /// <summary>
        /// Ensures that a specified number of characters after the token start is available in the buffer.
        /// </summary>
        /// <param name="characterCountAfterTokenStart">The number of character after the token to make available.</param>
        /// <returns>true if at least the required number of characters is available; false if end of input was reached.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool EnsureAvailableCharacters(int characterCountAfterTokenStart)
        {
            // Fast path: Check if we already have enough characters in the buffer.
            if (this.tokenStartIndex + characterCountAfterTokenStart <= this.storedCharacterCount)
            {
                return true;
            }

            // Slow path: We need to read more characters from the input.
            return EnsureAvailableCharactersSlow(characterCountAfterTokenStart);
        }

        /// <summary>
        /// Ensures that a specified number of characters after the token start is available in the buffer.
        /// </summary>
        /// <param name="characterCountAfterTokenStart">The number of character after the token to make available.</param>
        /// <returns>true if at least the required number of characters is available; false if end of input was reached.</returns>
        [MethodImpl(MethodImplOptions.NoInlining)]
        private bool EnsureAvailableCharactersSlow(int characterCountAfterTokenStart)
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
        /// and returns them as a <see cref="ReadOnlySpan<char>"/>.
        /// </summary>
        /// <param name="characterCount">The number of characters after the token start to consume.</param>
        /// <returns>The string value of the consumed token.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private ReadOnlySpan<char> ConsumeTokenToSpan(int characterCount)
        {
            Debug.Assert(characterCount >= 0, "characterCount >= 0");
            Debug.Assert(this.tokenStartIndex + characterCount <= this.storedCharacterCount, "characterCount specified characters outside of the available range.");

            ReadOnlySpan<char> result = this.characterBuffer.AsSpan(this.tokenStartIndex, characterCount);
            this.tokenStartIndex += characterCount;

            return result;
        }

        /// <summary>
        /// Consumes the <paramref name="characterCount"/> characters starting at the start of the token
        /// and returns them as a <see cref="ReadOnlyMemory<char>"/>.
        /// </summary>
        /// <param name="characterCount">The number of characters after the token start to consume.</param>
        /// <returns>The string value of the consumed token.</returns>
        private ReadOnlyMemory<char> ConsumeTokenToMemory(int characterCount)
        {
            Debug.Assert(characterCount >= 0, "characterCount >= 0");
            Debug.Assert(this.tokenStartIndex + characterCount <= this.storedCharacterCount, "characterCount specified characters outside of the available range.");

            ReadOnlyMemory<char> result = this.characterBuffer.AsMemory(this.tokenStartIndex, characterCount);
            this.tokenStartIndex += characterCount;

            return result;
        }

        /// <summary>
        /// Consumes the <paramref name="characterCount"/> characters starting at the start of the token
        /// and append the token string to the <paramref name="builder"/>.
        /// </summary>
        /// <param name="builder">The StringBuilder instance to append the token to.</param>
        /// <param name="characterCount">The number of characters after the token start to consume.</param>
        private void ConsumeTokenAppendToBuilder(StringBuilder builder, int characterCount)
        {
            Debug.Assert(characterCount >= 0, "characterCount >= 0");
            Debug.Assert(this.tokenStartIndex + characterCount <= this.storedCharacterCount, "characterCount specified characters outside of the available range.");

            builder.Append(this.characterBuffer, this.tokenStartIndex, characterCount);
            this.tokenStartIndex += characterCount;
        }

        /// <summary>
        /// Flushes pending literal characters into <paramref name="builder"/> and resets <paramref name="bufferedLiteralLength"/> to 0.
        /// </summary>
        /// <param name="builder">The <see cref="StringBuilder"/> instance to append token to.</param>
        /// <param name="bufferedLiteralLength">Count of contiguous unflushed literal characters; reset to 0.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void FlushParsedLiteralToBuilder(StringBuilder builder, ref int bufferedLiteralLength)
        {
            if (bufferedLiteralLength == 0)
            {
                return;
            }

            // Append everything up to parsed length to the builder.
            this.ConsumeTokenAppendToBuilder(builder, bufferedLiteralLength);
            bufferedLiteralLength = 0;
        }

        /// <summary>
        /// Materializes the current JSON string literal (with optional escape resolution) into a managed string,
        /// flushing any remained buffered literal characters, validating and consuming the terminating quote.
        /// </summary>
        /// <param name="builder">
        /// Optional StringBuilder holding previously flushed segments - null if no escape sequences were resolved.
        /// </param>
        /// <param name="bufferedLiteralLength">
        /// Count of contiguous unflushed literal characters immediately preceding the terminating quote character.
        /// </param>
        /// <returns>The fully decoded string literal.</returns>
        private ReadOnlyMemory<char> FinalizeStringLiteral(StringBuilder builder, int bufferedLiteralLength)
        {
            // Consume everything up to the quote character
            ReadOnlyMemory<char> result;
            if (builder != null)
            {
                this.ConsumeTokenAppendToBuilder(builder, bufferedLiteralLength);
                result = builder.ToString().AsMemory();
            }
            else
            {
                result = this.ConsumeTokenToMemory(bufferedLiteralLength);
            }

#if DEBUG
            char quoteCharacter = this.characterBuffer[this.tokenStartIndex];
            Debug.Assert(quoteCharacter == '"' || quoteCharacter == '\'', "We should have consumed everything up to the quote character.");
#endif

            // Consume the quote character as well.
            this.tokenStartIndex++;

            return result;
        }

        /// <summary>
        /// Reads more characters from the input.
        /// </summary>
        /// <returns>true if more characters are available; false if end of input was reached.</returns>
        /// <remarks>This may move characters in the characterBuffer, so after this is called
        /// all indices to the characterBuffer are invalid except for tokenStartIndex.</remarks>
        private bool ReadInput()
        {
            Debug.Assert(this.tokenStartIndex >= 0 && this.tokenStartIndex <= this.storedCharacterCount, "The token start is out of stored characters range.");

            if (this.endOfInputReached)
            {
                return false;
            }

            this.CopyInputToBuffer();

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

        /// <summary>
        /// Asynchronously parses a "value", that is an array, object or primitive value.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.
        /// The value of the TResult parameter contains the node type to report to the user.</returns>
        private ValueTask<JsonNodeType> ParseValueAsync()
        {
            Debug.Assert(
                this.tokenStartIndex < this.storedCharacterCount && !IsWhitespaceCharacter(this.characterBuffer[this.tokenStartIndex]),
                "The SkipWhitespaces wasn't called or it didn't correctly skip all whitespace characters from the input.");
            Debug.Assert(this.scopes.Count >= 1 && this.scopes.Peek().Type != ScopeType.Object, "Value can only occur at the root, in array or as a property value.");

            // Increase the count of values under the current scope.
            this.scopes.Peek().ValueCount++;

            char currentCharacter = this.characterBuffer[this.tokenStartIndex];
            switch (currentCharacter)
            {
                case '{':
                    // Start of object
                    this.PushScope(ScopeType.Object);
                    this.tokenStartIndex++;
                    return ValueTask.FromResult(JsonNodeType.StartObject);

                case '[':
                    // Start of array
                    this.PushScope(ScopeType.Array);
                    this.tokenStartIndex++;
                    {
                        ValueTask<bool> skipWhitespacesTask = this.SkipWhitespacesAsync();
                        if (!skipWhitespacesTask.IsCompletedSuccessfully)
                        {
                            return AwaitArrayAsync(this, skipWhitespacesTask);
                        }

                        this.canStream =
                            this.characterBuffer[this.tokenStartIndex] == '"' ||
                            this.characterBuffer[this.tokenStartIndex] == '\'' ||
                            this.characterBuffer[this.tokenStartIndex] == 'n';

                        return ValueTask.FromResult(JsonNodeType.StartArray);
                    }

                case '"':
                case '\'':
                    // String primitive value
                    // Don't parse yet, as it may be a stream. Defer parsing until .Value is called.
                    this.canStream = true;
                    this.TryPopPropertyScope();
                    return ValueTask.FromResult(JsonNodeType.PrimitiveValue);

                case 'n':
                    // Null value
                    // Don't parse yet, as user may be streaming a stream. Defer parsing until .Value is called.
                    this.canStream = true;
                    this.TryPopPropertyScope();
                    return ValueTask.FromResult(JsonNodeType.PrimitiveValue);

                case 't':
                case 'f':
                    {
                        ValueTask<object> parseBooleanTask = this.ParseBooleanPrimitiveValueAsync();
                        if (parseBooleanTask.IsCompletedSuccessfully)
                        {
                            this.nodeValue = parseBooleanTask.Result;
                            this.TryPopPropertyScope();
                            return ValueTask.FromResult(JsonNodeType.PrimitiveValue);
                        }

                        return AwaitParseBooleanAsync(this, parseBooleanTask);
                    }
                default:
                    // COMPAT 47: JSON number can start with dot.
                    // The JSON spec doesn't allow numbers to start with ., but WCF DS does. We will follow the WCF DS behavior for compatibility.
                    if (Char.IsDigit(currentCharacter) || (currentCharacter == '-') || (currentCharacter == '.'))
                    {
                        ValueTask<object> parseNumberTask = this.ParseNumberPrimitiveValueAsync();
                        if (parseNumberTask.IsCompletedSuccessfully)
                        {
                            this.nodeValue = parseNumberTask.Result;
                            this.TryPopPropertyScope();
                            return ValueTask.FromResult(JsonNodeType.PrimitiveValue);
                        }

                        return AwaitParseNumberAsync(this, parseNumberTask);
                    }
                    else
                    {
                        // Unknown token - fail.
                        return ValueTask.FromException<JsonNodeType>(
                            JsonReaderExtensions.CreateException(SRResources.JsonReader_UnrecognizedToken));
                    }
            }

            static async ValueTask<JsonNodeType> AwaitArrayAsync(JsonReader thisParam, ValueTask<bool> pendingSkipWhitespacesTask)
            {
                await GetOrAwait(pendingSkipWhitespacesTask).ConfigureAwait(false);

                thisParam.canStream =
                    thisParam.characterBuffer[thisParam.tokenStartIndex] == '"' ||
                    thisParam.characterBuffer[thisParam.tokenStartIndex] == '\'' ||
                    thisParam.characterBuffer[thisParam.tokenStartIndex] == 'n';

                return JsonNodeType.StartArray;
            }

            static async ValueTask<JsonNodeType> AwaitParseBooleanAsync(JsonReader thisParam, ValueTask<object> pendingParseBooleanTask)
            {
                thisParam.nodeValue = await pendingParseBooleanTask.ConfigureAwait(false);
                thisParam.TryPopPropertyScope();
                return JsonNodeType.PrimitiveValue;
            }

            static async ValueTask<JsonNodeType> AwaitParseNumberAsync(JsonReader thisParam, ValueTask<object> pendingParseNumberTask)
            {
                thisParam.nodeValue = await pendingParseNumberTask.ConfigureAwait(false);
                thisParam.TryPopPropertyScope();
                return JsonNodeType.PrimitiveValue;
            }
        }

        /// <summary>
        /// Asynchronously parses a property name and the colon after it.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.
        /// The value of the TResult parameter contains the node type to report to the user.</returns>
        private ValueTask<JsonNodeType> ParsePropertyAsync()
        {
            // Increase the count of values under the object (the number of properties).
            Debug.Assert(this.scopes.Count >= 1 && this.scopes.Peek().Type == ScopeType.Object, "Property can only occur in an object.");
            this.scopes.Peek().ValueCount++;

            this.PushScope(ScopeType.Property);

            // Parse the name of the property
            ValueTask<ReadOnlyMemory<char>> parseNameTask = this.ParseNameAsync();
            if (!parseNameTask.IsCompletedSuccessfully)
            {
                return AwaitParseNameAsync(this, parseNameTask);
            }

            // Completed synchronously
            return ContinueAfterParseNameAsync(this, parseNameTask.Result);

            static ValueTask<JsonNodeType> ContinueAfterParseNameAsync(JsonReader thisParam, ReadOnlyMemory<char> nameToken)
            {
                if (nameToken.Span.IsEmpty)
                {
                    thisParam.nodeValue = string.Empty;

                    // The name can't be empty.
                    return ValueTask.FromException<JsonNodeType>(
                        JsonReaderExtensions.CreateException(
                            Error.Format(SRResources.JsonReader_InvalidPropertyNameOrUnexpectedComma, (string)thisParam.nodeValue)));
                }

                SetNodeValue(thisParam, nameToken);

                ValueTask<bool> preColonWhitespaceTask = thisParam.SkipWhitespacesAsync();
                if (!preColonWhitespaceTask.IsCompletedSuccessfully)
                {
                    return AwaitPreColonWhitespaceAsync(thisParam, preColonWhitespaceTask);
                }

                return ContinueAfterColonAsync(thisParam, preColonWhitespaceTask.Result);
            }

            static ValueTask<JsonNodeType> ContinueAfterColonAsync(JsonReader thisParam, bool hasNonWhitespace)
            {
                if (!hasNonWhitespace || thisParam.characterBuffer[thisParam.tokenStartIndex] != ':')
                {
                    // We need the colon character after the property name
                    return ValueTask.FromException<JsonNodeType>(
                        JsonReaderExtensions.CreateException(
                            Error.Format(SRResources.JsonReader_MissingColon, (string)thisParam.nodeValue)));
                }

                // Consume the colon.
                Debug.Assert(thisParam.characterBuffer[thisParam.tokenStartIndex] == ':', "The above should verify that there's a colon.");
                thisParam.tokenStartIndex++;

                ValueTask<bool> postColonWhitespaceTask = thisParam.SkipWhitespacesAsync();
                if (!postColonWhitespaceTask.IsCompletedSuccessfully)
                {
                    return AwaitPostColonWhitespaceAsync(thisParam, postColonWhitespaceTask);
                }

                return ContinueAfterColonWhitespaceAsync(thisParam);
            }

            static ValueTask<JsonNodeType> ContinueAfterColonWhitespaceAsync(JsonReader thisParam)
            {
                // if the content is nested json, we can stream
                thisParam.canStream = thisParam.characterBuffer[thisParam.tokenStartIndex] == '{' || thisParam.characterBuffer[thisParam.tokenStartIndex] == '[';

                return ValueTask.FromResult(JsonNodeType.Property);
            }

            static async ValueTask<JsonNodeType> AwaitParseNameAsync(
                JsonReader thisParam,
                ValueTask<ReadOnlyMemory<char>> pendingParseNameTask)
            {
                ReadOnlyMemory<char> nameToken = await pendingParseNameTask.ConfigureAwait(false);

                return await ContinueAfterParseNameAsync(thisParam, nameToken).ConfigureAwait(false);
            }

            static async ValueTask<JsonNodeType> AwaitPreColonWhitespaceAsync(
                JsonReader thisParam,
                ValueTask<bool> pendingPreColonWhitespaceTask)
            {
                bool nonWhitespaceFound = await GetOrAwait(pendingPreColonWhitespaceTask).ConfigureAwait(false);
                
                return await ContinueAfterColonAsync(thisParam, nonWhitespaceFound).ConfigureAwait(false);
            }

            static async ValueTask<JsonNodeType> AwaitPostColonWhitespaceAsync(
                JsonReader thisParam,
                ValueTask<bool> pendingPostColonWhitespaceTask)
            {
                await GetOrAwait(pendingPostColonWhitespaceTask).ConfigureAwait(false);

                return await ContinueAfterColonWhitespaceAsync(thisParam);
            }
        }

        /// <summary>
        /// Asynchronously parses a primitive string value.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.
        /// The value of the TResult parameter contains a tuple comprising of the value of the string primitive value 
        /// and a value of true if the first character in the string has a backlash; otherwise false.</returns>
        /// <remarks>
        /// Assumes that the current token position points to the opening quote.
        /// Note that the string parsing can never end with EndOfInput, since we're already seen the quote.
        /// So it can either return a string successfully or fail.</remarks>
        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Splitting the function would make it hard to understand.")]
        private ValueTask<(ReadOnlyMemory<char> Value, bool HasLeadingBackslash)> ParseStringPrimitiveValueAsync()
        {
            Debug.Assert(this.tokenStartIndex < this.storedCharacterCount, "At least the quote must be present.");

            bool hasLeadingBackslash = false;

            // COMPAT 45: We allow both double and single quotes around a primitive string
            // Even though JSON spec only allows double quotes.
            char openingQuoteCharacter = this.characterBuffer[this.tokenStartIndex];
            Debug.Assert(openingQuoteCharacter == '"' || openingQuoteCharacter == '\'', "The quote character must be the current character when this method is called.");

            // Consume the quote character
            this.tokenStartIndex++;

            // String builder to be used if we need to resolve escape sequences.
            StringBuilder valueBuilder = null;

            int tokenCharOffset = 0;
            while (true)
            {
                // Ensure at least one more character
                if (this.tokenStartIndex + tokenCharOffset >= this.storedCharacterCount)
                {
                    // Attempt a non-awaiting async read (may complete synchronously)
                    ValueTask<bool> readInputTask = this.ReadInputAsync();
                    if (!readInputTask.IsCompletedSuccessfully)
                    {
                        return ParseStringPrimitiveValueResumeAsync(openingQuoteCharacter, valueBuilder, tokenCharOffset, hasLeadingBackslash, readInputTask);
                    }

                    // Detect EOF
                    if (!readInputTask.Result)
                    {
                        return ValueTask.FromException<(ReadOnlyMemory<char>, bool)>(
                            JsonReaderExtensions.CreateException(SRResources.JsonReader_UnexpectedEndOfString));
                    }

                    Debug.Assert((this.tokenStartIndex + tokenCharOffset) < this.storedCharacterCount, "ReadInputAsync didn't read more data but returned true.");

                    // After refill, restart the loop to pick up the character.
                    continue;
                }

                char character = this.characterBuffer[this.tokenStartIndex + tokenCharOffset];
                if (character == '\\')
                {
                    // If we're at the beginning of the string
                    // (means that relative token index must be 0 and we must not have consumed anything into our value builder yet)
                    if (tokenCharOffset == 0 && valueBuilder == null)
                    {
                        hasLeadingBackslash = true;
                    }

                    // We will need the StringBuilder to resolve the escape sequences.
                    EnsureStringValueBuilderInitialized(ref valueBuilder);
                    // Append everything up to the \ character to the value.
                    FlushParsedLiteralToBuilder(valueBuilder, ref tokenCharOffset);
                    Debug.Assert(this.characterBuffer[this.tokenStartIndex] == '\\', "We should have consumed everything up to the escape character.");

                    try
                    {
                        // Attempt to process the escape sequence synchronously
                        if (!this.TryProcessEscapeSequence(valueBuilder))
                        {
                            return ProcessEscapeSequenceResumeAsync(openingQuoteCharacter, valueBuilder, tokenCharOffset, hasLeadingBackslash);
                        }

                        continue;
                    }
                    catch (ODataException ex)
                    {
                        return ValueTask.FromException<(ReadOnlyMemory<char>, bool)>(ex);
                    }
                }
                else if (character == openingQuoteCharacter)
                {
                    // Consume everything up to the quote character
                    ReadOnlyMemory<char> result = FinalizeStringLiteral(valueBuilder, tokenCharOffset);

                    return ValueTask.FromResult((result, hasLeadingBackslash));
                }
                else
                {
                    // Normal character, just skip over it - it will become part of the value as is.
                    tokenCharOffset++;
                }
            }
        }

        /// <summary>
        /// Ensures <paramref name="builder"/> references a cleared reusable <see cref="StringBuilder"/> instance (lazily allocates once).
        /// </summary>
        /// <param name="builder">A <see cref="StringBuilder"/> instance to use for building escaped string literals.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void EnsureStringValueBuilderInitialized(ref StringBuilder builder)
        {
            if (builder != null)
            {
                return;
            }

            if (this.stringValueBuilder == null)
            {
                this.stringValueBuilder = new StringBuilder();
            }
            else
            {
                this.stringValueBuilder.Length = 0;
            }

            builder = this.stringValueBuilder;
        }

        /// <summary>
        /// Ensures at least <paramref name="charactersRequiredAfterTokenStart"/> characters after the token start index.
        /// </summary>
        /// <param name="charactersRequiredAfterTokenStart">The number of characters required after the token start index.</param>
        /// <returns><c>true</c> if required characters are available; <c>false</c> on EOF or pending asynchronous read.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool TryEnsureAvailableCharacters(int charactersRequiredAfterTokenStart)
        {
            return this.tokenStartIndex + charactersRequiredAfterTokenStart <= this.storedCharacterCount;

            // NOTE: TryEnsureAvailableCharacters is intentionally a pure availability predicate.
            // It currently doesn't trigger I/O but we could potentially call ReadInputAsync here
            // to try fetch more data synchronously if required characters are not available.
            // HOWEVER, if we did that, we would need to ensure that if ReadInputAsync doesn't complete
            // synchronously it is awaited in an async method somewhere else before any other ReadInputAsync calls.
            // ReadInputAsync is not re-entrant and we must not call it concurrently. The buffer gets
            // corrupted if we do that due to double compaction.
            // One approach we could use is maintaining a "pendingReadInputTask" field on the reader
            // that is set when ReadInputAsync doesn't complete synchronously and cleared when the task
            // is awaited elsewhere. Due to the complexity of managing that state, we leave that as a
            // future improvement.
        }

        /// <summary>
        /// Attempts to synchronously process an escape sequence starting at the current '\' position.
        /// </summary>
        /// <param name="builder">The <see cref="StringBuilder"/> instance to use for building escaped string literals.</param>
        /// <returns><c>true</c> if the escape sequence was processed successfully;
        /// <c>false</c> on EOF or pending asynchronous read.</returns>
        private bool TryProcessEscapeSequence(StringBuilder builder)
        {
            Debug.Assert(this.characterBuffer[this.tokenStartIndex] == '\\', "Expected backslash.");

            // Escape sequence - we need at least two characters, the backslash and the one character after it.
            if (!this.TryEnsureAvailableCharacters(2))
            {
                return false; // Defer to the async path
            }

            char escapeChar = this.characterBuffer[this.tokenStartIndex + 1];

            if (escapeChar == 'u')
            {
                // For \uXXXX we need 6 characters: '\', 'u' and 4 hex characters.
                if (!this.TryEnsureAvailableCharacters(6))
                {
                    return false; // Defer to the async path
                }

                return TryProcessUnicodeEscapeSequence(builder);
            }

            // We now know we have enough characters to process the escape sequence synchronously
            switch (escapeChar)
            {
                case 'b':
                    builder.Append('\b');
                    this.tokenStartIndex += 2;
                    return true;
                case 'f':
                    builder.Append('\f');
                    this.tokenStartIndex += 2;
                    return true;
                case 'n':
                    builder.Append('\n');
                    this.tokenStartIndex += 2;
                    return true;
                case 'r':
                    builder.Append('\r');
                    this.tokenStartIndex += 2;
                    return true;
                case 't':
                    builder.Append('\t');
                    this.tokenStartIndex += 2;
                    return true;
                case '\\':
                case '\"':
                case '\'':
                case '/':
                    builder.Append(escapeChar);
                    this.tokenStartIndex += 2;
                    return true;
                default:
                    throw JsonReaderExtensions.CreateException(Error.Format(SRResources.JsonReader_UnrecognizedEscapeSequence, "\\" + escapeChar));
            }
        }

        /// <summary>
        /// Attempts to a '\uXXXX' unicode escape sequence starting at the current '\' position;
        /// caller guarantees 4 hex characters are available.
        /// </summary>
        /// <param name="builder">The <see cref="StringBuilder"/> instance to use for building escaped string literals.</param>
        /// <returns><c>true</c> if the escape sequence was processed successfully;
        /// <c>false</c> on EOF or pending asynchronous read.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool TryProcessUnicodeEscapeSequence(StringBuilder builder)
        {
            Debug.Assert(this.characterBuffer[this.tokenStartIndex + 1] == 'u', "Expected 'u' after backslash.");

            // Consume '\' and 'u'
            this.tokenStartIndex += 2;

            int characterValue = this.ParseUnicodeHexValue();
            builder.Append((char)characterValue);

            return true;
        }

        /// <summary>
        /// Asynchronous slow path string literal parsing when buffer refills or escapes require awaiting.
        /// Continues from the first unconsumed character after the opening quote.
        /// </summary>
        /// <param name="quoteCharacter">The quote character for the string literal being parsed.</param>
        /// <param name="builder">The <see cref="StringBuilder"/> instance to use for building escaped string literals.</param>
        /// <param name="bufferedLiteralLength">Count of buffered literal characters.</param>
        /// <param name="hasLeadingBackslash">Indicates if the string has a leading backslash.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// The value of the TResult parameter contains a tuple comprising of the value of the string primitive value 
        /// and a value of true if the first character in the string has a backlash; otherwise false.
        /// </returns>
        [MethodImpl(MethodImplOptions.NoInlining)]
        private async ValueTask<(ReadOnlyMemory<char> Value, bool HasLeadingBackslash)> ParseStringPrimitiveValueResumeAsync(
            char quoteCharacter,
            StringBuilder builder,
            int bufferedLiteralLength,
            bool hasLeadingBackslash,
            ValueTask<bool> pendingReadInputTask)
        {
            if (pendingReadInputTask != default)
            {
                // Detect EOF
                if (!await GetOrAwait(pendingReadInputTask).ConfigureAwait(false))
                {
                    throw JsonReaderExtensions.CreateException(SRResources.JsonReader_UnexpectedEndOfString);
                }
            }

            while (true)
            {
                // Ensure at least one more character
                if (this.tokenStartIndex + bufferedLiteralLength >= this.storedCharacterCount)
                {
                    // Detect EOF
                    if (!await this.ReadInputAsync().ConfigureAwait(false))
                    {
                        throw JsonReaderExtensions.CreateException(SRResources.JsonReader_UnexpectedEndOfString);
                    }
                }

                char ch = this.characterBuffer[this.tokenStartIndex + bufferedLiteralLength];
                if (ch == '\\')
                {
                    // If we're at the beginning of the string
                    // (means that relative token index must be 0 and we must not have consumed anything into our value builder yet)
                    if (bufferedLiteralLength == 0 && builder == null)
                    {
                        hasLeadingBackslash = true;
                    }

                    // We will need the StringBuilder to resolve the escape sequences.
                    EnsureStringValueBuilderInitialized(ref builder);
                    // Append everything up to the \ character to the value.
                    FlushParsedLiteralToBuilder(builder, ref bufferedLiteralLength);
                    await ProcessEscapeSequenceCoreAsync(builder).ConfigureAwait(false);

                    continue;
                }
                else if (ch == quoteCharacter)
                {
                    // Consume everything up to the quote character
                    ReadOnlyMemory<char> result = FinalizeStringLiteral(builder, bufferedLiteralLength);

                    return (result, hasLeadingBackslash);
                }
                else
                {
                    // Normal character, just skip over it - it will become part of the value as is.
                    bufferedLiteralLength++;
                }
            }
        }

        /// <summary>
        /// Asynchronous slow path for processing an escape sequence that cannot be completed synchronously.
        /// </summary>
        /// <param name="quoteCharacter">The quote character for the string literal being parsed.</param>
        /// <param name="builder">The <see cref="StringBuilder"/> instance to use for building escaped string literals.</param>
        /// <param name="bufferedLiteralLength">Count of buffered literal characters.</param>
        /// <param name="hasLeadingBackslash">Indicates if the string has a leading backslash.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// The value of the TResult parameter contains a tuple comprising of the value of the string primitive value 
        /// and a value of true if the first character in the string has a backlash; otherwise false.
        /// </returns>
        [MethodImpl(MethodImplOptions.NoInlining)]
        private async ValueTask<(ReadOnlyMemory<char> Value, bool HasLeadingBackslash)> ProcessEscapeSequenceResumeAsync(
            char quoteCharacter,
            StringBuilder builder,
            int bufferedLiteralLength,
            bool hasLeadingBackslash)
        {
            Debug.Assert(this.characterBuffer[this.tokenStartIndex] == '\\', "We should have consumed everything up to the escape character.");
            Debug.Assert(builder != null, "builder must be initialized prior to asynchronous processing of escape sequences.");

            await ProcessEscapeSequenceCoreAsync(builder).ConfigureAwait(false);

            // Continue with asynchronous literal parsing
            return await ParseStringPrimitiveValueResumeAsync(quoteCharacter, builder, 0, hasLeadingBackslash, default).ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously processes an escape sequence.
        /// </summary>
        /// <param name="builder">The <see cref="StringBuilder"/> instance to use for building escaped string literals.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        [MethodImpl(MethodImplOptions.NoInlining)]
        private async ValueTask ProcessEscapeSequenceCoreAsync(StringBuilder builder)
        {
            if (!await this.EnsureAvailableCharactersAsync(2).ConfigureAwait(false))
            {
                throw JsonReaderExtensions.CreateException(
                    Error.Format(SRResources.JsonReader_UnrecognizedEscapeSequence, "\\"));
            }

            char escapeChar = this.characterBuffer[this.tokenStartIndex + 1];

            if (escapeChar == 'u')
            {
                if (!await this.EnsureAvailableCharactersAsync(6).ConfigureAwait(false))
                {
                    string badUnicodeHexValue = new string(this.characterBuffer, this.tokenStartIndex, this.storedCharacterCount - this.tokenStartIndex);
                    throw JsonReaderExtensions.CreateException(
                        Error.Format(SRResources.JsonReader_UnrecognizedEscapeSequence, badUnicodeHexValue));
                }

                // Consume '\' and 'u'
                this.tokenStartIndex += 2;

                int characterValue = this.ParseUnicodeHexValue();
                builder.Append((char)characterValue);

                return;
            }

            // Non-unicode escape sequence
            this.tokenStartIndex += 2;
            builder.Append(escapeChar switch
            {
                'b' => '\b',
                'f' => '\f',
                'n' => '\n',
                'r' => '\r',
                't' => '\t',
                '\\' or '\"' or '\'' or '/' => escapeChar,
                _ => throw JsonReaderExtensions.CreateException(
                    Error.Format(SRResources.JsonReader_UnrecognizedEscapeSequence, "\\" + escapeChar))
            });
        }

        /// <summary>
        /// Asynchronously parses the null primitive value.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.
        /// The value of the TResult parameter contains null if successful; otherwise throws.</returns>
        /// <remarks>Assumes that the current token position points to the 'n' character.</remarks>
        private ValueTask<object> ParseNullPrimitiveValueAsync()
        {
            Debug.Assert(
                this.tokenStartIndex < this.storedCharacterCount && this.characterBuffer[this.tokenStartIndex] == 'n',
                "The method should only be called when the 'n' character is the start of the token.");

            // We can call ParseNameAsync since we know the first character is 'n' and thus it won't be quoted.
            ValueTask<ReadOnlyMemory<char>> parseNameTask = this.ParseNameAsync();
            if (parseNameTask.IsCompletedSuccessfully)
            {
                ReadOnlyMemory<char> token = parseNameTask.Result;
                if (!token.Span.SequenceEqual(JsonConstants.JsonNullLiteral.AsSpan()))
                {
                    // Return a faulted Task (rather than throw synchronously).
                    return ValueTask.FromException<object>(
                        JsonReaderExtensions.CreateException(Error.Format(SRResources.JsonReader_UnexpectedToken, token)));
                }

                return ValueTask.FromResult<object>(null);
            }

            // Slow path: allocate state machine only if we really have to await.
            return AwaitParseNameAsync(this, parseNameTask);

            static async ValueTask<object> AwaitParseNameAsync(JsonReader thisParam, ValueTask<ReadOnlyMemory<char>> pendingParseNameTask)
            {
                ReadOnlyMemory<char> token = await pendingParseNameTask.ConfigureAwait(false);

                if (!token.Span.SequenceEqual(JsonConstants.JsonNullLiteral.AsSpan()))
                {
                    throw JsonReaderExtensions.CreateException(Error.Format(SRResources.JsonReader_UnexpectedToken, token));
                }

                return null;
            }
        }

        /// <summary>
        /// Asynchronously parses the true or false primitive values.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.
        /// The value of the TResult parameter contains true or false if successful; otherwise throws.</returns>
        /// <remarks>Assumes that the current token position points to the 't' or 'f' character.</remarks>
        private ValueTask<object> ParseBooleanPrimitiveValueAsync()
        {
            Debug.Assert(
                this.tokenStartIndex < this.storedCharacterCount && (this.characterBuffer[this.tokenStartIndex] == 't' || this.characterBuffer[this.tokenStartIndex] == 'f'),
                "The method should only be called when the 't' or 'f' character is the start of the token.");

            // We can call ParseNameAsync since we know the first character is 't' or 'f' and thus it won't be quoted.
            ValueTask<ReadOnlyMemory<char>> parseNameTask = this.ParseNameAsync();
            if (parseNameTask.IsCompletedSuccessfully)
            {
                ReadOnlyMemory<char> token = parseNameTask.Result;

                if (token.Span.SequenceEqual(JsonConstants.JsonFalseLiteral.AsSpan()))
                {
                    return ValueTask.FromResult<object>(false);
                }

                if (token.Span.SequenceEqual(JsonConstants.JsonTrueLiteral.AsSpan()))
                {
                    return ValueTask.FromResult<object>(true);
                }

                return ValueTask.FromException<object>(
                    JsonReaderExtensions.CreateException(Error.Format(SRResources.JsonReader_UnexpectedToken, token)));
            }

            // Slow path: allocate state machine only if we really have to await.
            return AwaitParseNameAsync(parseNameTask);

            static async ValueTask<object> AwaitParseNameAsync(ValueTask<ReadOnlyMemory<char>> pendingParseNameTask)
            {
                ReadOnlyMemory<char> token = await pendingParseNameTask.ConfigureAwait(false);

                if (token.Span.SequenceEqual(JsonConstants.JsonFalseLiteral.AsSpan()))
                {
                    return false;
                }

                if (token.Span.SequenceEqual(JsonConstants.JsonTrueLiteral.AsSpan()))
                {
                    return true;
                }

                throw JsonReaderExtensions.CreateException(Error.Format(SRResources.JsonReader_UnexpectedToken, token));
            }
        }

        /// <summary>
        /// Asynchronously parses the number primitive values.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.
        /// The value of the TResult parameter contains an Int32, Decimal or Double value; otherwise throws.</returns>
        /// <remarks>Assumes that the current token position points to the first character of the number, so either digit, dot or dash.</remarks>
        private ValueTask<object> ParseNumberPrimitiveValueAsync()
        {
            Debug.Assert(
                this.tokenStartIndex < this.storedCharacterCount && (this.characterBuffer[this.tokenStartIndex] == '.' || this.characterBuffer[this.tokenStartIndex] == '-' || Char.IsDigit(this.characterBuffer[this.tokenStartIndex])),
                "The method should only be called when a digit, dash or dot character is the start of the token.");

            // Walk over all characters which might belong to the number
            // Skip the first one since we already verified it belongs to the number.
            int currentCharacterTokenRelativeIndex = 1;

            // Fast path: consume what is already buffered, performing only synchronous refills
            while (true)
            {
                if ((this.tokenStartIndex + currentCharacterTokenRelativeIndex) < this.storedCharacterCount)
                {
                    char character = this.characterBuffer[this.tokenStartIndex + currentCharacterTokenRelativeIndex];
                    if (IsNumberChar(character))
                    {
                        currentCharacterTokenRelativeIndex++;
                        continue;
                    }

                    break; // Not a number char
                }

                // Need more data
                ValueTask<bool> readInputTask = this.ReadInputAsync();
                if (!readInputTask.IsCompletedSuccessfully)
                {
                    // Slow path: Await operation
                    return AwaitParseNumberAsync(this, readInputTask, currentCharacterTokenRelativeIndex);
                }

                // EOF
                if (!readInputTask.Result)
                {
                    break;
                }
            }

            try
            {
                ReadOnlyMemory<char> numberMemory = this.ConsumeTokenToMemory(currentCharacterTokenRelativeIndex);

                return ValueTask.FromResult(this.ParseNumericToken(numberMemory.Span));
            }
            catch (ODataException ex)
            {
                return ValueTask.FromException<object>(ex);
            }

            static async ValueTask<object> AwaitParseNumberAsync(JsonReader thisParam, ValueTask<bool> pendingReadInputTask, int relativeIndex)
            {
                while (true)
                {
                    bool moreInputAvailable = await GetOrAwait(pendingReadInputTask).ConfigureAwait(false);
                    if (!moreInputAvailable)
                    {
                        break; // EOF
                    }

                    // Consume as much as possible from buffer
                    while ((thisParam.tokenStartIndex + relativeIndex) < thisParam.storedCharacterCount)
                    {
                        char character = thisParam.characterBuffer[thisParam.tokenStartIndex + relativeIndex];
                        if (IsNumberChar(character))
                        {
                            relativeIndex++;
                            continue;
                        }

                        ReadOnlyMemory<char> numMemory = thisParam.ConsumeTokenToMemory(relativeIndex);
                        return thisParam.ParseNumericToken(numMemory.Span);
                    }

                    // Need to read more input
                    pendingReadInputTask = thisParam.ReadInputAsync();
                    continue;
                }

                // EOF: Parse what we have
                ReadOnlyMemory<char> numberMemory = thisParam.ConsumeTokenToMemory(relativeIndex);
                return thisParam.ParseNumericToken(numberMemory.Span);
            }
        }

        /// <summary>
        /// Asynchronously parses a name token.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.
        /// The value of the TResult parameter contains the name token value.</returns>
        /// <remarks>Name tokens are (for backward compat reasons) either
        /// - string value quoted with double quotes.
        /// - string value quoted with single quotes.
        /// - sequence of letters, digits, underscores and dollar signs (without quoted and in any order).</remarks>
        private ValueTask<ReadOnlyMemory<char>> ParseNameAsync()
        {
            Debug.Assert(this.tokenStartIndex < this.storedCharacterCount, "Must have at least one character available.");

            char firstCharacter = this.characterBuffer[this.tokenStartIndex];
            if ((firstCharacter == '"') || (firstCharacter == '\''))
            {
                ValueTask<(ReadOnlyMemory<char> Value, bool HasLeadingBackslash)> parseQuotedNameTask = this.ParseStringPrimitiveValueAsync();
                if (parseQuotedNameTask.IsCompletedSuccessfully)
                {
                    return ValueTask.FromResult(parseQuotedNameTask.Result.Value);
                }

                return AwaitParseQuotedNameAsync(this, parseQuotedNameTask);
            }

            // Pass unquoted string
            int currentCharacterTokenRelativeIndex = 0;
            while (true)
            {
                Debug.Assert(this.tokenStartIndex < this.storedCharacterCount, "Must have at least one character available.");

                if ((this.tokenStartIndex + currentCharacterTokenRelativeIndex) < this.storedCharacterCount)
                {
                    char character = this.characterBuffer[this.tokenStartIndex + currentCharacterTokenRelativeIndex];
                    // COMPAT 46: JSON property names don't require quotes and they allow any letter, digit, underscore or dollar sign in them.
                    if (IsCharacterAllowedInPropertyName(character))
                    {
                        currentCharacterTokenRelativeIndex++;
                        continue;
                    }

                    break; // Not a valid property name character
                }

                // Read more data
                ValueTask<bool> readInputTask = this.ReadInputAsync();
                if (!readInputTask.IsCompletedSuccessfully)
                {
                    return AwaitParseUnquotedNameAsync(this, readInputTask, currentCharacterTokenRelativeIndex);
                }

                if (!readInputTask.Result)
                {
                    break; // EOF
                }
            }

            ReadOnlyMemory<char> nameMemory = this.ConsumeTokenToMemory(currentCharacterTokenRelativeIndex);
            return ValueTask.FromResult(nameMemory);

            static async ValueTask<ReadOnlyMemory<char>> AwaitParseQuotedNameAsync(JsonReader thisParam, ValueTask<(ReadOnlyMemory<char> Value, bool HasLeadingBackslash)> pendingParseQuotedNameTask)
            {
                (ReadOnlyMemory<char> Value, bool HasLeadingBackslash) = await pendingParseQuotedNameTask.ConfigureAwait(false);

                return Value;
            }

            static async ValueTask<ReadOnlyMemory<char>> AwaitParseUnquotedNameAsync(JsonReader thisParam, ValueTask<bool> pendingReadInputTask, int relativeIndex)
            {
                while (true)
                {
                    bool moreInputAvailable = await GetOrAwait(pendingReadInputTask).ConfigureAwait(false);
                    if (!moreInputAvailable)
                    {
                        break; // EOF
                    }

                    // Consume as many allowed characters as possible
                    while (thisParam.tokenStartIndex + relativeIndex < thisParam.storedCharacterCount)
                    {
                        char character = thisParam.characterBuffer[thisParam.tokenStartIndex + relativeIndex];
                        if (thisParam.IsCharacterAllowedInPropertyName(character))
                        {
                            relativeIndex++;
                            continue;
                        }

                        return thisParam.ConsumeTokenToMemory(relativeIndex);
                    }

                    pendingReadInputTask = thisParam.ReadInputAsync();
                }

                // EOF: Return whatever we have accumulated so far
                return thisParam.ConsumeTokenToMemory(relativeIndex);
            }
        }

        /// <summary>
        /// Asynchronously reads bytes from the current string value.
        /// </summary>
        /// <param name="chars">The character buffer to populate</param>
        /// <param name="offset">The number of characters offset into the buffer to read</param>
        /// <param name="maxLength">The maximum number of characters to read into the buffer</param>
        /// <returns>A task that represents the asynchronous operation.
        /// The value of the TResult parameter contains the number of characters read.</returns>
        /// <remarks>
        /// Assumes that the current token position points to the opening quote.
        /// Note that the string parsing can never end with EndOfInput, since we're already seen the quote.
        /// So it can either return a string successfully or fail.</remarks>
        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Splitting the function would make it hard to understand.")]
        private async Task<int> ReadCharsAsync(char[] chars, int offset, int maxLength)
        {
            if (!readingStream)
            {
                return 0;
            }

            int charsRead = 0;

            while (charsRead < maxLength && (this.tokenStartIndex < this.storedCharacterCount || await this.ReadInputAsync().ConfigureAwait(false)))
            {
                char character = this.characterBuffer[this.tokenStartIndex];
                bool advance = true;

                if (character == GetClosingQuoteCharacter(this.streamOpeningQuoteCharacter) && --this.balancedQuoteCount < 1)
                {
                    if (character != '"')
                    {
                        // The character is part of the JSON stream; copy it to the output buffer
                        chars[charsRead + offset] = character;
                        charsRead++;
                        this.scopes.Pop();
                    }

                    // Consume the closing quote character.
                    this.tokenStartIndex++;
                    readingStream = false;
                    this.scopes.Peek().ValueCount++;

                    // move to next node
                    await this.ReadAsync().ConfigureAwait(false);
                    return charsRead;
                }

                if (character == this.streamOpeningQuoteCharacter)
                {
                    this.balancedQuoteCount++;
                }

                if (character == '\\')
                {
                    // Consume the escape
                    this.tokenStartIndex++;
                    if (!await this.EnsureAvailableCharactersAsync(1).ConfigureAwait(false))
                    {
                        throw JsonReaderExtensions.CreateException(Error.Format(SRResources.JsonReader_UnrecognizedEscapeSequence, "\\uXXXX"));
                    }

                    character = this.characterBuffer[this.tokenStartIndex];

                    switch (character)
                    {
                        case 'b':
                            character = '\b';
                            break;
                        case 'f':
                            character = '\f';
                            break;
                        case 'n':
                            character = '\n';
                            break;
                        case 'r':
                            character = '\r';
                            break;
                        case 't':
                            character = '\t';
                            break;
                        case '\\':
                        case '\"':
                        case '\'':
                        case '/':
                            // Use the (now unescaped) character from reader;
                            break;
                        case 'u':

                            // Consume the "u"
                            tokenStartIndex++;

                            // We need 4 hex characters
                            if (!await this.EnsureAvailableCharactersAsync(4).ConfigureAwait(false))
                            {
                                throw JsonReaderExtensions.CreateException(Error.Format(SRResources.JsonReader_UnrecognizedEscapeSequence, "\\uXXXX"));
                            }

                            character = (char)this.ParseUnicodeHexValue();

                            // We are already positioned on the next character, so don't advance at the end
                            advance = false;
                            break;
                        default:
                            throw JsonReaderExtensions.CreateException(Error.Format(SRResources.JsonReader_UnrecognizedEscapeSequence, "\\" + character));
                    }
                }

                chars[charsRead + offset] = character;
                charsRead++;

                if (advance)
                {
                    this.tokenStartIndex++;
                }
            }

            // we reached the end of the file without finding a closing quote character
            if (charsRead < maxLength)
            {
                throw JsonReaderExtensions.CreateException(SRResources.JsonReader_UnexpectedEndOfString);
            }

            return charsRead;
        }

        /// <summary>
        /// Asynchronously skips all whitespace characters in the input.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.
        /// The value of the TResult parameter contains true if a non-whitespace character was found,
        /// in which case the <see cref="tokenStartIndex"/> is pointing at that character;
        /// otherwise false if there are no non-whitespace characters left in the input.</returns>
        private ValueTask<bool> SkipWhitespacesAsync()
        {
            // Fast path: scan currently buffered characters without awaiting.
            while (this.tokenStartIndex < this.storedCharacterCount)
            {
                int nonWhitespaceIndex = FindFirstNonWhitespace(this.characterBuffer, this.tokenStartIndex, this.storedCharacterCount);
                if (nonWhitespaceIndex >= 0)
                {
                    this.tokenStartIndex += nonWhitespaceIndex;
                    return ValueTask.FromResult(true);
                }

                // All remaining characters are whitespace, advance to end
                this.tokenStartIndex = this.storedCharacterCount;
            }

            // No more buffered characters - attempt to read. ReadInputAsync may complete synchronously.
            while (true)
            {
                ValueTask<bool> readInputTask = this.ReadInputAsync();
                if (!readInputTask.IsCompletedSuccessfully)
                {
                    return AwaitReadInputAsync(this, readInputTask);
                }

                if (!readInputTask.Result)
                {
                    // EOF (only whitespace was left)
                    return ValueTask.FromResult(false);
                }

                while (this.tokenStartIndex < this.storedCharacterCount)
                {
                    int nonWhitespaceIndex = FindFirstNonWhitespace(this.characterBuffer, this.tokenStartIndex, this.storedCharacterCount);
                    if (nonWhitespaceIndex >= 0)
                    {
                        this.tokenStartIndex += nonWhitespaceIndex;
                        return ValueTask.FromResult(true);
                    }

                    // All remaining characters are whitespace, advance to end
                    this.tokenStartIndex = this.storedCharacterCount;
                }
            }

            // Async slow path: only allocated when an awaited read is pending.
            static async ValueTask<bool> AwaitReadInputAsync(JsonReader thisParam, ValueTask<bool> pendingReadInputTask)
            {
                while (true)
                {
                    bool moreInputAvailable = await GetOrAwait(pendingReadInputTask).ConfigureAwait(false);
                    if (!moreInputAvailable)
                    {
                        return false; // EOF
                    }

                    while (thisParam.tokenStartIndex < thisParam.storedCharacterCount)
                    {
                        int nonWhitespaceIndex = FindFirstNonWhitespace(thisParam.characterBuffer, thisParam.tokenStartIndex, thisParam.storedCharacterCount);
                        if (nonWhitespaceIndex >= 0)
                        {
                            thisParam.tokenStartIndex += nonWhitespaceIndex;
                            return true;
                        }

                        // All remaining characters are whitespace, advance to end
                        thisParam.tokenStartIndex = thisParam.storedCharacterCount;
                    }

                    pendingReadInputTask = thisParam.ReadInputAsync();
                }
            }
        }

        /// <summary>
        /// Asynchronously ensures that a specified number of characters after the token start is available in the buffer.
        /// </summary>
        /// <param name="characterCountAfterTokenStart">The number of character after the token to make available.</param>
        /// <returns>A task that represents the asynchronous operation.
        /// The value of the TResult parameter contains true if at least the required number of characters is available; 
        /// otherwise false if end of input was reached.</returns>
        private ValueTask<bool> EnsureAvailableCharactersAsync(int characterCountAfterTokenStart)
        {
            // Fast path: already have enough buffered characters.
            if (this.tokenStartIndex + characterCountAfterTokenStart <= this.storedCharacterCount)
            {
                return ValueTask.FromResult(true);
            }

            while (true)
            {
                // Need to read more data. This may complete synchronously.
                ValueTask<bool> readInputTask = this.ReadInputAsync();
                if (!readInputTask.IsCompletedSuccessfully)
                {
                    return AwaitReadInputAsync(this, readInputTask, characterCountAfterTokenStart);
                }

                if (!readInputTask.Result)
                {
                    // EOF
                    return ValueTask.FromResult(false);
                }

                if (this.tokenStartIndex + characterCountAfterTokenStart <= this.storedCharacterCount)
                {
                    return ValueTask.FromResult(true);
                }
            }

            static async ValueTask<bool> AwaitReadInputAsync(JsonReader thisParam, ValueTask<bool> pendingReadInputTask, int neededCharactersAfterTokenStart)
            {
                while (true)
                {
                    bool moreInputAvailable = await GetOrAwait(pendingReadInputTask).ConfigureAwait(false);
                    if (!moreInputAvailable)
                    {
                        return false; // EOF
                    }

                    if (thisParam.tokenStartIndex + neededCharactersAfterTokenStart <= thisParam.storedCharacterCount)
                    {
                        return true;
                    }

                    pendingReadInputTask = thisParam.ReadInputAsync();
                }
            }
        }

        /// <summary>
        /// Asynchronously reads more characters from the input asynchronously.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.
        /// The value of the TResult parameter contains true if more characters are available;
        /// otherwise false if end of input was reached.</returns>
        /// <remarks>This may move characters in the <see cref="characterBuffer"/>, so after this is called
        /// all indices to the <see cref="characterBuffer"/> are invalid except for <see cref="tokenStartIndex"/>.</remarks>
        private ValueTask<bool> ReadInputAsync()
        {
            Debug.Assert(this.tokenStartIndex >= 0 && this.tokenStartIndex <= this.storedCharacterCount, "The token start is out of stored characters range.");

            if (this.endOfInputReached)
            {
                return ValueTask.FromResult(false);
            }

            this.CopyInputToBuffer();

            // Read more characters from the input.
            // Use the Read method which returns any character as soon as it's available
            // we don't want to wait for the entire buffer to fill if the input doesn't have
            // the characters ready.
            Task<int> readTask = this.reader.ReadAsync(
                this.characterBuffer,
                this.storedCharacterCount,
                this.characterBuffer.Length - this.storedCharacterCount);

            if (readTask.IsCompletedSuccessfully)
            {
                int readCount = readTask.Result;
                if (readCount == 0)
                {
                    // No more characters available, end of input.
                    this.endOfInputReached = true;
                    return ValueTask.FromResult(false);
                }

                this.storedCharacterCount += readCount;
                return ValueTask.FromResult(true);
            }

            // Slow path: asynchronously await the read task.
            return AwaitReadInputAsync(this, readTask);

            static async ValueTask<bool> AwaitReadInputAsync(JsonReader thisParam, Task<int> pendingReadTask)
            {
                int readCount = await pendingReadTask.ConfigureAwait(false);
                if (readCount == 0)
                {
                    // No more characters available, end of input.
                    thisParam.endOfInputReached = true;
                    return false;
                }

                thisParam.storedCharacterCount += readCount;
                return true;
            }
        }

        private void CopyInputToBuffer()
        {
            // initialize the buffer
            if (this.characterBuffer == null)
            {
                this.characterBuffer = BufferUtils.RentFromBuffer(ArrayPool, InitialCharacterBufferSize);
            }

            Debug.Assert(this.storedCharacterCount <= this.characterBuffer.Length, "We can only store as many characters as fit into our buffer.");

            // If the buffer is empty (all characters were consumed from it), just start over.
            if (this.tokenStartIndex == this.storedCharacterCount)
            {
                this.tokenStartIndex = 0;
                this.storedCharacterCount = 0;
            }
            else if (this.storedCharacterCount == this.characterBuffer.Length)
            {
                // No more room in the buffer, move or grow the buffer.
                if (this.tokenStartIndex < this.characterBuffer.Length / 4)
                {
                    // The entire buffer is full of unconsumed characters
                    // We need to grow the buffer. Double the size of the buffer.
                    if (this.characterBuffer.Length == int.MaxValue)
                    {
                        throw JsonReaderExtensions.CreateException(SRResources.JsonReader_MaxBufferReached);
                    }

                    int newBufferSize = this.characterBuffer.Length * 2;
                    newBufferSize = newBufferSize < 0 ? int.MaxValue : newBufferSize; // maybe overflow

                    char[] newCharacterBuffer = BufferUtils.RentFromBuffer(ArrayPool, newBufferSize);

                    // Copy the existing characters to the new buffer.
                    Array.Copy(this.characterBuffer, this.tokenStartIndex, newCharacterBuffer, 0,
                        this.storedCharacterCount - this.tokenStartIndex);
                    this.storedCharacterCount = this.storedCharacterCount - this.tokenStartIndex;
                    this.tokenStartIndex = 0;

                    // And switch the buffers
                    BufferUtils.ReturnToBuffer(ArrayPool, this.characterBuffer);
                    this.characterBuffer = newCharacterBuffer;
                }
                else
                {
                    // Some characters were consumed, we can just move them in the buffer
                    // to get more room without allocating.
                    Array.Copy(this.characterBuffer, this.tokenStartIndex, this.characterBuffer, 0,
                        this.storedCharacterCount - this.tokenStartIndex);
                    this.storedCharacterCount -= this.tokenStartIndex;
                    this.tokenStartIndex = 0;
                }
            }

            Debug.Assert(
                this.storedCharacterCount < this.characterBuffer.Length,
                "We should have more room in the buffer by now.");
        }

        /// <summary>
        /// Parses a unicode hex value.
        /// </summary>
        /// <returns>32-bit signed integer equivalent.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int ParseUnicodeHexValue()
        {
            Debug.Assert(this.tokenStartIndex + 4 <= this.storedCharacterCount, "4 specified characters outside of the available range.");

            char hexChar1 = this.characterBuffer[this.tokenStartIndex++];
            char hexChar2 = this.characterBuffer[this.tokenStartIndex++];
            char hexChar3 = this.characterBuffer[this.tokenStartIndex++];
            char hexChar4 = this.characterBuffer[this.tokenStartIndex++];

            int characterValue = ParseFourHexDigits(hexChar1, hexChar2, hexChar3, hexChar4);
            if (characterValue < 0)
            {
                throw JsonReaderExtensions.CreateException(Error.Format(SRResources.JsonReader_UnrecognizedEscapeSequence, "\\u" + new string(new char[] {hexChar1, hexChar2, hexChar3, hexChar4})));
            }

            return characterValue;
        }

        /// <summary>
        /// Converts four hexadecimal characters to their integer value.
        /// </summary>
        /// <param name="hexChar1">The first hexadecimal character.</param>
        /// <param name="hexChar2">The second hexadecimal character.</param>
        /// <param name="hexChar3">The third hexadecimal character.</param>
        /// <param name="hexChar4">The fourth hexadecimal character.</param>
        /// <returns>
        /// The integer value represented by the four hexadecimal characters, or -1 if any character is not a valid hexadecimal digit.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int ParseFourHexDigits(char hexChar1, char hexChar2, char hexChar3, char hexChar4)
        {
            int digit1 = HexCharToInt(hexChar1);
            int digit2 = HexCharToInt(hexChar2);
            int digit3 = HexCharToInt(hexChar3);
            int digit4 = HexCharToInt(hexChar4);

            if ((digit1 | digit2 | digit3 | digit4) < 0)
            {
                return -1;
            }

            return (digit1 << 12) | (digit2 << 8) | (digit3 << 4) | digit4;
        }

        /// <summary>
        /// Converts a single hexadecimal character to its integer value.
        /// </summary>
        /// <param name="hexChar">The hexadecimal character to convert.</param>
        /// <returns>
        /// The integer value of the hexadecimal character (0-15), or -1 if the character is not a valid hexadecimal digit.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int HexCharToInt(char hexChar)
        {
            if ((uint)(hexChar - '0') <= 9u) return hexChar - '0';
            if ((uint)(hexChar - 'A') <= 5u) return hexChar - 'A' + 10;
            if ((uint)(hexChar - 'a') <= 5u) return hexChar - 'a' + 10;
            return -1;
        }

        /// <summary>
        /// Determines if a given character is allowed in a JSON property name.
        /// </summary>
        /// <param name="character">The character to check.</param>
        /// <returns>true if the character is allowed in a JSON property name; otherwise false.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool IsCharacterAllowedInPropertyName(char character)
        {
            return Char.IsLetterOrDigit(character) ||
                character == '_' ||
                character == '$' ||
                (this.allowAnnotations && (character == '.' || character == '@'));
        }

        /// <summary>
        /// Converts the JSON number text to a boxed numeric value.
        /// Order: Int32 (if fits) -> Decimal (when not IEEE754 compatible) -> Double; throws if none succeed.
        /// </summary>
        /// <param name="numberString">Canonical numeric token (no surrounding whitespace).</param>
        /// <returns>Boxed int, decimal or double.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private object ParseNumericToken(ReadOnlySpan<char> numberSpan)
        {
            // We will first try and convert the value to Int32. If it succeeds, use that.
            // And then, we will try Decimal, since it will lose precision while expected type is specified.
            // Otherwise, we will try and convert the value into a double.
            if (Int32.TryParse(numberSpan, NumberStyles.Integer, NumberFormatInfo.InvariantInfo, out int intValue))
            {
                return intValue;
            }

            // if it is not Ieee754Compatible, decimal will be parsed before double to keep precision
            if (!this.isIeee754Compatible && Decimal.TryParse(numberSpan, NumberStyles.Number, NumberFormatInfo.InvariantInfo, out decimal decimalValue))
            {
                return decimalValue;
            }

            if (Double.TryParse(numberSpan, NumberStyles.Float, NumberFormatInfo.InvariantInfo, out double doubleValue))
            {
                return doubleValue;
            }

            throw JsonReaderExtensions.CreateException(Error.Format(SRResources.JsonReader_InvalidNumberFormat, numberSpan.ToString()));
        }

        /// <summary>
        /// Determines whether the specified character is valid within a numeric literal.
        /// </summary>
        /// <param name="ch">The character to evaluate.</param>
        /// <returns>
        /// <c>true</c> if the character is a digit, decimal point, exponent indicator ('e' or 'E'), or sign ('+' or '-'); otherwise, <c>false</c>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsNumberChar(char ch)
        {
            return Char.IsDigit(ch) ||
                ch == '.' ||
                ch == 'E' || ch == 'e' ||
                ch == '-' || ch == '+';
        }

        /// <summary>
        /// Returns a shared string instance for common OData property names or values, otherwise returns a new string.
        /// </summary>
        /// <remarks>
        /// This method reduces memory usage by returning static instances for known property names or values.
        /// For uncommon or unique strings, it returns a new string instance.
        /// </remarks>
        /// <param name="span">A read-only span of characters representing the input string to process.</param>
        /// <returns>
        /// A shared string instance if the input matches a predefined common value or property name; otherwise, a new string instance representing the input.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static string GetCommonOrNewString(ReadOnlySpan<char> span)
        {
            if (span.IsEmpty)
            {
                return string.Empty;
            }

            // For known property names, return static interned instances
            if (ODataJsonUtils.TryGetMatchingCommonValueString(span, out string commonValue))
            {
                return commonValue;
            }

            return span.ToString();
        }

        /// <summary>
        /// Finds the index of the first non-whitespace character in the specified character buffer range.
        /// Whitespace characters are defined by the JSON specification: space (' '), tab ('\t'), carriage return ('\r'), and newline ('\n').
        /// </summary>
        /// <param name="buffer">The character buffer to search.</param>
        /// <param name="start">The starting index (inclusive) in the buffer.</param>
        /// <param name="end">The ending index (exclusive) in the buffer.</param>
        /// <returns>
        /// The zero-based index (relative to <paramref name="start"/>) of the first non-whitespace character,
        /// or -1 if all characters in the specified range are whitespace.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int FindFirstNonWhitespace(char[] buffer, int start, int end)
        {
            ReadOnlySpan<char> span = buffer.AsSpan(start, end - start);
            return span.IndexOfAnyExcept(jsonWhitespaceSearchValues);
        }

        /// <summary>
        /// Returns the result of a <see cref="ValueTask{TResult}"/> if already completed successfully; otherwise, awaits it.
        /// </summary>
        /// <param name="pendingTask">The <see cref="ValueTask{TResult}"/> to evaluate or await.</param>
        /// <returns>A <see cref="ValueTask{TResult}"/> that completes with the result of the original task.</returns
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static ValueTask<bool> GetOrAwait(ValueTask<bool> pendingTask)
        {
            return pendingTask.IsCompletedSuccessfully
                ? ValueTask.FromResult(pendingTask.Result)
                : pendingTask;
        }

        /// <summary>
        /// Assigns the node value from the token if it is not already set.
        /// </summary>
        /// <param name="memory">The character span to convert to a string.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void SetNodeValue(JsonReader thisParam, ReadOnlyMemory<char> memory)
        {
            if (MemoryMarshal.TryGetString(memory, out string primitiveValue, out int length, out int start) && start == 0 && length == memory.Length)
            {
                thisParam.nodeValue = primitiveValue;
            }
            else
            {
                thisParam.nodeValue = GetCommonOrNewString(memory.Span);
            }
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
