//---------------------------------------------------------------------
// <copyright file="TestDimension.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts.TestConfiguration
{
    using System.Collections;
    using System.Collections.Generic;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Test Dimension that contains the name of the dimension and an exploration strategy that 
    /// when explored will return the values
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "IEnumerable exists for easy creation of objects")]
    public class TestDimension : IEnumerable
    {
        /// <summary>
        /// Initializes a new instance of the TestDimension class.
        /// </summary>
        /// <param name="name">Name of test dimesion</param>
        /// <param name="values">Values of the test dimension</param>
        public TestDimension(string name, params string[] values)
        {
            this.Name = name;
            this.Values = values;
            this.RequiredTestDimensionIfValueEqual = new Dictionary<string, TestDimension>();
        }

        /// <summary>
        /// Gets the Name
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets othe Values
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays", Justification = "Required as its treated as one set of values")]
        public string[] Values { get; private set; }

        /// <summary>
        /// Gets or sets an Unspecified value for the dimension
        /// </summary>
        public string UnspecifiedValue { get; set; }

        /// <summary>
        /// Gets the Required TestDimensions to add if the test dimension value is equal to a particular value
        /// Example of if the Provider = EF, then we can add another test dimension for DBContext or ObjectContext
        /// </summary>
        public IDictionary<string, TestDimension> RequiredTestDimensionIfValueEqual { get; private set; }

        /// <summary>
        /// Adds a dependent Dimension Pair to the list of dependent dimensions
        /// </summary>
        /// <param name="dependentDimensionPair">Name and Dependent dimension</param>
        public void Add(KeyValuePair<string, TestDimension> dependentDimensionPair)
        {
            ExceptionUtilities.CheckArgumentNotNull(dependentDimensionPair, "dependentDimensionPair");
            ExceptionUtilities.CheckObjectNotNull(dependentDimensionPair.Value.UnspecifiedValue, "Unspecified value must not be null");

            this.RequiredTestDimensionIfValueEqual.Add(dependentDimensionPair.Key, dependentDimensionPair.Value);
        }

        /// <summary>
        /// This method is not supported and throws <see cref="TaupoNotSupportedException"/>
        /// </summary>
        /// <returns>This method never returns a result.</returns>
        public IEnumerator GetEnumerator()
        {
            throw new TaupoNotSupportedException(ExceptionUtilities.EnumerableNotImplementedExceptionMessage);
        }
    }
}
