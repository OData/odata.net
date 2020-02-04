//---------------------------------------------------------------------
// <copyright file="ODataJsonTextWriter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Text;
    using Microsoft.OData.Buffers;
    using Microsoft.OData.Json;

    /// <summary>
    /// Wrapper for TextWriter to escape special characters.
    /// </summary>
    internal sealed class ODataJsonTextWriter : TextWriter
    {
        /// <summary>
        /// The underlying textWriter for writing the JSON.
        /// </summary>
        private readonly TextWriter textWriter;

        /// <summary>
        /// The escape option to use in writing text.
        /// </summary>
        private readonly ODataStringEscapeOption escapeOption = ODataStringEscapeOption.EscapeOnlyControls;

        /// <summary>
        /// The buffer to help when converting binary values.
        /// </summary>
        private char[] streamingBuffer;

        /// <summary>
        /// Get/sets the character buffer pool for streaming.
        /// </summary>
        private ICharArrayPool bufferPool;

        /// <summary>
        /// A TextWriter for writing properly escaped JSON text.
        /// </summary>
        /// <param name="textWriter">The underlying TextWriter used when writing JSON</param>
        /// <param name="buffer">Buffer used when converting binary values.</param>
        /// <param name="bufferPool">Buffer pool used for renting a buffer.</param>
        internal ODataJsonTextWriter(TextWriter textWriter, ref char[] buffer, ICharArrayPool bufferPool)
            : base(System.Globalization.CultureInfo.InvariantCulture)
        {
            ExceptionUtils.CheckArgumentNotNull(textWriter, "textWriter");

            this.textWriter = textWriter;
            this.streamingBuffer = buffer;
            this.bufferPool = bufferPool;
        }

        /// <inheritdoc/>
        public override Encoding Encoding
        {
            get
            {
                return this.textWriter.Encoding;
            }
        }

        /// <inheritdoc/>
        public override string NewLine
        {
            get
            {
                return this.textWriter.NewLine;
            }

            set
            {
                this.textWriter.NewLine = value;
            }
        }

        /// <inheritdoc/>
        public override IFormatProvider FormatProvider
        {
            get
            {
                return this.textWriter.FormatProvider;
            }
        }

        /// <inheritdoc/>
        public override void Flush()
        {
            this.textWriter.Flush();
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return this.textWriter.GetHashCode();
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return this.textWriter.ToString();
        }

        #region Write methods
        /// <inheritdoc/>
        public override void Write(char value)
        {
            this.WriteEscapedCharValue(value);
        }

        /// <inheritdoc/>
        public override void Write(bool value)
        {
            this.textWriter.Write(value);
        }

        /// <inheritdoc/>
        public override void Write(string value)
        {
            this.WriteEscapedStringValue(value);
        }

        /// <inheritdoc/>
        public override void Write(char[] buffer)
        {
            this.WriteEscapedCharArray(buffer, 0, buffer.Length);
        }

        /// <inheritdoc/>
        public override void Write(char[] buffer, int index, int count)
        {
            this.WriteEscapedCharArray(buffer, index, count);
        }

        /// <inheritdoc/>
        public override void Write(string format, params object[] arg)
        {
            this.WriteEscapedStringValue(string.Format(this.FormatProvider, format, arg));
        }

        /// <inheritdoc/>
        public override void Write(decimal value)
        {
            this.textWriter.Write(value);
        }

        /// <inheritdoc/>
        public override void Write(object value)
        {
            this.textWriter.Write(value);
        }

        /// <inheritdoc/>
        public override void Write(double value)
        {
            this.textWriter.Write(value);
        }

        /// <inheritdoc/>
        public override void Write(float value)
        {
            this.textWriter.Write(value);
        }

        /// <inheritdoc/>
        public override void Write(int value)
        {
            this.textWriter.Write(value);
        }

        /// <inheritdoc/>
        public override void Write(long value)
        {
            this.textWriter.Write(value);
        }

        /// <inheritdoc/>
        public override void Write(uint value)
        {
            this.textWriter.Write(value);
        }

        /// <inheritdoc/>
        public override void Write(ulong value)
        {
            this.textWriter.Write(value);
        }

        #endregion

        #region WriteLine methods
        /// <inheritdoc/>
        public override void WriteLine()
        {
            this.textWriter.WriteLine();
        }

        /// <inheritdoc/>
        public override void WriteLine(bool value)
        {
            this.textWriter.WriteLine(value);
        }

        /// <inheritdoc/>
        public override void WriteLine(char value)
        {
            this.Write(value);
            this.textWriter.WriteLine();
        }

        /// <inheritdoc/>
        public override void WriteLine(char[] buffer)
        {
            this.Write(buffer);
            this.textWriter.WriteLine();
        }

        /// <inheritdoc/>
        public override void WriteLine(char[] buffer, int index, int count)
        {
            this.Write(buffer, index, count);
            this.textWriter.WriteLine();
        }

        /// <inheritdoc/>
        public override void WriteLine(decimal value)
        {
            this.textWriter.WriteLine(value);
        }

        /// <inheritdoc/>
        public override void WriteLine(double value)
        {
            this.textWriter.WriteLine(value);
        }

        /// <inheritdoc/>
        public override void WriteLine(float value)
        {
            this.textWriter.WriteLine(value);
        }

        /// <inheritdoc/>
        public override void WriteLine(int value)
        {
            this.textWriter.WriteLine(value);
        }

        /// <inheritdoc/>
        public override void WriteLine(long value)
        {
            this.textWriter.WriteLine(value);
        }

        /// <inheritdoc/>
        public override void WriteLine(object value)
        {
            this.textWriter.WriteLine(value);
        }

        /// <inheritdoc/>
        public override void WriteLine(string format, params object[] arg)
        {
            this.Write(format, arg);
            this.textWriter.WriteLine();
        }

        /// <inheritdoc/>
        public override void WriteLine(string value)
        {
            this.Write(value);
            this.textWriter.WriteLine();
        }

        /// <inheritdoc/>
        public override void WriteLine(uint value)
        {
            this.textWriter.WriteLine(value);
        }

        /// <inheritdoc/>
        public override void WriteLine(ulong value)
        {
            this.textWriter.WriteLine(value);
        }

        #endregion

        #region async methods

 
        #endregion

        /// <summary>
        /// Disposes the object.
        /// </summary>
        /// <param name="disposing">True if called from Dispose; false if called from the finalizer.</param>
        protected override void Dispose(bool disposing)
        {
            this.textWriter.Dispose();
            base.Dispose(disposing);
        }

        #region private methods
        private void WriteEscapedCharValue(char value)
        {
            JsonValueUtils.WriteValue(this.textWriter, value, escapeOption);
        }

        private void WriteEscapedStringValue(string value)
        {
            JsonValueUtils.WriteEscapedJsonStringValue(this.textWriter, value, escapeOption, ref streamingBuffer, this.bufferPool);
        }

        private void WriteEscapedCharArray(char[] value, int offset, int count)
        {
            JsonValueUtils.WriteEscapedCharArray(this.textWriter, value, offset, count, escapeOption, ref streamingBuffer, this.bufferPool);
        }
        #endregion
    }
}