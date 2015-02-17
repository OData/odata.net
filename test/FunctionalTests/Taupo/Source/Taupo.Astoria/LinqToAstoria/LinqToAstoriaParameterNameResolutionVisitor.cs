//---------------------------------------------------------------------
// <copyright file="LinqToAstoriaParameterNameResolutionVisitor.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.LinqToAstoria
{
    using Microsoft.Test.Taupo.Contracts;
    using Microsoft.Test.Taupo.Query.Contracts;
    using Microsoft.Test.Taupo.Query.Contracts.Linq.Expressions;
    using Microsoft.Test.Taupo.Query.Linq;

    /// <summary>
    /// Resolves parameter names in lambda expressions.
    /// </summary>
    public class LinqToAstoriaParameterNameResolutionVisitor : LinqToAstoriaExpressionReplacingVisitor
    {
        private LinqParameterNameResolutionVisitorHelper helper;

        /// <summary>
        /// Initializes a new instance of the LinqToAstoriaParameterNameResolutionVisitor class.
        /// </summary>
        /// <param name="parameterNameGenerator">The parameter name generator.</param>
        public LinqToAstoriaParameterNameResolutionVisitor(IIdentifierGenerator parameterNameGenerator)
        {
            this.helper = new LinqParameterNameResolutionVisitorHelper(parameterNameGenerator);
        }

        /// <summary>
        /// Resolves the parameter names in the query expression (ensures that each parameter has a name).
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <returns>Resolved expression</returns>
        public QueryExpression ResolveParameterNames(QueryExpression expression)
        {
            return expression.Accept(this);
        }

        /// <summary>
        /// Visits a QueryExpression tree whose root node is the LinqParameterExpression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>Replaced expression.</returns>
        public override QueryExpression Visit(LinqParameterExpression expression)
        {
            return this.helper.EnsureParameterHasName(expression);
        }
    }
}
