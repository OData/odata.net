//---------------------------------------------------------------------
// <copyright file="ODataUtf8JsonWriter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

#if NETCOREAPP3_1_OR_GREATER
using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Text.Encodings.Web;
using Microsoft.OData.Edm;
using System.Threading.Tasks;
using System.Buffers;

namespace Microsoft.OData.Json
{
    /// <summary>
    /// Implementation of <see cref="IJsonWriter"/> and <see cref="IJsonWriterAsync"/>that is based on
    /// <see cref="Utf8JsonWriter"/>.
    /// </summary>
    internal sealed class ODataUtf8JsonWriter : IJsonWriter, IDisposable, IJsonWriterAsync, IAsyncDisposable
    {
        private const int DefaultBufferSize = 16 * 1024;
        private readonly float bufferFlushThreshold;
        private readonly static ReadOnlyMemory<byte> parentheses = new byte[] { (byte)'(', (byte)')' };
        private readonly static ReadOnlyMemory<byte> itemSeparator = new byte[] { (byte)',' };

        private readonly Stream outputStream;
        private readonly Stream writeStream;
        private readonly Utf8JsonWriter writer;
        private readonly ArrayBufferWriter<byte> bufferWriter;
        private readonly int bufferSize;
        private readonly bool isIeee754Compatible;
        private readonly bool leaveStreamOpen;
        private readonly Encoding outputEncoding;
        private bool disposed;
        /// The Utf8JsonWriter internally keeps track of when to write the item separtor ','
        /// between key-value pairs in an object and between items in an array
        /// However, we bypass the Utf8JsonWriter in our implementation of WriteRawValue
        /// and write directly to the destination stream. This means that we have to manually
        /// write an item separator when writing raw values. And since the Utf8JsonWriter is not
        /// aware of this write, we have to manually keep track of state to synchronise with the Utf8JsonWriter
        /// to ensure we don't write extra separators or skip a separator where it's needed, both of
        /// which would result in invalid JSON.
        /// 
        /// In particular, we have to ensure write a separator in the following scenarios:
        /// - inside a JSON object, before writing a property name that's preceded by a raw value
        /// - inside an array, before writing an item that's preceded by one or more consecutive raw values at the start of the array.
        /// We only need to do this for the first non-raw value after a sequence of raw values. That's because the Utf8JsonWriter
        /// (not aware of the raw values) will think that this is the first item in the array and not include a separator before it.
        /// - inside an array, before writing a raw value that is not at the start of the array
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
        /// Whether we're about the first element in array. This helps
        /// decide whether we should manually write a separator.
        /// </summary>
        private bool isWritingFirstElementInArray = false;
        /// <summary>
        /// Whether we're currently writing a raw value at the start of an array
        /// or preceeded by a sequence of consecutive raw values at the start of
        /// an array e.g. ["rawValue1", "rawValue2", "rawValue3"
        /// </summary>
        private bool isWritingConsecutiveRawValuesAtStartOfArray = false;
        

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
            this.bufferWriter = new ArrayBufferWriter<byte>(bufferSize);
            // flush when we're close to the buffer capacity to avoid allocating bigger buffers
            this.bufferFlushThreshold = 0.9f * this.bufferSize;
            this.leaveStreamOpen = leaveStreamOpen;
            this.outputEncoding = encoding;

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

            this.writer = new Utf8JsonWriter(
                this.bufferWriter,
                new JsonWriterOptions {
                    // we don't need to perform validation here since the higher-level
                    // writers already perform validation
                    SkipValidation = true,
                    Encoder = encoder ?? JavaScriptEncoder.Default
                });
        }

        public void Flush()
        {
            this.CommitWriterContentsToBuffer();
            this.writeStream.Write(this.bufferWriter.WrittenMemory.Span);
            this.bufferWriter.Clear();
            this.writeStream.Flush();
        }

        private void FlushIfBufferThresholdReached()
        {
            if ((this.writer.BytesPending + this.bufferWriter.WrittenCount) >= this.bufferFlushThreshold)
            {
                this.Flush();
            }
        }

        /// <summary>
        /// Commits the contents of the <see cref="Utf8JsonWriter"/> to the
        /// bufferWriter. This should be called before writing directly
        /// to the bufferWriter.
        /// </summary>
        private void CommitWriterContentsToBuffer()
        {
            this.writer.Flush();
        }

