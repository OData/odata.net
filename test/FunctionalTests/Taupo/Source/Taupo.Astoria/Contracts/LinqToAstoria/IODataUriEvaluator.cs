//---------------------------------------------------------------------
// <copyright file="IODataUriEvaluator.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.LinqToAstoria
{
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Query.Contracts;

    /// <summary>
    /// Contract for processing a uri using the conventions of an OData server implementation to produce an expected value
    /// </summary>
    [ImplementationSelector("ODataUriEvaluator", DefaultImplementation = "Default")]
    public interface IODataUriEvaluator
    {
        /// <summary>
        /// Processes the given uri and produces an expected value according to the conventions of an OData server implementation
        /// </summary>
        /// <param name="uri">The uri to process</param>
        /// <param name="applySelectAndExpand">A value indicating whether or not $select and $expand should be applied to the query values</param>
        /// <param name="applyPaging">A value indicating whether server-driven paging should be applied to the query values</param>
        /// <returns>The value resulting from processing the uri</returns>
        QueryValue Evaluate(ODataUri uri, bool applySelectAndExpand, bool applyPaging);
    }
}