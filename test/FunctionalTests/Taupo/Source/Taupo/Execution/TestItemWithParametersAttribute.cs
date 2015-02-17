//---------------------------------------------------------------------
// <copyright file="TestItemWithParametersAttribute.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Execution
{
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;

    /// <summary>
    /// Common base class for test item attributes.
    /// </summary>
    public abstract class TestItemWithParametersAttribute : TestItemBaseAttribute
    {
        /// <summary>
        /// Initializes a new instance of the TestItemWithParametersAttribute class
        /// </summary>
        protected TestItemWithParametersAttribute()
        {
            this.Parameters = new object[0];
        }

        /// <summary>
        /// Gets or sets constructor parameter (shorthand notation for single parameter).
        /// </summary>
        public object Parameter
        {
            get
            {
                return this.Parameters.Single();
            }

            set
            {
                this.Parameters = new object[] { value };
            }
        }

        /// <summary>
        /// Gets or sets array of constructor parameters.
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1819", Justification = "Enables construction")]
        public object[] Parameters { get; set; }
    }
}