        public void StartPaddingFunctionScope()
        {
            this.CommitWriterContentsToBuffer();
            ReadOnlySpan<byte> buf = stackalloc byte[] { (byte)'(' };
            this.bufferWriter.Write(buf);
            this.FlushIfBufferThresholdReached();
        }

        public void WritePaddingFunctionName(string functionName)
        {
            this.CommitWriterContentsToBuffer();
            this.bufferWriter.Write(Encoding.UTF8.GetBytes(functionName));
            this.FlushIfBufferThresholdReached();
        }

        public void EndPaddingFunctionScope()
        {
            this.CommitWriterContentsToBuffer();
            ReadOnlySpan<byte> buf = stackalloc byte[] { (byte)')' };
            this.bufferWriter.Write(buf);
            this.FlushIfBufferThresholdReached();
        }

        public void StartObjectScope()
        {
            this.WriteSeparatorIfNecessary();
            this.EnterObjectScope();
            this.writer.WriteStartObject();
            this.FlushIfBufferThresholdReached();
        }

        public void EndObjectScope()
        {
            this.writer.WriteEndObject();
            this.ExitScope();
            this.FlushIfBufferThresholdReached();
        }

        public void StartArrayScope()
        {
            this.WriteSeparatorIfNecessary();
            this.EnterArrayScope();
            this.writer.WriteStartArray();
            this.FlushIfBufferThresholdReached();
        }

        public void EndArrayScope()
        {
            this.writer.WriteEndArray();
            this.ExitScope();
            this.FlushIfBufferThresholdReached();
        }

        public void WriteName(string name)
        {
            this.WriteSeparatorIfNecessary();
            this.writer.WritePropertyName(name);
            this.FlushIfBufferThresholdReached();
        }

        public void WriteValue(bool value)
        {
            this.WriteSeparatorIfNecessary();
            this.writer.WriteBooleanValue(value);
            this.FlushIfBufferThresholdReached();
        }

        public void WriteValue(int value)
        {
            this.WriteSeparatorIfNecessary();
            this.writer.WriteNumberValue(value);
            this.FlushIfBufferThresholdReached();
        }

        public void WriteValue(float value)
        {
            this.WriteSeparatorIfNecessary();
            this.writer.WriteNumberValue(value);
            this.FlushIfBufferThresholdReached();
        }

        public void WriteValue(short value)
        {
            this.WriteSeparatorIfNecessary();
            this.writer.WriteNumberValue(value);
            this.FlushIfBufferThresholdReached();
        }

        public void WriteValue(long value)
        {
            this.WriteSeparatorIfNecessary();
            if (this.isIeee754Compatible)
            {
                this.writer.WriteStringValue(value.ToString(CultureInfo.InvariantCulture));
            }
            else
            {
                this.writer.WriteNumberValue(value);
            }
            
            this.FlushIfBufferThresholdReached();
        }

        public void WriteValue(double value)
        {
            this.WriteSeparatorIfNecessary();
            this.writer.WriteNumberValue(value);
            this.FlushIfBufferThresholdReached();
        }

        public void WriteValue(Guid value)
        {
            this.WriteSeparatorIfNecessary();
            this.writer.WriteStringValue(value);
            this.FlushIfBufferThresholdReached();
        }

        public void WriteValue(decimal value)
        {
            this.WriteSeparatorIfNecessary();
            if (this.isIeee754Compatible)
            {
                this.writer.WriteStringValue(value.ToString(CultureInfo.InvariantCulture));
            }
            else
            {
                this.writer.WriteNumberValue(value);
            }

            this.FlushIfBufferThresholdReached();
        }

        public void WriteValue(DateTimeOffset value)
        {
            this.WriteSeparatorIfNecessary();
            this.writer.WriteStringValue(value);
            this.FlushIfBufferThresholdReached();
        }

        public void WriteValue(TimeSpan value)
        {
            this.WriteSeparatorIfNecessary();
            string stringValue = EdmValueWriter.DurationAsXml(value);
            this.writer.WriteStringValue(stringValue);
            this.FlushIfBufferThresholdReached();
        }

        public void WriteValue(Date value)
        {
            this.WriteSeparatorIfNecessary();
            this.writer.WriteStringValue(value.ToString());
            this.FlushIfBufferThresholdReached();
        }

        public void WriteValue(TimeOfDay value)
        {
            this.WriteSeparatorIfNecessary();
            this.writer.WriteStringValue(value.ToString());
            this.FlushIfBufferThresholdReached();
        }

