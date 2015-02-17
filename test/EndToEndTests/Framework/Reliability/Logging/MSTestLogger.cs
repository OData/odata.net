//---------------------------------------------------------------------
// <copyright file="MSTestLogger.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Framework.Reliability
{
    using System;
    using System.Diagnostics;
    using Microsoft.Test.DataDriven.ResultsManagement;
    using Microsoft.VisualStudio.TestTools.Common;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// The MStestlogger
    /// </summary>
    public class MsTestLogger : ILogger
    {
        /// <summary>
        /// The logger id of the MsTestLogger
        /// </summary>
        private static readonly Guid LoggerID = Guid.NewGuid();

        /// <summary>
        /// Test Context
        /// </summary>
        private static TestContext testContext;

        /// <summary>
        /// Gets the logger id
        /// </summary>
        public Guid ID
        {
            get { return LoggerID; }
        }

        internal static bool DelayAssert { get; set; }

        /// <summary>
        /// Attach the test context
        /// </summary>
        /// <param name="context">The test context</param>
        public static void Attach(TestContext context)
        {
            testContext = context;
        }

        /// <summary>
        /// Add file
        /// </summary>
        /// <param name="callid">The call id</param>
        /// <param name="filepath">The file path</param>
        public void AddFile(string callid, string filepath)
        {
            if (testContext != null)
            {
                testContext.AddResultFile(filepath);
            }
        }

        /// <summary>
        /// Log a message
        /// </summary>
        /// <param name="type">Message type</param>
        /// <param name="callid">Message callid</param>
        /// <param name="msg">Message txt</param>
        public void Log(TraceEventType type, string callid, string msg)
        {
            if (type != TraceEventType.Transfer)
            {
                if (type == TraceEventType.Start)
                {
                    DelayAssert = false;
                }

                var testResultContext = TestResultContext.CurrentContext;
                if (type == TraceEventType.Error)
                {
                    // If in data driven context, mark the result directly
                    if (testResultContext.ResultStackDepth == 1)
                    {
                        DelayAssert = true;
                    }
                    else
                    {
                        testResultContext.CurrentResult.Outcome = TestOutcome.Failed;
                    }
                }

                if (testResultContext.ResultStackDepth == 1 && testContext != null)
                {
                    testContext.WriteLine("{0} {1} {2}", type.ToString().ToBracketString(), callid.ToBracketString(), msg);
                }
                else
                {
                    Console.WriteLine("{0} {1} {2}", type.ToString().ToBracketString(), callid.ToBracketString(), msg);
                }

                if (type == TraceEventType.Stop)
                {
                    if (NeedDelayAssert())
                    {
                        DelayAssert = false;
                        throw new AssertFailedException("Test failed.");
                    }
                }
            }
        }

        internal static bool NeedDelayAssert()
        {
            if (DelayAssert)
            {
                if (testContext == null)
                {
                    return true;
                }

                switch (testContext.CurrentTestOutcome)
                {
                    case UnitTestOutcome.InProgress:
                    case UnitTestOutcome.Inconclusive:
                    case UnitTestOutcome.Passed:
                    case UnitTestOutcome.Unknown:
                        return true;
                }
            }

            return false;
        }
    }
}
