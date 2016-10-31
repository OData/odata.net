//   OData .NET Libraries ver. 5.6.3
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 
//   MIT License
//   Permission is hereby granted, free of charge, to any person obtaining a copy of
//   this software and associated documentation files (the "Software"), to deal in
//   the Software without restriction, including without limitation the rights to use,
//   copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the
//   Software, and to permit persons to whom the Software is furnished to do so,
//   subject to the following conditions:

//   The above copyright notice and this permission notice shall be included in all
//   copies or substantial portions of the Software.

//   THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
//   FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
//   COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
//   IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
//   CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

namespace Microsoft.Data.OData
{
    using System;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Text;
    using Microsoft.Data.OData.Json;

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
            DebugUtils.CheckNoExternalCallers();
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
                DebugUtils.CheckNoExternalCallers();
                return this.textWriter;
            }
        }

        /// <summary>
        /// Disposes the RawValueWriter. It flushes itself and then disposes its inner TextWriter.
        /// </summary>
        public void Dispose()
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(this.textWriter != null, "The text writer has not been initialized yet.");

            this.textWriter.Dispose();
            this.textWriter = null;
        }

        /// <summary>
        /// Start writing a raw output. This should only be called once.
        /// </summary>
        internal void Start()
        {
            DebugUtils.CheckNoExternalCallers();
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
            DebugUtils.CheckNoExternalCallers();
            if (this.settings.HasJsonPaddingFunction())
            {
                this.textWriter.Write(JsonConstants.EndPaddingFunctionScope);
            }
        }

        /// <summary>
        /// Converts the specified <paramref name="value"/> into its raw format and writes it to the output.
        /// The value has to be of primitive type. Only one WriteRawValue call should be made before this object gets disposed.
        /// </summary>
        /// <param name="value">The (non-binary) value to write.</param>
        /// <remarks>We do not accept binary values here; WriteBinaryValue should be used for binary data.</remarks>
        internal void WriteRawValue(object value)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(!(value is byte[]), "!(value is byte[])");

            string valueAsString;
            if (!AtomValueUtils.TryConvertPrimitiveToString(value, out valueAsString))
            {
                // throw an exception because the value is not primitive
                throw new ODataException(Strings.ODataUtils_CannotConvertValueToRawPrimitive(value.GetType().FullName));
            }

            this.textWriter.Write(valueAsString);
        }

        /// <summary>
        /// Flushes the RawValueWriter. 
        /// The call gets pushed to the TextWriter (if there is one). In production code, this is StreamWriter.Flush, which turns into Stream.Flush.
        /// In the synchronous case the underlying stream is the message stream itself, which will then Flush as well.
        /// In the async case the underlying stream is the async buffered stream, which ignores Flush call.
        /// </summary>
        internal void Flush()
        {   
            DebugUtils.CheckNoExternalCallers();
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
