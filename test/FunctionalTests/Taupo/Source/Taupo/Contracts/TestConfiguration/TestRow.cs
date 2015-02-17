//---------------------------------------------------------------------
// <copyright file="TestRow.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts.TestConfiguration
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Represents a test configuration row
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "IEnumerable exists for easy creation of objects")]
    public class TestRow : IEnumerable<KeyValuePair<string, string>>
    {
        private IDictionary<string, string> values;

        internal TestRow(IDictionary<string, string> values)
        {
            this.values = values;
        }

        /// <summary>
        /// Gets a String representation of the test row
        /// </summary>
        public string StringRepresentation
        {
            get
            {
                return string.Join(",", this.values.Select(p => string.Join("=", p.Key, p.Value)));
            }
        }

        /// <summary>
        /// Gets the value of the particular column in this row
        /// </summary>
        /// <param name="name">Name of the column</param>
        /// <returns>The value of the column in this row</returns>
        public string this[string name]
        {
            get
            {
                return this.values[name];
            }
        }

        /// <summary>
        /// Gets the enumerator
        /// </summary>
        /// <returns>List of Key Value Pair of string and string of values</returns>
        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        {
            return this.values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return (IEnumerator<KeyValuePair<string, string>>)this.GetEnumerator();
        }
    }
}
