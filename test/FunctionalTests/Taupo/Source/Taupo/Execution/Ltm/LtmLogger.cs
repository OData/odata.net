//---------------------------------------------------------------------
// <copyright file="LtmLogger.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Execution.Ltm
{
    using System;
    using System.Security;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Utilities;

    /// <summary>
    /// Implementation of <see cref="Logger" /> that writes to LTM console.
    /// </summary>
    public class LtmLogger : Logger
    {
        private ITestLog testLog;

        private LtmLogger()
        {
        }

        /// <summary>
        /// Singleton instance.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")]
        public static readonly LtmLogger Instance = new LtmLogger();

        /// <summary>
        /// Writes formatted log message to LTM output window.
        /// </summary>
        /// <param name="logLevel">Log level</param>
        /// <param name="text">Formatted message</param>
        [SecuritySafeCritical]
        protected override void WriteToOutput(LogLevel logLevel, string text)
        {
            ITestLog ltmLog = GetLtmLog();

            switch (logLevel)
            {
                case LogLevel.Warning:
                    // UGH!!!!!
                    ltmLog.Error(TestResult.Warning, TestLogFlags.Text, null, null, null, text, null, null, 0);
                    break;

                case LogLevel.Error:
                    // UGH!!!!!
                    ltmLog.Error(TestResult.Exception, TestLogFlags.Text, null, null, null, text, null, null, 0);
                    break;

                case LogLevel.Trace:
                case LogLevel.Verbose:
                    ltmLog.WriteLine(TestLogFlags.Ignore, text);
                    break;

                default:
                case LogLevel.Info:
                    ltmLog.WriteLine(TestLogFlags.Text, text);
                    break;
            }
        }

        [SecurityCritical]
        private ITestLog GetLtmLog()
        {
            if (this.testLog == null)
                this.testLog = (ITestLog)new LtmContext();
            return this.testLog;
        }

        [SecuritySafeCritical]
        internal void HandleException(Exception ex, TestResult result)
        {
            ITestLog ltmLog = GetLtmLog();

            while (ex != null)
            {
                DataComparisonException mex = ex as DataComparisonException;
                object expected = null;
                object actual = null;
                if (mex != null)
                {
                    actual = mex.ActualValue;
                    expected = mex.ExpectedValue;
                }

                ltmLog.Error(result, TestLogFlags.Text, ToStringConverter.ConvertObjectToString(actual), ToStringConverter.ConvertObjectToString(expected), ex.Source, ex.Message, ex.StackTrace, null, 0);
                ex = ex.InnerException;
            }
        }
    }
}
