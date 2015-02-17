//---------------------------------------------------------------------
// <copyright file="ParameterReplacingVisitor.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.LinqToAstoria
{
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Query.Contracts;
    using Microsoft.Test.Taupo.Query.Contracts.Linq.Expressions;

    /// <summary>
    /// Visitor class used to replace parameters within an expression
    /// </summary>
    public class ParameterReplacingVisitor : LinqToAstoriaExpressionReplacingVisitor
    {
        private LinqParameterExpression oldParameter;
        private LinqParameterExpression newParameter;

        /// <summary>
        /// Initializes a new instance of the ParameterReplacingVisitor class
        /// </summary>
        /// <param name="oldParameter">The parameter to replace</param>
        /// <param name="newParameter">The parameter to use as a replacement</param>
        public ParameterReplacingVisitor(LinqParameterExpression oldParameter, LinqParameterExpression newParameter)
        {
            ExceptionUtilities.CheckArgumentNotNull(oldParameter, "oldParameter");
            ExceptionUtilities.CheckArgumentNotNull(newParameter, "newParameter");

            this.oldParameter = oldParameter;
            this.newParameter = newParameter;
        }

        /// <summary>
        /// Visits the given parameter expression, replacing it if needed
        /// </summary>
        /// <param name="expression">The expression to visit</param>
        /// <returns>The expression, possibly replaced</returns>
        public override QueryExpression Visit(LinqParameterExpression expression)
        {
            if (object.ReferenceEquals(this.oldParameter, expression))
            {
                return this.newParameter;
            }

            return expression;
        }
    }
}
