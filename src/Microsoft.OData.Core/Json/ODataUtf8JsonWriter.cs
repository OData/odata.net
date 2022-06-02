//---------------------------------------------------------------------
// <copyright file="ODataUtf8JsonWriter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

#if NETCOREAPP3_1_OR_GREATER
using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Globalization;
using Microsoft.OData.Edm;

namespace Microsoft.OData.Json
{
    /// <summary>
    /// Implement of <see cref="IJsonWriter"/> that is based on
    /// <see cref="Utf8JsonWriter"/>.
    /// </summary>
    internal sealed class ODataUtf8JsonWriter : IJsonWriter, IDisposable
    {
        private const int DefaultBufferSize = 16 * 1024;
        private readonly float bufferFlushThreshold;

        private readonly Stream outputStream;
        private readonly Utf8JsonWriter writer;
        private readonly int bufferSize;
        private readonly bool isIeee754Compatible;
        private readonly bool leaveStreamOpen;
        private readonly Encoding encoding = Encoding.UTF8;
        private bool disposed;

        public ODataUtf8JsonWriter(Stream outputStream, bool isIeee754Compatible, int bufferSize = DefaultBufferSize, bool leaveStreamOpen = false)
        {
            Debug.Assert(outputStream != null, "outputStream != null");
            this.outputStream = outputStream;
            this.isIeee754Compatible = isIeee754Compatible;
            this.bufferSize = bufferSize;
            // flush when we're close to the buffer capacity to avoid allocating bigger buffers
            this.bufferFlushThreshold = 0.9f * this.bufferSize;
            this.writer = new Utf8JsonWriter(
                outputStream,
                // we don't need to perform validation here since the higher-level
                // writers already perform validation
                new JsonWriterOptions { SkipValidation = true });
            this.leaveStreamOpen = leaveStreamOpen;
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
            this.outputStream.WriteByte((byte)'(');
        }

        public void WritePaddingFunctionName(string functionName)
        {
            this.Flush();
            this.outputStream.Write(this.encoding.GetBytes(functionName));
        }

        public void EndPaddingFunctionScope()
        {
            this.Flush();
            this.outputStream.WriteByte((byte)')');
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

            this.Flush(); // ensure we don't write to the stream while there are still pending data in the buffer
            this.outputStream.Write(this.encoding.GetBytes(rawValue));
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
                this.writer.Dispose();

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
