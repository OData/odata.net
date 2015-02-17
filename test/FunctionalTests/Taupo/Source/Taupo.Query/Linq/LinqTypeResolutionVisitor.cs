//---------------------------------------------------------------------
// <copyright file="LinqTypeResolutionVisitor.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Query.Linq
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Query.Common;
    using Microsoft.Test.Taupo.Query.Contracts;
    using Microsoft.Test.Taupo.Query.Contracts.Linq;
    using Microsoft.Test.Taupo.Query.Contracts.Linq.Expressions;

    /// <summary>
    /// Visitor which resolves types of Linq specific nodes in the query expressions.
    /// </summary>
    public class LinqTypeResolutionVisitor : CommonExpressionTypeResolutionVisitor, ILinqExpressionVisitor<QueryExpression>
    {
        private Dictionary<LinqParameterExpression, LinqParameterExpression> parameterMappings;

        /// <summary>
        /// Initializes a new instance of the LinqTypeResolutionVisitor class.
        /// </summary>
        /// <param name="typeLibrary">The query type library.</param>
        public LinqTypeResolutionVisitor(QueryTypeLibrary typeLibrary)
            : base(typeLibrary)
        {
            this.parameterMappings = new Dictionary<LinqParameterExpression, LinqParameterExpression>();
        }

        /// <summary>
        /// Resolves types for the specified expression.
        /// </summary>
        /// <param name="expression">The expression to resolve types for.</param>
        /// <returns>Expression with resolved types.</returns>
        public QueryExpression Visit(LinqAllExpression expression)
        {
            var source = this.ResolveTypes(expression.Source);
            var sourceType = ValidateSourceIsACollection(source);
            var lambda = this.ResolveLambdaTypes(expression.Lambda, sourceType);
            var boolType = this.EvaluationStrategy.BooleanType;
            
            return new LinqAllExpression(source, lambda, boolType);
        }

        /// <summary>
        /// Resolves types for the specified expression.
        /// </summary>
        /// <param name="expression">The expression to resolve types for.</param>
        /// <returns>Expression with resolved types.</returns>
        public QueryExpression Visit(LinqAnyExpression expression)
        {
            var source = this.ResolveTypes(expression.Source);
            var sourceType = ValidateSourceIsACollection(source);
            var lambda = expression.Lambda != null ? this.ResolveLambdaTypes(expression.Lambda, sourceType) : null;
            var boolType = this.EvaluationStrategy.BooleanType;

            return new LinqAnyExpression(source, lambda, boolType);
        }

        /// <summary>
        /// Resolves types for the specified expression.
        /// </summary>
        /// <param name="expression">The expression to resolve types for.</param>
        /// <returns>Expression with resolved types.</returns>
        public QueryExpression Visit(LinqAsEnumerableExpression expression)
        {
            var source = this.ResolveTypes(expression.Source);

            return new LinqAsEnumerableExpression(source, source.ExpressionType);
        }

        /// <summary>
        /// Resolves types for the specified expression.
        /// </summary>
        /// <param name="expression">The expression to resolve types for.</param>
        /// <returns>Expression with resolved types.</returns>
        public QueryExpression Visit(LinqBitwiseAndExpression expression)
        {
            return this.VisitBinaryArithmeticExpression(expression, (l, r, t) => new LinqBitwiseAndExpression(l, r, t));
        }

        /// <summary>
        /// Resolves types for the specified expression.
        /// </summary>
        /// <param name="expression">The expression to resolve types for.</param>
        /// <returns>Expression with resolved types.</returns>
        public QueryExpression Visit(LinqBitwiseOrExpression expression)
        {
            return this.VisitBinaryArithmeticExpression(expression, (l, r, t) => new LinqBitwiseOrExpression(l, r, t));
        }

        /// <summary>
        /// Resolves types for the specified expression.
        /// </summary>
        /// <param name="expression">The expression to resolve types for.</param>
        /// <returns>Expression with resolved types.</returns>
        public QueryExpression Visit(LinqBuiltInFunctionCallExpression expression)
        {
            var resolvedArguemnts = this.ResolveTypesForArguments(expression.Arguments);

            var resolvedResultType = this.ResolveBuiltInFunctionResultType(expression.LinqBuiltInFunction.BuiltInFunction, resolvedArguemnts.Select(a => a.ExpressionType).ToArray());

            return new LinqBuiltInFunctionCallExpression(resolvedResultType, expression.LinqBuiltInFunction, resolvedArguemnts);
        }

        /// <summary>
        /// Resolves types for the specified expression.
        /// </summary>
        /// <param name="expression">The expression to resolve types for.</param>
        /// <returns>Expression with resolved types.</returns>
        public QueryExpression Visit(LinqCastExpression expression)
        {
            var source = this.ResolveTypes(expression.Source);

            return source.CastEnumerable(expression.TypeToOperateAgainst);
        }

        /// <summary>
        /// Resolves types for the specified expression.
        /// </summary>
        /// <param name="expression">The expression to resolve types for.</param>
        /// <returns>Expression with resolved types.</returns>
        public QueryExpression Visit(LinqConcatExpression expression)
        {
            var outer = this.ResolveTypes(expression.Outer);
            var inner = this.ResolveTypes(expression.Inner);

            return new LinqConcatExpression(outer, inner, outer.ExpressionType);
        }

        /// <summary>
        /// Resolves types for the specified expression.
        /// </summary>
        /// <param name="expression">The expression to resolve types for.</param>
        /// <returns>Expression with resolved types.</returns>
        public QueryExpression Visit(LinqContainsExpression expression)
        {
            var source = this.ResolveTypes(expression.Source);
            var value = this.ResolveTypes(expression.Value);
            var sourceType = ValidateSourceIsACollection(source);

            // For now, only add support for primitive types
            if (sourceType.ElementType is QueryScalarType)
            {
                ExceptionUtilities.Assert(
                    value.ExpressionType is QueryScalarType,
                    "Element type of a source collection is a primitive but the argument is not. They should both be same types.");

                ExceptionUtilities.Assert(
                    ((IQueryClrType)sourceType.ElementType).ClrType.Equals(((IQueryClrType)value.ExpressionType).ClrType),
                    "Type of the argument to contains should be the same as element type of a source collection.");
            }

            return new LinqContainsExpression(source, value, this.EvaluationStrategy.BooleanType);
        }

        /// <summary>
        /// Resolves types for the specified expression.
        /// </summary>
        /// <param name="expression">The expression to resolve types for.</param>
        /// <returns>Expression with resolved types.</returns>
        public QueryExpression Visit(LinqCountExpression expression)
        {
            var source = this.ResolveTypes(expression.Source);
            var sourceType = ValidateSourceIsACollection(source);
            var lambda = expression.Lambda != null ? this.ResolveLambdaTypes(expression.Lambda, sourceType) : null;
            var intType = this.EvaluationStrategy.IntegerType;

            return new LinqCountExpression(source, lambda, intType);
        }

        /// <summary>
        /// Resolves types for the specified expression.
        /// </summary>
        /// <param name="expression">The expression to resolve types for.</param>
        /// <returns>Expression with resolved types.</returns>
        public QueryExpression Visit(LinqDefaultIfEmptyExpression expression)
        {
            var source = this.ResolveTypes(expression.Source);
            var sourceType = ValidateSourceIsACollection(source);

            if (expression.DefaultValue != null)
            {
                var defaultValue = this.ResolveTypes(expression.DefaultValue);
                return new LinqDefaultIfEmptyExpression(source, sourceType, defaultValue);
            }
            else
            {
                return new LinqDefaultIfEmptyExpression(source, sourceType);
            }
        }

        /// <summary>
        /// Resolves types for the specified expression.
        /// </summary>
        /// <param name="expression">The expression to resolve types for.</param>
        /// <returns>Expression with resolved types.</returns>
        public QueryExpression Visit(LinqDistinctExpression expression)
        {
            var source = this.ResolveTypes(expression.Source);

            return new LinqDistinctExpression(source, source.ExpressionType);
        }

        /// <summary>
        /// Resolves types for the specified expression.
        /// </summary>
        /// <param name="expression">The expression to resolve types for.</param>
        /// <returns>Expression with resolved types.</returns>
        public QueryExpression Visit(LinqExceptExpression expression)
        {
            var outer = this.ResolveTypes(expression.Outer);
            var inner = this.ResolveTypes(expression.Inner);

            ExceptionUtilities.Assert(outer.ExpressionType.IsAssignableFrom(inner.ExpressionType), "Incompatible collection types for linq EXCEPT expression");

            return new LinqExceptExpression(outer, inner, outer.ExpressionType);
        }

        /// <summary>
        /// Resolves types for the specified expression.
        /// </summary>
        /// <param name="expression">The expression to resolve types for.</param>
        /// <returns>Expression with resolved types.</returns>
        public QueryExpression Visit(LinqExclusiveOrExpression expression)
        {
            return this.VisitBinaryArithmeticExpression(expression, (l, r, t) => new LinqExclusiveOrExpression(l, r, t));
        }

        /// <summary>
        /// Resolves types for the specified expression.
        /// </summary>
        /// <param name="expression">The expression to resolve types for.</param>
        /// <returns>Expression with resolved types.</returns>
        public QueryExpression Visit(LinqFirstExpression expression)
        {
            var source = this.ResolveTypes(expression.Source);
            var sourceType = ValidateSourceIsACollection(source);
            var lambda = expression.Lambda != null ? this.ResolveLambdaTypes(expression.Lambda, sourceType) : null;

            return new LinqFirstExpression(source, lambda, sourceType.ElementType);
        }

        /// <summary>
        /// Resolves types for the specified expression.
        /// </summary>
        /// <param name="expression">The expression to resolve types for.</param>
        /// <returns>Expression with resolved types.</returns>
        public QueryExpression Visit(LinqFirstOrDefaultExpression expression)
        {
            var source = this.ResolveTypes(expression.Source);
            var sourceType = ValidateSourceIsACollection(source);
            var lambda = expression.Lambda != null ? this.ResolveLambdaTypes(expression.Lambda, sourceType) : null;

            return new LinqFirstOrDefaultExpression(source, lambda, sourceType.ElementType);
        }

        /// <summary>
        /// Resolves types for the specified expression.
        /// </summary>
        /// <param name="expression">The expression to resolve types for.</param>
        /// <returns>Expression with resolved types.</returns>
        public QueryExpression Visit(LinqFreeVariableExpression expression)
        {
            return expression;
        }

        /// <summary>
        /// Resolves types for the specified expression.
        /// </summary>
        /// <param name="expression">The expression to resolve types for.</param>
        /// <returns>Expression with resolved types.</returns>
        public QueryExpression Visit(LinqGroupByExpression expression)
        {
            var source = this.ResolveTypes(expression.Source);
            var sourceType = ValidateSourceIsACollection(source);
            var keySelector = this.ResolveLambdaTypes(expression.KeySelector, sourceType);
            var elementSelector = expression.ElementSelector != null ? this.ResolveLambdaTypes(expression.ElementSelector, sourceType) : null;
            var keyCollectionType = keySelector.Body.ExpressionType.CreateCollectionType();
            var elementCollectionType = expression.ElementSelector != null ? elementSelector.Body.ExpressionType.CreateCollectionType() : sourceType;

            if (expression.ResultSelector != null)
            {
                // resolve lambda strips the collection out of parameters, but the second parameter to the result is a collection
                // so we pass collection of collections
                var resultSelector = this.ResolveLambdaTypes(expression.ResultSelector, keyCollectionType, elementCollectionType.CreateCollectionType());
                var resultType = resultSelector.Body.ExpressionType;

                return new LinqGroupByExpression(source, keySelector, elementSelector, resultSelector, resultType.CreateCollectionType());
            }
            else
            {
                var groupingType = new QueryGroupingType(keySelector.Body.ExpressionType, elementCollectionType.ElementType, this.EvaluationStrategy);

                return new LinqGroupByExpression(source, keySelector, elementSelector, null, groupingType.CreateCollectionType());
            }
        }

        /// <summary>
        /// Resolves types for the specified expression.
        /// </summary>
        /// <param name="expression">The expression to resolve types for.</param>
        /// <returns>Expression with resolved types.</returns>
        public QueryExpression Visit(LinqGroupJoinExpression expression)
        {
            var outer = this.ResolveTypes(expression.Outer);
            var inner = this.ResolveTypes(expression.Inner);
            var outerType = ValidateSourceIsACollection(outer);
            var innerType = ValidateSourceIsACollection(inner);
            var outerKeySelector = this.ResolveLambdaTypes(expression.OuterKeySelector, outerType);
            var innerKeySelector = this.ResolveLambdaTypes(expression.InnerKeySelector, innerType);

            var resultSelector = this.ResolveLambdaTypes(expression.ResultSelector, outerType, innerType.CreateCollectionType());

            return LinqBuilder.GroupJoin(outer, inner, outerKeySelector, innerKeySelector, resultSelector, resultSelector.Body.ExpressionType.CreateCollectionType());
        }

        /// <summary>
        /// Resolves types for the specified expression.
        /// </summary>
        /// <param name="expression">The expression to resolve types for.</param>
        /// <returns>Expression with resolved types.</returns>
        public QueryExpression Visit(LinqJoinExpression expression)
        {
            var outer = this.ResolveTypes(expression.Outer);
            var inner = this.ResolveTypes(expression.Inner);
            var outerType = ValidateSourceIsACollection(outer);
            var innerType = ValidateSourceIsACollection(inner);
            var outerKeySelector = this.ResolveLambdaTypes(expression.OuterKeySelector, outerType);
            var innerKeySelector = this.ResolveLambdaTypes(expression.InnerKeySelector, innerType);

            var resultSelector = this.ResolveLambdaTypes(expression.ResultSelector, outerType, innerType);
            
            return LinqBuilder.Join(outer, inner, outerKeySelector, innerKeySelector, resultSelector, resultSelector.Body.ExpressionType.CreateCollectionType());
        }

        /// <summary>
        /// Resolves types for the specified expression.
        /// </summary>
        /// <param name="expression">The expression to resolve types for.</param>
        /// <returns>Expression with resolved types.</returns>
        public QueryExpression Visit(LinqLambdaExpression expression)
        {
            var parameters = expression.Parameters.Select(this.ResolveTypes).Cast<LinqParameterExpression>().ToArray();
            var body = this.ResolveTypes(expression.Body);
            var type = new LinqLambdaType(body.ExpressionType, parameters.Select(p => p.ExpressionType), this.EvaluationStrategy);

            return LinqBuilder.Lambda(body, parameters.ToArray(), type);
        }

        /// <summary>
        /// Resolves types for the specified expression.
        /// </summary>
        /// <param name="expression">The expression to resolve types for.</param>
        /// <returns>Expression with resolved types.</returns>
        public QueryExpression Visit(LinqLengthPropertyExpression expression)
        {
            var instance = this.ResolveTypes(expression.Instance);

            return new LinqLengthPropertyExpression(instance, this.EvaluationStrategy.IntegerType);
        }

        /// <summary>
        /// Resolves types for the specified expression.
        /// </summary>
        /// <param name="expression">The expression to resolve types for.</param>
        /// <returns>Expression with resolved types.</returns>
        public QueryExpression Visit(LinqLongCountExpression expression)
        {
            var source = this.ResolveTypes(expression.Source);
            var sourceType = ValidateSourceIsACollection(source);
            var lambda = expression.Lambda != null ? this.ResolveLambdaTypes(expression.Lambda, sourceType) : null;
            var longType = this.EvaluationStrategy.LongIntegerType;

            return new LinqLongCountExpression(source, lambda, longType);
        }

        /// <summary>
        /// Resolves types for the specified expression.
        /// </summary>
        /// <param name="expression">The expression to resolve types for.</param>
        /// <returns>Expression with resolved types.</returns>
        public QueryExpression Visit(LinqMaxExpression expression)
        {
            var source = this.ResolveTypes(expression.Source);
            var sourceType = ValidateSourceIsACollection(source);
            var lambda = expression.Lambda != null ? this.ResolveLambdaTypes(expression.Lambda, sourceType) : null;
            var expressionType = lambda != null ? lambda.Body.ExpressionType : sourceType.ElementType;

            ExceptionUtilities.Assert(expressionType is QueryScalarType, "Max expression without predicate can only be applied on collections of scalars.");
            
            return new LinqMaxExpression(source, lambda, expressionType);
        }

        /// <summary>
        /// Resolves types for the specified expression.
        /// </summary>
        /// <param name="expression">The expression to resolve types for.</param>
        /// <returns>Expression with resolved types.</returns>
        public QueryExpression Visit(LinqMemberMethodExpression expression)
        {
            var resolvedSource = this.ResolveTypes(expression.Source, this.EvaluationStrategy);
            var resolvedResultType = this.GetDefaultQueryType(expression.MemberMethod.ReturnType);
            var resolvedArguemnts = this.ResolveTypesForArguments(expression.Arguments);

            return new LinqMemberMethodExpression(resolvedSource, expression.MemberMethod, resolvedResultType, resolvedArguemnts);
        }

        /// <summary>
        /// Resolves types for the specified expression.
        /// </summary>
        /// <param name="expression">The expression to resolve types for.</param>
        /// <returns>Expression with resolved types.</returns>
        public QueryExpression Visit(LinqMinExpression expression)
        {
            var source = this.ResolveTypes(expression.Source);
            var sourceType = ValidateSourceIsACollection(source);
            var lambda = expression.Lambda != null ? this.ResolveLambdaTypes(expression.Lambda, sourceType) : null;
            var expressionType = lambda != null ? lambda.Body.ExpressionType : sourceType.ElementType;
            
            ExceptionUtilities.Assert(expressionType is QueryScalarType, "Min expression without predicate can only be applied on collections of scalars.");
            
            return new LinqMinExpression(source, lambda, expressionType);
        }

        /// <summary>
        /// Resolves types for the specified expression.
        /// </summary>
        /// <param name="expression">The expression to resolve types for.</param>
        /// <returns>Expression with resolved types.</returns>
        public QueryExpression Visit(LinqNewArrayExpression expression)
        {
            var resolvedChildExpressions = this.ResolveTypesForArguments(expression.Expressions).ToList();
            var expressionType = expression.ExpressionType;
            if (expressionType.IsUnresolved)
            {
                ExceptionUtilities.Assert(resolvedChildExpressions.Count > 0, "Cannot resolve type of empty array expression");
                var elementType = resolvedChildExpressions[0].ExpressionType;
                for (int i = 1; i < resolvedChildExpressions.Count; i++)
                {
                    var nextType = resolvedChildExpressions[i].ExpressionType;

                    // if the next type is less-derived than the current one, we need to move it
                    // however, because equivalent types are also assignable, we can always take it if this
                    // check is true
                    if (nextType.IsAssignableFrom(elementType))
                    {
                        elementType = nextType;
                    }
                }

                expressionType = elementType.CreateCollectionType();
            }

            return new LinqNewArrayExpression(expressionType, resolvedChildExpressions);
        }

        /// <summary>
        /// Resolves types for the specified expression.
        /// </summary>
        /// <param name="expression">The expression to resolve types for.</param>
        /// <returns>Expression with resolved types.</returns>
        public QueryExpression Visit(LinqNewExpression expression)
        {
            var members = expression.Members.Select(this.ResolveTypes).ToList();
            var type = new QueryAnonymousStructuralType(this.EvaluationStrategy);
            for (int i = 0; i < members.Count; i++)
            {
                type.Add(QueryProperty.Create(expression.MemberNames[i], members[i].ExpressionType));
            }

            return LinqBuilder.New(expression.MemberNames, members, type);
        }

        /// <summary>
        /// Resolves types for the specified expression.
        /// </summary>
        /// <param name="expression">The expression to resolve types for.</param>
        /// <returns>Expression with resolved types.</returns>
        public QueryExpression Visit(LinqNewInstanceExpression expression)
        {
            var constructorArguments = expression.ConstructorArguments.Select(this.ResolveTypes).ToList();
            var members = expression.Members.Select(this.ResolveTypes).ToList();

            return LinqBuilder.NewInstance(constructorArguments, expression.MemberNames, members, expression.ExpressionType);
        }

        /// <summary>
        /// Resolves types for the specified expression.
        /// </summary>
        /// <param name="expression">The expression to resolve types for.</param>
        /// <returns>Expression with resolved types.</returns>
        public QueryExpression Visit(LinqOrderByExpression expression)
        {
            var source = this.ResolveTypes(expression.Source);
            var sourceType = ValidateSourceIsACollection(source);
            var keySelectors = expression.KeySelectors.Select(s => this.ResolveLambdaTypes(s, sourceType)).ToArray();

            return LinqBuilder.OrderBy(source, keySelectors, expression.AreDescending, sourceType);
        }

        /// <summary>
        /// Resolves types for the specified expression.
        /// </summary>
        /// <param name="expression">The expression to resolve types for.</param>
        /// <returns>Expression with resolved types.</returns>
        public QueryExpression Visit(LinqParameterExpression expression)
        {
            return this.parameterMappings[expression];
        }

        /// <summary>
        /// Resolves types for the specified expression.
        /// </summary>
        /// <param name="expression">The expression to resolve types for.</param>
        /// <returns>Expression with resolved types.</returns>
        public QueryExpression Visit(LinqSelectExpression expression)
        {
            var source = this.ResolveTypes(expression.Source);
            var sourceType = ValidateSourceIsACollection(source);
            var lambda = this.ResolveLambdaTypes(expression.Lambda, sourceType);

            return new LinqSelectExpression(source, lambda, lambda.Body.ExpressionType.CreateCollectionType());
        }

        /// <summary>
        /// Resolves types for the specified expression.
        /// </summary>
        /// <param name="expression">The expression to resolve types for.</param>
        /// <returns>Expression with resolved types.</returns>
        public QueryExpression Visit(LinqSelectManyExpression expression)
        {
            var source = this.ResolveTypes(expression.Source);
            var sourceType = ValidateSourceIsACollection(source);
            var collectionSelector = this.ResolveLambdaTypes(expression.CollectionSelector, sourceType);
            var elementCollectionType = ValidateSourceIsACollection(collectionSelector.Body);

            if (expression.ResultSelector != null)
            {
                var resultSelector = this.ResolveLambdaTypes(expression.ResultSelector, sourceType, elementCollectionType);

                return new LinqSelectManyExpression(source, collectionSelector, resultSelector, resultSelector.Body.ExpressionType.CreateCollectionType());
            }
            else
            {
                return new LinqSelectManyExpression(source, collectionSelector, null, elementCollectionType);
            }
        }

        /// <summary>
        /// Resolves types for the specified expression.
        /// </summary>
        /// <param name="expression">The expression to resolve types for.</param>
        /// <returns>Expression with resolved types.</returns>
        public QueryExpression Visit(LinqSingleExpression expression)
        {
            var source = this.ResolveTypes(expression.Source);
            var sourceType = ValidateSourceIsACollection(source);
            var lambda = expression.Lambda != null ? this.ResolveLambdaTypes(expression.Lambda, sourceType) : null;

            return new LinqSingleExpression(source, lambda, sourceType.ElementType);
        }

        /// <summary>
        /// Resolves types for the specified expression.
        /// </summary>
        /// <param name="expression">The expression to resolve types for.</param>
        /// <returns>Expression with resolved types.</returns>
        public QueryExpression Visit(LinqSingleOrDefaultExpression expression)
        {
            var source = this.ResolveTypes(expression.Source);
            var sourceType = ValidateSourceIsACollection(source);
            var lambda = expression.Lambda != null ? this.ResolveLambdaTypes(expression.Lambda, sourceType) : null;

            return new LinqSingleOrDefaultExpression(source, lambda, sourceType.ElementType);
        }

        /// <summary>
        /// Resolves types for the specified expression.
        /// </summary>
        /// <param name="expression">The expression to resolve types for.</param>
        /// <returns>Expression with resolved types.</returns>
        public QueryExpression Visit(LinqSkipExpression expression)
        {
            var source = this.ResolveTypes(expression.Source);
            var skipCount = this.ResolveTypes(expression.SkipCount);

            return new LinqSkipExpression(source, skipCount, source.ExpressionType);
        }

        /// <summary>
        /// Resolves types for the specified expression.
        /// </summary>
        /// <param name="expression">The expression to resolve types for.</param>
        /// <returns>Expression with resolved types.</returns>
        public QueryExpression Visit(LinqTakeExpression expression)
        {
            var source = this.ResolveTypes(expression.Source);
            var sourceType = ValidateSourceIsACollection(source);
            var takeCount = this.ResolveTypes(expression.TakeCount);

            return new LinqTakeExpression(source, takeCount, sourceType);
        }

        /// <summary>
        /// Resolves types for the specified expression.
        /// </summary>
        /// <param name="expression">The expression to resolve types for.</param>
        /// <returns>Expression with resolved types.</returns>
        public QueryExpression Visit(LinqUnionExpression expression)
        {
            var firstSource = this.ResolveTypes(expression.FirstSource);
            var secondSource = this.ResolveTypes(expression.SecondSource);
            var firstSourceType = ValidateSourceIsACollection(firstSource);
            var secondSourceType = ValidateSourceIsACollection(secondSource);

            // For now, only add support for primitive types
            ExceptionUtilities.Assert(
            ((IQueryClrType)firstSourceType.ElementType).ClrType.Equals(((IQueryClrType)secondSourceType.ElementType).ClrType),
            "Expecting same types in both collections.");

            return new LinqUnionExpression(firstSource, secondSource, firstSourceType);
        }

        /// <summary>
        /// Resolves types for the specified expression.
        /// </summary>
        /// <param name="expression">The expression to resolve types for.</param>
        /// <returns>Expression with resolved types.</returns>
        public QueryExpression Visit(LinqWhereExpression expression)
        {
            var source = this.ResolveTypes(expression.Source);
            var sourceType = ValidateSourceIsACollection(source);
            var lambda = this.ResolveLambdaTypes(expression.Lambda, sourceType);

            return new LinqWhereExpression(source, lambda, sourceType);
        }

        /// <summary>
        /// Validates that source is a collection.
        /// </summary>
        /// <param name="source">Source to verify.</param>
        /// <returns>Type of the source.</returns>
        protected static QueryCollectionType ValidateSourceIsACollection(QueryExpression source)
        {
            var result = source.ExpressionType as QueryCollectionType;
            ExceptionUtilities.Assert(result != null, "Expecting collection type.");

            return result;
        }

        private LinqLambdaExpression ResolveLambdaTypes(LinqLambdaExpression lambda, params QueryCollectionType[] parameterCollectionTypes)
        {
            ExceptionUtilities.Assert(lambda.Parameters.Count == parameterCollectionTypes.Length, "Lambda expression parameter numbers must match the length of parameterCollectionTypes.");

            for (int i = 0; i < lambda.Parameters.Count; i++)
            {
                var lambdaParameter = lambda.Parameters[i];
                this.parameterMappings.Add(lambdaParameter, LinqBuilder.Parameter(lambdaParameter.Name, parameterCollectionTypes[i].ElementType));
            }

            try
            {
                return (LinqLambdaExpression)this.ResolveTypes(lambda);
            }
            finally
            {
                foreach (var p in lambda.Parameters)
                {
                    this.parameterMappings.Remove(p);
                }
            }
        }
    }
}
