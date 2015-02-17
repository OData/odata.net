//---------------------------------------------------------------------
// <copyright file="TextWriterLogger.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Utilities
{
    using System.IO;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Logger implementation that writes to a <see cref="TextWriter" />
    /// </summary>
    public class TextWriterLogger : Logger
    {
        private TextWriter textWriter;

        /// <summary>
        /// Initializes a new instance of the TextWriterLogger class for a given <see cref="TextWriter"/>.
        /// </summary>
        /// <param name="textWriter">Text writer to use</param>
        public TextWriterLogger(TextWriter textWriter)
        {
            this.textWriter = textWriter;
        }

        /// <summary>
        /// Writes formatted log message to text writer.
        /// </summary>
        /// <param name="logLevel">Log level (ignored)</param>
        /// <param name="text">Formatted log message</param>
        protected override void WriteToOutput(LogLevel logLevel, string text)
        {
            this.textWriter.WriteLine(text);
        }
    }
}
