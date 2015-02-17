//---------------------------------------------------------------------
// <copyright file="IVerifyServiceActionQueryResult.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.ResponseVerification
{
    using Microsoft.Test.Taupo.Query.Contracts;

    /// <summary>
    /// Gets the expected query value based on the action query
    /// </summary>
    public interface IVerifyServiceActionQueryResult
    {
        /// <summary>
        /// Gets the expected query value for the service operation request
        /// </summary>
        /// <param name="initialExpectedResults">Initial expected values for an action</param>
        /// <param name="parameterValues">Parameter values for the action</param>
        /// <returns>A query Value that is the expected value</returns>
        QueryValue GetExpectedQueryValue(QueryValue initialExpectedResults, params QueryValue[] parameterValues); 
    }
}
