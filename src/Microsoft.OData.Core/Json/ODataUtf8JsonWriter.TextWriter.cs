//---------------------------------------------------------------------
// <copyright file="ODataUtf8JsonWriter.TextWriter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

#if NETCOREAPP
namespace Microsoft.OData.Json
{
    using System;
    using System.Buffers;
    using System.IO;
    using System.Text;
    using System.Threading.Tasks;

    internal sealed partial class ODataUtf8JsonWriter
    {
        /// <summary>
        /// Starts a TextWriter value scope with the specified content type.
        /// </summary>
        /// <param name="contentType">The content type of the TextWriter value scope.</param>
        /// <returns>A TextWriter for writing JSON data.</returns>
        public TextWriter StartTextWriterValueScope(string contentType)
        {
            this.CommitUtf8JsonWriterContentsToBuffer();

            WriteItemWithSeparatorIfNeeded();

            this.currentTextWriterContentType = contentType;
            if (!IsWritingJson)
            {
                this.bufferWriter.Write(this.DoubleQuote.Slice(0, 1).Span);
            }

            this.Flush();

            this.textWriter = new ODataUtf8JsonTextWriter(this);

            return this.textWriter;
        }

        /// <summary>
        /// Ends a TextWriter value scope.
        /// </summary>
        public void EndTextWriterValueScope()
        {
            if (!IsWritingJson)
            {
                this.bufferWriter.Write(this.DoubleQuote.Slice(0, 1).Span);
                this.textWriter.Dispose();
                this.Flush();

                CheckIfSeparatorNeeded();
                CheckIfManualValueAtArrayStart();
            }
        }

        /// <summary>
        /// Asynchronously starts a TextWriter value scope with the specified content type.
        /// </summary>
        /// <param name="contentType">The content type of the TextWriter value scope.</param>
        /// <returns>A task representing the asynchronous operation. The task result is a TextWriter for writing JSON data.</returns>
        public async Task<TextWriter> StartTextWriterValueScopeAsync(string contentType)
        {
            await this.FlushAsync().ConfigureAwait(false);

            WriteItemWithSeparatorIfNeeded();

            this.currentTextWriterContentType = contentType;
            if (!IsWritingJson)
            {
                this.bufferWriter.Write(this.DoubleQuote.Slice(0, 1).Span);
            }

            await this.FlushAsync().ConfigureAwait(false);

            this.textWriter = new ODataUtf8JsonTextWriter(this);

            return this.textWriter;
        }

        /// <summary>
        /// Asynchronously ends a TextWriter value scope.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task EndTextWriterValueScopeAsync()
        {
            if (!IsWritingJson)
            {
                this.bufferWriter.Write(this.DoubleQuote.Slice(0, 1).Span);
                await this.textWriter.DisposeAsync();
                await this.FlushIfBufferThresholdReachedAsync().ConfigureAwait(false);

                CheckIfSeparatorNeeded();
                CheckIfManualValueAtArrayStart();
            }
        }

        /// <summary>
        /// Whether the current TextWriter is writing JSON
        /// </summary>
        /// <returns></returns>
        private bool IsWritingJson
        {
            get
            {
                return String.IsNullOrEmpty(this.currentTextWriterContentType) || this.currentTextWriterContentType.StartsWith(MimeConstants.MimeApplicationJson, StringComparison.Ordinal);
            }
        }

        /// <summary>
        /// Represents a TextWriter implementation for writing UTF-8 JSON data in an OData context.
        /// </summary>
        internal class ODataUtf8JsonTextWriter : TextWriter
        {
            ODataUtf8JsonWriter jsonWriter = null;
            private char[] buffer;
            int numOfCharsNotWrittenFromPreviousChunk = 0;

            public ODataUtf8JsonTextWriter(ODataUtf8JsonWriter jsonWriter)
            {
                this.jsonWriter = jsonWriter;
            }

            public override Encoding Encoding => throw new NotImplementedException();

