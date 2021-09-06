//---------------------------------------------------------------------
// <copyright file="PlaintextLogWriterBase.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Runners
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Security;
    using System.Security.Permissions;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Execution;

    /// <summary>
    /// Implementation of <see cref="ITestLogWriter"/> which creates human-readable
    /// indented test log.
    /// </summary>
    public abstract class PlaintextLogWriterBase : ITestLogWriter
    {
        private const int IndentStep = 2;
        private int currentIndentLevel;

        /// <summary>
        /// Initializes a new instance of the PlaintextLogWriterBase class.
        /// </summary>
        protected PlaintextLogWriterBase()
        {
            this.ExceptionFormatter = FormatException;
        }

        /// <summary>
        /// Gets or sets the function which formats exception information.
        /// </summary>
        internal Func<ExceptionDetails, string> ExceptionFormatter { get; set; }

        /// <summary>
        /// Begins the test suite.
        /// </summary>
        public void BeginTestSuite()
        {
            this.WriteStatusText(LogLevel.Info, this.currentIndentLevel, "Test Suite Starting");
        }

        /// <summary>
        /// Ends the test suite.
        /// </summary>
        public void EndTestSuite()
        {
            this.WriteStatusText(LogLevel.Info, this.currentIndentLevel, "Test Suite Completed.");
        }

        /// <summary>
        /// Begins the test module.
        /// </summary>
        /// <param name="testModule">The test module.</param>
        public void BeginTestModule(TestModuleData testModule)
        {
            this.WriteBegin(testModule);
        }

        /// <summary>
        /// Ends the test module.
        /// </summary>
        /// <param name="testModule">The test module.</param>
        /// <param name="result">The result.</param>
        /// <param name="exception">The exception.</param>
        /// <param name="statisticsByPriority">The statistics for the module, broken down by priority.</param>
        /// <param name="globalStatistics">The global statistics for the module.</param>
        public void EndTestModule(TestModuleData testModule, TestResult result, ExceptionDetails exception, IDictionary<int, RunStatistics> statisticsByPriority, RunStatistics globalStatistics)
        {
            this.WriteResult(testModule, result, exception);
        }

        /// <summary>
        /// Begins the test case.
        /// </summary>
        /// <param name="testCase">The test case.</param>
        public void BeginTestCase(TestItemData testCase)
        {
            this.WriteBegin(testCase);
        }

        /// <summary>
        /// Ends the test case.
        /// </summary>
        /// <param name="testCase">The test case.</param>
        /// <param name="result">The result.</param>
        /// <param name="exception">The exception.</param>
        public void EndTestCase(TestItemData testCase, TestResult result, ExceptionDetails exception)
        {
            this.WriteResult(testCase, result, exception);
        }

        /// <summary>
        /// Begins the variation.
        /// </summary>
        /// <param name="variation">The variation.</param>
        public void BeginVariation(TestItemData variation)
        {
            this.WriteBegin(variation);
        }

        /// <summary>
        /// Ends the variation.
        /// </summary>
        /// <param name="variation">The variation.</param>
        /// <param name="result">The result.</param>
        /// <param name="exception">The exception.</param>
        public void EndVariation(TestItemData variation, TestResult result, ExceptionDetails exception)
        {
            this.WriteResult(variation, result, exception);
        }

        /// <summary>
        /// Writes the line to the test log.
        /// </summary>
        /// <param name="logLevel">The log level.</param>
        /// <param name="message">The message.</param>
        public void WriteLine(LogLevel logLevel, string message)
        {
            this.WriteToOutput(logLevel, this.currentIndentLevel, message);
        }

        /// <summary>
        /// Writes the specified text to the output.
        /// </summary>
        /// <param name="severity">The severity.</param>
        /// <param name="indentLevel">The indentation level.</param>
        /// <param name="text">The text to be written.</param>
        protected abstract void WriteToOutput(LogLevel severity, int indentLevel, string text);

        /// <summary>
        /// Writes the specified status text to the output.
        /// </summary>
        /// <param name="severity">The severity.</param>
        /// <param name="indentLevel">The indentation level.</param>
        /// <param name="text">The text to be written.</param>
        protected abstract void WriteStatusText(LogLevel severity, int indentLevel, string text);

        [SecuritySafeCritical]
        [PermissionSet(SecurityAction.Assert, Unrestricted = true)]
        [AssertJustification("Some exceptions (such as SecurityExceptions) need elevated permissions to read all of their information.")]
        private static string FormatException(ExceptionDetails ex)
        {
            return ex.ExceptionAsString;
        }

        private void WriteBegin(TestItemData testItem)
        {
            this.WriteStatusText(LogLevel.Info, this.currentIndentLevel, string.Format(CultureInfo.InvariantCulture, "{0} '{1}' Starting", this.GetItemTypeName(testItem), testItem.Metadata.Description));
            this.Indent();
        }

        private void WriteResult(TestItemData testItem, TestResult result, ExceptionDetails exception)
        {
            if (exception != null)
            {
                this.WriteStatusText(LogLevel.Error, this.currentIndentLevel, this.ExceptionFormatter(exception));
            }

            this.Unindent();
            this.WriteStatusText(LogLevel.Info, this.currentIndentLevel, string.Format(CultureInfo.InvariantCulture, "{0} '{1}' {2}", this.GetItemTypeName(testItem), testItem.Metadata.Description, result.ToString()));
        }

        private string GetItemTypeName(TestItemData testItem)
        {
            string itemKind = string.Empty;
            if (testItem.IsVariation)
            {
                itemKind = "Variation";
            }
            else if (testItem.IsTestCase)
            {
                itemKind = "Test Case";
            }
            else if (testItem is TestModuleData)
            {
                itemKind = "Test Module";
            }

            return itemKind;
        }

        private void Indent()
        {
            this.currentIndentLevel += IndentStep;
        }

        private void Unindent()
        {
            this.currentIndentLevel -= IndentStep;
        }
    }
}
