//---------------------------------------------------------------------
// <copyright file="LinqToAstoriaEvaluator.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.LinqToAstoria
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Test.Taupo.Astoria.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Astoria.Contracts.LinqToAstoria;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Query.Common;
    using Microsoft.Test.Taupo.Query.Contracts;
    using Microsoft.Test.Taupo.Query.Contracts.CommonExpressions;
    using Microsoft.Test.Taupo.Query.Contracts.Linq;
    using Microsoft.Test.Taupo.Query.Contracts.Linq.Expressions;
    using Microsoft.Test.Taupo.Query.Linq;

    /// <summary>
    /// Evaluates query expressions for a given query data set.
    /// </summary>
    [ImplementationName(typeof(ILinqToAstoriaExpressionEvaluator), "Default")]
    public class LinqToAstoriaEvaluator : ILinqToAstoriaExpressionEvaluator
    {
        /// <summary>
        /// Gets or sets the query resolver.
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public ILinqToAstoriaQueryResolver QueryResolver { get; set; }

        /// <summary>
        /// Gets or sets the query data set.
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public IQueryDataSet QueryDataSet { get; set; }

        /// <summary>
        /// Evaluates the specified expression.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <returns>Value of the expression.</returns>
        public QueryValue Evaluate(QueryExpression expression)
        {
            return this.Evaluate(expression, new Dictionary<string, QueryExpression>());
        }

        /// <summary>
        /// Evaluates the specified expression.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <param name="freeVariableAssignments">Free variable assignments.</param>
        /// <returns>Value of the expression.</returns>
        public QueryValue Evaluate(QueryExpression expression, IDictionary<string, QueryExpression> freeVariableAssignments)
        {
            ExceptionUtilities.CheckArgumentNotNull(expression, "expression");
            ExceptionUtilities.CheckArgumentNotNull(freeVariableAssignments, "freeVariableAssignments");

            var visitor = new LinqToAstoriaEvaluatingVisitor(this.QueryDataSet, freeVariableAssignments);

            expression = this.QueryResolver.Resolve(expression);
            return visitor.Evaluate(expression);
        }

        /// <summary>
        /// Temporarily replaces the evaluator's data-set with the one given
        /// </summary>
        /// <param name="temporary">The temporary query data-set</param>
        /// <returns>A token that, when disposed, will reset the query data-set back to the original.</returns>
        public IDisposable WithTemporaryDataSet(IQueryDataSet temporary)
        {
            var original = this.QueryDataSet;
            this.QueryDataSet = temporary;
            return new DelegateBasedDisposable(() => this.QueryDataSet = original);
        }

        /// <summary>
        /// Evaluates Linq-specific expression trees.
        /// </summary>
        internal class LinqToAstoriaEvaluatingVisitor : LinqEvaluatingVisitor, ILinqToAstoriaExpressionVisitor<QueryValue>
        {
            /// <summary>
            /// Initializes a new instance of the LinqToAstoriaEvaluatingVisitor class.
            /// </summary>
            /// <param name="dataSet">The data set.</param>
            /// <param name="freeVariableAssignments">Free variable assignments.</param>
            internal LinqToAstoriaEvaluatingVisitor(IQueryDataSet dataSet, IDictionary<string, QueryExpression> freeVariableAssignments)
                : base(dataSet, freeVariableAssignments)
            {
            }

            /// <summary>
            /// Evaluates the specified expression.
            /// </summary>
            /// <param name="expression">The expression to evaluate.</param>
            /// <returns>Value of the expression.</returns>
            public override QueryValue Visit(LinqCountExpression expression)
            {
                return this.VisitCollectionElementPrimitiveOrComplexTypeError(
                    expression,
                    delegate
                    {
                        return base.Visit(expression);
                    });
            }

            /// <summary>
            /// Visits a LinqDistinctExpression. This expression is not supported in Linq to Astoria.
            /// </summary>
            /// <param name="expression">The expression.</param>
            /// <returns>The result of visiting this expression.</returns>
            public override QueryValue Visit(LinqDistinctExpression expression)
            {
                var source = this.EvaluateCollection(expression.Source);

                var sourceError = QueryError.GetErrorFromValues(source.Elements);
                var error = QueryError.Combine(sourceError, new QueryError("This expression is not supported in Linq to Astoria."));

                return expression.Source.ExpressionType.CreateErrorValue(error);
            }

            /// <summary>
            /// Evaluates the specified expression.
            /// </summary>
            /// <param name="expression">The expression to evaluate.</param>
            /// <returns>Value of the expression.</returns>
            public override QueryValue Visit(LinqLongCountExpression expression)
            {
                return this.VisitCollectionElementPrimitiveOrComplexTypeError(
                    expression,
                    delegate
                    {
                        return base.Visit(expression);
                    });
            }

            /// <summary>
            /// Evaluates the specified expression.
            /// </summary>
            /// <param name="expression">The expression to evaluate.</param>
            /// <returns>Value of the expression.</returns>
            public override QueryValue Visit(LinqNewInstanceExpression expression)
            {
                ExceptionUtilities.CheckArgumentNotNull(expression, "expression");

                if (expression.ExpressionType is AstoriaQueryStreamType)
                {
                    // Handle expressions like: new DataServiceStreamLink(c, "Photo")
                    const int ExpectedArgumentCount = 2;
                    int actualArgumentCount = expression.ConstructorArguments.Count;
                    ExceptionUtilities.Assert(
                        actualArgumentCount == ExpectedArgumentCount,
                        "Expected {0} arguments in the constructor. Actual: {1}",
                        ExpectedArgumentCount,
                        actualArgumentCount);

                    var source = (QueryStructuralValue)this.Evaluate(expression.ConstructorArguments[0]);
                    string streamName = (string)((QueryScalarValue)this.Evaluate(expression.ConstructorArguments[1])).Value;

                    return source.GetValue(streamName);
                }
                else
                {
                    return base.Visit(expression);
                }
            }

            /// <summary>
            /// Evaluates the specified expression.
            /// </summary>
            /// <param name="expression">The expression to evaluate.</param>
            /// <returns>Value of the expression.</returns>
            public override QueryValue Visit(LinqOrderByExpression expression)
            {
                var value = this.VisitCollectionElementPrimitiveOrComplexTypeError(
                    expression,
                    delegate
                    {
                        return base.Visit(expression);
                    });

                var collection = value as QueryCollectionValue;
                var strategy = collection.Type.ElementType.EvaluationStrategy as ILinqToAstoriaQueryEvaluationStrategy;
                ExceptionUtilities.CheckObjectNotNull(strategy, "Cannot get astoria-specific evaluation strategy from collection value.");

                if (strategy.IsCollectionOrderPredictable)
                {
                    return QueryCollectionValue.Create(collection.Type.ElementType, collection.Elements, true);
                }

                return value;
            }

            /// <summary>
            /// Evaluates the specified expression.
            /// </summary>
            /// <param name="expression">The expression to evaluate.</param>
            /// <returns>Value of the expression.</returns>
            public override QueryValue Visit(LinqSelectExpression expression)
            {
                return this.VisitCollectionElementPrimitiveOrComplexTypeError(
                    expression,
                    delegate
                    {
                        return base.Visit(expression);
                    });
            }

            /// <summary>
            /// Evaluates the specified expression.
            /// </summary>
            /// <param name="expression">The expression to evaluate.</param>
            /// <returns>Value of the expression.</returns>
            public override QueryValue Visit(LinqSkipExpression expression)
            {
                return this.VisitCollectionElementPrimitiveOrComplexTypeError(
                    expression,
                    delegate
                    {
                        return base.Visit(expression);
                    });
            }

            /// <summary>
            /// Evaluates the specified expression.
            /// </summary>
            /// <param name="expression">The expression to evaluate.</param>
            /// <returns>Value of the expression.</returns>
            public override QueryValue Visit(LinqTakeExpression expression)
            {
                return this.VisitCollectionElementPrimitiveOrComplexTypeError(
                    expression,
                    delegate
                    {
                        return base.Visit(expression);
                    });
            }

            /// <summary>
            /// Visits a LinqAddQueryOptionExpression.
            /// </summary>
            /// <param name="expression">The expression.</param>
            /// <returns>Value of the expression</returns>
            public QueryValue Visit(LinqToAstoriaAddQueryOptionExpression expression)
            {
                ExceptionUtilities.CheckArgumentNotNull(expression, "expression");
                string queryOption = expression.QueryOption;

                if (queryOption.Equals("$top", StringComparison.OrdinalIgnoreCase) || queryOption.Equals("$skip", StringComparison.OrdinalIgnoreCase))
                {
                    var source = this.EvaluateCollection(expression.Source);
                    var evalStrategy = new LinqToAstoriaClrQueryEvaluationStrategy();
                    QueryScalarValue count = new QueryScalarValue(evalStrategy.IntegerType, (int)expression.QueryValue, null, evalStrategy);

                    if (queryOption.Equals("$top", StringComparison.OrdinalIgnoreCase))
                    {
                        return source.Take(count);
                    }
                    else
                    {
                        return source.Skip(count);
                    }
                }
                else
                {
                    return this.Evaluate(expression.Source);
                }
            }

            /// <summary>
            /// Evaluates the specified expression.
            /// </summary>
            /// <param name="expression">The expression to evaluate.</param>
            /// <returns>Value of the expression.</returns>
            public virtual QueryValue Visit(LinqToAstoriaConditionalExpression expression)
            {
                var binaryQueryExpressionValue = (QueryScalarValue)expression.Condition.Accept(this);
                ExceptionUtilities.CheckObjectNotNull(binaryQueryExpressionValue, "Conditional expression evaluated to null");

                bool binaryValue = (bool)binaryQueryExpressionValue.Value;
                if (binaryValue)
                {
                    return expression.IfTrue.Accept(this);
                }
                else
                {
                    return expression.IfFalse.Accept(this);
                }
            }

            /// <summary>
            /// Visits a LinqExpandExpression.
            /// </summary>
            /// <param name="expression">The expression.</param>
            /// <returns>Value of the expression</returns>
            public QueryValue Visit(LinqToAstoriaExpandExpression expression)
            {
                // expand is not handled here, instead it's handled in the trimming phase after
                // the whole expression has been evaluated
                var expanded = this.Evaluate(expression.Source);

                // if expanding a collection using sql strategy, we do not guarantee the order of top level set.
                var collection = expanded as QueryCollectionValue;
                if (collection != null)
                {
                    var strategy = collection.Type.ElementType.EvaluationStrategy as ILinqToAstoriaQueryEvaluationStrategy;
                    ExceptionUtilities.CheckObjectNotNull(strategy, "Cannot get astoria-specific evaluation strategy from collection value.");

                    if (!strategy.IsCollectionOrderPredictable)
                    {
                        return QueryCollectionValue.Create(collection.Type.ElementType, collection.Elements, false);
                    }
                }

                return expanded;
            }

            /// <summary>
            /// Visits a LinqToAstoriaExpandLambdaExpression.
            /// </summary>
            /// <param name="expression">The expression.</param>
            /// <returns>The result of visiting this expression.</returns>
            public QueryValue Visit(LinqToAstoriaExpandLambdaExpression expression)
            {
                return this.Evaluate(expression.ToLinqToAstoriaExpandExpression());
            }

            /// <summary>
            /// Visits a LinqKeyExpression.
            /// </summary>
            /// <param name="expression">The expression.</param>
            /// <returns>Value of the expression</returns>
            public QueryValue Visit(LinqToAstoriaKeyExpression expression)
            {
                ExceptionUtilities.CheckArgumentNotNull(expression, "expression");

                var collectionType = expression.ExpressionType as QueryCollectionType;
                var replaced = expression.Source.Where(expression.Lambda);
                if (collectionType != null)
                {
                    replaced = replaced.OfType(collectionType.ElementType);
                }
                else
                {
                    replaced = replaced.SingleOrDefault().As(expression.ExpressionType);
                }

                return this.Evaluate(replaced);
            }

            /// <summary>
            /// Visits a LinqToAstoriaLinksExpression.
            /// </summary>
            /// <param name="expression">The expression.</param>
            /// <returns>The result of visiting this expression.</returns>
            public QueryValue Visit(LinqToAstoriaLinksExpression expression)
            {
                return this.Evaluate(expression.Source);
            }

            /// <summary>
            /// Visits a LinqToAstoriaValueExpression.
            /// </summary>
            /// <param name="expression">The expression.</param>
            /// <returns>The result of visiting this expression.</returns>
            public QueryValue Visit(LinqToAstoriaValueExpression expression)
            {
                return this.Evaluate(expression.Source);
            }

            /// <summary>
            /// Evaluates the specified expression.
            /// </summary>
            /// <param name="expression">The expression to evaluate.</param>
            /// <returns>Value of the expression.</returns>
            public override QueryValue Visit(LinqWhereExpression expression)
            {
                return this.VisitCollectionElementPrimitiveOrComplexTypeError(
                    expression,
                    delegate
                    {
                        return base.Visit(expression);
                    });
            }

            /// <summary>
            /// Evaluates the specified expression.
            /// </summary>
            /// <param name="expression">The expression to evaluate</param>
            /// <returns>Value of the expression</returns>
            public override QueryValue Visit(QueryCustomFunctionCallExpression expression)
            {
                var result = base.Visit(expression);

                var svcOpAnnotation = expression.Function.Annotations.OfType<LegacyServiceOperationAnnotation>().SingleOrDefault();
                if (IsSingleResultQueryableOrEnumerable(svcOpAnnotation))
                {
                    var collection = result as QueryCollectionValue;
                    ExceptionUtilities.CheckObjectNotNull(collection, "Result should have been a collection for a queryable/enumerable svc op. Result was '{0}'", result);

                    result = collection.SingleOrDefault();
                }

                return result;
            }

            /// <summary>
            /// Evaluates the specified expression.
            /// </summary>
            /// <param name="expression">The expression to evaluate</param>
            /// <returns>Value of the expression</returns>
            public override QueryValue Visit(QueryPropertyExpression expression)
            {
                if (expression.ExpressionType is AstoriaQueryStreamType)
                {
                    var originalSource = this.Evaluate(expression.Instance);
                    var source = originalSource as QueryStructuralValue;

                    return source.GetValue(expression.Name);
                }
                else
                {
                    return base.Visit(expression);
                }
            }

            internal static bool IsSingleResultQueryableOrEnumerable(LegacyServiceOperationAnnotation svcOpAnnotation)
            {
                if (svcOpAnnotation == null)
                {
                    return false;
                }

                if (!svcOpAnnotation.SingleResult)
                {
                    return false;
                }

                return svcOpAnnotation.ReturnTypeQualifier == ServiceOperationReturnTypeQualifier.IEnumerable
                    || svcOpAnnotation.ReturnTypeQualifier == ServiceOperationReturnTypeQualifier.IQueryable;
            }

            /// <summary>
            /// Creates a visitor for replacing function parameter references.
            /// </summary>
            /// <param name="customFunction">The custom function.</param>
            /// <param name="arguments">The arguments for the function call.</param>
            /// <returns>Linq to Astoria visitor for replacing function parameter references.</returns>
            protected override IQueryExpressionReplacingVisitor CreateFunctionParameterReferenceReplacingVisitor(Function customFunction, IEnumerable<QueryExpression> arguments)
            {
                return new LinqToAstoriaReplaceFunctionParameterReferenceVisitor(customFunction, arguments);
            }

            /// <summary>
            /// Determines if the source that is visiting a LinqQueryMethodExpression is a QueryCollection of Primitive or ComplexTypes
            /// and if it is it writes an error in, other wise it continues to evaluate this using the base Linq evaluator
            /// </summary>
            /// <param name="expression">The expression to evaluate.</param>
            /// <param name="entityCollectionVisitAction">Base expression to visit</param>
            /// <returns>Value of the expression.</returns>
            private QueryValue VisitCollectionElementPrimitiveOrComplexTypeError(LinqQueryMethodExpression expression, Func<QueryValue> entityCollectionVisitAction)
            {
                QueryValue queryValue = null;
                var source = this.EvaluateCollection(expression.Source);

                if (source.Type.ElementType is QueryComplexType || (source.Type.ElementType is QueryScalarType && !(source.Type.ElementType is QueryClrSpatialType)))
                {
                    var sourceError = QueryError.GetErrorFromValues(source.Elements);
                    var error = QueryError.Combine(sourceError, new QueryError("This expression is not supported in Linq to Astoria."));

                    queryValue = expression.Source.ExpressionType.CreateErrorValue(error);
                }
                else
                {
                    queryValue = entityCollectionVisitAction();
                }

                return queryValue;
            }
        }
    }
}
