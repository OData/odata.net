//---------------------------------------------------------------------
// <copyright file="LinqToAstoriaCustomFunctionResolutionVisitor.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.LinqToAstoria
{
    using System.Linq;
    using Microsoft.Test.Taupo.Astoria.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Query.Contracts;
    using Microsoft.Test.Taupo.Query.Contracts.CommonExpressions;

    /// <summary>
    /// Performs type resolution for functions in the model
    /// </summary>
    public class LinqToAstoriaCustomFunctionResolutionVisitor : LinqToAstoriaExpressionReplacingVisitor
    {
        private LinqToAstoriaTypeResolutionVisitor typeResolver;
        private IQueryEvaluationStrategy queryEvaluationStrategy;

        /// <summary>
        /// Initializes a new instance of the LinqToAstoriaCustomFunctionResolutionVisitor class
        /// </summary>
        /// <param name="typeResolver">The Type Resolution visitor</param>
        internal LinqToAstoriaCustomFunctionResolutionVisitor(LinqToAstoriaTypeResolutionVisitor typeResolver)
        {
            this.typeResolver = typeResolver;
        }

        /// <summary>
        /// Resolves type and body expression for functions in the model
        /// </summary>
        /// <param name="expression">the expression to resolve</param>
        /// <param name="strategy">the query evaluation strategy</param>
        /// <returns>the resolved expression</returns>
        public QueryExpression ResolveCustomFunctions(QueryExpression expression, IQueryEvaluationStrategy strategy)
        {
            this.queryEvaluationStrategy = strategy;
            return expression.Accept(this);
        }

        /// <summary>
        /// Visits a function call expression, resolving the function body and type
        /// </summary>
        /// <param name="expression">the expression to resolve</param>
        /// <returns>the resolved expression</returns>
        public override QueryExpression Visit(QueryCustomFunctionCallExpression expression)
        {
            var bodyAnnotation = expression.Function.Annotations.OfType<FunctionBodyAnnotation>().SingleOrDefault();
            if (bodyAnnotation != default(FunctionBodyAnnotation))
            {
                this.typeResolver.CustomFunctionsCallStack.Push(expression.Function);
                var resolvedBody = this.typeResolver.ResolveTypes(bodyAnnotation.FunctionBody, this.queryEvaluationStrategy);
                this.typeResolver.CustomFunctionsCallStack.Pop();

                QueryCustomFunctionCallExpression resolved = base.Visit(
                    new QueryCustomFunctionCallExpression(
                            expression.ExpressionType,
                            expression.Function,
                            resolvedBody,
                            expression.IsRoot,
                            expression.IsCalledByNameOnly,
                            expression.Arguments.ToArray())) as QueryCustomFunctionCallExpression;

                // Replace the body expression with the resolved one
                bodyAnnotation.FunctionBody = resolved.FunctionBody;
                return resolved;
            }

            return base.Visit(expression);
        }
    }
}