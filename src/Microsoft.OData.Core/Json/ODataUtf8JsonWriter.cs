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
        private readonly static byte itemSeparator = (byte)',';

        private readonly Stream outputStream;
        private readonly Stream writeStream;
        private readonly Utf8JsonWriter writer;
        private readonly int bufferSize;
        private readonly bool isIeee754Compatible;
        private readonly bool leaveStreamOpen;
        private readonly Encoding outputEncoding;
        /// <summary>
        /// Whether we should write the item separator (,) before
        /// the next object or array entry.
        /// We need to keep track of this because the WriteRawValue bypasses the
        /// Utf8JsonWriter and writes to the stream directly, so the Utf8JsonWriter
        /// does not end up writing the separator.
        /// This will not be necessary when we switch to using Utf8JsonWriter.WriteRawValue()
        /// see: https://github.com/OData/odata.net/issues/2420
        /// </summary>
        private bool shouldWriteSeparator = false;
        /// <summary>
        /// Stack used to keep track of scope transitions and
        /// whether we're currently in an Object or an Array.
        /// </summary>
        private BitStack bitStack;
        /// <summary>
        /// Whether we're about the first element
        /// in an arrya
        /// </summary>
        private bool isWritingFirstElementInArray = false;
        private bool isWritingConsecutiveRawValuesAtStartOfArray = false;
        private bool disposed;

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
                this.writeStream,
                new JsonWriterOptions {
                    // we don't need to perform validation here since the higher-level
                    // writers already perform validation
                    SkipValidation = true,
                    Encoder = encoder ?? JavaScriptEncoder.Default
                });
        }

        public void Flush()
        {
            if (this.writer.BytesPending > 0)
            {
                this.writer.Flush();
            }
        }

        private void FlushIfBufferThresholdReached()
        {
            if (this.writer.BytesPending >= this.bufferFlushThreshold)
            {
                this.Flush();
            }
        }

        public void StartPaddingFunctionScope()
        {
            this.Flush();
            this.writeStream.WriteByte((byte)'(');
        }

        public void WritePaddingFunctionName(string functionName)
        {
            this.Flush();
            this.writeStream.Write(Encoding.UTF8.GetBytes(functionName));
        }

        public void EndPaddingFunctionScope()
        {
            this.Flush();
            this.writeStream.WriteByte((byte)')');
        }

        public void StartObjectScope()
        {
            this.WriteSeparatorIfNecessary();
            this.EnterObjectScope();
            this.writer.WriteStartObject();
        }

        public void EndObjectScope()
        {
            this.writer.WriteEndObject();
            this.ExitScope();
        }

        public void StartArrayScope()
        {
            this.WriteSeparatorIfNecessary();
            this.EnterArrayScope();
            this.writer.WriteStartArray();
        }

        public void EndArrayScope()
        {
            this.writer.WriteEndArray();
            this.ExitScope();
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

            // ensure we don't write to the stream directly while there are still pending data in the Utf8JsonWriter buffer
            this.Flush(); 
            if (IsInArray() && !isWritingFirstElementInArray)
            {
                // if we're inside an array we have to manually pre-pend separator
                // be writing the raw value directly
                this.writeStream.WriteByte(itemSeparator);
            }

            // Consider using Utf8JsonWriter.WriteRawValue() in .NET 6+
            // see: https://github.com/OData/odata.net/issues/2420
            this.writeStream.Write(Encoding.UTF8.GetBytes(rawValue));
            this.FlushIfBufferThresholdReached();
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
        }

        private void WriteSeparatorIfNecessary()
        {
            if (shouldWriteSeparator)
            {
                // We don't need to write separators inside arrays
                // because Utf8JsonWriter writes one before
                // each value (except for the first)
                this.WriteItemSeparator();
            }

            this.shouldWriteSeparator = false;
            this.isWritingFirstElementInArray = false;
            this.isWritingConsecutiveRawValuesAtStartOfArray = false;
        }

        private void WriteItemSeparator()
        {
            this.Flush();
            this.writeStream.WriteByte(itemSeparator);
        }

        private void ClearSeparator()
        {
            shouldWriteSeparator = false;
        }

        private void EnterObjectScope()
        {
            this.isWritingConsecutiveRawValuesAtStartOfArray = false;
            this.isWritingFirstElementInArray = false;
            this.bitStack.PushTrue();
        }

        private void EnterArrayScope()
        {
            this.isWritingConsecutiveRawValuesAtStartOfArray = false;
            this.isWritingFirstElementInArray = true;
            this.bitStack.PushFalse();
        }

        private void ExitScope()
        {
            this.isWritingFirstElementInArray = false;
            this.shouldWriteSeparator = false;
            this.bitStack.Pop();
        }

        private bool IsInArray()
        {
            if (!TryPeekScope(out bool isInObject))
            {
                return false;
            }

            return !isInObject;
        }

        private bool TryPeekScope(out bool isInObject)
        {
            isInObject = false;
            if (this.bitStack.CurrentDepth == 0)
            {
                return false;
            }

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
            await this.FlushAsync().ConfigureAwait(false);
            await this.writeStream.WriteAsync(parentheses[..1]).ConfigureAwait(false);
        }

        public async Task WritePaddingFunctionNameAsync(string functionName)
        {
            await this.FlushAsync().ConfigureAwait(false);
            await this.writeStream.WriteAsync(Encoding.UTF8.GetBytes(functionName)).ConfigureAwait(false);
        }

        public async Task EndPaddingFunctionScopeAsync()
        {
            await this.FlushAsync().ConfigureAwait(false);
            await this.writeStream.WriteAsync(parentheses[1..2]).ConfigureAwait(false);
        }

        public async Task StartObjectScopeAsync()
        {
            this.writer.WriteStartObject();
            await this.FlushIfBufferThresholdReachedAsync().ConfigureAwait(false);
        }

        public async Task EndObjectScopeAsync()
        {
            this.writer.WriteEndObject();
            await this.FlushIfBufferThresholdReachedAsync().ConfigureAwait(false);
        }

        public async Task StartArrayScopeAsync()
        {
            this.writer.WriteStartArray();
            await this.FlushIfBufferThresholdReachedAsync().ConfigureAwait(false);
        }

        public async Task EndArrayScopeAsync()
        {
            this.writer.WriteEndArray();
            await this.FlushIfBufferThresholdReachedAsync().ConfigureAwait(false);
        }

        public async Task WriteNameAsync(string name)
        {
            this.writer.WritePropertyName(name);
            await this.FlushIfBufferThresholdReachedAsync().ConfigureAwait(false);
        }

        public async Task WriteValueAsync(bool value)
        {
            this.writer.WriteBooleanValue(value);
            await this.FlushIfBufferThresholdReachedAsync().ConfigureAwait(false);
        }

        public async Task WriteValueAsync(int value)
        {
            this.writer.WriteNumberValue(value);
            await this.FlushIfBufferThresholdReachedAsync().ConfigureAwait(false);
        }

        public async Task WriteValueAsync(float value)
        {
            this.writer.WriteNumberValue(value);
            await this.FlushIfBufferThresholdReachedAsync().ConfigureAwait(false);
        }

        public async Task WriteValueAsync(short value)
        {
            this.writer.WriteNumberValue(value);
            await this.FlushIfBufferThresholdReachedAsync().ConfigureAwait(false);
        }

        public async Task WriteValueAsync(long value)
        {
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
            this.writer.WriteNumberValue(value);
            await this.FlushIfBufferThresholdReachedAsync().ConfigureAwait(false);
        }

        public async Task WriteValueAsync(Guid value)
        {
            this.writer.WriteStringValue(value);
            await this.FlushIfBufferThresholdReachedAsync().ConfigureAwait(false);
        }

        public async Task WriteValueAsync(decimal value)
        {
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
            this.writer.WriteStringValue(value.ToString());
            await this.FlushIfBufferThresholdReachedAsync().ConfigureAwait(false);
        }

        public async Task WriteValueAsync(TimeOfDay value)
        {
            this.writer.WriteStringValue(value.ToString());
            await this.FlushIfBufferThresholdReachedAsync().ConfigureAwait(false);
        }

        public async Task WriteValueAsync(byte value)
        {
            this.writer.WriteNumberValue(value);
            await this.FlushIfBufferThresholdReachedAsync().ConfigureAwait(false);
        }

        public async Task WriteValueAsync(sbyte value)
        {
            this.writer.WriteNumberValue(value);
            await this.FlushIfBufferThresholdReachedAsync().ConfigureAwait(false);
        }

        public async Task WriteValueAsync(string value)
        {
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

            // Consider using Utf8JsonWriter.WriteRawValue() in .NET 6+
            // see: https://github.com/OData/odata.net/issues/2420

            await this.FlushAsync().ConfigureAwait(false); // ensure we don't write to the stream while there are still pending data in the buffer
            await this.writeStream.WriteAsync(Encoding.UTF8.GetBytes(rawValue)).ConfigureAwait(false);
            await this.FlushIfBufferThresholdReachedAsync().ConfigureAwait(false);
        }

        public async Task FlushAsync()
        {
            if (this.writer.BytesPending > 0)
            {
                await this.writer.FlushAsync().ConfigureAwait(false);
            }
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
            if (this.writer.BytesPending >= this.bufferFlushThreshold)
            {
                await this.FlushAsync().ConfigureAwait(false);
            }
        }

        #endregion

    }
}
#endif
