//---------------------------------------------------------------------
// <copyright file="IClrCanonicalFunctionEvaluator.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.LinqToAstoria
{
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Query.Contracts;

    /// <summary>
    /// Clr canonical function evaluator
    /// </summary>
    [ImplementationSelector("ClrCanonicalFunctionEvaluator", DefaultImplementation = "Default")]
    public interface IClrCanonicalFunctionEvaluator
    {
        /// <summary>
        /// Evaluates the specified result type.
        /// </summary>
        /// <param name="resultType">Type of the result.</param>
        /// <param name="functionName">Name of the function.</param>
        /// <param name="arguments">The arguments.</param>
        /// <returns>Query value which is the result of function evaluation.</returns>
        QueryValue Evaluate(QueryType resultType, string functionName, params QueryValue[] arguments);
    }
}
