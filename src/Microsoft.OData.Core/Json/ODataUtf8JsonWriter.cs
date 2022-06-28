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

namespace Microsoft.OData.Json
{
    /// <summary>
    /// Implementation of <see cref="IJsonWriter"/> that is based on
    /// <see cref="Utf8JsonWriter"/>.
    /// </summary>
    internal sealed class ODataUtf8JsonWriter : IJsonWriter, IDisposable
    {
        private const int DefaultBufferSize = 16 * 1024;
        private readonly float bufferFlushThreshold;

        private readonly Stream outputStream;
        private readonly Stream writeStream;
        private readonly Utf8JsonWriter writer;
        private readonly int bufferSize;
        private readonly bool isIeee754Compatible;
        private readonly bool leaveStreamOpen;
        private readonly Encoding outputEncoding;
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

        public void FlushIfBufferThresholdReached()
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
            this.writer.WriteStartObject();
        }

        public void EndObjectScope()
        {
            this.writer.WriteEndObject();
        }

        public void StartArrayScope()
        {
            this.writer.WriteStartArray();
        }

        public void EndArrayScope()
        {
            this.writer.WriteEndArray();
        }

        public void WriteName(string name)
        {
            this.writer.WritePropertyName(name);
            this.FlushIfBufferThresholdReached();
        }

        public void WriteValue(bool value)
        {
            this.writer.WriteBooleanValue(value);
            this.FlushIfBufferThresholdReached();
        }

        public void WriteValue(int value)
        {
            this.writer.WriteNumberValue(value);
            this.FlushIfBufferThresholdReached();
        }

        public void WriteValue(float value)
        {
            this.writer.WriteNumberValue(value);
            this.FlushIfBufferThresholdReached();
        }

        public void WriteValue(short value)
        {
            this.writer.WriteNumberValue(value);
            this.FlushIfBufferThresholdReached();
        }

        public void WriteValue(long value)
        {
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
            this.writer.WriteNumberValue(value);
            this.FlushIfBufferThresholdReached();
        }

        public void WriteValue(Guid value)
        {
            this.writer.WriteStringValue(value);
            this.FlushIfBufferThresholdReached();
        }

        public void WriteValue(decimal value)
        {
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
            this.writer.WriteStringValue(value);
            this.FlushIfBufferThresholdReached();
        }

        public void WriteValue(TimeSpan value)
        {
            string stringValue = EdmValueWriter.DurationAsXml(value);
            this.writer.WriteStringValue(stringValue);
            this.FlushIfBufferThresholdReached();
        }

        public void WriteValue(Date value)
        {
            this.writer.WriteStringValue(value.ToString());
            this.FlushIfBufferThresholdReached();
        }

        public void WriteValue(TimeOfDay value)
        {
            this.writer.WriteStringValue(value.ToString());
            this.FlushIfBufferThresholdReached();
        }

        public void WriteValue(byte value)
        {
            this.writer.WriteNumberValue(value);
            this.FlushIfBufferThresholdReached();
        }

        public void WriteValue(sbyte value)
        {
            this.writer.WriteNumberValue(value);
            this.FlushIfBufferThresholdReached();
        }

        public void WriteValue(string value)
        {
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

            // Consider using Utf8JsonWriter.WriteRawValue() in .NET 6+
            // see: https://github.com/OData/odata.net/issues/2420

            this.Flush(); // ensure we don't write to the stream while there are still pending data in the buffer
            this.writeStream.Write(Encoding.UTF8.GetBytes(rawValue));
            this.FlushIfBufferThresholdReached();
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
    }
}
#endif