            /// <summary>
            /// Flushes any buffered data to the underlying stream synchronously.
            /// </summary>
            public override void Flush()
            {
                // If there are unprocessed chars, encode and write them as the final block.
                ReadOnlySpan<char> charsNotProcessedFromPreviousChunk = this.buffer.AsSpan().Slice(numOfCharsNotWrittenFromPreviousChunk);

                if (!charsNotProcessedFromPreviousChunk.IsEmpty)
                {
                    this.jsonWriter.WriteStringChunk(charsNotProcessedFromPreviousChunk, true);

                    // set numOfCharsNotWrittenFromPreviousChunk to 0
                    this.numOfCharsNotWrittenFromPreviousChunk = 0;
                }

                this.jsonWriter.Flush();
            }

            /// <summary>
            /// Flushes any buffered data to the underlying stream asynchronously.
            /// </summary>
            /// <param name="cancellationToken">A cancellation token to observe while waiting for the flush operation to complete.</param>
            /// <returns>A task representing the asynchronous flush operation.</returns>
            public override async Task FlushAsync()
            {
                // If there are unprocessed chars, encode and write them as the final block.
                ReadOnlyMemory<char> charsNotProcessedFromPreviousChunk = this.buffer.AsMemory().Slice(0, numOfCharsNotWrittenFromPreviousChunk);

                if (!charsNotProcessedFromPreviousChunk.IsEmpty)
                {
                    this.jsonWriter.WriteStringChunk(charsNotProcessedFromPreviousChunk.Span, true);

                    // set numOfCharsNotWrittenFromPreviousChunk to 0
                    this.numOfCharsNotWrittenFromPreviousChunk = 0;
                }

                await this.jsonWriter.FlushAsync();
            }

            /// <summary>
            /// Disposes the object.
            /// </summary>
            /// <param name="disposing">True if called from Dispose; false if called form the finalizer.</param>
            protected override void Dispose(bool disposing)
            {
                this.Flush();

                if (this.buffer != null)
                {
                    ArrayPool<char>.Shared.Return(this.buffer);
                }

                base.Dispose(disposing);
            }

            /// <summary>
            /// Asynchronously disposes of the current object and performs cleanup operations.
            /// </summary>
            /// <returns>A <see cref="ValueTask"/> representing the asynchronous operation.</returns>
            public override async ValueTask DisposeAsync()
            {
                await this.FlushAsync();

                if (this.buffer != null)
                {
                    ArrayPool<char>.Shared.Return(this.buffer);
                }

                await this.jsonWriter.FlushAsync().ConfigureAwait(false);
            }

            /// <summary>
            /// Writes a specified number of characters from the given character array to the ODataUtf8JsonWriter.
            /// </summary>
            /// <param name="value">The character array from which to write characters.</param>
            /// <param name="index">The starting index in the character array from which to begin writing characters.</param>
            /// <param name="count">The number of characters to write from the character array.</param>
            public override void Write(char[] value, int index, int count)
            {
                ReadOnlySpan<char> charValuesToWrite = value.AsSpan().Slice(index, count);
                this.WritingCharValuesInChunks(charValuesToWrite);
            }

            /// <summary>
            /// Asynchronously writes a specified number of characters from the given character array to theODataUtf8JsonWriter.
            /// </summary>
            /// <param name="value">The character array from which to write characters.</param>
            /// <param name="index">The starting index in the character array from which to begin writing characters.</param>
            /// <param name="count">The number of characters to write from the character array.</param>
            /// <returns>A task representing the asynchronous write operation.</returns>
            public override async Task WriteAsync(char[] value, int index, int count)
            {
                ReadOnlyMemory<char> charValuesToWrite = value.AsMemory().Slice(index, count);
                await this.WritingCharValuesInChunksAsync(charValuesToWrite);
            }

