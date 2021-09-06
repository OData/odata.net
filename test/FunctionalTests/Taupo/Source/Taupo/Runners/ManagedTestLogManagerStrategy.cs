//---------------------------------------------------------------------
// <copyright file="ManagedTestLogManagerStrategy.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Runners
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// A test logging strategy for Managed applications
    /// </summary>
    public class ManagedTestLogManagerStrategy : ITestLogManagerStrategy
    {
        private Dictionary<CompositeTestLogWriter, List<TextWriter>> compositeToTextWritersLookup = new Dictionary<CompositeTestLogWriter, List<TextWriter>>();

        /// <summary>
        /// Initializes the runner strategy and returns a test logger
        /// </summary>
        /// <param name="runnerStrategy">The Runner strategy for the corresponding Taupo test runner</param>
        /// <param name="writers"> Any additional writers to add to the loggers</param>
        /// <returns>A test log writer to log results from this test execution</returns>
        public ITestLogWriter Initialize(RunnerStrategy runnerStrategy, params ITestLogWriter[] writers)
        {
            ExceptionUtilities.CheckArgumentNotNull(runnerStrategy, "runnerStrategy");

            List<TextWriter> textWriters = new List<TextWriter>();
            List<ITestLogWriter> testLogWriters = new List<ITestLogWriter>();
            testLogWriters.AddRange(writers);

            string resultFileName = null;
            if (runnerStrategy.Parameters.TryGetValue("resultfile", out resultFileName))
            {
                TextWriter resultsTextWriter = runnerStrategy.CreateLogTextWriter(resultFileName);
                textWriters.Add(resultsTextWriter);
                TextWriterLogWriter textWriterLog = new TextWriterLogWriter(resultsTextWriter);
                textWriterLog.MaxLogLevel = LogLevel.Info;
                testLogWriters.Add(textWriterLog);
            }

            string logFile = null;
            if (runnerStrategy.Parameters.TryGetValue("logFile", out logFile))
            {
                TextWriter xmlLogWriter = runnerStrategy.CreateLogTextWriter(logFile);
                textWriters.Add(xmlLogWriter);
                var ltmLogger = new LtmXmlLogWriter(xmlLogWriter);
                testLogWriters.Add(ltmLogger);
            }

            string durationFile = null;
            if (runnerStrategy.Parameters.TryGetValue("durationFile", out durationFile))
            {
                var durationTextWriter = runnerStrategy.CreateLogTextWriter(durationFile);
                textWriters.Add(durationTextWriter);
                var durationLogger = new DurationLogWriter(durationTextWriter);
                testLogWriters.Add(durationLogger);
            }

            CompositeTestLogWriter compositeTestLogWriter = new CompositeTestLogWriter(testLogWriters.ToArray());

            this.compositeToTextWritersLookup.Add(compositeTestLogWriter, textWriters);

            return compositeTestLogWriter;
        }

        /// <summary>
        /// Safely disposes the test log writer
        /// </summary>
        /// <param name="testLogWriter">The test log writer to dispose</param>
        public void Terminate(ITestLogWriter testLogWriter)
        {
            List<TextWriter> textWriters = this.compositeToTextWritersLookup[testLogWriter as CompositeTestLogWriter];

            foreach (TextWriter writer in textWriters)
            {
                writer.Dispose();
            }
        }
    }
}
