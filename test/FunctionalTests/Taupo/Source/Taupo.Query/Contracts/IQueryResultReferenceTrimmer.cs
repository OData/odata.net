//---------------------------------------------------------------------
// <copyright file="IQueryResultReferenceTrimmer.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Query.Contracts
{
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Trims references that are not expected to be materialized in the query result
    /// </summary>
    [ImplementationSelector("QueryResultReferenceTrimmer", DefaultImplementation = "Default")]
    public interface IQueryResultReferenceTrimmer
    {
        /// <summary>
        /// Trims references that will not be materialized in the result, taking provided span paths into account.
        /// </summary>
        /// <param name="queryResult">Result to trim.</param>
        /// <param name="spanPaths">Span paths, these references will not be trimmed even if they should be.</param>
        /// <returns>Trimmed result.</returns>
        QueryValue TrimReferences(QueryValue queryResult, params string[] spanPaths);
    }
}