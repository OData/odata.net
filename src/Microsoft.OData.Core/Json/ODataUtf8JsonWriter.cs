//---------------------------------------------------------------------
// <copyright file="ODataUtf8JsonWriter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using System.Buffers;
using System.Runtime.CompilerServices;
using Microsoft.OData.Edm;
using System.Text.Unicode;
using System.Buffers.Text;
using System.Xml;

namespace Microsoft.OData.Json
{
    /// <summary>
    /// Implementation of <see cref="IJsonWriter"/> that is based on
    /// <see cref="Utf8JsonWriter"/>.
    /// </summary>
    internal sealed partial class ODataUtf8JsonWriter : IJsonWriter, IDisposable, IAsyncDisposable
    {
        private const int DefaultBufferSize = 16 * 1024;
        private readonly float bufferFlushThreshold;
        private readonly static ReadOnlyMemory<byte> parentheses = new byte[] { (byte)'(', (byte)')' };
        private readonly static ReadOnlyMemory<byte> itemSeparator = new byte[] { (byte)',' };
        private readonly static ReadOnlyMemory<byte> nanValue = new byte[] { (byte)'N', (byte)'a', (byte)'N' };
        private readonly static ReadOnlyMemory<byte> positiveInfinityValue = new byte[] { (byte)'I', (byte)'N', (byte)'F' };
        private readonly static ReadOnlyMemory<byte> negativeInfinityValue = new byte[] { (byte)'-', (byte)'I', (byte)'N', (byte)'F' };

        private readonly Stream outputStream;
        private readonly Stream writeStream;
        private readonly Utf8JsonWriter utf8JsonWriter;
        private readonly PooledByteBufferWriter bufferWriter;
        private readonly JavaScriptEncoder encoder;
        private readonly int bufferSize;
        private readonly bool isIeee754Compatible;
        private readonly bool leaveStreamOpen;
        private readonly Encoding outputEncoding;
        private bool disposed;
        private const int chunkSize = 2048;
        /// <summary>
        /// Represents the threshold value used for determining whether to use stackalloc for char arrays.
        /// </summary>
        private const int StackallocCharThreshold = 128;
        // In the worst case, an ASCII character represented as a single utf-8 byte could expand 6x when escaped.
        // For example: '+' becomes '\u0043'
        // Escaping surrogate pairs (represented by 3 or 4 utf-8 bytes) would expand to 12 bytes (which is still <= 6x).
        // The same factor applies to utf-16 characters.
        private const int MaxExpansionFactorWhileEscaping = 6;
        /// The Utf8JsonWriter internally keeps track of when to write the item separtor ','
        /// between key-value pairs in an object and between items in an array
        /// However, we bypass the Utf8JsonWriter in our implementation of WriteRawValue
        /// and write directly to the bufferWriter that was ppased to Utf8JsonWriter. This means that we have to manually
        /// write an item separator when writing raw values. And since the Utf8JsonWriter is not
        /// aware of this write, we have to manually keep track of state to synchronise with the Utf8JsonWriter
        /// to ensure we don't write extra separators or skip a separator where it's needed, both of
        /// which would result in invalid JSON.
        /// 
        /// In particular, we have to ensure we write a separator in the following scenarios:
        /// - inside a JSON object, before writing a property name that's preceded by a raw value
        /// - inside an array, before writing an item that's preceded by one or more consecutive raw values at the start of the array.
        /// We only need to do this for the first non-raw value after a sequence of raw values. That's because the Utf8JsonWriter
        /// (not aware of the raw values) will think that this is the first item in the array and not include a separator before it.
        /// - inside an array, before writing a raw value that is not at the start of the array
        /// 
        /// We write to the bufferWriter directly instead of the stream because it would be too expensive to write and flush
        /// the stream frequently.
        /// 
        /// The following variables are used to keep track of state that allows us to figure out where to manually place a separator
        /// This will not be necessary when we implement WriteRawValue using Utf8JsonWriter.WriteRawValue()
        /// see: https://github.com/OData/odata.net/issues/2420

        /// <summary>
        /// Whether we should manually write the item separator (,) before
        /// the next object or array entry.
        /// </summary>
        private bool shouldWriteSeparator = false;
        /// <summary>
        /// Stack used to keep track of scope transitions i.e,
        /// whether we're currently in an Object or an Array.
        /// </summary>
        private BitStack bitStack;
        /// <summary>
        /// Whether we're about to write the first element in array. This helps
        /// decide whether we should manually write a separator.
        /// </summary>
        private bool isWritingAtStartOfArray = false;
        /// <summary>
        /// Whether we're currently writing a raw value at the start of an array
        /// or preceeded by a sequence of consecutive raw values at the start of
        /// an array e.g. ["rawValue1", "rawValue2", "rawValue3"
        /// </summary>
        private bool isWritingConsecutiveRawValuesAtStartOfArray = false;
        /// <summary>
        /// Rrepresents a read-only memory block containing the byte representation of the double quote character ("). 
        /// </summary>
        private readonly ReadOnlyMemory<byte> DoubleQuote = new byte[] { (byte)'"' };

