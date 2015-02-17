//---------------------------------------------------------------------
// <copyright file="ODataUriEvaluatorExtensions.cs" company="Microsoft">
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
    public static class ODataUriEvaluatorExtensions
    {
        /// <summary>
        /// Processes the given uri and produces an expected value according to the conventions of an OData server implementation
        /// </summary>
        /// <param name="evaluator">The evaluator</param>
        /// <param name="uri">The uri to process</param>
        /// <returns>The value resulting from processing the uri</returns>
        public static QueryValue Evaluate(this IODataUriEvaluator evaluator, ODataUri uri)
        {
            ExceptionUtilities.CheckArgumentNotNull(evaluator, "evaluator");
            return evaluator.Evaluate(uri, true, true);
        }
    }
}