        public void WriteValue(byte value)
        {
            this.WriteSeparatorIfNecessary();
            this.writer.WriteNumberValue(value);
            this.FlushIfBufferThresholdReached();
        }

        public void WriteValue(sbyte value)
        {
            this.WriteSeparatorIfNecessary();
            this.writer.WriteNumberValue(value);
            this.FlushIfBufferThresholdReached();
        }

        public void WriteValue(string value)
        {
            this.WriteSeparatorIfNecessary();
            if (value == null)
            {
                this.writer.WriteNullValue();
            }
            else
            {
                this.writer.WriteStringValue(value);
            }

            this.FlushIfBufferThresholdReached();
        }

        public void WriteValue(byte[] value)
        {
            if (value == null)
            {
                this.writer.WriteNullValue();
            }
            else
            {
                this.writer.WriteBase64StringValue(value);
            }
            
            this.FlushIfBufferThresholdReached();
        }

        public void WriteRawValue(string rawValue)
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
            this.CommitWriterContentsToBuffer();
            if (IsInArray() && !isWritingFirstElementInArray)
            {
                // Place a separator before the raw value if
                // this is an array, unless this is the first item in the array.
                this.bufferWriter.Write(itemSeparator.Slice(0, 1).Span);
            }


            this.bufferWriter.Write(Encoding.UTF8.GetBytes(rawValue));
            // In the worst case, a single UTF-16 character could be expanded to 3 UTF-8 bytes
            //Span<byte> buf = this.bufferWriter.GetSpan(rawValue.Length * 3);
            //Encoding.UTF8.GetEncoder().Convert(rawValue.AsSpan(), buf, flush: false, out int charsUsed, out int bytesUsed, out bool completed);
            //bufferWriter.Advance(bytesUsed);

            // since we bypass the Utf8JsonWriter, we need to signal to other
            // Write methods that a separator should be written first
            if ((this.isWritingFirstElementInArray || this.isWritingConsecutiveRawValuesAtStartOfArray) || !this.IsInArray())
            {
                this.shouldWriteSeparator = true;
            }

            if (this.isWritingFirstElementInArray || this.isWritingConsecutiveRawValuesAtStartOfArray)
            {
                this.isWritingConsecutiveRawValuesAtStartOfArray = true;
            }
            