        /// <summary>
        /// Creates an instance of <see cref="ODataUtf8JsonWriter"/>.
        /// </summary>
        /// <param name="outputStream">The stream to write to.</param>
        /// <param name="isIeee754Compatible">When true, longs and decimals will be written out as strings.</param>
        /// <param name="encoding">The text encoding to write to.</param>
        /// <param name="bufferSize">The internal buffer size.</param>
        /// <param name="leaveStreamOpen">Whether to leave the <paramref name="outputStream"/> open when this writer gets disposed.</param>
        public ODataUtf8JsonWriter(Stream outputStream, bool isIeee754Compatible, Encoding encoding, JavaScriptEncoder encoder = null, int bufferSize = DefaultBufferSize, bool leaveStreamOpen = false)
        {
            Debug.Assert(outputStream != null, "outputStream != null");
            this.outputStream = outputStream;
            this.isIeee754Compatible = isIeee754Compatible;
            this.bufferSize = bufferSize;
            this.bufferWriter = new PooledByteBufferWriter(bufferSize);
            // flush when we're close to the buffer capacity to avoid allocating bigger buffers
            this.bufferFlushThreshold = 0.9f * this.bufferSize;
            this.leaveStreamOpen = leaveStreamOpen;
            this.outputEncoding = encoding;
            this.encoder = encoder ?? JavaScriptEncoder.UnsafeRelaxedJsonEscaping;

            if (this.outputEncoding.CodePage == Encoding.UTF8.CodePage)
            {
                this.writeStream = this.outputStream;
            }
            else
            {
                this.writeStream = new TranscodingWriteStream(
                    this.outputStream,
                    outputEncoding,
                    Encoding.UTF8,
                    leaveOpen: true); // leave open because we manually dispose the output stream if necessary
            }

            this.utf8JsonWriter = new Utf8JsonWriter(
                this.bufferWriter,
                new JsonWriterOptions
                {
                    // we don't need to perform validation here since the higher-level
                    // writers already perform validation
                    SkipValidation = true,
                    Encoder = this.encoder
                });
        }

