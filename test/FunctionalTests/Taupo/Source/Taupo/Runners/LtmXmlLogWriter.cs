//---------------------------------------------------------------------
// <copyright file="LtmXmlLogWriter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Runners
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Xml;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Execution;

    /// <summary>
    /// Implementation of <see cref="ITestLogWriter"/> which creates LTM-compatible
    /// XML log file.
    /// </summary>
    public sealed class LtmXmlLogWriter : ITestLogWriter 
    {
        private Stack<TestItemData> testItemStack = new Stack<TestItemData>();
        private DateTime startTime = DateTime.MinValue;
        private XmlWriter writer;

        /// <summary>
        /// Initializes a new instance of the LtmXmlLogWriter class.
        /// </summary>
        /// <param name="textWriter">The text writer.</param>
        public LtmXmlLogWriter(TextWriter textWriter)
        {
            var writterSettings = new XmlWriterSettings { Indent = true, CloseOutput = true, CheckCharacters = false };

            this.writer = XmlWriter.Create(textWriter, writterSettings);
            this.GetCurrentTime = () => DateTime.Now;
            this.ExceptionFormatter = FormatException;
        }

        internal Func<ExceptionDetails, string> ExceptionFormatter { get; set; }

        internal Func<DateTime> GetCurrentTime { get; set; }

        /// <summary>
        /// Begins the test suite.
        /// </summary>
        public void BeginTestSuite()
        {
            this.testItemStack.Clear();

            this.writer.WriteStartElement("Ltm");

            this.writer.WriteStartElement("Suite");
            this.writer.WriteAttributeString("Name", "DefaultSuite");
            this.startTime = this.GetCurrentTime();
            this.writer.WriteAttributeString("StartTime", FormatDate(this.startTime));
            this.writer.WriteEndElement();

            this.writer.WriteStartElement("Alias");

            this.writer.WriteStartElement("Client");
            this.writer.WriteEndElement();

            this.writer.Flush();
        }

        /// <summary>
        /// Ends the test suite.
        /// </summary>
        public void EndTestSuite()
        {
            // TODO: assert test-item stack is empty?
            this.writer.WriteEndElement(); // Alias

            DateTime endTime = this.GetCurrentTime();

            this.writer.WriteStartElement("SuiteEnd");
            this.writer.WriteAttributeString("Name", "DefaultSuite");
            this.writer.WriteAttributeString("Status", "Complete");
            TimeSpan elapsed = endTime - this.startTime;
            string runTime = string.Format(CultureInfo.InvariantCulture, "{0:00}:{1:00}:{2:00}", elapsed.Hours, elapsed.Minutes, elapsed.Seconds);
            this.writer.WriteAttributeString("RunTime", runTime);
            this.writer.WriteAttributeString("StopTime", FormatDate(endTime));
            this.writer.WriteEndElement(); // SuiteEnd

            this.writer.WriteEndElement(); // Ltm
            this.writer.Flush();
            this.writer.Close();
        }

        /// <summary>
        /// Begins the test module.
        /// </summary>
        /// <param name="testModule">The test module.</param>
        public void BeginTestModule(TestModuleData testModule)
        {
            this.testItemStack.Push(testModule);
            var metadata = testModule.Metadata;

            // <Module  Name="System.Data.BVT"  Desc=string.Empty Version="00.00.0000.00"  Pri="0"  Variations="2"  Owner="nvalluri"  Inheritance="True"  Implemented="True"  Skipped="False"  Error="False"  Manual="False"  Security="0"  Stress="False"  Timeout="0"  Threads="1"  Repeat="0" >
            this.writer.WriteStartElement("Module");
            this.writer.WriteAttributeString("Name", metadata.Name);
            this.writer.WriteAttributeString("Desc", metadata.Description);
            this.writer.WriteAttributeString("Version", metadata.Version);
            this.writer.WriteAttributeString("Pri", XmlConvert.ToString(metadata.Priority));
            this.writer.WriteAttributeString("Owner", metadata.Owner);
            this.writer.WriteString("\r\n");
            this.writer.Flush();
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
            TestModuleData currentTestModule = this.testItemStack.Pop() as TestModuleData;
            ExceptionUtilities.Assert(currentTestModule == testModule, "Invalid test module on stack.");

            this.WriteResult(result, exception);

            this.writer.WriteEndElement();

            this.writer.WriteStartElement("Summary");
            this.OutputCounts(globalStatistics);

            foreach (var kvp in statisticsByPriority.OrderBy(c => c.Key))
            {
                this.writer.WriteStartElement("Property");
                this.OutputCounts(kvp.Value);

                this.writer.WriteAttributeString("Name", "Pri");
                this.writer.WriteAttributeString("Value", XmlConvert.ToString(kvp.Key));
                this.writer.WriteEndElement();
            }

            this.writer.WriteEndElement();
            this.writer.Flush();
        }

        /// <summary>
        /// Begins the test case.
        /// </summary>
        /// <param name="testCase">The test case.</param>
        public void BeginTestCase(TestItemData testCase)
        {
            this.testItemStack.Push(testCase);
            var metadata = testCase.Metadata;

            this.writer.WriteStartElement("TestCase");
            this.writer.WriteAttributeString("Name", metadata.Name);
            this.writer.WriteAttributeString("Desc", metadata.Description);
            this.writer.WriteString("\r\n");
            this.writer.Flush();
        }

        /// <summary>
        /// Ends the test case.
        /// </summary>
        /// <param name="testCase">The test case.</param>
        /// <param name="result">The result.</param>
        /// <param name="exception">The exception.</param>
        public void EndTestCase(TestItemData testCase, TestResult result, ExceptionDetails exception)
        {
            var currentTestCase = this.testItemStack.Pop();
            ExceptionUtilities.Assert(testCase == currentTestCase, "Invalid test case on stack.");

            // Working around Aruba LTM driver bug with nested test cases. When we write an
            // end element for TestCase, the driver thinks we are now writing data about the
            // TestModule. This assumption is invalid because we could be processing nested TestCases.
            // If we have violated this assumption then writing the Result element will cause the
            // driver to think that this is the result for the TestModule and therefore stop processing
            // the log file. This workaround gets around most cases except for when a failure/warning/skip
            // occurs in TestCase.Terminate, or when a skip occurs in TestCase.Init.
            if (result != TestResult.Passed)
            {
                this.WriteResult(result, exception);
            }

            this.writer.WriteEndElement();
            this.writer.WriteString("\r\n");
            this.writer.Flush();
        }

        /// <summary>
        /// Begins the variation.
        /// </summary>
        /// <param name="variation">The variation.</param>
        public void BeginVariation(TestItemData variation)
        {
            this.testItemStack.Push(variation);
            this.writer.WriteStartElement("Variation");
            this.writer.WriteAttributeString("Pri", XmlConvert.ToString(variation.Metadata.Priority));
            this.writer.WriteAttributeString("Id", XmlConvert.ToString(variation.Metadata.Id));
            this.writer.WriteAttributeString("Desc", variation.Metadata.Description);
            this.writer.WriteString("\r\n");
            this.writer.Flush();
        }

        /// <summary>
        /// Ends the variation.
        /// </summary>
        /// <param name="variation">The variation.</param>
        /// <param name="result">The result.</param>
        /// <param name="exception">The exception.</param>
        public void EndVariation(TestItemData variation, TestResult result, ExceptionDetails exception)
        {
            var currentVariation = this.testItemStack.Pop();
            ExceptionUtilities.Assert(currentVariation == variation, "Invalid test case on stack.");

            this.WriteResult(result, exception);
            this.writer.WriteEndElement();
            this.writer.WriteString("\r\n");
            this.writer.Flush();
        }

        /// <summary>
        /// Writes the line to the test log.
        /// </summary>
        /// <param name="logLevel">The log level.</param>
        /// <param name="message">The message.</param>
        public void WriteLine(LogLevel logLevel, string message)
        {
            // calls to writeline before BeginSuite are ignored because the resulting XML is invalid
            // this is primarily for cases where the composite writer will contain both a console-writer and this writer
            // so its valid for one and not for the other
            if (this.testItemStack.Count == 0)
            {
                return;
            }

            string text = EncodeXmlString(message) + "\r\n";

            if (logLevel >= LogLevel.Warning)
            {
                this.writer.WriteStartElement("Compare");
                this.writer.WriteAttributeString("Message", message);
                this.writer.WriteElementString("Result", logLevel.ToString().ToUpperInvariant());
                this.writer.WriteEndElement();
            }
            else if (logLevel < LogLevel.Info)
            {
                this.writer.WriteElementString("Ignore", text);
            }
            else
            {
                this.writer.WriteString(text);
            }

            this.writer.Flush();
        }

        private static string EncodeXmlString(string toEncode)
        {
            return toEncode.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;").Replace("\"", "&quot;").Replace("'", "&apos;");
        }

        private static string FormatDate(DateTime dateTime)
        {
            return dateTime.ToString();
        }

        private static string FormatException(ExceptionDetails ex)
        {
            return ex.ExceptionAsString;
        }

        private void WriteResult(TestResult result, ExceptionDetails exception)
        {
            if (result != TestResult.Passed && exception != null)
            {
                this.writer.WriteStartElement("Compare");
                this.writer.WriteAttributeString("Message", exception.Message);
                if (!string.IsNullOrEmpty(exception.Source))
                {
                    this.writer.WriteAttributeString("Source", exception.Source);
                }

                this.writer.WriteStartElement("Details");
                this.writer.WriteCData(this.ExceptionFormatter(exception));
                this.writer.WriteEndElement(); // Details

                this.writer.WriteElementString("Result", result.ToString().ToUpperInvariant());
                this.writer.WriteEndElement(); // Compare
            }

            this.writer.WriteElementString("Result", result.ToString().ToUpperInvariant());
        }
        
        private void OutputCounts(RunStatistics statistics)
        {
            int variationsExecuted = statistics.Failures + statistics.Passed + statistics.Timeouts + statistics.Warnings;
            int successfulVariations = statistics.Passed + statistics.Warnings;
            double passRate = 0.0;

            if (variationsExecuted != 0)
            {
                passRate = Math.Round(100.0 * successfulVariations / variationsExecuted);
            }

            this.OutputSingleCount("Failures", statistics.Failures);
            this.OutputSingleCount("Warnings", statistics.Warnings);
            this.OutputSingleCount("Skipped", statistics.Skipped);
            this.OutputSingleCount("Passed", statistics.Passed);
            this.OutputSingleCount("Timeouts", statistics.Timeouts);

            this.writer.WriteAttributeString("Run", XmlConvert.ToString(variationsExecuted));
            this.writer.WriteAttributeString("Percent", XmlConvert.ToString(passRate));
        }

        private void OutputSingleCount(string attributeName, int value)
        {
            this.writer.WriteAttributeString(attributeName, XmlConvert.ToString(value));
        }
    }
}