            this.isWritingFirstElementInArray = false;
            this.FlushIfBufferThresholdReached();
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
            this.isWritingFirstElementInArray = false;
            this.isWritingConsecutiveRawValuesAtStartOfArray = false;
        }

        /// <summary>
        /// Manually writes an item separator ',' to the output, bypassing the Utf8JsonWriter.
        /// </summary>
        private void WriteItemSeparator()
        {
            this.CommitWriterContentsToBuffer();
            //this.bufferWriter.Write(itemSeparator.Span.Slice(0, 1));
            Span<byte> buf = this.bufferWriter.GetSpan(1);
            buf[0] = (byte)',';
            this.bufferWriter.Advance(1);
        }

        /// <summary>
        /// Mark that the writer has transitioned to write an object.
        /// </summary>
        private void EnterObjectScope()
        {
            this.isWritingConsecutiveRawValuesAtStartOfArray = false;
            this.isWritingFirstElementInArray = false;
            this.bitStack.PushTrue();
        }

        /// <summary>
        /// Mark that the writer has transitioned to write an array.
        /// </summary>
        private void EnterArrayScope()
        {
            this.isWritingConsecutiveRawValuesAtStartOfArray = false;
            this.isWritingFirstElementInArray = true;
            this.bitStack.PushFalse();
        }

        /// <summary>
        /// Exit the current object or array scope.
        /// </summary>
        private void ExitScope()
        {
            this.isWritingFirstElementInArray = false;
            this.shouldWriteSeparator = false;
            this.bitStack.Pop();
        }

        /// <summary>
        /// Checks whether the writer is currently in an array scope.
        /// </summary>
        /// <returns>Whether the writer is currently in an array scope.</returns>
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
        /// True if the writer is currently in an object scope, false if the writer is in an array scope.</param>
        /// <returns>True if the current scope was successfully determined, false if it was not able to determined the scope
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
                this.writer.Dispose();

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
            this.CommitWriterContentsToBuffer();
            this.bufferWriter.Write(parentheses[..1].Span);
            await this.FlushIfBufferThresholdReachedAsync().ConfigureAwait(false);
        }

        public async Task WritePaddingFunctionNameAsync(string functionName)
        {
            this.CommitWriterContentsToBuffer();
            this.bufferWriter.Write(Encoding.UTF8.GetBytes(functionName));
            await this.FlushIfBufferThresholdReachedAsync().ConfigureAwait(false);
        }

        public async Task EndPaddingFunctionScopeAsync()
        {
            this.CommitWriterContentsToBuffer();
            this.bufferWriter.Write(parentheses[1..2].Span);
            await this.FlushIfBufferThresholdReachedAsync().ConfigureAwait(false);
        }

        public async Task StartObjectScopeAsync()
        {
            this.WriteSeparatorIfNecessary();
            this.EnterObjectScope();
            this.writer.WriteStartObject();
            await this.FlushIfBufferThresholdReachedAsync().ConfigureAwait(false);
        }

        public async Task EndObjectScopeAsync()
        {
            this.writer.WriteEndObject();
            this.ExitScope();
            await this.FlushIfBufferThresholdReachedAsync().ConfigureAwait(false);
        }

        public async Task StartArrayScopeAsync()
        {
            this.WriteSeparatorIfNecessary();
            this.EnterArrayScope();
            this.writer.WriteStartArray();
            await this.FlushIfBufferThresholdReachedAsync().ConfigureAwait(false);
        }

        public async Task EndArrayScopeAsync()
        {
            this.writer.WriteEndArray();
            this.ExitScope();
            await this.FlushIfBufferThresholdReachedAsync().ConfigureAwait(false);
        }

        public async Task WriteNameAsync(string name)
        {
            this.WriteSeparatorIfNecessary();
            this.writer.WritePropertyName(name);
            await this.FlushIfBufferThresholdReachedAsync().ConfigureAwait(false);
        }

        public async Task WriteValueAsync(bool value)
        {
            this.WriteSeparatorIfNecessary();
            this.writer.WriteBooleanValue(value);
            await this.FlushIfBufferThresholdReachedAsync().ConfigureAwait(false);
        }

        public async Task WriteValueAsync(int value)
        {
            this.WriteSeparatorIfNecessary();
            this.writer.WriteNumberValue(value);
            await this.FlushIfBufferThresholdReachedAsync().ConfigureAwait(false);
        }

        public async Task WriteValueAsync(float value)
        {
            this.WriteSeparatorIfNecessary();
            this.writer.WriteNumberValue(value);
            await this.FlushIfBufferThresholdReachedAsync().ConfigureAwait(false);
        }

        public async Task WriteValueAsync(short value)
        {
            this.WriteSeparatorIfNecessary();
            this.writer.WriteNumberValue(value);
            await this.FlushIfBufferThresholdReachedAsync().ConfigureAwait(false);
        }

        public async Task WriteValueAsync(long value)
        {
            this.WriteSeparatorIfNecessary();
            if (this.isIeee754Compatible)
            {
                this.writer.WriteStringValue(value.ToString(CultureInfo.InvariantCulture));
            }
            else
            {
                this.writer.WriteNumberValue(value);
            }

            await this.FlushIfBufferThresholdReachedAsync().ConfigureAwait(false);
        }

        public async Task WriteValueAsync(double value)
        {
            this.WriteSeparatorIfNecessary();
            this.writer.WriteNumberValue(value);
            await this.FlushIfBufferThresholdReachedAsync().ConfigureAwait(false);
        }

        public async Task WriteValueAsync(Guid value)
        {
            this.WriteSeparatorIfNecessary();
            this.writer.WriteStringValue(value);
            await this.FlushIfBufferThresholdReachedAsync().ConfigureAwait(false);
        }

        public async Task WriteValueAsync(decimal value)
        {
            this.WriteSeparatorIfNecessary();
            if (this.isIeee754Compatible)
            {
                this.writer.WriteStringValue(value.ToString(CultureInfo.InvariantCulture));
            }
            else
            {
                this.writer.WriteNumberValue(value);
            }

            await this.FlushIfBufferThresholdReachedAsync().ConfigureAwait(false);
        }

        public async Task WriteValueAsync(DateTimeOffset value)
        {
            this.WriteSeparatorIfNecessary();
            this.writer.WriteStringValue(value);
            await this.FlushIfBufferThresholdReachedAsync().ConfigureAwait(false);
        }

        public async Task WriteValueAsync(TimeSpan value)
        {
            this.WriteSeparatorIfNecessary();
            string stringValue = EdmValueWriter.DurationAsXml(value);
            this.writer.WriteStringValue(stringValue);
            await this.FlushIfBufferThresholdReachedAsync().ConfigureAwait(false);
        }

        public async Task WriteValueAsync(Date value)
        {
            this.WriteSeparatorIfNecessary();
            this.writer.WriteStringValue(value.ToString());
            await this.FlushIfBufferThresholdReachedAsync().ConfigureAwait(false);
        }

        public async Task WriteValueAsync(TimeOfDay value)
        {
            this.WriteSeparatorIfNecessary();
            this.writer.WriteStringValue(value.ToString());
            await this.FlushIfBufferThresholdReachedAsync().ConfigureAwait(false);
        }

        public async Task WriteValueAsync(byte value)
        {
            this.WriteSeparatorIfNecessary();
            this.writer.WriteNumberValue(value);
            await this.FlushIfBufferThresholdReachedAsync().ConfigureAwait(false);
        }

        public async Task WriteValueAsync(sbyte value)
        {
            this.WriteSeparatorIfNecessary();
            this.writer.WriteNumberValue(value);
            await this.FlushIfBufferThresholdReachedAsync().ConfigureAwait(false);
        }

        public async Task WriteValueAsync(string value)
        {
            this.WriteSeparatorIfNecessary();
            if (value == null)
            {
                this.writer.WriteNullValue();
            }
            else
            {
                this.writer.WriteStringValue(value.ToString());
            }

            await this.FlushIfBufferThresholdReachedAsync().ConfigureAwait(false);
        }

        public async Task WriteValueAsync(byte[] value)
        {
            this.WriteSeparatorIfNecessary();
            if (value == null)
            {
                this.writer.WriteNullValue();
            }
            else
            {
                this.writer.WriteBase64StringValue(value);
            }

            await this.FlushIfBufferThresholdReachedAsync().ConfigureAwait(false);
        }

        public async Task WriteRawValueAsync(string rawValue)
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
            this.CommitWriterContentsToBuffer();
            if (IsInArray() && !isWritingFirstElementInArray)
            {
                // Place a separator before the raw value if
                // this is an array, unless this is the first item in the array.
                this.bufferWriter.Write(itemSeparator.Slice(0, 1).Span);
            }

            // Consider using Utf8JsonWriter.WriteRawValue() in .NET 6+
            // see: https://github.com/OData/odata.net/issues/2420
            this.bufferWriter.Write(Encoding.UTF8.GetBytes(rawValue));

            // since we bypass the Utf8JsonWriter, we need to signal to other
            // Write methods that a separator should be written first
            if ((this.isWritingFirstElementInArray || this.isWritingConsecutiveRawValuesAtStartOfArray) || !this.IsInArray())
            {
                this.shouldWriteSeparator = true;
            }

            if (this.isWritingFirstElementInArray || this.isWritingConsecutiveRawValuesAtStartOfArray)
            {
                this.isWritingConsecutiveRawValuesAtStartOfArray = true;
            }

            this.isWritingFirstElementInArray = false;
            await this.FlushIfBufferThresholdReachedAsync().ConfigureAwait(false);
        }

        public async Task FlushAsync()
        {
            this.CommitWriterContentsToBuffer();
            await this.writeStream.WriteAsync(this.bufferWriter.WrittenMemory).ConfigureAwait(false);
            this.bufferWriter.Clear();
            await this.writeStream.FlushAsync().ConfigureAwait(false);
        }

        public async ValueTask DisposeAsync()
        {
            if (this.writeStream != null)
            {
                await this.writeStream.FlushAsync().ConfigureAwait(false);
            }

            if (this.writer != null)
            {
                await this.writer.DisposeAsync().ConfigureAwait(false);
            }

            if (this.outputStream != this.writeStream && this.writeStream != null)
            {
                await this.writeStream.DisposeAsync().ConfigureAwait(false);
            }

            if (!this.leaveStreamOpen && this.outputStream != null)
            {
                await this.outputStream.DisposeAsync().ConfigureAwait(false);
            }

            this.Dispose(false);
        }

        private async ValueTask FlushIfBufferThresholdReachedAsync()
        {
            if ((this.writer.BytesPending + this.bufferWriter.WrittenCount) >= this.bufferFlushThreshold)
            {
                await this.FlushAsync().ConfigureAwait(false);
            }
        }

        #endregion

    }
}
#endif
