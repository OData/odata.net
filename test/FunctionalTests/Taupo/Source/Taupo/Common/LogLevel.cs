//---------------------------------------------------------------------
// <copyright file="LogLevel.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Common
{
    /// <summary>
    /// Available log levels.
    /// </summary>
    public enum LogLevel
    {
        /// <summary>
        /// Trace (very verbose) message,
        /// </summary>
        Trace,

        /// <summary>
        /// Verbose message.
        /// </summary>
        Verbose,

        /// <summary>
        /// Informational message.
        /// </summary>
        Info,

        /// <summary>
        /// Warning message.
        /// </summary>
        Warning,

        /// <summary>
        /// Error message.
        /// </summary>
        Error,
    }
}