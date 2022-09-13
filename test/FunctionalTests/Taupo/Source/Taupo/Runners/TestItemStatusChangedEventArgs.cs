//---------------------------------------------------------------------
// <copyright file="TestItemStatusChangedEventArgs.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Runners
{
    using System;
    using Microsoft.Test.Taupo.Execution;

    /// <summary>
    /// Arguments for TestItemCompleted on <see cref="AsynchronousTestModuleRunnerBase"/>.
    /// </summary>
    [Serializable]
    public class TestItemStatusChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the TestItemStatusChangedEventArgs class.
        /// </summary>
        /// <param name="testItemData">The test item.</param>
        /// <param name="result">The result.</param>
        public TestItemStatusChangedEventArgs(TestItemData testItemData, TestResult result)
        {
            this.TestItemData = testItemData;
            this.TestResult = result;
        }

        /// <summary>
        /// Gets the data for the test item that has completed.
        /// </summary>
        public TestItemData TestItemData { get; private set; }

        /// <summary>
        /// Gets the test result.
        /// </summary>
        public TestResult TestResult { get; private set; }
    }
}
