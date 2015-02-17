//---------------------------------------------------------------------
// <copyright file="IndentedTextWriter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

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
    /// Writes text indented as per the indentation level setting
    /// </summary>
    internal sealed class IndentedTextWriter : TextWriterWrapper
    {
        /// <summary>
        /// The indentation string to prepand to each line for each indentation level.
        /// </summary>
        private const string IndentationString = "  ";

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
        public IndentedTextWriter(TextWriter writer)
            : base(writer.FormatProvider)
        {
            this.writer = writer;
        }

        /// <summary>
        /// Increases the level of indentation applied to the output.
        /// </summary>
        public override void IncreaseIndentation()
        {
            this.indentLevel++;
        }

        /// <summary>
        /// Decreases the level of indentation applied to the output.
        /// </summary>
        public override void DecreaseIndentation()
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

#if ORCAS
        /// <summary>
        /// Closes the underlying writer.
        /// </summary>
        public override void Close()
        {
            InternalCloseOrDispose();
        }
#endif
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
            base.WriteLine();
            this.indentationPending = true;
        }

        /// <summary>
        /// Writes the tabs depending on the indent level.
        /// </summary>
        private void WriteIndentation()
        {
            if (!this.indentationPending)
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
