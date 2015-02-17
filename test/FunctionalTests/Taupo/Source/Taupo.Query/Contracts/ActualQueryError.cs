//---------------------------------------------------------------------
// <copyright file="ActualQueryError.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Query.Contracts
{
    using System;

    /// <summary>
    /// Holds an exception that can be compared against an ExpectedQueryError.
    /// </summary>
    /// <remarks>Must inherit from QueryError so it can be held by a QueryValue.</remarks>
    public class ActualQueryError : QueryError
    {
        /// <summary>
        /// Initializes a new instance of the ActualQueryError class.
        /// </summary>
        /// <param name="actualException">The actual Exception</param>
        public ActualQueryError(Exception actualException)
            : base(null)
        {
            this.Exception = actualException;
        }

        /// <summary>
        /// Gets the Actual Exception.
        /// </summary>
        public Exception Exception { get; private set; }
    }
}
