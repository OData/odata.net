//---------------------------------------------------------------------
// <copyright file="ConsoleLogger.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Utilities
{
    using System;
    using System.Security;
    using System.Security.Permissions;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Logger implementation that outputs to Windows Console.
    /// </summary>
    /// <remarks>
    /// Color-coding is used to distinguish between different log levels.
    /// </remarks>
    public class ConsoleLogger : Logger
    {
        private static object lockObject = new object();

        /// <summary>
        /// Writes formatted log message to console.
        /// </summary>
        /// <param name="logLevel">Log level.</param>
        /// <param name="text">Formatted log message.</param>
        [SecuritySafeCritical]
        [PermissionSet(SecurityAction.Assert, Unrestricted = true)]
        [AssertJustification("Setting Console.ForegroundColor demands UIPermission (SafeTopLevelWindows flag).")]
        protected override void WriteToOutput(LogLevel logLevel, string text)
        {
            lock (lockObject)
            {
                ConsoleColor oldColor = Console.ForegroundColor;

                try
                {
                    switch (logLevel)
                    {
                        case LogLevel.Trace:
                            Console.ForegroundColor = ConsoleColor.DarkGray;
                            break;

                        case LogLevel.Info:
                            Console.ForegroundColor = ConsoleColor.White;
                            break;

                        case LogLevel.Error:
                            Console.ForegroundColor = ConsoleColor.Red;
                            break;

                        case LogLevel.Verbose:
                            Console.ForegroundColor = ConsoleColor.Gray;
                            break;

                        case LogLevel.Warning:
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            break;
                    }

                    Console.WriteLine(text);
                }
                finally
                {
                    Console.ForegroundColor = oldColor;
                }
            }
        }
    }
}
