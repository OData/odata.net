//---------------------------------------------------------------------
// <copyright file="TextWriterWrapper.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Json
{
    #region Namespaces
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Text;
    using System.Threading.Tasks;
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
        /// Increases the level of indentation applied to the output asynchronously.
        /// </summary>
        public virtual Task IncreaseIndentationAsync()
        {
            return TaskUtils.CompletedTask;
        }

        /// <summary>
        /// Decreases the level of indentation applied to the output asynchronously.
        /// </summary>
        public virtual Task DecreaseIndentationAsync()
        {
            return TaskUtils.CompletedTask;
        }

        /// <summary>
        /// Clears the buffer of the current writer asynchronously.
        /// </summary>
        public override Task FlushAsync()
        {
            return this.writer.FlushAsync();
        }

        /// <summary>
        /// Closes or disposes the underlying writer.
        /// </summary>
        protected static void InternalCloseOrDispose()
        {
            Debug.Assert(false, "Should never call Close or Dispose on the TextWriter.");

            // This is done to make sure we don't accidentally close or dispose the underlying stream.
            // Since we don't own the stream, we should never close or dispose it.
            throw new NotImplementedException();
        }
    }
}


