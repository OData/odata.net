//---------------------------------------------------------------------
// <copyright file="Logger.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Common
{
    using System;
    using System.Globalization;

    /// <summary>
    /// Logger used for printing diagnostics messages.
    /// </summary>
    public abstract class Logger
    {
        /// <summary>
        /// Gets null logger which discards log messages.
        /// </summary>
        public static Logger Null
        {
            get { return NullLogger.Instance; }
        }

        /// <summary>
        /// Writes the specified string to diagnostics output.
        /// </summary> 
        /// <param name="level">Log level.</param>
        /// <param name="text">Text to write</param>
        public void WriteLine(LogLevel level, string text)
        {
            this.WriteToOutput(level, text);
        }

        /// <summary>
        /// Writes the specified formatted string to diagnostics output.
        /// </summary> 
        /// <param name="level">Log level.</param>
        /// <param name="text">Text to write</param>
        /// <param name="arguments">format string parameters (Same as string.Format())</param>
        public void WriteLine(LogLevel level, string text, params object[] arguments)
        {
            this.WriteToOutput(level, string.Format(CultureInfo.InvariantCulture, text, arguments));
        }

        /// <summary>
        /// Writes the specified formatted string to diagnostics output.
        /// </summary> 
        /// <param name="level">Log level.</param>
        /// <param name="formatProvider">Format provider to be used</param>
        /// <param name="text">Text to write</param>
        /// <param name="arguments">format string parameters (Same as string.Format())</param>
        public void WriteLine(LogLevel level, IFormatProvider formatProvider, string text, params object[] arguments)
        {
            this.WriteToOutput(level, string.Format(formatProvider, text, arguments));
        }

        /// <summary>
        /// Writes formatted text to the log output.
        /// </summary>
        /// <param name="logLevel">Log level.</param>
        /// <param name="text">Formatted text</param>
        protected abstract void WriteToOutput(LogLevel logLevel, string text);

        #region Null Logger

        /// <summary>
        /// Null logger which discards all messages.
        /// </summary>
        private class NullLogger : Logger
        {
            /// <summary>
            /// Initializes static members of the NullLogger class.
            /// </summary>
            static NullLogger()
            {
                Instance = new NullLogger();
            }

            private NullLogger()
            {
            }

            /// <summary>
            /// Gets singleton instance of <see cref="Logger"/>.
            /// </summary>
            public static Logger Instance { get; private set; }

            /// <summary>
            /// Does nothing
            /// </summary>
            /// <param name="logLevel">Log level (Ignored)</param>
            /// <param name="text">Formatted log message (Ignored)</param>
            protected override void WriteToOutput(LogLevel logLevel, string text)
            {
            }
        }
        #endregion
    }
}