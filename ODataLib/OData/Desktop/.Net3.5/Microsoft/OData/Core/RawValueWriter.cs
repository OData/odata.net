//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Core
{
    using System;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Text;
    using Microsoft.OData.Core.Json;
    using Microsoft.Spatial;

    /// <summary>
    /// Class that hanldes writing top level raw values to a stream.
    /// </summary>
    internal sealed class RawValueWriter : IDisposable
    {
        /// <summary>
        /// Writer settings.
        /// </summary>
        private readonly ODataMessageWriterSettings settings;

        /// <summary>
        /// Underlying stream. 
        /// </summary>
        private readonly Stream stream;

        /// <summary>
        /// Encoding that the TextWriter should use.
        /// </summary>
        private readonly Encoding encoding;

        /// <summary>
        /// TextWriter instance for writing values.
        /// </summary>
        private TextWriter textWriter;

        /// <summary>
        /// Initializes a new instance of the <see cref="RawValueWriter"/> class.
        /// Initializes the TextWriter.
        /// </summary>
        /// <param name="settings">The writer settings.</param>
        /// <param name="stream">The stream.  It should be the same underlying stream the TextWriter uses.</param>
        /// <param name="encoding">The encoding to use in the text writer.</param>
        internal RawValueWriter(ODataMessageWriterSettings settings, Stream stream, Encoding encoding)
        {
            this.settings = settings;
            this.stream = stream;
            this.encoding = encoding;
            this.InitializeTextWriter();
        }

        /// <summary>
        /// Gets the text writer.
        /// </summary>
        internal TextWriter TextWriter
        {
            get
            {
                return this.textWriter;
            }
        }

        /// <summary>
        /// Disposes the RawValueWriter. It flushes itself and then disposes its inner TextWriter.
        /// </summary>
        public void Dispose()
        {
            Debug.Assert(this.textWriter != null, "The text writer has not been initialized yet.");

            this.textWriter.Dispose();
            this.textWriter = null;
        }

        /// <summary>
        /// Start writing a raw output. This should only be called once.
        /// </summary>
        internal void Start()
        {
            if (this.settings.HasJsonPaddingFunction())
            {
                this.textWriter.Write(this.settings.JsonPCallback);
                this.textWriter.Write(JsonConstants.StartPaddingFunctionScope);
            }
        }

        /// <summary>
        /// End the writing of a raw output. This should be the last thing called.
        /// </summary>
        internal void End()
        {
            if (this.settings.HasJsonPaddingFunction())
            {
                this.textWriter.Write(JsonConstants.EndPaddingFunctionScope);
            }
        }

        /// <summary>
        /// Converts the specified <paramref name="value"/> into its raw format and writes it to the output.
        /// The value has to be of enumeration or primitive type. Only one WriteRawValue call should be made before this object gets disposed.
        /// </summary>
        /// <param name="value">The (non-binary) value to write.</param>
        /// <remarks>We do not accept binary values here; WriteBinaryValue should be used for binary data.</remarks>
        internal void WriteRawValue(object value)
        {
            Debug.Assert(!(value is byte[]), "!(value is byte[])");

            string valueAsString;
            ODataEnumValue enumValue = value as ODataEnumValue;
            if (enumValue != null)
            {
                this.textWriter.Write(enumValue.Value);
            }
            else if (value is Geometry || value is Geography)
            {
                PrimitiveConverter.Instance.TryWriteAtom(value, textWriter);
            }
            else if (AtomValueUtils.TryConvertPrimitiveToString(value, out valueAsString))
            {
                this.textWriter.Write(valueAsString);
            }
            else
            {
                // throw an exception because the value is neither enum nor primitive
                throw new ODataException(Strings.ODataUtils_CannotConvertValueToRawString(value.GetType().FullName));
            }
        }

        /// <summary>
        /// Flushes the RawValueWriter. 
        /// The call gets pushed to the TextWriter (if there is one). In production code, this is StreamWriter.Flush, which turns into Stream.Flush.
        /// In the synchronous case the underlying stream is the message stream itself, which will then Flush as well.
        /// In the async case the underlying stream is the async buffered stream, which ignores Flush call.
        /// </summary>
        internal void Flush()
        {
            if (this.TextWriter != null)
            {
                this.TextWriter.Flush();
            }
        }

        /// <summary>
        /// Initialized a new text writer over the message payload stream.
        /// </summary>
        /// <remarks>This can only be called if the text writer was not yet initialized or it has been closed.
        /// It can be called several times with CloseWriter calls in between though.</remarks>
        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "We create a NonDisposingStream which doesn't need to be disposed, even though it's IDisposable.")]
        private void InitializeTextWriter()
        {
            // We must create the text writer over a stream which will ignore Dispose, since we need to be able to Dispose 
            // the writer without disposing the underlying message stream.
            Stream nonDisposingStream;
            if (MessageStreamWrapper.IsNonDisposingStream(this.stream) || this.stream is AsyncBufferedStream)
            {
                // AsynBufferedStream ignores Dispose
                nonDisposingStream = this.stream;
            }
            else
            {
                nonDisposingStream = MessageStreamWrapper.CreateNonDisposingStream(this.stream);
            }

            this.textWriter = new StreamWriter(nonDisposingStream, this.encoding);
        }
    }
}
