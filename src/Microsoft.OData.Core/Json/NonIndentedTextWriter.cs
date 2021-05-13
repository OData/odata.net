//---------------------------------------------------------------------
// <copyright file="NonIndentedTextWriter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.IO;
using System.Threading.Tasks;

namespace Microsoft.OData.Json
{
    /// <summary>
    /// Writes text indented as per the indentation level setting
    /// </summary>
    internal sealed class NonIndentedTextWriter : TextWriterWrapper
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="writer">The underlying writer to wrap.</param>
        public NonIndentedTextWriter(TextWriter writer)
            : base(writer.FormatProvider)
        {
            this.writer = writer;
        }

        /// <summary>
        /// Writes the given string value to the underlying writer.
        /// </summary>
        /// <param name="s">String value to be written.</param>
        public override void Write(string s)
        {
            this.writer.Write(s);
        }

        /// <summary>
        /// Writes the given string value to the underlying writer asynchronously.
        /// </summary>
        /// <param name="s">String value to be written.</param>
        /// <returns>A task that represents the asynchronous write operation.</returns>
        public override Task WriteAsync(string s)
        {
            return this.writer.WriteAsync(s);
        }

        /// <summary>
        /// Writes the given char value to the underlying writer.
        /// </summary>
        /// <param name="value">Char value to be written.</param>
        public override void Write(char value)
        {
            this.writer.Write(value);
        }

        /// <summary>
        /// Writes the given char value to the underlying writer asynchronously.
        /// </summary>
        /// <param name="value">Char value to be written.</param>
        /// <returns>A task that represents the asynchronous write operation.</returns>
        public override Task WriteAsync(char value)
        {
            return this.writer.WriteAsync(value);
        }

        /// <summary>
        /// Writes a new line.
        /// </summary>
        public override void WriteLine()
        {
        }

        /// <summary>
        /// Writes a new line asynchronously.
        /// </summary>
        /// <returns>A task that represents the asynchronous write operation.</returns>
        public override Task WriteLineAsync()
        {
            return TaskUtils.CompletedTask;
        }
    }
}
