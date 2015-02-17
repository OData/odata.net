//---------------------------------------------------------------------
// <copyright file="ExpectedExceptions.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Query.Contracts
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Query.Common;
    
    /// <summary>
    /// Represents a collection of expected exceptions.
    /// </summary>
    public class ExpectedExceptions : QueryError
    {
        /// <summary>
        /// Initializes a new instance of the ExpectedExceptions class.
        /// </summary>
        /// <param name="expectedException">The expected exception.</param>
        public ExpectedExceptions(ExpectedExceptionVerifier expectedException)
            : this(expectedException, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the ExpectedExceptions class.
        /// </summary>
        /// <param name="expectedException">The expected exception.</param>
        /// <param name="ignoredExceptions">The ignored exceptions.</param>
        public ExpectedExceptions(ExpectedExceptionVerifier expectedException, params ExpectedExceptionVerifier[] ignoredExceptions)
            : base(null)
        {
            ExceptionUtilities.Assert(expectedException != null || ignoredExceptions != null, "There must be at least one ExpectedException.");

            if (ignoredExceptions == null)
            {
                ignoredExceptions = new ExpectedExceptionVerifier[] { };
            }

            this.ExpectedException = expectedException;
            this.IgnoredExceptions = new List<ExpectedExceptionVerifier>(ignoredExceptions);
        }

        /// <summary>
        /// Gets the Expected exception.
        /// </summary>
        public ExpectedExceptionVerifier ExpectedException { get; private set; }

        /// <summary>
        /// Gets the Ignored exceptions.
        /// </summary>
        public ICollection<ExpectedExceptionVerifier> IgnoredExceptions { get; private set; }
    }
}
