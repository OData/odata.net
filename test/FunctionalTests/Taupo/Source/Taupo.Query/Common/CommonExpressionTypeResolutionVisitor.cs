//---------------------------------------------------------------------
// <copyright file="CommonExpressionTypeResolutionVisitor.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Query.Common
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Contracts.EntityModel.Edm;
    using Microsoft.Test.Taupo.Contracts.Types;
    using Microsoft.Test.Taupo.Query.Contracts;
    using Microsoft.Test.Taupo.Query.Contracts.CommonExpressions;

    /// <summary>
    /// Base implementation of a visitor which resolves types of query expressions.
    /// </summary>
    public abstract class CommonExpressionTypeResolutionVisitor : ICommonExpressionVisitor<QueryExpression>
    {
        /// <summary>
        /// Initializes a new instance of the CommonExpressionTypeResolutionVisitor class.
        /// </summary>
        /// <param name="typeLibrary">The query type library.</param>
        protected CommonExpressionTypeResolutionVisitor(QueryTypeLibrary typeLibrary)
        {
            ExceptionUtilities.CheckArgumentNotNull(typeLibrary, "typeLibrary");
            this.TypeLibrary = typeLibrary;
            this.CustomFunctionsCallStack = new Stack<Function>();
        }

        /// <summary>
        /// Gets the stack of currently visited functions.
        /// </summary>
        public Stack<Function> CustomFunctionsCallStack { get; private set; }

        /// <summary>
        /// Gets the evaluation strategy.
        /// </summary>
        protected IQueryEvaluationStrategy EvaluationStrategy { get; private set; }

        /// <summary>
        /// Gets the query type library.
        /// </summary>
        protected QueryTypeLibrary TypeLibrary { get; private set; }

        /// <summary>
        /// Resolves types for the specified expression.
        /// </summary>
        /// <param name="expression">The expression to resolve types for.</param>
        /// <param name="evaluationStrategy">Evaluation strategy.</param>
        /// <returns>Expression with resolved types.</returns>
        public QueryExpression ResolveTypes(QueryExpression expression, IQueryEvaluationStrategy evaluationStrategy)
        {
            // TODO: possibly inject evaluation strategy, and move it to constructor
            this.EvaluationStrategy = evaluationStrategy;

            return this.ResolveTypes(expression);
        }

        /// <summary>
        /// Resolves types for the specified expression.
        /// </summary>
        /// <param name="expression">The expression to resolve types for.</param>
        /// <returns>Expression with resolved types.</returns>
        public QueryExpression Visit(QueryAddExpression expression)
        {
            return this.VisitBinaryArithmeticExpression(expression, CommonQueryBuilder.Add);
        }

        /// <summary>
        /// Resolves types for the specified expression.
        /// </summary>
        /// <param name="expression">The expression to resolve types for.</param>
        /// <returns>Expression with resolved types.</returns>
        public QueryExpression Visit(QueryAndExpression expression)
        {
            return this.VisitBinaryExpressionWithBooleanResult(expression, CommonQueryBuilder.And);
        }

        /// <summary>
        /// Resolves types for the specified expression.
        /// </summary>
        /// <param name="expression">The expression to resolve types for.</param>
        /// <returns>Expression with resolved types.</returns>
        public QueryExpression Visit(QueryAsExpression expression)
        {
            var source = this.ResolveTypes(expression.Source);

            return source.As(expression.TypeToOperateAgainst);
        }

        /// <summary>
        /// Resolves types for the specified expression.
        /// </summary>
        /// <param name="expression">The expression to resolve types for.</param>
        /// <returns>Expression with resolved types.</returns>
        public QueryExpression Visit(QueryCastExpression expression)
        {
            var source = this.ResolveTypes(expression.Source);

            return source.Cast(expression.TypeToOperateAgainst);
        }

        /// <summary>
        /// Resolves types for the specified expression.
        /// </summary>
        /// <param name="expression">The expression to resolve types for.</param>
        /// <returns>Expression with resolved types.</returns>
        public QueryExpression Visit(QueryConstantExpression expression)
        {
            if (expression.ExpressionType.IsUnresolved)
            {
                var value = expression.ScalarValue.Value;
                ExceptionUtilities.Assert(value != null, "Constant expression with unresolved type cannot have null value.");
                var queryType = this.GetQueryScalarTypeFromObjectValue(value);
                return CommonQueryBuilder.Constant(queryType.CreateValue(value));
            }

            return expression;
        }

        /// <summary>
        /// Resolves types for the specified expression.
        /// </summary>
        /// <param name="expression">The expression to resolve types for.</param>
        /// <returns>Expression with resolved types.</returns>
        public virtual QueryExpression Visit(QueryCustomFunctionCallExpression expression)
        {
            var resolvedArguments = this.ResolveTypesForArguments(expression.Arguments);

            QueryExpression functionBody = null;
            if (expression.FunctionBody != null)
            {
                ExceptionUtilities.Assert(!this.CustomFunctionsCallStack.Contains(expression.Function), "Recursive function calls are not supported. Function: '{0}'.", expression.Function.FullName);

                this.CustomFunctionsCallStack.Push(expression.Function);
                functionBody = this.ResolveTypes(expression.FunctionBody);
                this.CustomFunctionsCallStack.Pop();
            }

            QueryType resolvedResultType;
            if (expression.Function.ReturnType != null)
            {
                resolvedResultType = this.GetDefaultQueryType(expression.Function.ReturnType);
            }
            else
            {
                ExceptionUtilities.CheckObjectNotNull(functionBody, "Function body cannot be null when function's return type is not defined. Function: '{0}'.", expression.Function.FullName);
                resolvedResultType = functionBody.ExpressionType;
            }

            var resolved = new QueryCustomFunctionCallExpression(resolvedResultType, expression.Function, functionBody, expression.IsRoot, expression.IsCalledByNameOnly, resolvedArguments);

            return resolved;
        }

        /// <summary>
        /// Resolves types for the specified expression.
        /// </summary>
        /// <param name="expression">The expression to resolve types for.</param>
        /// <returns>Expression with resolved types.</returns>
        public QueryExpression Visit(QueryDivideExpression expression)
        {
            return this.VisitBinaryArithmeticExpression(expression, CommonQueryBuilder.Divide);
        }

        /// <summary>
        /// Resolves types for the specified expression.
        /// </summary>
        /// <param name="expression">The expression to resolve types for.</param>
        /// <returns>Expression with resolved types.</returns>
        public QueryExpression Visit(QueryEqualToExpression expression)
        {
            return this.VisitBinaryExpressionWithBooleanResult(expression, CommonQueryBuilder.EqualTo);
        }

        /// <summary>
        /// Resolves types for the specified expression.
        /// </summary>
        /// <param name="expression">The expression to resolve types for.</param>
        /// <returns>Expression with resolved types.</returns>
        public QueryExpression Visit(QueryFunctionImportCallExpression expression)
        {
            ExceptionUtilities.CheckObjectNotNull(expression.FunctionImport.ReturnTypes.SingleOrDefault(), "Function import must have return type specified. FunctionImport: '{0}'.", expression.FunctionImport.Name);

            var resolvedArguments = this.ResolveTypesForArguments(expression.Arguments);
            var returnType = expression.FunctionImport.ReturnTypes.Single();
            QueryType resolvedResultType = this.GetDefaultQueryType(returnType.DataType);

            var resolved = new QueryFunctionImportCallExpression(resolvedResultType, expression.FunctionImport, expression.IsRoot, resolvedArguments);

            return resolved;
        }

        /// <summary>
        /// Resolves types for the specified expression.
        /// </summary>
        /// <param name="expression">The expression to resolve types for.</param>
        /// <returns>Expression with resolved types.</returns>
        public QueryExpression Visit(QueryFunctionParameterReferenceExpression expression)
        {
            Function function = this.CustomFunctionsCallStack.Peek();
            FunctionParameter parameter = function.Parameters.SingleOrDefault(p => p.Name == expression.ParameterName);
            ExceptionUtilities.CheckObjectNotNull(parameter, "Parameter '{0}' does not exist in Function '{1}'.", expression.ParameterName, function.Name);

            QueryType queryType = this.GetDefaultQueryType(parameter.DataType);
            return new QueryFunctionParameterReferenceExpression(expression.ParameterName, queryType);
        }

        /// <summary>
        /// Resolves types for the specified expression.
        /// </summary>
        /// <param name="expression">The expression to resolve types for.</param>
        /// <returns>Expression with resolved types.</returns>
        public QueryExpression Visit(QueryGreaterThanExpression expression)
        {
            return this.VisitBinaryExpressionWithBooleanResult(expression, CommonQueryBuilder.GreaterThan);
        }

        /// <summary>
        /// Resolves types for the specified expression.
        /// </summary>
        /// <param name="expression">The expression to resolve types for.</param>
        /// <returns>Expression with resolved types.</returns>
        public QueryExpression Visit(QueryGreaterThanOrEqualToExpression expression)
        {
            return this.VisitBinaryExpressionWithBooleanResult(expression, CommonQueryBuilder.GreaterThanOrEqualTo);
        }

        /// <summary>
        /// Resolves types for the specified expression.
        /// </summary>
        /// <param name="expression">The expression to resolve types for.</param>
        /// <returns>Expression with resolved types.</returns>
        public QueryExpression Visit(QueryIsNotNullExpression expression)
        {
            var argument = this.ResolveTypes(expression.Argument);
            return new QueryIsNotNullExpression(argument, this.EvaluationStrategy.BooleanType);
        }

        /// <summary>
        /// Resolves types for the specified expression.
        /// </summary>
        /// <param name="expression">The expression to resolve types for.</param>
        /// <returns>Expression with resolved types.</returns>
        public QueryExpression Visit(QueryIsNullExpression expression)
        {
            var argument = this.ResolveTypes(expression.Argument);
            return new QueryIsNullExpression(argument, this.EvaluationStrategy.BooleanType);
        }

        /// <summary>
        /// Resolves types for the specified expression.
        /// </summary>
        /// <param name="expression">The expression to resolve types for.</param>
        /// <returns>Expression with resolved types.</returns>
        public QueryExpression Visit(QueryIsOfExpression expression)
        {
            var source = this.ResolveTypes(expression.Source);

            return source.IsOf(expression.TypeToOperateAgainst);
        }

        /// <summary>
        /// Resolves types for the specified expression.
        /// </summary>
        /// <param name="expression">The expression to resolve types for.</param>
        /// <returns>Expression with resolved types.</returns>
        public QueryExpression Visit(QueryLessThanExpression expression)
        {
            return this.VisitBinaryExpressionWithBooleanResult(expression, CommonQueryBuilder.LessThan);
        }

        /// <summary>
        /// Resolves types for the specified expression.
        /// </summary>
        /// <param name="expression">The expression to resolve types for.</param>
        /// <returns>Expression with resolved types.</returns>
        public QueryExpression Visit(QueryLessThanOrEqualToExpression expression)
        {
            return this.VisitBinaryExpressionWithBooleanResult(expression, CommonQueryBuilder.LessThanOrEqualTo);
        }

        /// <summary>
        /// Resolves types for the specified expression.
        /// </summary>
        /// <param name="expression">The expression to resolve types for.</param>
        /// <returns>Expression with resolved types.</returns>
        public QueryExpression Visit(QueryModuloExpression expression)
        {
            return this.VisitBinaryArithmeticExpression(expression, CommonQueryBuilder.Modulo);
        }

        /// <summary>
        /// Resolves types for the specified expression.
        /// </summary>
        /// <param name="expression">The expression to resolve types for.</param>
        /// <returns>Expression with resolved types.</returns>
        public QueryExpression Visit(QueryMultiplyExpression expression)
        {
            return this.VisitBinaryArithmeticExpression(expression, CommonQueryBuilder.Multiply);
        }

        /// <summary>
        /// Resolves types for the specified expression.
        /// </summary>
        /// <param name="expression">The expression to resolve types for.</param>
        /// <returns>Expression with resolved types.</returns>
        public QueryExpression Visit(QueryNotEqualToExpression expression)
        {
            return this.VisitBinaryExpressionWithBooleanResult(expression, CommonQueryBuilder.NotEqualTo);
        }

        /// <summary>
        /// Resolves types for the specified expression.
        /// </summary>
        /// <param name="expression">The expression to resolve types for.</param>
        /// <returns>Expression with resolved types.</returns>
        public QueryExpression Visit(QueryNotExpression expression)
        {
            var argument = this.ResolveTypes(expression.Argument);
            var boolType = this.EvaluationStrategy.BooleanType;

            return argument.Not(boolType);
        }

        /// <summary>
        /// Resolves types for the specified expression.
        /// </summary>
        /// <param name="expression">The expression to resolve types for.</param>
        /// <returns>Expression with resolved types.</returns>
        public QueryExpression Visit(QueryNullExpression expression)
        {
            return expression;
        }

        /// <summary>
        /// Resolves types for the specified expression.
        /// </summary>
        /// <param name="expression">The expression to resolve types for.</param>
        /// <returns>Expression with resolved types.</returns>
        public QueryExpression Visit(QueryOfTypeExpression expression)
        {
            var source = this.ResolveTypes(expression.Source);

            return source.OfType(expression.TypeToOperateAgainst);
        }

        /// <summary>
        /// Resolves types for the specified expression.
        /// </summary>
        /// <param name="expression">The expression to resolve types for.</param>
        /// <returns>Expression with resolved types.</returns>
        public QueryExpression Visit(QueryOrExpression expression)
        {
            return this.VisitBinaryExpressionWithBooleanResult(expression, CommonQueryBuilder.Or);
        }

        /// <summary>
        /// Resolves types for the specified expression.
        /// </summary>
        /// <param name="expression">The expression to resolve types for.</param>
        /// <returns>Expression with resolved types.</returns>
        public virtual QueryExpression Visit(QueryPropertyExpression expression)
        {
            var instance = this.ResolvePropertyInstance(expression.Instance);

            // TODO: should we relax this?
            var instanceExpressionType = instance.GetExpressionTypeOrElementType();
            var typeWithProperties = instanceExpressionType as IQueryTypeWithProperties;
            if (typeWithProperties == null)
            {
                var referenceType = instanceExpressionType as QueryReferenceType;
                if (referenceType != null)
                {
                    typeWithProperties = referenceType.QueryEntityType as IQueryTypeWithProperties;
                }
            }

            if (typeWithProperties == null)
            {
                throw new TaupoArgumentException("Given expression must be of a query type with properties. Actual: " + instance.ExpressionType.GetType().Name + ".");
            }

            if (!typeWithProperties.Properties.Any(m => m.Name == expression.Name))
            {
                throw new TaupoArgumentException("Given property was not found: " + expression.Name + ".");
            }

            var type = typeWithProperties.Properties.Single(m => m.Name == expression.Name).PropertyType;

            return instance.Property(expression.Name, type);
        }

        /// <summary>
        /// Resolves types for the specified expression.
        /// </summary>
        /// <param name="expression">The expression to resolve types for.</param>
        /// <returns>Expression with resolved types.</returns>
        public QueryExpression Visit(QueryRootExpression expression)
        {
            EntitySet entitySet = expression.UnderlyingEntitySet;
            if (entitySet != null && expression.ExpressionType.IsUnresolved)
            {
                var expressionType = this.TypeLibrary.GetQueryEntityTypeForEntitySet(entitySet).CreateCollectionType();
                return new QueryRootExpression(entitySet, expressionType);
            }

            return expression;
        }

        /// <summary>
        /// Resolves types for the specified expression.
        /// </summary>
        /// <param name="expression">The expression to resolve types for.</param>
        /// <returns>Expression with resolved types.</returns>
        public QueryExpression Visit(QuerySubtractExpression expression)
        {
            return this.VisitBinaryArithmeticExpression(expression, CommonQueryBuilder.Subtract);
        }

        /// <summary>
        /// Resolves types for the specified expression.
        /// </summary>
        /// <param name="expression">The expression to resolve types for.</param>
        /// <returns>Expression with resolved types.</returns>
        protected QueryExpression ResolveTypes(QueryExpression expression)
        {
            return expression != null ? expression.Accept(this) : null;
        }

        /// <summary>
        /// Resolves types for binary arithmetic expression.
        /// </summary>
        /// <param name="expression">The expression to resolve types for.</param>
        /// <param name="builderFunc">Func used to build the expression.</param>
        /// <returns>Expression with resolved types.</returns>
        protected QueryExpression VisitBinaryArithmeticExpression(QueryBinaryExpression expression, Func<QueryExpression, QueryExpression, QueryType, QueryExpression> builderFunc)
        {
            var left = this.ResolveTypes(expression.Left);
            var right = this.ResolveTypes(expression.Right);
            var type = this.EvaluationStrategy.GetCommonType((QueryScalarType)left.ExpressionType, (QueryScalarType)right.ExpressionType);

            type = this.EvaluationStrategy.RemoveLengthConstraints(type);

            return builderFunc(left, right, type);
        }

        /// <summary>
        /// Resolves types for arguments.
        /// </summary>
        /// <param name="arguments">The arguments to resolve types for.</param>
        /// <returns>Expressions with resolved types.</returns>
        protected QueryExpression[] ResolveTypesForArguments(IEnumerable<QueryExpression> arguments)
        {
            var replacedArguments = arguments.Select(a => this.ResolveTypes(a)).ToArray();
            return replacedArguments;
        }

        /// <summary>
        /// Resolves types for instance of QueryPropertyExpresssion.
        /// TODO: Semi-hack needed for Astoria, breaks SOLID principles. Think of other ways to do it.
        /// </summary>
        /// <param name="expression">Instance expression to resolve types for</param>
        /// <returns>Instance expression with resolved types</returns>
        protected virtual QueryExpression ResolvePropertyInstance(QueryExpression expression)
        {
            return this.ResolveTypes(expression);
        }

        /// <summary>
        /// Resolves built-in function result type
        /// </summary>
        /// <param name="function">The funciton to resolve the result type for.</param>
        /// <param name="resolvedArgumentTypes">Resolved argument types.</param>
        /// <returns>Resolved function result type.</returns>
        protected QueryType ResolveBuiltInFunctionResultType(QueryBuiltInFunction function, QueryType[] resolvedArgumentTypes)
        {
            var resolvedType = function.ReturnTypeResolver != null ? function.ReturnTypeResolver(resolvedArgumentTypes) : this.GetDefaultQueryType(function.ReturnType);
            return resolvedType;
        }

        /// <summary>
        /// Gets the default query type for the given data type.
        /// </summary>
        /// <param name="dataType">The data type.</param>
        /// <returns>Query type.</returns>
        protected QueryType GetDefaultQueryType(DataType dataType)
        {
            return this.TypeLibrary.GetDefaultQueryType(dataType);
        }

        private QueryExpression VisitBinaryExpressionWithBooleanResult(QueryBinaryExpression expression, Func<QueryExpression, QueryExpression, QueryType, QueryExpression> builderFunc)
        {
            var left = this.ResolveTypes(expression.Left);
            var right = this.ResolveTypes(expression.Right);
            var type = this.EvaluationStrategy.BooleanType;

            return builderFunc(left, right, type);
        }

        private QueryScalarType GetQueryScalarTypeFromObjectValue(object value)
        {
            var clrType = value.GetType();

            // special-cased because the default precision and scale loses information
            if (value is decimal)
            {
                return this.GetQueryScalarTypeFromDecimalValue((decimal)value);
            }

            return this.TypeLibrary.GetDefaultQueryScalarType(clrType);
        }

        private QueryScalarType GetQueryScalarTypeFromDecimalValue(decimal value)
        {
            value = Math.Abs(value);
            var truncated = decimal.Truncate(value);
            var integralDigitsCount = truncated.ToString(CultureInfo.InvariantCulture).Length;

            // subtract 2 for leading "0."
            var fractionalDigitsCount = Math.Max((value - truncated).ToString(CultureInfo.InvariantCulture).Length - 2, 0);
            return (QueryScalarType)this.GetDefaultQueryType(EdmDataTypes.Decimal(integralDigitsCount + fractionalDigitsCount, fractionalDigitsCount));
        }
    }
}