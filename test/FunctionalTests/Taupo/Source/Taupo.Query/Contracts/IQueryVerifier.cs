//---------------------------------------------------------------------
// <copyright file="IQueryVerifier.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Query.Contracts
{
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Interface for QueryVerifiers.
    /// </summary>
    [ImplementationSelector("QueryVerifier", HelpText = "The verifier to use when running query tests")]
    public interface IQueryVerifier
    {
        /// <summary>
        /// Verify the passed query tree. Verification will generally (but not always) involve both execution and results verification.
        /// </summary>
        /// <param name="expression">The query tree which will be verified</param>
        void Verify(QueryExpression expression);
    }
}
