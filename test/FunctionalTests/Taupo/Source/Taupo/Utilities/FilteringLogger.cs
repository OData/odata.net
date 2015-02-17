//---------------------------------------------------------------------
// <copyright file="FilteringLogger.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Utilities
{
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Filtering logger which forwards log messages of certain level and above to the another logger.
    /// </summary>
    public class FilteringLogger : Logger
    {
        /// <summary>
        /// Initializes a new instance of the FilteringLogger class.
        /// </summary>
        /// <param name="underlyingLogger">The underlying logger.</param>
        /// <param name="minimumLevel">The minimum level of messages to be forwarded to the underlying logger.</param>
        public FilteringLogger(Logger underlyingLogger, LogLevel minimumLevel)
        {
            this.UnderlyingLogger = underlyingLogger;
            this.MinimumLevel = minimumLevel;
        }

        /// <summary>
        /// Gets the underlying logger.
        /// </summary>
        /// <value>The underlying logger.</value>
        public Logger UnderlyingLogger { get; private set; }

        /// <summary>
        /// Gets or sets the minimum log level of messages that should be forwarded to the underlying logger.
        /// </summary>
        /// <value>The minimum level.</value>
        public LogLevel MinimumLevel { get; set; }

        /// <summary>
        /// Writes formatted text to the log output.
        /// </summary>
        /// <param name="logLevel">Log level.</param>
        /// <param name="text">Formatted text</param>
        protected override void WriteToOutput(LogLevel logLevel, string text)
        {
            if (logLevel >= this.MinimumLevel)
            {
                this.UnderlyingLogger.WriteLine(logLevel, text);
            }
        }
    }
}
