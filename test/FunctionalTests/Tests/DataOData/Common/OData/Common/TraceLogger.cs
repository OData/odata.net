//---------------------------------------------------------------------
// <copyright file="TraceLogger.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Common
{
    using System.Diagnostics;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Implementation of logger which writes to System.Diagnostics.Trace
    /// </summary>
    public class TraceLogger : Logger
    {
        /// <summary>
        /// Writes formatted log message to System.Diagnostics.Trace
        /// </summary>
        /// <param name="logLevel">Log level (ignored)</param>
        /// <param name="text">Formatted log message</param>
        protected override void WriteToOutput(LogLevel logLevel, string text)
        {
            switch (logLevel)
            {
                case LogLevel.Error:
                    Trace.TraceError(text);
                    break;
                case LogLevel.Warning:
                    Trace.TraceWarning(text);
                    break;
                case LogLevel.Info:
                    Trace.TraceInformation(text);
                    break;
                default:
                    Trace.WriteLine(text);
                    break;
            }
        }
    }
}
