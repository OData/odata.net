//---------------------------------------------------------------------
// <copyright file="StringCollectionLogger.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Utilities
{
    using System.Collections.Generic;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Logger implementation that appends logged strings to a given collection.
    /// </summary>
    public class StringCollectionLogger : Logger
    {
        private ICollection<string> output;

        /// <summary>
        /// Initializes a new instance of the StringCollectionLogger class on a given collection.
        /// </summary>
        /// <param name="output">Collection to output log messages to</param>
        public StringCollectionLogger(ICollection<string> output)
        {
            this.output = output;
        }

        /// <summary>
        /// Appends specified log message to the collection.
        /// </summary>
        /// <param name="logLevel">Log level (ignored)</param>
        /// <param name="text">Formatted message</param>
        protected override void WriteToOutput(LogLevel logLevel, string text)
        {
            this.output.Add(text);
        }
    }
}
