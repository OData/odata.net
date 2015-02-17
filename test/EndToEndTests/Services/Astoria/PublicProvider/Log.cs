//---------------------------------------------------------------------
// <copyright file="Log.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Services.PublicProvider
{
    using System.Diagnostics;

    /// <summary>
    /// Log class
    /// </summary>
    internal static class Log
    {
        /// <summary>
        /// Trace to debug TraceSource
        /// </summary>
        /// <param name="args">The arguments to trace</param>
        public static void Trace(params  object[] args)
        {
            string callee = GetCallerIdentity(2);
            string caller = GetCallerIdentity(3);
            string message = string.Empty;
            if (args != null)
            {
                message = string.Join(" ", args);
            }
            System.Diagnostics.Trace.WriteLine(string.Format("{0}, {1}, {2}", caller, callee, message));
        }

        /// <summary>
        /// Get the caller identity from the stacktrace
        /// </summary>
        /// <param name="skipFrames">The level of stack frame to skip</param>
        /// <returns>The caller function</returns>
        private static string GetCallerIdentity(int skipFrames)
        {
            var frame = new StackFrame(skipFrames, true);
            if (null != frame.GetMethod() && null != frame.GetMethod().DeclaringType)
            {
                return string.Format("{0}.{1}", frame.GetMethod().DeclaringType.Name, frame.GetMethod().Name);
            }
            return string.Empty;
        }
    }
}