//---------------------------------------------------------------------
// <copyright file="DurationLogWriter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Runners
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Execution;

    /// <summary>
    /// Implementation of <see cref="ITestLogWriter"/> which a duration file that looks like this:
    ///     TestItemType, Name, Milliseconds
    ///     V, Protocol test module.Protocol CUD Test Case.DELETE protocol tests.Matrix - Tunnelled=False.Delete an entity, 7572.3291
    ///     V, Protocol test module.Protocol CUD Test Case.DELETE protocol tests.Matrix - Tunnelled=False, 14455.9262
    ///     TC, Protocol test module.Protocol CUD Test Case.DELETE protocol tests, 14487.286
    ///     TM, Protocol test module, 138112.8425
    ///     S, "Suite", 138207.0563
    /// </summary>
    public sealed class DurationLogWriter : ITestLogWriter
    {
        private readonly Dictionary<TestItemData, double> testItemRunningTotalLookup = new Dictionary<TestItemData, double>();
        private readonly Stack<KeyValuePair<TestItemData, DateTimeOffset>> testItemTimeStack = new Stack<KeyValuePair<TestItemData, DateTimeOffset>>();
        private readonly TextWriter writer;
        private DateTimeOffset startTime = DateTimeOffset.MinValue;
        
        /// <summary>
        /// Initializes a new instance of the DurationLogWriter class.
        /// </summary>
        /// <param name="textWriter">The text writer.</param>
        public DurationLogWriter(TextWriter textWriter)
        {
            this.writer = textWriter;
            this.GetCurrentTime = () => DateTimeOffset.Now;
        }

        internal Func<DateTimeOffset> GetCurrentTime { get; set; }

        /// <summary>
        /// Begins the test suite.
        /// </summary>
        public void BeginTestSuite()
        {
            this.testItemTimeStack.Clear();
            this.testItemRunningTotalLookup.Clear();

            this.startTime = this.GetCurrentTime();
            this.writer.WriteLine("TestItemType, Name, Milliseconds");
            this.writer.Flush();
        }

        /// <summary>
        /// Ends the test suite.
        /// </summary>
        public void EndTestSuite()
        {
            DateTimeOffset endTime = this.GetCurrentTime();

            TimeSpan elapsed = endTime - this.startTime;

            this.writer.WriteLine("S, Suite, {0}", elapsed.TotalMilliseconds);
            
            this.writer.Flush();
            this.writer.Close();
        }

        /// <summary>
        /// Begins the test module.
        /// </summary>
        /// <param name="testModule">The test module.</param>
        public void BeginTestModule(TestModuleData testModule)
        {
            this.testItemTimeStack.Push(new KeyValuePair<TestItemData, DateTimeOffset>(testModule, this.GetCurrentTime()));
            this.testItemRunningTotalLookup.Add(testModule, 0); 
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
            this.WriteTestItemDataDurationResult(testModule);
        }

        /// <summary>
        /// Begins the test case.
        /// </summary>
        /// <param name="testCase">The test case.</param>
        public void BeginTestCase(TestItemData testCase)
        {
            this.testItemRunningTotalLookup.Add(testCase, 0); 
            this.testItemTimeStack.Push(new KeyValuePair<TestItemData, DateTimeOffset>(testCase, this.GetCurrentTime()));
        }

        /// <summary>
        /// Ends the test case.
        /// </summary>
        /// <param name="testCase">The test case.</param>
        /// <param name="result">The result.</param>
        /// <param name="exception">The exception.</param>
        public void EndTestCase(TestItemData testCase, TestResult result, ExceptionDetails exception)
        {
            this.WriteTestItemDataDurationResult(testCase);
        }

        /// <summary>
        /// Begins the variation.
        /// </summary>
        /// <param name="variation">The variation.</param>
        public void BeginVariation(TestItemData variation)
        {
            this.testItemTimeStack.Push(new KeyValuePair<TestItemData, DateTimeOffset>(variation, this.GetCurrentTime()));
        }

        /// <summary>
        /// Ends the variation.
        /// </summary>
        /// <param name="variation">The variation.</param>
        /// <param name="result">The result.</param>
        /// <param name="exception">The exception.</param>
        public void EndVariation(TestItemData variation, TestResult result, ExceptionDetails exception)
        {
            this.WriteTestItemDataDurationResult(variation);
        }

        /// <summary>
        /// Writes the line to the test log.
        /// </summary>
        /// <param name="logLevel">The log level.</param>
        /// <param name="message">The message.</param>
        public void WriteLine(LogLevel logLevel, string message)
        {
            // Ignoring all calls as this writer is only focused on writing out test durations
        }

        private void WriteTestItemDataDurationResult(TestItemData testItem)
        {
            var currentTestCaseWithTime = this.testItemTimeStack.Pop();
            ExceptionUtilities.Assert(testItem == currentTestCaseWithTime.Key, "Invalid test case on stack.");

            var elapsed = this.GetCurrentTime() - currentTestCaseWithTime.Value;

            bool writeOutInitTerminateTime = false;

            // Adding information on whether the information recorded is for a Variation - V, TestCase - TC, or TestModule - TM
            string type = "V";
            
            if (testItem.IsTestCase)
            {
                type = "TC";
                writeOutInitTerminateTime = true;
            }
            else if (testItem is TestModuleData)
            {
                type = "TM";
                writeOutInitTerminateTime = true;
            }

            // Getting TestCase Name, escaping the commas as it makes it more difficult
            // to import into excel
            var nestedName = this.GetNestedTestName(currentTestCaseWithTime.Key).Replace(',', ';');
            this.writer.WriteLine("{0}, {1}, {2}", type, nestedName, elapsed.TotalMilliseconds);

            // Add to running total of time if there is a parent
            if (testItem.Parent != null)
            {
                this.testItemRunningTotalLookup[testItem.Parent] = this.testItemRunningTotalLookup[testItem.Parent] + elapsed.TotalMilliseconds;
            }

            if (writeOutInitTerminateTime)
            {
                var runningTotal = this.testItemRunningTotalLookup[testItem];
                var startupTearDownTime = elapsed.TotalMilliseconds - runningTotal;

                // Write out the time that occurs between running the parent test and the child tests
                this.writer.WriteLine("{0}, {1}, {2}", type + "_I-T", nestedName, startupTearDownTime);
            }

            // Remove this item as its unneeded now
            this.testItemRunningTotalLookup.Remove(testItem);

            this.writer.Flush();
        }

        private string GetNestedTestName(TestItemData itemData)
        {
            string name = null;
            var currentItemData = itemData;
            while (currentItemData != null)
            {
                if (name == null)
                {
                    name = currentItemData.Metadata.Name;
                }
                else
                {
                    name = currentItemData.Metadata.Name + "." + name;
                }

                currentItemData = currentItemData.Parent;
            }

            return name;
        }
    }
}
