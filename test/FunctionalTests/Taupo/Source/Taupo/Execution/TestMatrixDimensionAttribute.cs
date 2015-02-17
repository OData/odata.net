//---------------------------------------------------------------------
// <copyright file="TestMatrixDimensionAttribute.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Execution
{
    using System;

    /// <summary>
    /// Attribute for representing one dimension of a test matrix
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1813:AvoidUnsealedAttributes", Justification = "Useful to be able to derive and provide customized values at runtime")]
    public class TestMatrixDimensionAttribute : Attribute
    {
        private object[] values = new object[0];

        /// <summary>
        /// Initializes a new instance of the TestMatrixDimensionAttribute class.
        /// </summary>
        public TestMatrixDimensionAttribute()
        {
            this.Position = -1;
        }

        /// <summary>
        /// Gets or sets the position of the dimension relative to other dimension
        /// </summary>
        public int Position { get; set; }

        /// <summary>
        /// Gets or sets the name of the dimension
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the type of values allowed for the dimension. If not specified defaults to object.
        /// When type is boolean or enum providing <see cref="Values"/> for the dimension is optional.
        /// </summary>
        /// <example>
        /// [TestMatrixDimension(Position = 1, Name = "CascadeDelete", Domain = typeof(bool?))]
        /// [TestMatrixDimension(Position = 2, Name = "Multiplicity", Domain = typeof(MultiplicityPattern))]
        /// </example>
        public Type Domain { get; set; }

        /// <summary>
        /// Gets or sets the values of the dimension
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays", Justification = "For easier construction")]
        public virtual object[] Values 
        {
            get { return this.values; }
            set { this.values = value; }
        }
    }
}
