//---------------------------------------------------------------------
// <copyright file="IQueryExpressionEvaluator.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Query.Contracts
{
    using System.Collections.Generic;

    /// <summary>
    /// Evaluates query expression tree.
    /// </summary>
    public interface IQueryExpressionEvaluator
    {
        /// <summary>
        /// Evaluates the specified expression.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <returns>Value of the expression.</returns>
        QueryValue Evaluate(QueryExpression expression);

        /// <summary>
        /// Evaluates the specified expression.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <param name="freeVariableAssignments">Free variable assignments.</param>
        /// <returns>Value of the expression.</returns>
        QueryValue Evaluate(QueryExpression expression, IDictionary<string, QueryExpression> freeVariableAssignments);
    }
}