            /// <summary>
            /// Writes the characters represented by the provided read-only span in chunks, processing them for escaping if necessary, and flushing the writer if the buffer threshold is reached.
            /// </summary>
            /// <param name="value">The read-only span containing the characters to be written.</param>
            private void WritingCharValuesInChunks(ReadOnlySpan<char> value)
            {
                // Process the input value in chunks
                for (int i = 0; i < value.Length; i += ODataUtf8JsonWriter.chunkSize)
                {
                    int remainingChars = Math.Min(ODataUtf8JsonWriter.chunkSize, value.Length - i);
                    bool isFinalBlock = false;
                    ReadOnlySpan<char> chunk = value.Slice(i, remainingChars);

                    // If the buffer is not empty, then we copy the chars from the buffer
                    // to the current chunk being processed.
                    if (numOfCharsNotWrittenFromPreviousChunk > 0)
                    {
                        // Get unprocessed chars from the buffer.
                        ReadOnlySpan<char> charsNotProcessedFromPreviousChunk = this.buffer.AsSpan().Slice(0, numOfCharsNotWrittenFromPreviousChunk);
                        int totalLength = charsNotProcessedFromPreviousChunk.Length + chunk.Length;

                        char[] combinedArray = ArrayPool<char>.Shared.Rent(totalLength);

                        // Copy chars from charsNotProcessedFromPreviousChunk to the combined array
                        charsNotProcessedFromPreviousChunk.CopyTo(combinedArray);

                        // Copy chars from chunk to the combined array, starting from the end of charsNotProcessedFromPreviousChunk
                        chunk.CopyTo(combinedArray.AsSpan().Slice(charsNotProcessedFromPreviousChunk.Length));

                        // Write the chunk.
                        this.jsonWriter.WriteStringChunk(combinedArray, isFinalBlock);

                        ArrayPool<char>.Shared.Return(combinedArray);
                    }
                    else
                    {
                        this.jsonWriter.WriteStringChunk(chunk, isFinalBlock);
                    }

                    // Flush the writer if the buffer threshold is reached
                    this.jsonWriter.FlushIfBufferThresholdReached();
                }
            }

            /// <summary>
            /// Writes character values represented by the provided read-only memory in chunks, processing them for escaping if necessary, and asynchronously flushes the writer if the buffer threshold is reached.
            /// </summary>
            /// <param name="value">The read-only memory containing the character values to be written.</param>
            /// <returns>A ValueTask representing the asynchronous operation.</returns>
            private async ValueTask WritingCharValuesInChunksAsync(ReadOnlyMemory<char> value)
            {
                // Process the input value in chunks
                for (int i = 0; i < value.Length; i += ODataUtf8JsonWriter.chunkSize)
                {
                    int remainingChars = Math.Min(ODataUtf8JsonWriter.chunkSize, value.Length - i);
                    bool isFinalBlock = false;
                    ReadOnlyMemory<char> chunk = value.Slice(i, remainingChars);

                    // If the buffer is not empty, then we copy the chars from the buffer
                    // to the current chunk being processed.
                    if (numOfCharsNotWrittenFromPreviousChunk > 0)
                    {
                        // Get unprocessed chars from the buffer.
                        ReadOnlyMemory<char> charsNotProcessedFromPreviousChunk = this.buffer.AsMemory().Slice(0, numOfCharsNotWrittenFromPreviousChunk);
                        int totalLength = charsNotProcessedFromPreviousChunk.Length + chunk.Length;

                        char[] combinedArray = ArrayPool<char>.Shared.Rent(totalLength);

                        // Copy chars from charsNotProcessedFromPreviousChunk to the combined array
                        charsNotProcessedFromPreviousChunk.CopyTo(combinedArray);

                        // Copy chars from chunk to the combined array, starting from the end of charsNotProcessedFromPreviousChunk
                        chunk.CopyTo(combinedArray.AsMemory().Slice(charsNotProcessedFromPreviousChunk.Length));

                        // Write the chunk.
                        this.jsonWriter.WriteStringChunk(combinedArray, isFinalBlock);

                        ArrayPool<char>.Shared.Return(combinedArray);
                    }
                    else
                    {
                        this.jsonWriter.WriteStringChunk(chunk.Span, isFinalBlock);
                    }

                    // Flush the writer if the buffer threshold is reached
                    await this.jsonWriter.FlushIfBufferThresholdReachedAsync().ConfigureAwait(false);
                }
            }
        }
    }
}
#endif