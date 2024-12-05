//---------------------------------------------------------------------
// <copyright file="ODataUtf8JsonWriter.TextWriter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Json
{
    using System;
    using System.Buffers;
    using System.IO;
    using System.Text;
    using System.Text.Unicode;
    using System.Threading.Tasks;

    internal sealed partial class ODataUtf8JsonWriter
    {
        /// <summary>
        /// Current textwriter for writing a stream property.
        /// </summary>
        private TextWriter textWriter = null;

        /// <summary>
        /// Flag indicating whether the current TextWriter is writing JSON.
        /// </summary>
        private bool isWritingJson;

        /// <summary>
        /// Content type of a value being written using TextWriter
        /// </summary>
        private string currentTextWriterContentType;

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
            this.isWritingJson = CheckIfWritingJson(this.currentTextWriterContentType);

            // We don't perform escaping when the content type is not JSON
            // since we assume the input to be already a valid JSON with all escaping handled by the caller.
            if (!this.isWritingJson)
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
            if (!this.isWritingJson)
            {
                this.bufferWriter.Write(this.DoubleQuote.Slice(0, 1).Span);
            }

            this.textWriter?.Dispose();
            this.Flush();

            CheckIfSeparatorNeeded();
            CheckIfManualValueAtArrayStart();
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

            this.isWritingJson = CheckIfWritingJson(this.currentTextWriterContentType);

            if (!this.isWritingJson)
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
            if (!this.isWritingJson)
            {
                this.bufferWriter.Write(this.DoubleQuote.Slice(0, 1).Span);
            }

            if (this.textWriter != null)
            {
                await this.textWriter.DisposeAsync();
            }

            await this.DrainBufferIfThresholdReachedAsync().ConfigureAwait(false);

            CheckIfSeparatorNeeded();
            CheckIfManualValueAtArrayStart();
        }

        /// <summary>
        /// Checks whether the current TextWriter is writing JSON
        /// </summary>
        /// <returns></returns>
        private static bool CheckIfWritingJson(string currentContentType)
        {
            return string.IsNullOrEmpty(currentContentType) || currentContentType.StartsWith(MimeConstants.MimeApplicationJson, StringComparison.Ordinal);
        }

        /// <summary>
        /// Represents a TextWriter implementation for writing UTF-8 JSON data to an ODataUtf8JsonWriter.
        /// </summary>
        internal sealed class ODataUtf8JsonTextWriter : TextWriter
        {
            private readonly ODataUtf8JsonWriter jsonWriter = null;
            // Buffer used to store chars that could not be encoded due to
            // insufficient data in the input buffer. The chars will be prepended
            // to the next chunk of input.
            private char[] buffer;
            // Number of chars in the buffer that are left over from the previous chunk.
            private int numOfCharsNotWrittenFromPreviousChunk = 0;
            // This buffer is used by Write(char) to store the char so
            // that we can re-use our Write(char[], ...) method.
            private char[] singleCharBuffer;

            public ODataUtf8JsonTextWriter(ODataUtf8JsonWriter jsonWriter)
            {
                this.jsonWriter = jsonWriter;
            }

            /// <summary>
            /// Gets the character encoding in which the output is written. This is not supported by this text writer.
            /// </summary>
            public override Encoding Encoding => throw new NotSupportedException();

            /// <summary>
            /// Flushes any buffered data to the underlying stream synchronously.
            /// </summary>
            public override void Flush()
            {
                this.jsonWriter.Flush();
            }

            /// <summary>
            /// Flushes any buffered data to the underlying stream asynchronously.
            /// </summary>
            /// <returns>A task representing the asynchronous flush operation.</returns>
            public override async Task FlushAsync()
            {
                await this.jsonWriter.FlushAsync();
            }

            /// <summary>
            /// Disposes the object.
            /// </summary>
            /// <param name="disposing">true if called from Dispose; false if called form the finalizer.</param>
            protected override void Dispose(bool disposing)
            {
                if (this.buffer != null)
                {
                    ArrayPool<char>.Shared.Return(this.buffer);
                }

                if (this.singleCharBuffer != null)
                {
                    ArrayPool<char>.Shared.Return(this.singleCharBuffer);
                }

                this.Flush();
                base.Dispose(disposing);
            }

            /// <summary>
            /// Asynchronously disposes of the current object and performs cleanup operations.
            /// </summary>
            /// <returns>A <see cref="ValueTask"/> representing the asynchronous operation.</returns>
            public override async ValueTask DisposeAsync()
            {
                if (this.buffer != null)
                {
                    ArrayPool<char>.Shared.Return(this.buffer);
                }

                if (this.singleCharBuffer != null)
                {
                    ArrayPool<char>.Shared.Return(this.singleCharBuffer);
                }

                await this.FlushAsync().ConfigureAwait(false);
            }

            /// <summary>
            /// Writes the specified character to the ODataUtf8JsonWriter.
            /// </summary>
            /// <param name="value">The character to write.</param>
            public override void Write(char value)
            {
                ReadOnlySpan<char> charToWrite = stackalloc char[1] { value };
                if (!this.jsonWriter.isWritingJson)
                {
                    this.WriteCharsInChunks(charToWrite);
                }
                else
                {
                    this.WriteCharsInChunksWithoutEscaping(charToWrite);
                }
            }

            /// <summary>
            /// Writes the specifid character to the ODataUtf8JsonWriter.
            /// </summary>
            /// <param name="value">The character to write.</param>
            public override async Task WriteAsync(char value)
            {
                this.singleCharBuffer ??= ArrayPool<char>.Shared.Rent(1);
                this.singleCharBuffer[0] = value;
                ReadOnlyMemory<char> input = this.singleCharBuffer.AsMemory().Slice(0, 1);

                if (!this.jsonWriter.isWritingJson)
                {
                    await this.WriteCharsInChunksAsync(input).ConfigureAwait(false);
                }
                else
                {
                    await this.WriteCharsInChunksWithoutEscapingAsync(input).ConfigureAwait(false);
                }
            }

            /// <summary>
            /// Writes a specified number of characters from the given character array to the ODataUtf8JsonWriter.
            /// </summary>
            /// <param name="value">The character array from which to write characters.</param>
            /// <param name="index">The starting index in the character array from which to begin writing characters.</param>
            /// <param name="count">The number of characters to write from the character array.</param>
            public override void Write(char[] value, int index, int count)
            {
                if (!this.jsonWriter.isWritingJson)
                {
                    ReadOnlySpan<char> charsToWrite = value.AsSpan().Slice(index, count);
                    this.WriteCharsInChunks(charsToWrite);
                }
                else
                {
                    ReadOnlySpan<char> charsToWrite = value.AsSpan().Slice(index, count);
                    this.WriteCharsInChunksWithoutEscaping(charsToWrite);
                }
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
                if (!this.jsonWriter.isWritingJson)
                {
                    ReadOnlyMemory<char> charsToWrite = value.AsMemory().Slice(index, count);
                    await this.WriteCharsInChunksAsync(charsToWrite).ConfigureAwait(false);
                }
                else
                {
                    ReadOnlyMemory<char> charsToWrite = value.AsMemory().Slice(index, count);
                    await this.WriteCharsInChunksWithoutEscapingAsync(charsToWrite).ConfigureAwait(false);
                }
            }

            /// <summary>
            /// Asynchronously writes the characters represented by the provided read-only span in chunks, processing them for escaping if necessary, and flushing the writer if the buffer threshold is reached.
            /// </summary>
            /// <param name="value">The read-only span containing the characters to be written.</param>
            private void WriteCharsInChunks(ReadOnlySpan<char> value)
            {
                // Process the input value in chunks
                for (int i = 0; i < value.Length; i += ODataUtf8JsonWriter.chunkSize)
                {
                    int remainingChars = Math.Min(ODataUtf8JsonWriter.chunkSize, value.Length - i);
                    bool isFinalBlock = false;
                    ReadOnlySpan<char> chunk = value.Slice(i, remainingChars);

                    // If the buffer is not empty, then we copy the chars from the buffer
                    // to the current chunk being processed.
                    if (this.numOfCharsNotWrittenFromPreviousChunk > 0)
                    {
                        // Get unprocessed chars from the buffer.
                        ReadOnlySpan<char> charsNotProcessedFromPreviousChunk = this.buffer.AsSpan().Slice(0, this.numOfCharsNotWrittenFromPreviousChunk);
                        int totalLength = charsNotProcessedFromPreviousChunk.Length + chunk.Length;

                        char[] combinedArray = ArrayPool<char>.Shared.Rent(totalLength);

                        // Copy chars from charsNotProcessedFromPreviousChunk to the combined array
                        charsNotProcessedFromPreviousChunk.CopyTo(combinedArray);

                        // Copy chars from chunk to the combined array, starting from the end of charsNotProcessedFromPreviousChunk
                        chunk.CopyTo(combinedArray.AsSpan().Slice(charsNotProcessedFromPreviousChunk.Length));
                        
                        int firstIndexToEscape = this.jsonWriter.NeedsEscaping(combinedArray);

                        // Write the chunk.
                        this.WriteChunk(combinedArray.AsSpan().Slice(0, totalLength), totalLength, firstIndexToEscape, isFinalBlock);

                        ArrayPool<char>.Shared.Return(combinedArray);
                    }
                    else
                    {
                        int firstIndexToEscape = this.jsonWriter.NeedsEscaping(chunk);
                        this.WriteChunk(chunk, chunk.Length, firstIndexToEscape, isFinalBlock);
                    }

                    // Flush the writer if the buffer threshold is reached
                    this.jsonWriter.DrainBufferIfThresholdReached();
                }
            }

            /// <summary>
            /// Writes character values represented by the provided read-only memory in chunks, processing them for escaping if necessary, and asynchronously flushes the writer if the buffer threshold is reached.
            /// </summary>
            /// <param name="value">The read-only memory containing the character values to be written.</param>
            /// <returns>A ValueTask representing the asynchronous operation.</returns>
            private async ValueTask WriteCharsInChunksAsync(ReadOnlyMemory<char> value)
            {
                // Process the input value in chunks
                for (int i = 0; i < value.Length; i += ODataUtf8JsonWriter.chunkSize)
                {
                    int remainingChars = Math.Min(ODataUtf8JsonWriter.chunkSize, value.Length - i);
                    bool isFinalBlock = false;
                    ReadOnlyMemory<char> chunk = value.Slice(i, remainingChars);

                    // If the buffer is not empty, then we copy the chars from the buffer
                    // to the current chunk being processed.
                    if (this.numOfCharsNotWrittenFromPreviousChunk > 0)
                    {
                        // Get unprocessed chars from the buffer.
                        ReadOnlyMemory<char> charsNotProcessedFromPreviousChunk = this.buffer.AsMemory().Slice(0, this.numOfCharsNotWrittenFromPreviousChunk);
                        int totalLength = charsNotProcessedFromPreviousChunk.Length + chunk.Length;

                        char[] combinedArray = ArrayPool<char>.Shared.Rent(totalLength);

                        // Copy chars from charsNotProcessedFromPreviousChunk to the combined array
                        charsNotProcessedFromPreviousChunk.CopyTo(combinedArray);

                        // Copy chars from chunk to the combined array, starting from the end of charsNotProcessedFromPreviousChunk
                        chunk.CopyTo(combinedArray.AsMemory().Slice(charsNotProcessedFromPreviousChunk.Length));

                        int firstIndexToEscape = this.jsonWriter.NeedsEscaping(combinedArray);

                        // Write the chunk.
                        this.WriteChunk(combinedArray.AsSpan().Slice(0, totalLength), totalLength, firstIndexToEscape, isFinalBlock);

                        ArrayPool<char>.Shared.Return(combinedArray);
                    }
                    else
                    {
                        int firstIndexToEscape = this.jsonWriter.NeedsEscaping(chunk.Span);

                        // Write the chunk.
                        this.WriteChunk(chunk.Span, chunk.Length, firstIndexToEscape, isFinalBlock);
                    }

                    // Flush the writer if the buffer threshold is reached
                    await this.jsonWriter.DrainBufferIfThresholdReachedAsync().ConfigureAwait(false);
                }
            }

            /// <summary>
            /// Writes the characters represented by the provided read-only span in chunks, processing them for escaping if necessary, and flushing the writer if the buffer threshold is reached.
            /// </summary>
            /// <param name="value">The read-only span containing the characters to be written.</param>
            private void WriteCharsInChunksWithoutEscaping(ReadOnlySpan<char> value)
            {
                // Process the input value in chunks
                for (int i = 0; i < value.Length; i += ODataUtf8JsonWriter.chunkSize)
                {
                    int remainingChars = Math.Min(ODataUtf8JsonWriter.chunkSize, value.Length - i);
                    bool isFinalBlock = false;
                    ReadOnlySpan<char> chunk = value.Slice(i, remainingChars);

                    // If the buffer is not empty, then we copy the chars from the buffer
                    // to the current chunk being processed.
                    if (this.numOfCharsNotWrittenFromPreviousChunk > 0)
                    {
                        // Get unprocessed chars from the buffer.
                        ReadOnlySpan<char> charsNotProcessedFromPreviousChunk = this.buffer.AsSpan().Slice(0, this.numOfCharsNotWrittenFromPreviousChunk);
                        int totalLength = charsNotProcessedFromPreviousChunk.Length + chunk.Length;

                        char[] combinedArray = ArrayPool<char>.Shared.Rent(totalLength);

                        // Copy chars from charsNotProcessedFromPreviousChunk to the combined array
                        charsNotProcessedFromPreviousChunk.CopyTo(combinedArray);

                        // Copy chars from chunk to the combined array, starting from the end of charsNotProcessedFromPreviousChunk
                        chunk.CopyTo(combinedArray.AsSpan().Slice(charsNotProcessedFromPreviousChunk.Length));

                        // Write the chunk.
                        this.WriteChunkWithoutEscaping(combinedArray.AsSpan().Slice(0, totalLength), totalLength, isFinalBlock);

                        ArrayPool<char>.Shared.Return(combinedArray, true);
                    }
                    else
                    {
                        this.WriteChunkWithoutEscaping(chunk, chunk.Length, isFinalBlock);
                    }

                    // Flush the writer if the buffer threshold is reached
                    this.jsonWriter.DrainBufferIfThresholdReached();
                }
            }

            /// <summary>
            /// Writes the characters represented by the provided read-only span in chunks, processing them for escaping if necessary, and flushing the writer if the buffer threshold is reached.
            /// </summary>
            /// <param name="value">The read-only span containing the characters to be written.</param>
            private async ValueTask WriteCharsInChunksWithoutEscapingAsync(ReadOnlyMemory<char> value)
            {
                // Process the input value in chunks
                for (int i = 0; i < value.Length; i += ODataUtf8JsonWriter.chunkSize)
                {
                    int remainingChars = Math.Min(ODataUtf8JsonWriter.chunkSize, value.Length - i);
                    bool isFinalBlock = false;
                    ReadOnlyMemory<char> chunk = value.Slice(i, remainingChars);

                    // If the buffer is not empty, then we copy the chars from the buffer
                    // to the current chunk being processed.
                    if (this.numOfCharsNotWrittenFromPreviousChunk > 0)
                    {
                        // Get unprocessed chars from the buffer.
                        ReadOnlyMemory<char> charsNotProcessedFromPreviousChunk = this.buffer.AsMemory().Slice(0, this.numOfCharsNotWrittenFromPreviousChunk);
                        int totalLength = charsNotProcessedFromPreviousChunk.Length + chunk.Length;

                        char[] combinedArray = ArrayPool<char>.Shared.Rent(totalLength);

                        // Copy chars from charsNotProcessedFromPreviousChunk to the combined array
                        charsNotProcessedFromPreviousChunk.CopyTo(combinedArray);

                        // Copy chars from chunk to the combined array, starting from the end of charsNotProcessedFromPreviousChunk
                        chunk.CopyTo(combinedArray.AsMemory().Slice(charsNotProcessedFromPreviousChunk.Length));

                        // Write the chunk.
                        this.WriteChunkWithoutEscaping(combinedArray.AsSpan().Slice(0, totalLength), totalLength, isFinalBlock);

                        ArrayPool<char>.Shared.Return(combinedArray, true);
                    }
                    else
                    {
                        this.WriteChunkWithoutEscaping(chunk.Span, chunk.Length, isFinalBlock);
                    }

                    // Flush the writer if the buffer threshold is reached
                    await this.jsonWriter.DrainBufferIfThresholdReachedAsync().ConfigureAwait(false);
                }
            }

            private void WriteChunk(ReadOnlySpan<char> chunk, int chunkLength, int firstIndexToEscape, bool isFinalBlock)
            {
                if (firstIndexToEscape != -1)
                {
                    this.jsonWriter.WriteEscapedStringChunk(chunk, firstIndexToEscape, isFinalBlock, out this.numOfCharsNotWrittenFromPreviousChunk);

                    if (this.numOfCharsNotWrittenFromPreviousChunk > 0)
                    {
                        if (this.buffer == null)
                        {
                            this.buffer = ArrayPool<char>.Shared.Rent(ODataUtf8JsonWriter.chunkSize);
                        }

                        // Update the buffer with unprocessed bytes from the current chunk.
                        chunk.Slice(chunkLength - this.numOfCharsNotWrittenFromPreviousChunk).CopyTo(this.buffer.AsSpan());
                    }
                }
                else
                {
                    this.jsonWriter.WriteStringChunk(chunk, isFinalBlock);
                }
            }

            private void WriteStringChunkWithoutEscaping(ReadOnlySpan<char> chunk, bool isFinalBlock, out int charsFromPrevChunkNotProcessed)
            {
                int maxUtf8Length = Encoding.UTF8.GetMaxByteCount(chunk.Length);

                Span<byte> output = this.jsonWriter.bufferWriter.GetSpan(maxUtf8Length);
                OperationStatus status = Utf8.FromUtf16(chunk, output, out int charsRead, out int charsWritten, isFinalBlock);

                charsFromPrevChunkNotProcessed = chunk.Length - charsRead;

                // notify the bufferWriter of the write
                this.jsonWriter.bufferWriter.Advance(charsWritten);
            }

            private void WriteChunkWithoutEscaping(ReadOnlySpan<char> chunk, int chunkLength, bool isFinalBlock)
            {
                this.WriteStringChunkWithoutEscaping(chunk, isFinalBlock, out this.numOfCharsNotWrittenFromPreviousChunk);

                if (this.numOfCharsNotWrittenFromPreviousChunk > 0)
                {
                    if (this.buffer == null)
                    {
                        this.buffer = ArrayPool<char>.Shared.Rent(ODataUtf8JsonWriter.chunkSize);
                    }

                    // Update the buffer with unprocessed bytes from the current chunk.
                    chunk.Slice(chunkLength - this.numOfCharsNotWrittenFromPreviousChunk).CopyTo(this.buffer.AsSpan());
                }
            }
        }
    }
}
