//---------------------------------------------------------------------
// <copyright file="IClientSideProjectionReplacingVisitor.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.Client
{
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Query.Contracts;

    /// <summary>
    /// Contract for replacing parts of a projection that will be evaluated by the astoria client without being sent to the server
    /// </summary>
    [ImplementationSelector("ClientSideProjectionReplacingVisitor", DefaultImplementation = "Default")]
    public interface IClientSideProjectionReplacingVisitor
    {
        /// <summary>
        /// Replaces projections that will be evaluated client-side with what will be sent to the server.
        /// </summary>
        /// <param name="queryExpression">The query expression to do replacement on.</param>
        /// <returns>The expression with replacements</returns>
        QueryExpression ReplaceClientSideProjections(QueryExpression queryExpression);
    }
}