        public void Flush()
        {
            this.WriteToStream();
            this.writeStream.Flush();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void DrainBufferIfThresholdReached()
        {
            if ((this.utf8JsonWriter.BytesPending + this.bufferWriter.WrittenCount) >= this.bufferFlushThreshold)
            {
                this.WriteToStream();
            }
        }

        /// <summary>
        /// If buffer threshold is reached, writes the buffered data
        /// to the stream and clears the buffer.
        /// </summary>
        private void WriteToStream()
        {
            this.CommitUtf8JsonWriterContentsToBuffer();
            this.writeStream.Write(this.bufferWriter.WrittenMemory.Span);
            this.bufferWriter.Clear();
        }

        /// <summary>
        /// Commits the contents of the <see cref="Utf8JsonWriter"/> to the
        /// bufferWriter. This should be called before writing directly
        /// to the bufferWriter.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void CommitUtf8JsonWriterContentsToBuffer()
        {
            this.utf8JsonWriter.Flush();
        }

        public void StartPaddingFunctionScope()
        {
            this.CommitUtf8JsonWriterContentsToBuffer();
            this.bufferWriter.Write(parentheses[..1].Span);
            this.DrainBufferIfThresholdReached();
        }

        public void WritePaddingFunctionName(string functionName)
        {
            this.CommitUtf8JsonWriterContentsToBuffer();
            this.bufferWriter.Write(Encoding.UTF8.GetBytes(functionName));
            this.DrainBufferIfThresholdReached();
        }

        public void EndPaddingFunctionScope()
        {
            this.CommitUtf8JsonWriterContentsToBuffer();
            this.bufferWriter.Write(parentheses[1..2].Span);
            this.DrainBufferIfThresholdReached();
        }

        public void StartObjectScope()
        {
            this.WriteSeparatorIfNecessary();
            this.EnterObjectScope();
            this.utf8JsonWriter.WriteStartObject();
            this.DrainBufferIfThresholdReached();
        }

        public void EndObjectScope()
        {
            this.utf8JsonWriter.WriteEndObject();
            this.ExitScope();
            this.DrainBufferIfThresholdReached();
        }

        public void StartArrayScope()
        {
            this.WriteSeparatorIfNecessary();
            this.EnterArrayScope();
            this.utf8JsonWriter.WriteStartArray();
            this.DrainBufferIfThresholdReached();
        }

        public void EndArrayScope()
        {
            this.utf8JsonWriter.WriteEndArray();
            this.ExitScope();
            this.DrainBufferIfThresholdReached();
        }

        public void WriteName(string name)
        {
            this.WriteSeparatorIfNecessary();
            this.utf8JsonWriter.WritePropertyName(name);
            this.DrainBufferIfThresholdReached();
        }

        public void WriteValue(bool value)
        {
            this.WriteSeparatorIfNecessary();
            this.utf8JsonWriter.WriteBooleanValue(value);
            this.DrainBufferIfThresholdReached();
        }

        public void WriteValue(int value)
        {
            this.WriteSeparatorIfNecessary();
            this.utf8JsonWriter.WriteNumberValue(value);
            this.DrainBufferIfThresholdReached();
        }

        public void WriteValue(float value)
        {
            this.WriteSeparatorIfNecessary();
            if (float.IsNaN(value))
            {
                this.utf8JsonWriter.WriteStringValue(nanValue.Span);
            }
            else if (float.IsPositiveInfinity(value))
            {
                this.utf8JsonWriter.WriteStringValue(positiveInfinityValue.Span);
            }
            else if (float.IsNegativeInfinity(value))
            {
                this.utf8JsonWriter.WriteStringValue(negativeInfinityValue.Span);
            }
            else
            {
                this.utf8JsonWriter.WriteNumberValue(value);
            }

            this.DrainBufferIfThresholdReached();
        }

        public void WriteValue(short value)
        {
            this.WriteSeparatorIfNecessary();
            this.utf8JsonWriter.WriteNumberValue(value);
            this.DrainBufferIfThresholdReached();
        }

        public void WriteValue(long value)
        {
            this.WriteSeparatorIfNecessary();
            if (this.isIeee754Compatible)
            {
                this.utf8JsonWriter.WriteStringValue(value.ToString(CultureInfo.InvariantCulture));
            }
            else
            {
                this.utf8JsonWriter.WriteNumberValue(value);
            }

            this.DrainBufferIfThresholdReached();
        }

        public void WriteValue(double value)
        {
            this.WriteSeparatorIfNecessary();
            if (double.IsNaN(value))
            {
                this.utf8JsonWriter.WriteStringValue(nanValue.Span);
            }
            else if (double.IsPositiveInfinity(value))
            {
                this.utf8JsonWriter.WriteStringValue(positiveInfinityValue.Span);
            }
            else if (double.IsNegativeInfinity(value))
            {
                this.utf8JsonWriter.WriteStringValue(negativeInfinityValue.Span);
            }
            else
            {
                // whereas double.MinValue and double.MaxValue have 16 digits scale. Hence we need
                // to use XmlConvert in all other cases, except infinity
                string valueToWrite = XmlConvert.ToString(value);

                // We write doubles like 100 as 100.0 (with .0 decimal point).
                // We do this to retain consistency with the legacy JsonWriter and with older
                // versions. However, it's not a hard requirement. Clients should not rely on the .0
                // to decide whether a value is an integer or double. We should consider
                // dropping this in the future (potentially behind a compatibility flag) to avoid
                // the cost of converting to a string and checking whether a decimal point is needed.
                bool needsFractionalPart = valueToWrite.IndexOfAny(JsonValueUtils.DoubleIndicatingCharacters) == -1;
                this.utf8JsonWriter.WriteRawValue(needsFractionalPart ? $"{valueToWrite}.0" : valueToWrite, skipInputValidation: true);
            }

            this.DrainBufferIfThresholdReached();
        }

        public void WriteValue(Guid value)
        {
            this.WriteSeparatorIfNecessary();
            this.utf8JsonWriter.WriteStringValue(value);
            this.DrainBufferIfThresholdReached();
        }

        public void WriteValue(decimal value)
        {
            this.WriteSeparatorIfNecessary();
            if (this.isIeee754Compatible)
            {
                this.utf8JsonWriter.WriteStringValue(value.ToString(CultureInfo.InvariantCulture));
            }
            else
            {
                this.utf8JsonWriter.WriteNumberValue(value);
            }

            this.DrainBufferIfThresholdReached();
        }

        public void WriteValue(DateTimeOffset value)
        {
            this.WriteSeparatorIfNecessary();
            if (value.Offset == TimeSpan.Zero)
            {
                // The default JsonWriter uses the format yyyy-MM-ddThh:mm:ss.fffffffZ when the offset is 0
                // Utf8JsonWriter uses the format yyyy-MM-ddThh:mm:ss.fffffff+00:00
                // While both formats are valid IS0 8601, we decided to keep the output
                // consistent between the two writers to avoid breaking any client that may
                // have dependency on the original format.
                string formattedValue = JsonValueUtils.FormatDateTimeOffset(value, ODataJsonDateTimeFormat.ISO8601DateTime);
                this.utf8JsonWriter.WriteStringValue(formattedValue);
            }
            else
            {
                this.utf8JsonWriter.WriteStringValue(value);
            }

            this.DrainBufferIfThresholdReached();
        }

        public void WriteValue(TimeSpan value)
        {
            this.WriteSeparatorIfNecessary();
            string stringValue = EdmValueWriter.DurationAsXml(value);
            this.utf8JsonWriter.WriteStringValue(stringValue);
            this.DrainBufferIfThresholdReached();
        }

        public void WriteValue(Date value)
        {
            this.WriteSeparatorIfNecessary();
            this.utf8JsonWriter.WriteStringValue(value.ToString());
            this.DrainBufferIfThresholdReached();
        }

        public void WriteValue(TimeOfDay value)
        {
            this.WriteSeparatorIfNecessary();
            this.utf8JsonWriter.WriteStringValue(value.ToString());
            this.DrainBufferIfThresholdReached();
        }

        public void WriteValue(byte value)
        {
            this.WriteSeparatorIfNecessary();
            this.utf8JsonWriter.WriteNumberValue(value);
            this.DrainBufferIfThresholdReached();
        }

        public void WriteValue(sbyte value)
        {
            this.WriteSeparatorIfNecessary();
            this.utf8JsonWriter.WriteNumberValue(value);
            this.DrainBufferIfThresholdReached();
        }

        public void WriteValue(string value)
        {
            if (value == null)
            {
                this.WriteSeparatorIfNecessary();
                this.utf8JsonWriter.WriteNullValue();
            }
            else if (value.Length > bufferWriter.FreeCapacity || value.Length > chunkSize)
            {
                WriteStringValueInChunks(value.AsSpan());
            }
            else
            {
                this.WriteSeparatorIfNecessary();
                this.utf8JsonWriter.WriteStringValue(value);
            }

            this.DrainBufferIfThresholdReached();
        }

        /// <summary>
        /// Writes a string value into the buffer in chunks, handling escaping if necessary.
        /// </summary>
        /// <param name="value">The string value to write.</param>
        /// <remarks>
        /// This method writes the provided string value into the buffer in manageable chunks to avoid
        /// excessive memory allocations and buffer overflows. If the string requires escaping, it is
        /// processed accordingly to ensure correct JSON formatting.
        /// </remarks>
        private void WriteStringValueInChunks(ReadOnlySpan<char> value)
        {
            this.CommitUtf8JsonWriterContentsToBuffer();

            WriteItemWithSeparatorIfNeeded();

            this.bufferWriter.Write(this.DoubleQuote.Slice(0, 1).Span);

            this.WriteToStream();

            int charsNotProcessedFromPreviousChunk = 0;

            for (int i = 0; i < value.Length; i += chunkSize)
            {
                int remainingChars = Math.Min(chunkSize, value.Length - i);
                bool isFinalBlock = (i + remainingChars) == value.Length;
                ReadOnlySpan<char> chunk = value.Slice(i - charsNotProcessedFromPreviousChunk, charsNotProcessedFromPreviousChunk + remainingChars);
                int firstIndexToEscape = NeedsEscaping(chunk);

                Debug.Assert(firstIndexToEscape >= -1 && firstIndexToEscape < value.Length);

                if (firstIndexToEscape != -1)
                {
                    WriteEscapedStringChunk(chunk, firstIndexToEscape, isFinalBlock, out charsNotProcessedFromPreviousChunk);
                }
                else
                {
                    WriteStringChunk(chunk, isFinalBlock);
                }

                // Flush the buffer if needed
                this.DrainBufferIfThresholdReached();
            }

            this.bufferWriter.Write(this.DoubleQuote.Slice(0, 1).Span);

            // since we bypass the Utf8JsonWriter, we need to signal to other
            // Write methods that a separator should be written first
            CheckIfSeparatorNeeded();

            CheckIfManualValueAtArrayStart();
        }

        /// <summary>
        /// Determines if any characters in the provided string span require escaping according to the specified encoder.
        /// </summary>
        /// <param name="value">The string span to analyze for characters needing escaping.</param>
        /// <returns>
        /// The index of the first character requiring escaping, or -1 if no characters need escaping or if the string span is empty.
        /// </returns>
        private unsafe int NeedsEscaping(ReadOnlySpan<char> value)
        {
            // Some implementations of JavaScriptEncoder.FindFirstCharacterToEncode may not accept
            // null pointers and guard against that. Hence, check up-front to return -1.
            if (value.IsEmpty)
            {
                return -1;
            }

            fixed (char* ptr = value)
            {
                return this.encoder.FindFirstCharacterToEncode(ptr, value.Length);
            }
        }

        /// <summary>
        /// Writes an escaped string chunk into the buffer.
        /// </summary>
        /// <param name="sourceChunk">The source chunk of the string to be escaped.</param>
        /// <param name="firstIndexToEscape">The index of the first character that requires escaping.</param>
        /// <param name="isFinalBlock">Indicates whether the chunk is the final block of the string.</param>
        /// <remarks>
        /// This method escapes characters within the provided string chunk that require escaping
        /// according to the specified encoder. It allocates memory for the escaped string and handles
        /// buffer management using either stack allocation or array pooling. Once the string is escaped,
        /// it is written into the buffer. If the encoding operation fails, an exception is thrown. 
        /// </remarks>
        private void WriteEscapedStringChunk(ReadOnlySpan<char> sourceChunk, int firstIndexToEscape, bool isFinalBlock, out int preChunkNotProcessed)
        {
            char[] valueArray = null;

            int maxSize = firstIndexToEscape + MaxExpansionFactorWhileEscaping * (sourceChunk.Length - firstIndexToEscape);

            Span<char> destination = maxSize <= StackallocCharThreshold
                ? stackalloc char[maxSize]
                : valueArray = ArrayPool<char>.Shared.Rent(maxSize);

            preChunkNotProcessed = 0;

            OperationStatus status = this.encoder.Encode(
                   sourceChunk,
                   destination,
                   out int charsConsumed,
                   out int charsWritten,
                   isFinalBlock
               );

            Debug.Assert(status == OperationStatus.Done || status == OperationStatus.NeedMoreData);

            preChunkNotProcessed = sourceChunk.Length - charsConsumed;
            WriteStringChunk(destination.Slice(0, charsWritten), isFinalBlock);

            if (valueArray != null)
            {
                ArrayPool<char>.Shared.Return(valueArray);
            }
        }

        /// <summary>
        /// Writes a chunk of the string into the buffer, converting it to UTF-8 encoding.
        /// </summary>
        /// <param name="chunk">The chunk of the string to be written into the buffer.</param>
        /// <param name="isFinalBlock">Indicates whether the chunk is the final block of the string.</param>
        /// <remarks>
        /// This method converts the provided chunk of the string from UTF-16 encoding to UTF-8 encoding
        /// and writes it into the buffer. It ensures that the buffer has sufficient space for the encoded
        /// string and notifies the buffer writer of the write operation. This method assumes that the UTF-8
        /// encoding does not exceed the maximum byte count specified by the UTF-8 encoding for the given
        /// chunk of the string.
        /// </remarks>
        private void WriteStringChunk(ReadOnlySpan<char> chunk, bool isFinalBlock)
        {
            int maxUtf8Length = Encoding.UTF8.GetMaxByteCount(chunk.Length);

            Span<byte> output = bufferWriter.GetSpan(maxUtf8Length);
            OperationStatus status = Utf8.FromUtf16(chunk, output, out int charsRead, out int charsWritten, isFinalBlock);
            Debug.Assert(status == OperationStatus.Done || status == OperationStatus.NeedMoreData);

            // The charsRead will always be equal to chunk.Length. This is because the characters
            // that would cause utf-8 encoding to result in partial processing is already 
            // taken care of by the JavascriptEncoder when escaping special characters. 
            Debug.Assert(charsRead == chunk.Length);

            // notify the bufferWriter of the write
            bufferWriter.Advance(charsWritten);
        }

        /// <summary>
        /// Determines whether a separator should be written before the next value.
        /// </summary>
        /// <remarks>
        /// This method checks the current writing context to decide whether a separator should be written
        /// before the next value in the JSON output. It sets the flag indicating whether a separator should
        /// be written based on conditions such as whether the writer is at the start of an array, writing
        /// consecutive raw values at the start of an array, or if the writer is not currently in an array.
        /// </remarks>
        private void CheckIfSeparatorNeeded()
        {
            if (this.isWritingAtStartOfArray || this.isWritingConsecutiveRawValuesAtStartOfArray || !this.IsInArray())
            {
                this.shouldWriteSeparator = true;
            }
        }

        public void WriteValue(byte[] value)
        {
            if (value == null)
            {
                this.WriteSeparatorIfNecessary();
                this.utf8JsonWriter.WriteNullValue();
            }
            else if (value.Length > bufferWriter.FreeCapacity || value.Length > chunkSize)
            {
                WriteByteValueInChunks(value.AsSpan());
            }
            else
            {
                this.WriteSeparatorIfNecessary();
                this.utf8JsonWriter.WriteBase64StringValue(value);
            }

            this.DrainBufferIfThresholdReached();
        }

        /// <summary>
        /// Writes the byte value represented by the provided ReadOnlySpan in chunks using Base64 encoding.
        /// </summary>
        /// <param name="value">The ReadOnlySpan containing the byte value to be written.</param>
        private void WriteByteValueInChunks(ReadOnlySpan<byte> value)
        {
            this.CommitUtf8JsonWriterContentsToBuffer();

            WriteItemWithSeparatorIfNeeded();

            this.bufferWriter.Write(this.DoubleQuote.Slice(0, 1).Span);

            this.WriteToStream();

            int bytesNotWrittenFromPreviousChunk = 0;

            for (int i = 0; i < value.Length; i += chunkSize)
            {
                int remainingChars = Math.Min(chunkSize, value.Length - i);
                bool isFinalBlock = (i + remainingChars) == value.Length;

                ReadOnlySpan<byte> chunk = value.Slice(i - bytesNotWrittenFromPreviousChunk, remainingChars + bytesNotWrittenFromPreviousChunk);

                Base64EncodeAndWriteChunk(chunk, isFinalBlock, out bytesNotWrittenFromPreviousChunk);

                this.DrainBufferIfThresholdReached();
            }

            this.bufferWriter.Write(this.DoubleQuote.Slice(0, 1).Span);

            // since we bypass the Utf8JsonWriter, we need to signal to other
            // Write methods that a separator should be written first
            CheckIfSeparatorNeeded();

            CheckIfManualValueAtArrayStart();
        }

        /// <summary>
        /// Encodes the specified chunk of bytes using Base64 encoding and writes the encoded data to the buffer writer.
        /// </summary>
        /// <param name="chunk">The chunk of bytes to be encoded.</param>
        /// <param name="isFinalBlock">A boolean value indicating whether the chunk is the final block of data.</param>
        /// <param name="bytesNotConsumed">An out parameter indicating the number of bytes from the previous chunk that were not processed.</param>
        private void Base64EncodeAndWriteChunk(ReadOnlySpan<byte> chunk, bool isFinalBlock, out int bytesNotConsumed)
        {
            int encodingLength = Base64.GetMaxEncodedToUtf8Length(chunk.Length);
            var output = this.bufferWriter.GetSpan(encodingLength);
            bytesNotConsumed = 0;

            OperationStatus status = Base64.EncodeToUtf8(chunk, output, out int consumed, out int written, isFinalBlock);

            Debug.Assert(status == OperationStatus.Done || status == OperationStatus.NeedMoreData);

            bytesNotConsumed = chunk.Length - consumed;

            this.bufferWriter.Advance(written);
        }

        public void WriteValue(JsonElement value)
        {
            this.WriteSeparatorIfNecessary();
            value.WriteTo(this.utf8JsonWriter);
            this.DrainBufferIfThresholdReached();
        }

        public void WriteRawValue(string rawValue)
        {
            this.WriteRawValueCore(rawValue);
            this.DrainBufferIfThresholdReached();
        }

        /// <summary>
        /// Implementation of WriteRawValue logic, used by both <see cref="WriteRawValue(string)"/>
        /// and <see cref="WriteRawValueAsync(string)"/>.
        /// </summary>
        /// <param name="rawValue">The value to write.</param>
        private void WriteRawValueCore(string rawValue)
        {
            if (rawValue == null)
            {
                return;
            }

            // We transcode and write the raw value directly to the buffer writer
            // because Utf8JsonWriter.WriteRawValue is not available in .NET Core 3.1 and .NET Standard 2.0
            // Writing the value manually means we also have to manually keep track of whether the separator
            // should be written because Utf8JsonWriter is not aware of this write.
            // Consider using Utf8JsonWriter.WriteRawValue() in .NET 6+
            // see: https://github.com/OData/odata.net/issues/2420

            // ensure we don't write to the buffer directly while there are still pending data in the Utf8JsonWriter buffer
            this.CommitUtf8JsonWriterContentsToBuffer();
            if (IsInArray() && !isWritingAtStartOfArray)
            {
                // Place a separator before the raw value if
                // this is an array, unless this is the first item in the array.
                this.bufferWriter.Write(itemSeparator.Slice(0, 1).Span);
            }

            this.bufferWriter.Write(Encoding.UTF8.GetBytes(rawValue));

            // since we bypass the Utf8JsonWriter, we need to signal to other
            // Write methods that a separator should be written first
            CheckIfSeparatorNeeded();

            CheckIfManualValueAtArrayStart();
        }

        /// <summary>
        /// Writes an item with a separator if needed, based on the context of being within an array and not writing at the start of the array.
        /// </summary>
        /// <remarks>
        /// If the method is called within an array and it's not the first item being written, it places a separator before the value being written.
        /// </remarks>
        private void WriteItemWithSeparatorIfNeeded()
        {
            if (this.IsInArray() && !isWritingAtStartOfArray)
            {
                // Place a separator before the raw value if
                // this is an array, unless this is the first item in the array.
                this.bufferWriter.Write(itemSeparator.Slice(0, 1).Span);
            }
        }

        // <summary>
        /// Checks if the writer is at the start of a manual value array and updates the state accordingly.
        /// </summary>
        private void CheckIfManualValueAtArrayStart()
        {
            if(this.isWritingAtStartOfArray || this.isWritingConsecutiveRawValuesAtStartOfArray)
            {
                this.isWritingConsecutiveRawValuesAtStartOfArray = true;
            }

            this.isWritingAtStartOfArray = false;
        }

        /// <summary>
        /// Manually writes an item separator ','
        /// to the output if necessary, bypassing the Utf8JsonWriter.
        /// Should be used before property names in an object or values
        /// in an array.
        /// This method should not be called by <see cref="WriteRawValue(string)"/>.
        /// </summary>
        private void WriteSeparatorIfNecessary()
        {
            if (this.shouldWriteSeparator)
            {
                this.WriteItemSeparator();
            }

            // reset state since now we're back in sync with Utf8JsonWriter.
            this.shouldWriteSeparator = false;
            this.isWritingAtStartOfArray = false;
            this.isWritingConsecutiveRawValuesAtStartOfArray = false;
        }

        /// <summary>
        /// Manually writes an item separator ',' to the output, bypassing the Utf8JsonWriter.
        /// </summary>
        private void WriteItemSeparator()
        {
            this.CommitUtf8JsonWriterContentsToBuffer();
            Span<byte> buf = this.bufferWriter.GetSpan(1);
            buf[0] = itemSeparator.Span[0];
            this.bufferWriter.Advance(1);
        }

        /// <summary>
        /// Mark that the writer has transitioned to write an object.
        /// </summary>
        private void EnterObjectScope()
        {
            this.isWritingConsecutiveRawValuesAtStartOfArray = false;
            this.isWritingAtStartOfArray = false;
            this.bitStack.PushTrue();
        }

        /// <summary>
        /// Mark that the writer has transitioned to write an array.
        /// </summary>
        private void EnterArrayScope()
        {
            this.isWritingConsecutiveRawValuesAtStartOfArray = false;
            this.isWritingAtStartOfArray = true;
            this.bitStack.PushFalse();
        }

        /// <summary>
        /// Exit the current object or array scope.
        /// </summary>
        private void ExitScope()
        {
            this.isWritingAtStartOfArray = false;
            this.isWritingConsecutiveRawValuesAtStartOfArray = false;
            this.shouldWriteSeparator = false;
            this.bitStack.Pop();
        }

        /// <summary>
        /// Checks whether the writer is currently in an array scope.
        /// </summary>
        /// <returns>true if the writer is currently in an array scope; otherwise false.</returns>
        private bool IsInArray()
        {
            if (!TryPeekScope(out bool isInObject))
            {
                return false;
            }

            return !isInObject;
        }

        /// <summary>
        /// Tries to get the current scope.
        /// </summary>
        /// <param name="isInObject">
        /// <returns>True if the current scope was successfully determined, false if it was not possible to determine the scope
        /// (i.e. we're neither in an array or object).
        /// </returns>
        private bool TryPeekScope(out bool isInObject)
        {
            isInObject = false;
            if (this.bitStack.CurrentDepth == 0)
            {
                return false;
            }

            // BitStack doesn't implement a Peek()
            // method, so we pop then push back.
            isInObject = this.bitStack.Pop();
            if (isInObject)
            {
                this.bitStack.PushTrue();
            }
            else
            {
                this.bitStack.PushFalse();
            }

            return true;
        }

        private void Dispose(bool disposing)
        {
            if (this.disposed)
            {
                return;
            }

            if (disposing)
            {
                this.writeStream.Flush();
                this.utf8JsonWriter.Dispose();
                this.bufferWriter.Dispose();

                if (this.outputStream != this.writeStream)
                {
                    this.writeStream.Dispose();
                }

                if (!this.leaveStreamOpen)
                {
                    this.outputStream.Dispose();
                }
            }

            this.disposed = true;
        }

        public void Dispose()
        {
            this.Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        #region "Asynchronous API"

        public async Task StartPaddingFunctionScopeAsync()
        {
            this.CommitUtf8JsonWriterContentsToBuffer();
            this.bufferWriter.Write(parentheses[..1].Span);
            await this.DrainBufferIfThresholdReachedAsync().ConfigureAwait(false);
        }

        public async Task WritePaddingFunctionNameAsync(string functionName)
        {
            this.CommitUtf8JsonWriterContentsToBuffer();
            this.bufferWriter.Write(Encoding.UTF8.GetBytes(functionName));
            await this.DrainBufferIfThresholdReachedAsync().ConfigureAwait(false);
        }

        public async Task EndPaddingFunctionScopeAsync()
        {
            this.CommitUtf8JsonWriterContentsToBuffer();
            this.bufferWriter.Write(parentheses[1..2].Span);
            await this.DrainBufferIfThresholdReachedAsync().ConfigureAwait(false);
        }

        public async Task StartObjectScopeAsync()
        {
            this.WriteSeparatorIfNecessary();
            this.EnterObjectScope();
            this.utf8JsonWriter.WriteStartObject();
            await this.DrainBufferIfThresholdReachedAsync().ConfigureAwait(false);
        }

        public async Task EndObjectScopeAsync()
        {
            this.utf8JsonWriter.WriteEndObject();
            this.ExitScope();
            await this.DrainBufferIfThresholdReachedAsync().ConfigureAwait(false);
        }

        public async Task StartArrayScopeAsync()
        {
            this.WriteSeparatorIfNecessary();
            this.EnterArrayScope();
            this.utf8JsonWriter.WriteStartArray();
            await this.DrainBufferIfThresholdReachedAsync().ConfigureAwait(false);
        }

        public async Task EndArrayScopeAsync()
        {
            this.utf8JsonWriter.WriteEndArray();
            this.ExitScope();
            await this.DrainBufferIfThresholdReachedAsync().ConfigureAwait(false);
        }

        public async Task WriteNameAsync(string name)
        {
            this.WriteSeparatorIfNecessary();
            this.utf8JsonWriter.WritePropertyName(name);
            await this.DrainBufferIfThresholdReachedAsync().ConfigureAwait(false);
        }

        public async Task WriteValueAsync(bool value)
        {
            this.WriteSeparatorIfNecessary();
            this.utf8JsonWriter.WriteBooleanValue(value);
            await this.DrainBufferIfThresholdReachedAsync().ConfigureAwait(false);
        }

        public async Task WriteValueAsync(int value)
        {
            this.WriteSeparatorIfNecessary();
            this.utf8JsonWriter.WriteNumberValue(value);
            await this.DrainBufferIfThresholdReachedAsync().ConfigureAwait(false);
        }

        public async Task WriteValueAsync(float value)
        {
            this.WriteSeparatorIfNecessary();
            if (float.IsNaN(value))
            {
                this.utf8JsonWriter.WriteStringValue(nanValue.Span);
            }
            else if (float.IsPositiveInfinity(value))
            {
                this.utf8JsonWriter.WriteStringValue(positiveInfinityValue.Span);
            }
            else if (float.IsNegativeInfinity(value))
            {
                this.utf8JsonWriter.WriteStringValue(negativeInfinityValue.Span);
            }
            else
            {
                this.utf8JsonWriter.WriteNumberValue(value);
            }

            await this.DrainBufferIfThresholdReachedAsync().ConfigureAwait(false);
        }

        public async Task WriteValueAsync(short value)
        {
            this.WriteSeparatorIfNecessary();
            this.utf8JsonWriter.WriteNumberValue(value);
            await this.DrainBufferIfThresholdReachedAsync().ConfigureAwait(false);
        }

        public async Task WriteValueAsync(long value)
        {
            this.WriteSeparatorIfNecessary();
            if (this.isIeee754Compatible)
            {
                this.utf8JsonWriter.WriteStringValue(value.ToString(CultureInfo.InvariantCulture));
            }
            else
            {
                this.utf8JsonWriter.WriteNumberValue(value);
            }

            await this.DrainBufferIfThresholdReachedAsync().ConfigureAwait(false);
        }

        public async Task WriteValueAsync(double value)
        {
            this.WriteSeparatorIfNecessary();
            if (double.IsNaN(value))
            {
                this.utf8JsonWriter.WriteStringValue(nanValue.Span);
            }
            else if (double.IsPositiveInfinity(value))
            {
                this.utf8JsonWriter.WriteStringValue(positiveInfinityValue.Span);
            }
            else if (double.IsNegativeInfinity(value))
            {
                this.utf8JsonWriter.WriteStringValue(negativeInfinityValue.Span);
            }
            else
            {
                // whereas double.MinValue and double.MaxValue have 16 digits scale. Hence we need
                // to use XmlConvert in all other cases, except infinity
                string valueToWrite = XmlConvert.ToString(value);

                // We write doubles like 100 as 100.0 (with .0 decimal point).
                // We do this to retain consistency with the legacy JsonWriter and with older
                // versions. However, it's not a hard requirement. Clients should not rely on the .0
                // to decide whether a value is an integer or double. We should consider
                // dropping this in the future (potentially behind a compatibility flag) to avoid
                // the cost of converting to a string and checking whether a decimal point is needed.
                bool needsFractionalPart = valueToWrite.IndexOfAny(JsonValueUtils.DoubleIndicatingCharacters) == -1;
                this.utf8JsonWriter.WriteRawValue(needsFractionalPart ? $"{valueToWrite}.0" : valueToWrite, skipInputValidation: true);
            }

            await this.DrainBufferIfThresholdReachedAsync().ConfigureAwait(false);
        }

        public async Task WriteValueAsync(Guid value)
        {
            this.WriteSeparatorIfNecessary();
            this.utf8JsonWriter.WriteStringValue(value);
            await this.DrainBufferIfThresholdReachedAsync().ConfigureAwait(false);
        }

        public async Task WriteValueAsync(decimal value)
        {
            this.WriteSeparatorIfNecessary();
            if (this.isIeee754Compatible)
            {
                this.utf8JsonWriter.WriteStringValue(value.ToString(CultureInfo.InvariantCulture));
            }
            else
            {
                this.utf8JsonWriter.WriteNumberValue(value);
            }

            await this.DrainBufferIfThresholdReachedAsync().ConfigureAwait(false);
        }

        public async Task WriteValueAsync(DateTimeOffset value)
        {
            this.WriteSeparatorIfNecessary();
            if (value.Offset == TimeSpan.Zero)
            {

                // The default JsonWriter uses the format yyyy-MM-ddThh:mm:ss.fffffffZ when the offset is 0
                // Utf8JsonWriter uses the format yyyy-MM-ddThh:mm:ss.fffffff+00:00
                // While both formats are valid IS0 8601, we decided to keep the output
                // consistent between the two writers to avoid breaking any client that may
                // have dependency on the original format.
                string formattedValue = JsonValueUtils.FormatDateTimeOffset(value, ODataJsonDateTimeFormat.ISO8601DateTime);
                this.utf8JsonWriter.WriteStringValue(formattedValue);
            }
            else
            {
                this.utf8JsonWriter.WriteStringValue(value);
            }

            await this.DrainBufferIfThresholdReachedAsync().ConfigureAwait(false);
        }

        public async Task WriteValueAsync(TimeSpan value)
        {
            this.WriteSeparatorIfNecessary();
            string stringValue = EdmValueWriter.DurationAsXml(value);
            this.utf8JsonWriter.WriteStringValue(stringValue);
            await this.DrainBufferIfThresholdReachedAsync().ConfigureAwait(false);
        }

        public async Task WriteValueAsync(Date value)
        {
            this.WriteSeparatorIfNecessary();
            this.utf8JsonWriter.WriteStringValue(value.ToString());
            await this.DrainBufferIfThresholdReachedAsync().ConfigureAwait(false);
        }

        public async Task WriteValueAsync(TimeOfDay value)
        {
            this.WriteSeparatorIfNecessary();
            this.utf8JsonWriter.WriteStringValue(value.ToString());
            await this.DrainBufferIfThresholdReachedAsync().ConfigureAwait(false);
        }

        public async Task WriteValueAsync(byte value)
        {
            this.WriteSeparatorIfNecessary();
            this.utf8JsonWriter.WriteNumberValue(value);
            await this.DrainBufferIfThresholdReachedAsync().ConfigureAwait(false);
        }

        public async Task WriteValueAsync(sbyte value)
        {
            this.WriteSeparatorIfNecessary();
            this.utf8JsonWriter.WriteNumberValue(value);
            await this.DrainBufferIfThresholdReachedAsync().ConfigureAwait(false);
        }

        public async Task WriteValueAsync(string value)
        {
            if (value == null)
            {
                this.WriteSeparatorIfNecessary();
                this.utf8JsonWriter.WriteNullValue();
            }
            else if (value.Length > bufferWriter.FreeCapacity || value.Length > chunkSize)
            {
                await WriteStringValueInChunksAsync(value.AsMemory());
            }
            else
            {
                this.WriteSeparatorIfNecessary();
                this.utf8JsonWriter.WriteStringValue(value.ToString());
            }

            await this.DrainBufferIfThresholdReachedAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// Writes a string value into the buffer in chunks asynchronously, handling escaping if necessary.
        /// </summary>
        /// <param name="value">The string value to write.</param>
        /// <remarks>
        /// This method writes the provided string value into the buffer in manageable chunks to avoid
        /// excessive memory allocations and buffer overflows. If the string requires escaping, it is
        /// processed accordingly to ensure correct JSON formatting.
        /// </remarks>
        private async ValueTask WriteStringValueInChunksAsync(ReadOnlyMemory<char> value)
        {
            this.CommitUtf8JsonWriterContentsToBuffer();

            WriteItemWithSeparatorIfNeeded();

            this.bufferWriter.Write(this.DoubleQuote.Slice(0, 1).Span);

            await this.WriteToStreamAsync().ConfigureAwait(false);

            int charsNotProcessedFromPreviousChunk = 0;

            for (int i = 0; i < value.Length; i += chunkSize)
            {
                int remainingChars = Math.Min(chunkSize, value.Length - i);
                bool isFinalBlock = (i + remainingChars) == value.Length;
                ReadOnlyMemory<char> chunk = value.Slice(i - charsNotProcessedFromPreviousChunk, charsNotProcessedFromPreviousChunk + remainingChars);

                int firstIndexToEscape = NeedsEscaping(chunk.Span);

                Debug.Assert(firstIndexToEscape >= -1 && firstIndexToEscape < value.Length);

                if (firstIndexToEscape != -1)
                {
                    WriteEscapedStringChunk(chunk.Span, firstIndexToEscape, isFinalBlock, out charsNotProcessedFromPreviousChunk);
                }
                else
                {
                    WriteStringChunk(chunk.Span, isFinalBlock);
                }

                // Flush the buffer if needed
                await this.DrainBufferIfThresholdReachedAsync();
            }

            this.bufferWriter.Write(this.DoubleQuote.Slice(0, 1).Span);

            // since we bypass the Utf8JsonWriter, we need to signal to other
            // Write methods that a separator should be written first
            CheckIfSeparatorNeeded();

            CheckIfManualValueAtArrayStart();
        }

        public async Task WriteValueAsync(byte[] value)
        {
            if (value == null)
            {
                this.WriteSeparatorIfNecessary();
                this.utf8JsonWriter.WriteNullValue();
            }
            else if (value.Length > bufferWriter.FreeCapacity || value.Length > chunkSize)
            {
                await WriteByteValueInChunksAsync(value);
            }
            else
            {
                this.WriteSeparatorIfNecessary();
                this.utf8JsonWriter.WriteBase64StringValue(value);
            }

            await this.DrainBufferIfThresholdReachedAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// Writes the byte value represented by the provided ReadOnlySpan in chunks using Base64 encoding.
        /// </summary>
        /// <param name="value">The ReadOnlySpan containing the byte value to be written.</param>
        private async ValueTask WriteByteValueInChunksAsync(ReadOnlyMemory<byte> value)
        {
            this.CommitUtf8JsonWriterContentsToBuffer();

            WriteItemWithSeparatorIfNeeded();

            this.bufferWriter.Write(this.DoubleQuote.Slice(0, 1).Span);

            await WriteToStreamAsync().ConfigureAwait(false);

            int bytesNotWrittenFromPreviousChunk = 0;

            for (int i = 0; i < value.Length; i += chunkSize)
            {
                int remainingChars = Math.Min(chunkSize, value.Length - i);
                bool isFinalBlock = (i + remainingChars) == value.Length;

                ReadOnlyMemory<byte> chunk = value.Slice(i - bytesNotWrittenFromPreviousChunk, remainingChars + bytesNotWrittenFromPreviousChunk);

                Base64EncodeAndWriteChunk(chunk.Span, isFinalBlock, out bytesNotWrittenFromPreviousChunk);

                await DrainBufferIfThresholdReachedAsync().ConfigureAwait(false);
            }

            this.bufferWriter.Write(this.DoubleQuote.Slice(0, 1).Span);

            // since we bypass the Utf8JsonWriter, we need to signal to other
            // Write methods that a separator should be written first
            CheckIfSeparatorNeeded();

            CheckIfManualValueAtArrayStart();
        }

        public async Task WriteValueAsync(JsonElement value)
        {
            this.WriteSeparatorIfNecessary();
            value.WriteTo(utf8JsonWriter);
            await this.DrainBufferIfThresholdReachedAsync().ConfigureAwait(false);
        }

        public async Task WriteRawValueAsync(string rawValue)
        {
            this.WriteRawValueCore(rawValue);
            await this.DrainBufferIfThresholdReachedAsync().ConfigureAwait(false);
        }

        public async Task FlushAsync()
        {
            await this.WriteToStreamAsync().ConfigureAwait(false);
            await this.writeStream.FlushAsync().ConfigureAwait(false);
        }

        public async ValueTask DisposeAsync()
        {
            if (this.writeStream != null)
            {
                await this.writeStream.FlushAsync().ConfigureAwait(false);
            }

            if (this.utf8JsonWriter != null)
            {
                await this.utf8JsonWriter.DisposeAsync().ConfigureAwait(false);
            }

            if (this.outputStream != this.writeStream && this.writeStream != null)
            {
                await this.writeStream.DisposeAsync().ConfigureAwait(false);
            }

            if (!this.leaveStreamOpen && this.outputStream != null)
            {
                await this.outputStream.DisposeAsync().ConfigureAwait(false);
            }

            if (this.bufferWriter != null)
            {
                this.bufferWriter.Dispose();
            }

            this.Dispose(false);
        }

        /// <summary>
        /// If the buffer threshold is reached, write the buffered data
        /// to the underlying stream asynchronously, then clear the buffer.
        /// </summary>
        /// <returns>ValueTask representing the eventual completion of the asynchronous operation.</returns>
        private async ValueTask DrainBufferIfThresholdReachedAsync()
        {
            if ((this.utf8JsonWriter.BytesPending + this.bufferWriter.WrittenCount) >= this.bufferFlushThreshold)
            {
                await this.WriteToStreamAsync().ConfigureAwait(false);
            }
        }

        private async ValueTask WriteToStreamAsync()
        {
            this.CommitUtf8JsonWriterContentsToBuffer();
            await this.writeStream.WriteAsync(this.bufferWriter.WrittenMemory).ConfigureAwait(false);
            this.bufferWriter.Clear();
        }
        #endregion

    }
}
