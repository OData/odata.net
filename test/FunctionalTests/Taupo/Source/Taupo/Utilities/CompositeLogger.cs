//---------------------------------------------------------------------
// <copyright file="CompositeLogger.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Utilities
{
    using System.IO;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Logger implementation that writes to multiple underlying loggers at the same time.
    /// </summary>
    public class CompositeLogger : Logger
    {
        private Logger[] underlyingLogger;

        /// <summary>
        /// Initializes a new instance of the CompositeLogger class.
        /// </summary>
        /// <param name="underlyingLoggers">The underlying loggers.</param>
        public CompositeLogger(params Logger[] underlyingLoggers)
        {
            ExceptionUtilities.CheckCollectionNotEmpty(underlyingLoggers, "underlyingLoggers");
            this.underlyingLogger = underlyingLoggers;
        }

        /// <summary>
        /// Writes formatted log message to all underlying loggers.
        /// </summary>
        /// <param name="logLevel">Log level (ignored)</param>
        /// <param name="text">Formatted log message</param>
        protected override void WriteToOutput(LogLevel logLevel, string text)
        {
            foreach (var logger in this.underlyingLogger)
            {
                logger.WriteLine(logLevel, text);
            }
        }
    }
}
