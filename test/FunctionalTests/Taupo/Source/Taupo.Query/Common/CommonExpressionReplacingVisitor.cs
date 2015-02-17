//---------------------------------------------------------------------
// <copyright file="CommonExpressionReplacingVisitor.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Query.Common
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Query.Contracts;
    using Microsoft.Test.Taupo.Query.Contracts.CommonExpressions;

    /// <summary>
    /// Replaces the expression with a new copy, if any of the expression's children has changed. 
    /// If no changes were made, the same expression is returned.
    /// </summary>
    public abstract class CommonExpressionReplacingVisitor : IQueryExpressionReplacingVisitor
    {
        private Stack<FunctionCall> customFunctionsCallStack;

        /// <summary>
        /// Initializes a new instance of the CommonExpressionReplacingVisitor class.
        /// </summary>
        protected CommonExpressionReplacingVisitor()
        {
            this.customFunctionsCallStack = new Stack<FunctionCall>();
        }

        /// <summary>
        /// Replaces the expression.
        /// </summary>
        /// <param name="queryExpression">Expression to replace.</param>
        /// <returns>Replaced expression.</returns>
        public QueryExpression ReplaceExpression(QueryExpression queryExpression)
        {
            return queryExpression.Accept(this);
        }

        /// <summary>
        /// Replaces the given expression.
        /// </summary>
        /// <param name="expression">Expression to replace.</param>
        /// <returns>Replaced expression.</returns>
        public virtual QueryExpression Visit(QueryAddExpression expression)
        {
            return this.VisitBinaryExpression(expression, CommonQueryBuilder.Add);
        }

        /// <summary>
        /// Replaces the given expression.
        /// </summary>
        /// <param name="expression">Expression to replace.</param>
        /// <returns>Replaced expression.</returns>
        public virtual QueryExpression Visit(QueryAndExpression expression)
        {
            return this.VisitBinaryExpression(expression, CommonQueryBuilder.And);
        }

        /// <summary>
        /// Replaces the given expression.
        /// </summary>
        /// <param name="expression">Expression to replace.</param>
        /// <returns>Replaced expression.</returns>
        public virtual QueryExpression Visit(QueryAsExpression expression)
        {
            var source = this.ReplaceExpression(expression.Source);

            if (this.HasChanged(expression.Source, source))
            {
                return CommonQueryBuilder.As(source, expression.TypeToOperateAgainst);
            }

            return expression;
        }

        /// <summary>
        /// Replaces the given expression.
        /// </summary>
        /// <param name="expression">Expression to replace.</param>
        /// <returns>Replaced expression.</returns>
        public virtual QueryExpression Visit(QueryCastExpression expression)
        {
            var source = this.ReplaceExpression(expression.Source);

            if (this.HasChanged(expression.Source, source))
            {
                return source.Cast(expression.TypeToOperateAgainst);
            }

            return expression;
        }

        /// <summary>
        /// Replaces the given expression.
        /// </summary>
        /// <param name="expression">Expression to replace.</param>
        /// <returns>Replaced expression.</returns>
        public virtual QueryExpression Visit(QueryConstantExpression expression)
        {
            return expression;
        }

        /// <summary>
        /// Replaces the given expression.
        /// </summary>
        /// <param name="expression">Expression to replace.</param>
        /// <returns>Replaced expression.</returns>
        public virtual QueryExpression Visit(QueryCustomFunctionCallExpression expression)
        {
            var replacedArguments = this.ReplaceArguments(expression.Arguments);

            QueryExpression replacedFunctionBody = null;
            if (expression.FunctionBody != null)
            {
                ExceptionUtilities.Assert(!this.customFunctionsCallStack.Any(s => s.Function == expression.Function), "Recursive function calls are not supported. Function: '{0}'.", expression.Function.FullName);

                this.customFunctionsCallStack.Push(new FunctionCall(expression.Function, replacedArguments));
                replacedFunctionBody = this.ReplaceExpression(expression.FunctionBody);
                this.customFunctionsCallStack.Pop();
            }

            if (this.HasChanged(expression.FunctionBody, replacedFunctionBody) || this.HasChanged(expression.Arguments, replacedArguments))
            {
                return new QueryCustomFunctionCallExpression(expression.ExpressionType, expression.Function, replacedFunctionBody, expression.IsRoot, expression.IsCalledByNameOnly, replacedArguments.ToArray());
            }

            return expression;
        }

        /// <summary>
        /// Replaces the given expression.
        /// </summary>
        /// <param name="expression">Expression to replace.</param>
        /// <returns>Replaced expression.</returns>
        public virtual QueryExpression Visit(QueryDivideExpression expression)
        {
            return this.VisitBinaryExpression(expression, CommonQueryBuilder.Divide);
        }

        /// <summary>
        /// Replaces the given expression.
        /// </summary>
        /// <param name="expression">Expression to replace.</param>
        /// <returns>Replaced expression.</returns>
        public virtual QueryExpression Visit(QueryEqualToExpression expression)
        {
            return this.VisitBinaryExpression(expression, CommonQueryBuilder.EqualTo);
        }

        /// <summary>
        /// Replaces the given expression.
        /// </summary>
        /// <param name="expression">Expression to replace.</param>
        /// <returns>Replaced expression.</returns>
        public QueryExpression Visit(QueryFunctionImportCallExpression expression)
        {
            var replacedArguments = this.ReplaceArguments(expression.Arguments);

            if (this.HasChanged(expression.Arguments, replacedArguments))
            {
                return new QueryFunctionImportCallExpression(expression.ExpressionType, expression.FunctionImport, expression.IsRoot, replacedArguments.ToArray());
            }

            return expression;
        }

        /// <summary>
        /// Replaces the given expression.
        /// </summary>
        /// <param name="expression">Expression to replace.</param>
        /// <returns>Replaced expression.</returns>
        public virtual QueryExpression Visit(QueryFunctionParameterReferenceExpression expression)
        {
            return expression;
        }

        /// <summary>
        /// Replaces the given expression.
        /// </summary>
        /// <param name="expression">Expression to replace.</param>
        /// <returns>Replaced expression.</returns>
        public virtual QueryExpression Visit(QueryGreaterThanExpression expression)
        {
            return this.VisitBinaryExpression(expression, CommonQueryBuilder.GreaterThan);
        }

        /// <summary>
        /// Replaces the given expression.
        /// </summary>
        /// <param name="expression">Expression to replace.</param>
        /// <returns>Replaced expression.</returns>
        public virtual QueryExpression Visit(QueryGreaterThanOrEqualToExpression expression)
        {
            return this.VisitBinaryExpression(expression, CommonQueryBuilder.GreaterThanOrEqualTo);
        }

        /// <summary>
        /// Replaces the given expression.
        /// </summary>
        /// <param name="expression">Expression to replace.</param>
        /// <returns>Replaced expression.</returns>
        public QueryExpression Visit(QueryIsNotNullExpression expression)
        {
            var argument = this.ReplaceExpression(expression.Argument);

            if (this.HasChanged(expression.Argument, argument))
            {
                return new QueryIsNotNullExpression(argument, expression.ExpressionType);
            }
            else
            {
                return expression;
            }
        }

        /// <summary>
        /// Replaces the given expression.
        /// </summary>
        /// <param name="expression">Expression to replace.</param>
        /// <returns>Replaced expression.</returns>
        public QueryExpression Visit(QueryIsNullExpression expression)
        {
            var argument = this.ReplaceExpression(expression.Argument);

            if (this.HasChanged(expression.Argument, argument))
            {
                return new QueryIsNullExpression(argument, expression.ExpressionType);
            }
            else
            {
                return expression;
            }
        }

        /// <summary>
        /// Replaces the given expression.
        /// </summary>
        /// <param name="expression">Expression to replace.</param>
        /// <returns>Replaced expression.</returns>
        public virtual QueryExpression Visit(QueryIsOfExpression expression)
        {
            var source = this.ReplaceExpression(expression.Source);

            if (this.HasChanged(expression.Source, source))
            {
                return source.IsOf(expression.TypeToOperateAgainst);
            }

            return expression;
        }

        /// <summary>
        /// Replaces the given expression.
        /// </summary>
        /// <param name="expression">Expression to replace.</param>
        /// <returns>Replaced expression.</returns>
        public virtual QueryExpression Visit(QueryLessThanExpression expression)
        {
            return this.VisitBinaryExpression(expression, CommonQueryBuilder.LessThan);
        }

        /// <summary>
        /// Replaces the given expression.
        /// </summary>
        /// <param name="expression">Expression to replace.</param>
        /// <returns>Replaced expression.</returns>
        public virtual QueryExpression Visit(QueryLessThanOrEqualToExpression expression)
        {
            return this.VisitBinaryExpression(expression, CommonQueryBuilder.LessThanOrEqualTo);
        }

        /// <summary>
        /// Replaces the given expression.
        /// </summary>
        /// <param name="expression">Expression to replace.</param>
        /// <returns>Replaced expression.</returns>
        public virtual QueryExpression Visit(QueryModuloExpression expression)
        {
            return this.VisitBinaryExpression(expression, CommonQueryBuilder.Modulo);
        }

        /// <summary>
        /// Replaces the given expression.
        /// </summary>
        /// <param name="expression">Expression to replace.</param>
        /// <returns>Replaced expression.</returns>
        public virtual QueryExpression Visit(QueryMultiplyExpression expression)
        {
            return this.VisitBinaryExpression(expression, CommonQueryBuilder.Multiply);
        }

        /// <summary>
        /// Replaces the given expression.
        /// </summary>
        /// <param name="expression">Expression to replace.</param>
        /// <returns>Replaced expression.</returns>
        public virtual QueryExpression Visit(QueryNotEqualToExpression expression)
        {
            return this.VisitBinaryExpression(expression, CommonQueryBuilder.NotEqualTo);
        }

        /// <summary>
        /// Replaces the given expression.
        /// </summary>
        /// <param name="expression">Expression to replace.</param>
        /// <returns>Replaced expression.</returns>
        public virtual QueryExpression Visit(QueryNotExpression expression)
        {
            QueryExpression argument = this.ReplaceExpression(expression.Argument);

            if (this.HasChanged(expression.Argument, argument))
            {
                return new QueryNotExpression(argument, expression.ExpressionType);
            }

            return expression;
        }

        /// <summary>
        /// Replaces the given expression.
        /// </summary>
        /// <param name="expression">Expression to replace.</param>
        /// <returns>Replaced expression.</returns>
        public virtual QueryExpression Visit(QueryNullExpression expression)
        {
            return expression;
        }

        /// <summary>
        /// Replaces the given expression.
        /// </summary>
        /// <param name="expression">Expression to replace.</param>
        /// <returns>Replaced expression.</returns>
        public virtual QueryExpression Visit(QueryOfTypeExpression expression)
        {
            var source = this.ReplaceExpression(expression.Source);

            if (this.HasChanged(expression.Source, source))
            {
                return source.OfType(expression.TypeToOperateAgainst);
            }

            return expression;
        }

        /// <summary>
        /// Replaces the given expression.
        /// </summary>
        /// <param name="expression">Expression to replace.</param>
        /// <returns>Replaced expression.</returns>
        public virtual QueryExpression Visit(QueryOrExpression expression)
        {
            return this.VisitBinaryExpression(expression, CommonQueryBuilder.Or);
        }

        /// <summary>
        /// Replaces the given expression.
        /// </summary>
        /// <param name="expression">Expression to replace.</param>
        /// <returns>Replaced expression.</returns>
        public virtual QueryExpression Visit(QueryPropertyExpression expression)
        {
            QueryExpression instance = this.ReplaceExpression(expression.Instance);

            if (this.HasChanged(expression.Instance, instance))
            {
                return instance.Property(expression.Name, expression.ExpressionType);
            }

            return expression;
        }

        /// <summary>
        /// Replaces the given expression.
        /// </summary>
        /// <param name="expression">Expression to replace.</param>
        /// <returns>Replaced expression.</returns>
        public virtual QueryExpression Visit(QueryRootExpression expression)
        {
            return expression;
        }

        /// <summary>
        /// Replaces the given expression.
        /// </summary>
        /// <param name="expression">Expression to replace.</param>
        /// <returns>Replaced expression.</returns>
        public virtual QueryExpression Visit(QuerySubtractExpression expression)
        {
            return this.VisitBinaryExpression(expression, CommonQueryBuilder.Subtract);
        }

        /// <summary>
        /// Determines whether the expression has been changed during the replace process.
        /// </summary>
        /// <param name="originalExpression">Original expression.</param>
        /// <param name="replacedExpression">Replaced expression.</param>
        /// <returns>True if expression has changed, false otherwise.</returns>
        protected virtual bool HasChanged(QueryExpression originalExpression, QueryExpression replacedExpression)
        {
            // TODO: Possibly we need a robust way of comparing types, to verify that expression type has not changed as a result of replace
            return !ReferenceEquals(originalExpression, replacedExpression);
        }

        /// <summary>
        /// Determines whether any of the expressions the list has been changed during the replace process.
        /// </summary>
        /// <typeparam name="TExpression">Type of the expression.</typeparam>
        /// <param name="originalExpressions">Original expressions.</param>
        /// <param name="replacedExpressions">Replaced expressions.</param>
        /// <returns>True if expressions have changed, false otherwise.</returns>
        protected bool HasChanged<TExpression>(IList<TExpression> originalExpressions, IList<TExpression> replacedExpressions)
            where TExpression : QueryExpression
        {
            ExceptionUtilities.Assert(replacedExpressions.Count == originalExpressions.Count, "Number of original expressions do not match the number of replaced expressions.");
            bool anyChange = false;

            for (int i = 0; i < replacedExpressions.Count; i++)
            {
                if (this.HasChanged(originalExpressions[i], replacedExpressions[i]))
                {
                    anyChange = true;
                }
            }

            return anyChange;
        }

        /// <summary>
        /// Handles a binary expression
        /// </summary>
        /// <param name="expression">The expression</param>
        /// <param name="commonQueryBuilderCall">The func to contruct the binary expression.</param>
        /// <returns>Replaced expression</returns>
        protected QueryExpression VisitBinaryExpression(
            QueryBinaryExpression expression,
            Func<QueryExpression, QueryExpression, QueryType, QueryExpression> commonQueryBuilderCall)
        {
            QueryExpression left = this.ReplaceExpression(expression.Left);
            QueryExpression right = this.ReplaceExpression(expression.Right);

            if (this.HasChanged(expression.Left, left) || this.HasChanged(expression.Right, right))
            {
                return commonQueryBuilderCall(left, right, expression.ExpressionType);
            }

            return expression;
        }

        /// <summary>
        /// Replaces arguments
        /// </summary>
        /// <param name="arguments">The arguments to visit</param>
        /// <returns>Replaced arguments</returns>
        protected virtual IList<QueryExpression> ReplaceArguments(IEnumerable<QueryExpression> arguments)
        {
            var replacedArguments = arguments.Select(a => this.ReplaceExpression(a)).ToList();

            return replacedArguments;
        }

        /// <summary>
        /// Replaces function parameter reference expression with an expression represeting argument for the parameter
        /// </summary>
        /// <param name="expression">The function parameter reference expression</param>
        /// <returns>An expression represeting corresponding argument for the parameter</returns>
        protected QueryExpression ReplaceFunctionParameterReference(QueryFunctionParameterReferenceExpression expression)
        {
            string parameterName = expression.ParameterName;
            var functionCall = this.customFunctionsCallStack.Peek();
            var parameters = functionCall.Function.Parameters;
            var arguemnts = functionCall.Arguments;

            var matchingParameters = parameters.Where(p => p.Name == parameterName).ToList();
            ExceptionUtilities.Assert(
                matchingParameters.Count == 1,
                "Found {0} parameters with name '{1}'. Expected: 1. Function: '{2}'.",
                matchingParameters.Count,
                parameterName,
                functionCall.Function.FullName);

            int index = parameters.IndexOf(matchingParameters[0]);
            return arguemnts[index];
        }

        /// <summary>
        /// Pushes function call with provided arguments to the function call stack
        /// </summary>
        /// <param name="function">The function to push to the stack</param>
        /// <param name="arguments">The function call arguments</param>
        protected void PushFunctionCallToStack(Function function, IEnumerable<QueryExpression> arguments)
        {
            this.customFunctionsCallStack.Push(new FunctionCall(function, arguments));
        }

        /// <summary>
        /// Represents function call.
        /// </summary>
        private class FunctionCall
        {
            /// <summary>
            /// Initializes a new instance of the FunctionCall class.
            /// </summary>
            /// <param name="function">The function.</param>
            /// <param name="arguments">The arguments for the function call.</param>
            public FunctionCall(Function function, IEnumerable<QueryExpression> arguments)
            {
                this.Function = function;
                this.Arguments = arguments.ToList();
            }

            /// <summary>
            /// Gets the function of this function call.
            /// </summary>
            public Function Function { get; private set; }

            /// <summary>
            /// Gets the arguments of this function call.
            /// </summary>
            public IList<QueryExpression> Arguments { get; private set; }
        }
    }
}