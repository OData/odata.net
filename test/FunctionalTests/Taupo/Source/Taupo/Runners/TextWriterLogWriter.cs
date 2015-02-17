//---------------------------------------------------------------------
// <copyright file="TextWriterLogWriter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Runners
{
    using System.IO;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Implementation of <see cref="ITestLogWriter"/> which writes log file
    /// to the specified TextWriter.
    /// </summary>
    public class TextWriterLogWriter : PlaintextLogWriterBase
    {
        private TextWriter output;
        
        /// <summary>
        /// Initializes a new instance of the TextWriterLogWriter class.
        /// </summary>
        /// <param name="output">The output.</param>
        public TextWriterLogWriter(TextWriter output)
        {
            this.output = output;
            this.MaxLogLevel = LogLevel.Trace;
        }

        /// <summary>
        /// Gets or sets the LogLevel
        /// </summary>
        public LogLevel MaxLogLevel { get; set; }

        /// <summary>
        /// Writes the specified text to the output.
        /// </summary>
        /// <param name="severity">The severity.</param>
        /// <param name="indentLevel">The indentation level.</param>
        /// <param name="text">The text to be written.</param>
        protected override void WriteToOutput(LogLevel severity, int indentLevel, string text)
        {
            if (this.ShouldWrite(severity))
            {
                this.output.WriteLine(new string(' ', indentLevel) + text);
                this.output.Flush();
            }
        }

        /// <summary>
        /// Writes the specified status text to the output.
        /// </summary>
        /// <param name="severity">The severity.</param>
        /// <param name="indentLevel">The indentation level.</param>
        /// <param name="text">The text to be written.</param>
        protected override void WriteStatusText(LogLevel severity, int indentLevel, string text)
        {
            if (this.ShouldWrite(severity))
            {
                this.output.WriteLine(new string(' ', indentLevel) + text);
                this.output.Flush();
            }
        }

        private bool ShouldWrite(LogLevel logLevel)
        {
            if (logLevel >= LogLevel.Info)
            {
                return true;
            }

            return false;
        }
    }
}
