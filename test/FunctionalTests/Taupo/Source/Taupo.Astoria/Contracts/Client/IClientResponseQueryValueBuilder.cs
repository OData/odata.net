//---------------------------------------------------------------------
// <copyright file="IClientResponseQueryValueBuilder.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.Client
{
    using Microsoft.Test.Taupo.Astoria.Contracts.Http;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Query.Contracts;

    /// <summary>
    /// Contracts for building query value from response payload.
    /// </summary>
    [ImplementationSelector("ClientResponseQueryValueBuilder", DefaultImplementation = "Default")]
    public interface IClientResponseQueryValueBuilder
    {
        /// <summary>
        /// Build QueryValue from query expresion and server response.
        /// </summary>
        /// <param name="expression">The query expresion of client request.</param>
        /// <param name="response">The http response from the server.</param>
        /// <returns>The baseline QueryValue converted from payload.</returns>
        QueryValue BuildQueryValue(QueryExpression expression, HttpResponseData response);
    }
}
