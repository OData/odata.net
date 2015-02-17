//---------------------------------------------------------------------
// <copyright file="TestSuite.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Runners
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Xml;
    using System.Xml.Linq;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Information about the test suite.
    /// </summary>
    public class TestSuite
    {
        /// <summary>
        /// Initializes a new instance of the TestSuite class.
        /// </summary>
        public TestSuite()
        {
            this.Assemblies = new List<string>();
            this.Items = new List<TestSuiteItem>();
            this.Parameters = new Dictionary<string, string>();
        }

        /// <summary>
        /// Gets the assemblies to be loaded.
        /// </summary>
        /// <value>The assemblies.</value>
        public IList<string> Assemblies { get; private set; }

        /// <summary>
        /// Gets the suite parameters.
        /// </summary>
        /// <value>The parameters.</value>
        public IDictionary<string, string> Parameters { get; private set; }

        /// <summary>
        /// Gets the list of suite items.
        /// </summary>
        /// <value>The items.</value>
        public IList<TestSuiteItem> Items { get; private set; }
    }
}
