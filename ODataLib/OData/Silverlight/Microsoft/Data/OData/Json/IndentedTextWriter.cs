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

#if SPATIAL
namespace Microsoft.Data.Spatial
#else
namespace Microsoft.Data.OData.Json
#endif
{
    #region Namespaces
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Text;
    #endregion Namespaces

    /// <summary>
    /// Writes text indented as per the indentation level setting
    /// </summary>
    internal sealed class IndentedTextWriter : TextWriter
    {
        /// <summary>
        /// The indentation string to prepand to each line for each indentation level.
        /// </summary>
        private const string IndentationString = "  ";

        /// <summary>
        /// The underlying writer to write to.
        /// </summary>
        private readonly TextWriter writer;

        /// <summary>
        /// Set to true if the writer should actually indent or not.
        /// </summary>
        private readonly bool enableIndentation;

        /// <summary>
        /// Number which specifies the level of indentation. Starts with 0 which means no indentation.
        /// </summary>
        private int indentLevel;

        /// <summary>
        /// Set to true if indentation should be written before the next string is written.
        /// </summary>
        private bool indentationPending;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="writer">The underlying writer to wrap.</param>
        /// <param name="enableIndentation">Set to true if the writer should actually indent or not.</param>
        public IndentedTextWriter(TextWriter writer, bool enableIndentation)
            : base(writer.FormatProvider)
        {
            this.writer = writer;
            this.enableIndentation = enableIndentation;
        }

        /// <summary>
        /// Returns the Encoding for the given writer.
        /// </summary>
        public override Encoding Encoding
        {
            get
            {
                return this.writer.Encoding;
            }
        }

        /// <summary>
        /// Returns the new line character.
        /// </summary>
        public override string NewLine
        {
            get
            {
                return this.writer.NewLine;
            }
        }

        /// <summary>
        /// Increases the level of indentation applied to the output.
        /// </summary>
        public void IncreaseIndentation()
        {
            this.indentLevel++;
        }

        /// <summary>
        /// Decreases the level of indentation applied to the output.
        /// </summary>
        public void DecreaseIndentation()
        {
            Debug.Assert(this.indentLevel > 0, "Trying to decrease indentation below zero.");
            if (this.indentLevel < 1)
            {
                this.indentLevel = 0;
            }
            else
            {
                this.indentLevel--;
            }
        }

#if !PORTABLELIB
        /// <summary>
        /// Closes the underlying writer.
        /// </summary>
        public override void Close()
        {
            InternalCloseOrDispose();
        }
#endif

        /// <summary>
        /// Clears the buffer of the current writer.
        /// </summary>
        public override void Flush()
        {
            this.writer.Flush();
        }

        /// <summary>
        /// Writes the given string value to the underlying writer.
        /// </summary>
        /// <param name="s">String value to be written.</param>
        public override void Write(string s)
        {
            this.WriteIndentation();
            this.writer.Write(s);
        }

        /// <summary>
        /// Writes the given char value to the underlying writer.
        /// </summary>
        /// <param name="value">Char value to be written.</param>
        public override void Write(char value)
        {
            this.WriteIndentation();
            this.writer.Write(value);
        }

        /// <summary>
        /// Writes a new line.
        /// </summary>
        public override void WriteLine()
        {
            if (this.enableIndentation)
            {
                base.WriteLine();
            }

            this.indentationPending = true;
        }

        /// <summary>
        /// Closes or disposes the underlying writer.
        /// </summary>
        private static void InternalCloseOrDispose()
        {
            Debug.Assert(false, "Should never call Close or Dispose on the TextWriter.");

            // This is done to make sure we don't accidently close or dispose the underlying stream.
            // Since we don't own the stream, we should never close or dispose it.
            throw new NotImplementedException();
        }

        /// <summary>
        /// Writes the tabs depending on the indent level.
        /// </summary>
        private void WriteIndentation()
        {
            if (!this.enableIndentation || !this.indentationPending)
            {
                return;
            }

            for (int i = 0; i < this.indentLevel; i++)
            {
                this.writer.Write(IndentationString);
            }

            this.indentationPending = false;
        }
    }
}
