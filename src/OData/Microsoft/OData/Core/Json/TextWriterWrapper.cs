//   OData .NET Libraries ver. 6.9
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
namespace Microsoft.OData.Core.Json
#endif
{
    #region Namespaces
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Text;
    #endregion Namespaces

    /// <summary>
    /// Writes text non indented
    /// </summary>
    internal abstract class TextWriterWrapper : TextWriter
    {
        /// <summary>
        /// The underlying writer to write to.
        /// </summary>
        protected TextWriter writer;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="formatProvider">An System.IFormatProvider object that controls formatting.</param>
        protected TextWriterWrapper(IFormatProvider formatProvider)
            : base(formatProvider)
        {
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
        public virtual void IncreaseIndentation()
        {
        }

        /// <summary>
        /// Decreases the level of indentation applied to the output.
        /// </summary>
        public virtual void DecreaseIndentation()
        {
        }

        /// <summary>
        /// Clears the buffer of the current writer.
        /// </summary>
        public override void Flush()
        {
            this.writer.Flush();
        }

        /// <summary>
        /// Closes or disposes the underlying writer.
        /// </summary>
        protected static void InternalCloseOrDispose()
        {
            Debug.Assert(false, "Should never call Close or Dispose on the TextWriter.");

            // This is done to make sure we don't accidently close or dispose the underlying stream.
            // Since we don't own the stream, we should never close or dispose it.
            throw new NotImplementedException();
        }
    }
}


