//---------------------------------------------------------------------
// <copyright file="TestSuiteItem.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Runners
{
    using System.Collections.Generic;
    using System.Xml;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Information about the test suite item.
    /// </summary>
    public class TestSuiteItem
    {
        /// <summary>
        /// Initializes a new instance of the TestSuiteItem class.
        /// </summary>
        /// <param name="name">The name of the test item (with '*' as a wildcard).</param>
        /// <param name="isIncluded">If set to <c>true</c>, the item should be included in the test suite.</param>
        public TestSuiteItem(string name, bool isIncluded)
        {
            this.Name = name;
            this.IsIncluded = isIncluded;
        }

        /// <summary>
        /// Gets the name of the item.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this <see cref="TestSuiteItem"/> is included in the test suite.
        /// </summary>
        public bool IsIncluded { get; private set; }
    }
}
