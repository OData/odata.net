//---------------------------------------------------------------------
// <copyright file="IClientQueryVersionErrorCalculator.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.Client
{
    using Microsoft.Test.Taupo.Astoria.Contracts.Client;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Query.Contracts;

    /// <summary>
    /// Calculates the Error for the Query Expression
    /// </summary>
    [ImplementationSelector("ClientQueryVersionErrorCalculator", DefaultImplementation = "Default")]
    public interface IClientQueryVersionErrorCalculator
    {
        /// <summary>
        /// Returns the error expected for the Query
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <param name="usesClientQueryable">Whether the client uri is build from a Client Linq expression or not</param>
        /// <param name="clientMaxProtocolVersion">Client Max Protocol Version</param>
        /// <param name="maxProtocolVersion">Max Protocol version of the server</param>
        /// <returns>The Client Exception to be ensure exists</returns>
        ExpectedClientErrorBaseline CalculateExpectedClientVersionError(QueryExpression expression, bool usesClientQueryable, DataServiceProtocolVersion clientMaxProtocolVersion, DataServiceProtocolVersion maxProtocolVersion);
    }
}