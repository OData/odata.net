//---------------------------------------------------------------------
// <copyright file="ExpectedExceptionVerifier.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Query.Contracts
{
    using System;

    /// <summary>
    /// Represents an expected exception.
    /// </summary>
    public abstract class ExpectedExceptionVerifier
    {
        /// <summary>
        /// Initializes a new instance of the ExpectedExceptionVerifier class.
        /// </summary>
        /// <param name="innerExpectedException">The expected inner exception.</param>
        protected ExpectedExceptionVerifier(ExpectedExceptionVerifier innerExpectedException) 
        {
            this.InnerExpectedException = innerExpectedException;
        }

        /// <summary>
        /// Gets the expected inner exception.
        /// </summary>
        public ExpectedExceptionVerifier InnerExpectedException { get; private set; }

        /// <summary>
        /// Applies verification against an exception.
        /// </summary>
        /// <param name="actualException">The actual exception.</param>
        /// <returns>Whether the <paramref name="actualException"/> matches the expected error.</returns>
        public abstract bool VerifyException(Exception actualException);
    }
}
