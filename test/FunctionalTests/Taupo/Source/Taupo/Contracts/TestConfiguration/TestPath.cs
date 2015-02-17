//---------------------------------------------------------------------
// <copyright file="TestPath.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts.TestConfiguration
{
    /// <summary>
    /// Represents the a test path to include or exclude from a TestMatrix
    /// </summary>
    public class TestPath 
    {
        /// <summary>
        /// Initializes a new instance of the TestPath class.
        /// </summary>
        /// <param name="type">Whether to include or exclude the test path</param>
        /// <param name="path">Test path to set</param>
        public TestPath(TestPathType type, string path)
        {
            this.PathType = type;
            this.Value = path;
        }

        /// <summary>
        /// Gets the Value of the TestPath
        /// </summary>
        public string Value { get; private set; }

        /// <summary>
        /// Gets the value of whether to include or exclude the test path
        /// </summary>
        public TestPathType PathType { get; private set; }
    }
}
