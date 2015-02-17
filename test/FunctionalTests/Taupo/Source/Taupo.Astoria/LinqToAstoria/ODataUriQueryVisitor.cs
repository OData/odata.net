//---------------------------------------------------------------------
// <copyright file="ODataUriQueryVisitor.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.LinqToAstoria
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Test.Taupo.Astoria.Contracts;
    using Microsoft.Test.Taupo.Astoria.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Astoria.Contracts.LinqToAstoria;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;
    using Microsoft.Test.Taupo.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Query.Common;
    using Microsoft.Test.Taupo.Query.Contracts;
    using Microsoft.Test.Taupo.Query.Contracts.CommonExpressions;
    using Microsoft.Test.Taupo.Query.Contracts.Linq.Expressions;

    /// <summary>
    /// Visitor to build an OData uri from a query
    /// </summary>
    [ImplementationName(typeof(IQueryToODataUriConverter), "Default")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling",
        Justification = "Visitor pattern forces coupling")]
    public class ODataUriQueryVisitor : IQueryToODataUriConverter, ILinqToAstoriaExpressionVisitor<IList<ODataUriSegment>>
    {
        private QueryBasedODataUri uriInProgress;
        private LinqToAstoriaExpandExpression unparsedExpand;

        /// <summary>
        /// Gets or sets the query resolver to resolve untyped expressions to runtime-type bound expression
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public ILinqToAstoriaQueryResolver QueryResolver { get; set; }

        /// <summary>
        /// Gets or sets the formatter to use for key values
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public IODataLiteralConverter LiteralConverter { get; set; }

        /// <summary>
        /// Gets or sets the converter to use when building OData string uri out of a query expression
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public IQueryToODataUriExpressionConverter ExpressionConverter { get; set; }

        /// <summary>
        /// Constructs an odata uri for the given expression
        /// </summary>
        /// <param name="expression">The expression to build a uri from</param>
        /// <returns>A uri based on the given expression.</returns>
        public ODataUri ComputeUri(QueryExpression expression)
        {
            ExceptionUtilities.CheckArgumentNotNull(expression, "expression");
            ExceptionUtilities.CheckAllRequiredDependencies(this);
            
            this.unparsedExpand = null;

            expression = this.QueryResolver.Resolve(expression);

            this.uriInProgress = new QueryBasedODataUri();

            foreach (var segment in expression.Accept(this))
            {
                this.uriInProgress.Segments.Add(segment);
            }

            if (this.uriInProgress.OrderByExpressions.Count > 0)
            {
                List<string> orderByStrings = new List<string>();
                foreach (var orderBy in this.uriInProgress.OrderByExpressions)
                {
                    for (int i = 0; i < orderBy.KeySelectors.Count; i++)
                    {
                        var asString = this.ExpressionConverter.Convert(orderBy.KeySelectors[i].Body);

                        // TODO: how to specify no indicator?
                        if (orderBy.AreDescending[i])
                        {
                            asString += " desc";
                        }
                        else
                        {
                            asString += " asc";
                        }

                        orderByStrings.Add(asString);
                    }
                }

                this.uriInProgress.OrderBy = string.Join(",", orderByStrings.ToArray());
            }

            if (this.uriInProgress.FilterExpressions.Count > 0)
            {
                var filterExpression = this.uriInProgress.FilterExpressions.First().Body;
                for (int i = 1; i < this.uriInProgress.FilterExpressions.Count; i++)
                {
                    filterExpression = CommonQueryBuilder.And(filterExpression, this.uriInProgress.FilterExpressions[i].Body);
                }

                this.uriInProgress.Filter = this.ExpressionConverter.Convert(filterExpression);
            }

            if (this.unparsedExpand != null)
            {
                this.AddToSegmentPathCollection(this.uriInProgress.ExpandSegments, this.unparsedExpand.ExpressionType, this.unparsedExpand.ExpandString);
            }

            return this.uriInProgress;
        }

        /// <summary>
        /// Visits a QueryExpression tree whose root node is the LinqAllExpression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>Uri based on the given expression.</returns>
        public IList<ODataUriSegment> Visit(LinqAllExpression expression)
        {
            throw new TaupoNotSupportedException("Handled by separate visitor");
        }

        /// <summary>
        /// Visits a QueryExpression tree whose root node is the LinqAnyExpression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>Uri based on the given expression.</returns>
        public IList<ODataUriSegment> Visit(LinqAnyExpression expression)
        {
            throw new TaupoNotSupportedException("Handled by separate visitor");
        }

        /// <summary>
        /// Visits a QueryExpression tree whose root node is the LinqAsEnumerableExpression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>Uri based on the given expression.</returns>
        public IList<ODataUriSegment> Visit(LinqAsEnumerableExpression expression)
        {
            throw new TaupoNotSupportedException("Not supported");
        }

        /// <summary>
        /// Visits a QueryExpression tree whose root node is the LinqBitwiseAndExpression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>Uri based on the given expression.</returns>
        public IList<ODataUriSegment> Visit(LinqBitwiseAndExpression expression)
        {
            throw new TaupoNotSupportedException("Handled by separate visitor");
        }

        /// <summary>
        /// Visits a QueryExpression tree whose root node is the LinqBitwiseOrExpression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>Uri based on the given expression.</returns>
        public IList<ODataUriSegment> Visit(LinqBitwiseOrExpression expression)
        {
            throw new TaupoNotSupportedException("Handled by separate visitor");
        }

        /// <summary>
        /// Visits a QueryExpression tree whose root node is the LinqBuiltInFunctionCallExpression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>Uri based on the given expression.</returns>
        public IList<ODataUriSegment> Visit(LinqBuiltInFunctionCallExpression expression)
        {
            throw new TaupoNotSupportedException("This operation is not supported in URI.");
        }

        /// <summary>
        /// Visits a QueryExpression tree whose root node is the LinqCountExpression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>Uri based on the given expression.</returns>
        public IList<ODataUriSegment> Visit(LinqCastExpression expression)
        {
            throw new TaupoNotSupportedException("This operation is not supported in URI.");
        }

        /// <summary>
        /// Visits a QueryExpression tree whose root node is the LinqConcatExpression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>Uri based on the given expression.</returns>
        public IList<ODataUriSegment> Visit(LinqConcatExpression expression)
        {
            throw new TaupoNotSupportedException("Not supported");
        }

        /// <summary>
        /// Visits a QueryExpression tree whose root node is the LinqContainsExpression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>Uri based on the given expression.</returns>
        public IList<ODataUriSegment> Visit(LinqContainsExpression expression)
        {
            throw new TaupoNotSupportedException("This operation is not supported in URI.");
        }

        /// <summary>
        /// Visits a QueryExpression tree whose root node is the LinqCountExpression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>Uri based on the given expression.</returns>
        public IList<ODataUriSegment> Visit(LinqCountExpression expression)
        {
            var segments = expression.Source.Accept(this);
            segments.Add(SystemSegment.Count);
            return segments;
        }

        /// <summary>
        /// Visits a QueryExpression tree whose root node is the LinqDefaultIfEmptyExpression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>Uri based on the given expression.</returns>
        public IList<ODataUriSegment> Visit(LinqDefaultIfEmptyExpression expression)
        {
            throw new TaupoNotSupportedException("Not supported");
        }

        /// <summary>
        /// Visits a QueryExpression tree whose root node is the LinqDistinctExpression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>Uri based on the given expression.</returns>
        public IList<ODataUriSegment> Visit(LinqDistinctExpression expression)
        {
            throw new TaupoNotSupportedException("Not supported");
        }

        /// <summary>
        /// Visits a QueryExpression tree whose root node is the LinqExceptExpression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>Uri based on the given expression.</returns>
        public IList<ODataUriSegment> Visit(LinqExceptExpression expression)
        {
            throw new TaupoNotSupportedException("Not supported");
        }

        /// <summary>
        /// Visits a QueryExpression tree whose root node is the LinqExclusiveOrExpression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>Uri based on the given expression.</returns>
        public IList<ODataUriSegment> Visit(LinqExclusiveOrExpression expression)
        {
            throw new TaupoNotSupportedException("Handled by separate visitor");
        }

        /// <summary>
        /// Visits a QueryExpression tree whose root node is the LinqFirstExpression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>Uri based on the given expression.</returns>
        public IList<ODataUriSegment> Visit(LinqFirstExpression expression)
        {
            throw new TaupoNotSupportedException("Not supported");
        }

        /// <summary>
        /// Visits a QueryExpression tree whose root node is the LinqFirstOrDefaultExpression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>Uri based on the given expression.</returns>
        public IList<ODataUriSegment> Visit(LinqFirstOrDefaultExpression expression)
        {
            throw new TaupoNotSupportedException("Not supported");
        }

        /// <summary>
        /// Visits a QueryExpression tree whose root node is the LinqFreeVariableExpression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>Uri based on the given expression.</returns>
        public IList<ODataUriSegment> Visit(LinqFreeVariableExpression expression)
        {
            throw new TaupoNotSupportedException("Handled by separate visitor");
        }

        /// <summary>
        /// Visits a QueryExpression tree whose root node is the LinqGroupByExpression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>Uri based on the given expression.</returns>
        public IList<ODataUriSegment> Visit(LinqGroupByExpression expression)
        {
            throw new TaupoNotSupportedException("Not supported");
        }

        /// <summary>
        /// Visits a QueryExpression tree whose root node is the LinqGroupJoinExpression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>Uri based on the given expression.</returns>
        public IList<ODataUriSegment> Visit(LinqGroupJoinExpression expression)
        {
            throw new TaupoNotSupportedException("Not supported");
        }

        /// <summary>
        /// Visits a QueryExpression tree whose root node is the LinqJoinExpression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>Uri based on the given expression.</returns>
        public IList<ODataUriSegment> Visit(LinqJoinExpression expression)
        {
            throw new TaupoNotSupportedException("Not supported");
        }

        /// <summary>
        /// Visits a QueryExpression tree whose root node is the LinqLambdaExpression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>Uri based on the given expression.</returns>
        public IList<ODataUriSegment> Visit(LinqLambdaExpression expression)
        {
            throw new TaupoNotSupportedException("Handled by separate visitor");
        }

        /// <summary>
        /// Visits a QueryExpression tree whose root node is the LinqLengthPropertyExpression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>Uri based on the given expression.</returns>
        public IList<ODataUriSegment> Visit(LinqLengthPropertyExpression expression)
        {
            throw new TaupoNotSupportedException("This operation is not supported in URI.");
        }

        /// <summary>
        /// Visits a QueryExpression tree whose root node is the LinqLongCountExpression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>Uri based on the given expression.</returns>
        public IList<ODataUriSegment> Visit(LinqLongCountExpression expression)
        {
            throw new TaupoNotSupportedException("Handled by separate visitor");
        }

        /// <summary>
        /// Visits a QueryExpression tree whose root node is the LinqMaxExpression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>Uri based on the given expression.</returns>
        public IList<ODataUriSegment> Visit(LinqMaxExpression expression)
        {
            throw new TaupoNotSupportedException("Handled by separate visitor");
        }

        /// <summary>
        /// Visits a QueryExpression tree whose root node is the LinqMembermethodExpression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>Uri based on the given expression.</returns>
        public IList<ODataUriSegment> Visit(LinqMemberMethodExpression expression)
        {
            throw new TaupoNotSupportedException("This operation is not supported in URI.");
        }

        /// <summary>
        /// Visits a QueryExpression tree whose root node is the LinqMinExpression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>Uri based on the given expression.</returns>
        public IList<ODataUriSegment> Visit(LinqMinExpression expression)
        {
            throw new TaupoNotSupportedException("Handled by separate visitor");
        }

        /// <summary>
        /// Visits a QueryExpression tree whose root node is the LinqNewArrayExpression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>Uri based on the given expression.</returns>
        public IList<ODataUriSegment> Visit(LinqNewArrayExpression expression)
        {
            throw new TaupoNotSupportedException("This operation is not supported in URI.");
        }

        /// <summary>
        /// Visits a QueryExpression tree whose root node is the LinqNewExpression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>Uri based on the given expression.</returns>
        public IList<ODataUriSegment> Visit(LinqNewExpression expression)
        {
            return new List<ODataUriSegment>();
        }

        /// <summary>
        /// Visits a QueryExpression tree whose root node is the LinqNewInstanceExpression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>Uri based on the given expression.</returns>
        public IList<ODataUriSegment> Visit(LinqNewInstanceExpression expression)
        {
            return new List<ODataUriSegment>();
        }

        /// <summary>
        /// Visits a QueryExpression tree whose root node is the LinqOrderByExpression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>Uri based on the given expression.</returns>
        public IList<ODataUriSegment> Visit(LinqOrderByExpression expression)
        {
            var segments = expression.Source.Accept(this);
            this.uriInProgress.OrderByExpressions.Add(expression);
            return segments;
        }

        /// <summary>
        /// Visits a QueryExpression tree whose root node is the LinqParameterExpression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>Uri based on the given expression.</returns>
        public IList<ODataUriSegment> Visit(LinqParameterExpression expression)
        {
            return new List<ODataUriSegment>();
        }

        /// <summary>
        /// Visits a QueryExpression tree whose root node is the LinqSelectExpression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>Uri based on the given expression.</returns>
        public IList<ODataUriSegment> Visit(LinqSelectExpression expression)
        {
            var segments = expression.Source.Accept(this);
            IEnumerable<QueryExpression> memberExpressions = null;

            if (expression.Lambda.Body is LinqNewInstanceExpression)
            {
                memberExpressions = ((LinqNewInstanceExpression)expression.Lambda.Body).Members;
            }
            else if (expression.Lambda.Body is LinqNewExpression)
            {
                memberExpressions = ((LinqNewExpression)expression.Lambda.Body).Members;
            }

            ExceptionUtilities.CheckObjectNotNull(memberExpressions, "Unsupported expression type '{0}' inside LinqSelectExpressions", expression.Lambda.Body);

            // projection of anonymous types is translated into ?select=Id,Name
            foreach (var memberExpression in memberExpressions)
            {
                this.uriInProgress.SelectSegments.Add(memberExpression.Accept(this).ToList());
            }

            return segments;
        }

        /// <summary>
        /// Visits a QueryExpression tree whose root node is the LinqSelectManyExpression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>Uri based on the given expression.</returns>
        public IList<ODataUriSegment> Visit(LinqSelectManyExpression expression)
        {
            throw new TaupoNotSupportedException("Not supported");
        }

        /// <summary>
        /// Visits a QueryExpression tree whose root node is the LinqSingleExpression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>Throws a TaupoNotSupportedException.</returns>
        public IList<ODataUriSegment> Visit(LinqSingleExpression expression)
        {
            throw new TaupoNotSupportedException("Not supported");
        }

        /// <summary>
        /// Visits a QueryExpression tree whose root node is the LinqSingleOrDefaultExpression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>Throws a TaupoNotSupportedException.</returns>
        public IList<ODataUriSegment> Visit(LinqSingleOrDefaultExpression expression)
        {
            throw new TaupoNotSupportedException("Not supported");
        }

        /// <summary>
        /// Visits a QueryExpression tree whose root node is the QuerySkipExpression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>Uri based on the given expression.</returns>
        public IList<ODataUriSegment> Visit(LinqSkipExpression expression)
        {
            var segments = expression.Source.Accept(this);

            var skipCount = expression.SkipCount as QueryConstantExpression;
            ExceptionUtilities.CheckObjectNotNull(skipCount, "Skip count must be a constant value");
            this.uriInProgress.Skip = (int)skipCount.ScalarValue.Value;

            return segments;
        }

        /// <summary>
        /// Visits a QueryExpression tree whose root node is the QueryTakeExpression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>Uri based on the given expression.</returns>
        public IList<ODataUriSegment> Visit(LinqTakeExpression expression)
        {
            var segments = expression.Source.Accept(this);

            var takeCount = expression.TakeCount as QueryConstantExpression;
            ExceptionUtilities.CheckObjectNotNull(takeCount, "Take count must be a constant value");
            this.uriInProgress.Top = (int)takeCount.ScalarValue.Value;

            return segments;
        }

        /// <summary>
        /// Visits a QueryExpression tree whose root node is the LinqAddQueryOptionExpression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>Uri based on the given expression.</returns>
        public IList<ODataUriSegment> Visit(LinqToAstoriaAddQueryOptionExpression expression)
        {
            var segments = expression.Source.Accept(this);

            if (expression.QueryOption == QueryOptions.Select)
            {
                this.AddToSegmentPathCollection(this.uriInProgress.SelectSegments, expression.ExpressionType, (string)expression.QueryValue);
            }
            else if (expression.QueryOption == QueryOptions.Expand)
            {
                this.AddToSegmentPathCollection(this.uriInProgress.ExpandSegments, expression.ExpressionType, (string)expression.QueryValue);
            }
            else
            {
                // for top, skip, etc, this may cause duplicates, but thats intentional
                // TODO: consider flattening these, like the other query builder would
                this.uriInProgress.CustomQueryOptions[expression.QueryOption] = expression.QueryValue.ToString();
            }

            return segments;
        }

        /// <summary>
        /// Visits a QueryExpression tree whose root node is the LinqToAstoriaConditionalExpression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>Uri based on the given expression.</returns>
        public IList<ODataUriSegment> Visit(LinqToAstoriaConditionalExpression expression)
        {
            throw new TaupoNotSupportedException("This operation is not supported in URI.");
        }

        /// <summary>
        /// Visits a QueryExpression tree whose root node is the LinqExpandExpression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>Uri based on the given expression.</returns>
        public IList<ODataUriSegment> Visit(LinqToAstoriaExpandExpression expression)
        {
            var segments = expression.Source.Accept(this);

            // Parse the expand string at the end to avoid dealing with anonymous types.
            this.unparsedExpand = expression;

            return segments;
        }

        /// <summary>
        /// Visits a LinqToAstoriaExpandLambdaExpression.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <returns>The result of visiting this expression.</returns>
        public IList<ODataUriSegment> Visit(LinqToAstoriaExpandLambdaExpression expression)
        {
            return this.Visit(expression.ToLinqToAstoriaExpandExpression());
        }

        /// <summary>
        /// Visits a QueryExpression tree whose root node is the LinqToAstoriaKeyExpression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>Uri based on the given expression.</returns>
        public IList<ODataUriSegment> Visit(LinqToAstoriaKeyExpression expression)
        {
            var segments = expression.Source.Accept(this);

            var collectionType = expression.Source.ExpressionType as QueryCollectionType;
            ExceptionUtilities.CheckObjectNotNull(collectionType, "Source expression was not a collection. Source: {0}", expression.Source);

            var entityType = collectionType.ElementType as QueryEntityType;
            ExceptionUtilities.CheckObjectNotNull(entityType, "Source expression was not a collection of entities. Source: {0}", expression.Source);

            var pairs = expression.KeyProperties.Select(pair => new NamedValue(pair.Key.Name, pair.Value.ScalarValue.Value));

            segments.Add(ODataUriBuilder.Key(entityType.EntityType, pairs));

            return segments;
        }

        /// <summary>
        /// Visits a QueryExpression tree whose root node is the LinqToAstoriaLinksExpression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>Uri based on the given expression.</returns>
        public IList<ODataUriSegment> Visit(LinqToAstoriaLinksExpression expression)
        {
            var segments = expression.Source.Accept(this);
            segments.Add(SystemSegment.EntityReferenceLinks);
            return segments;
        }

        /// <summary>
        /// Visits a QueryExpression tree whose root node is the LinqToAstoriaValueExpression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>Uri based on the given expression.</returns>
        public IList<ODataUriSegment> Visit(LinqToAstoriaValueExpression expression)
        {
            var segments = expression.Source.Accept(this);
            segments.Add(SystemSegment.Value);
            return segments;
        }

        /// <summary>
        /// Visits a QueryExpression tree whose root node is the LinqUnionExpression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>Uri based on the given expression.</returns>
        public IList<ODataUriSegment> Visit(LinqUnionExpression expression)
        {
            throw new TaupoNotSupportedException("Not supported");
        }

        /// <summary>
        /// Visits a QueryExpression tree whose root node is the LinqWhereExpression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>Uri based on the given expression.</returns>
        public IList<ODataUriSegment> Visit(LinqWhereExpression expression)
        {
            var segments = expression.Source.Accept(this);

            var lambda = expression.Lambda;
            ExceptionUtilities.Assert(lambda.Parameters.Count == 1, "Only lambdas with 1 parameter are supported. Lambda was: '{0}'", lambda);
            
            if (this.uriInProgress.FilterExpressions.Count > 0)
            {
                var toReplace = lambda.Parameters.Single();
                var replaceWith = this.uriInProgress.FilterExpressions[0].Parameters.Single();
                lambda = (LinqLambdaExpression)new ParameterReplacingVisitor(toReplace, replaceWith).ReplaceExpression(lambda);
            }

            this.uriInProgress.FilterExpressions.Add(lambda);

            return segments;
        }

        /// <summary>
        /// Visits a QueryExpression tree whose root node is the QueryAddExpression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>Uri based on the given expression.</returns>
        public IList<ODataUriSegment> Visit(QueryAddExpression expression)
        {
            throw new TaupoNotSupportedException("Handled by separate visitor");
        }

        /// <summary>
        /// Visits a QueryExpression tree whose root node is the QueryAndExpression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>Uri based on the given expression.</returns>
        public IList<ODataUriSegment> Visit(QueryAndExpression expression)
        {
            throw new TaupoNotSupportedException("Handled by separate visitor");
        }

        /// <summary>
        /// Visits a QueryExpression tree whose root node is the QueryAsExpression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>Uri based on the given expression.</returns>
        public IList<ODataUriSegment> Visit(QueryAsExpression expression)
        {
            var segments = expression.Source.Accept(this);
            var queryEntityType = expression.TypeToOperateAgainst as QueryEntityType;
            ExceptionUtilities.CheckObjectNotNull(queryEntityType, "Astoria only supports of type against an EntityType");
            segments.Add(ODataUriBuilder.EntityType(queryEntityType.EntityType));
            return segments;
        }

        /// <summary>
        /// Visits a QueryExpression tree whose root node is the QueryCastExpression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>Uri based on the given expression.</returns>
        public IList<ODataUriSegment> Visit(QueryCastExpression expression)
        {
            throw new TaupoNotSupportedException("Handled by separate visitor");
        }

        /// <summary>
        /// Visits a QueryExpression tree whose root node is the QueryConstantExpression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>Uri based on the given expression.</returns>
        public IList<ODataUriSegment> Visit(QueryConstantExpression expression)
        {
            throw new TaupoNotSupportedException("Handled by separate visitor");
        }

        /// <summary>
        /// Visits a QueryExpression tree whose root node is the QueryCustomFunctionCallExpression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>Uri based on the given expression.</returns>
        public IList<ODataUriSegment> Visit(QueryCustomFunctionCallExpression expression)
        {
            var actionAnnotation = expression.Function.Annotations.OfType<ServiceOperationAnnotation>().SingleOrDefault();
            if (actionAnnotation != null)
            {
                return this.VisitAction(actionAnnotation, expression);
            }

            return this.VisitServiceOperation(expression);
        }
        
        /// <summary>
        /// Visits a QueryExpression tree whose root node is the QueryDivideExpression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>Uri based on the given expression.</returns>
        public IList<ODataUriSegment> Visit(QueryDivideExpression expression)
        {
            throw new TaupoNotSupportedException("Handled by separate visitor");
        }

        /// <summary>
        /// Visits a QueryExpression tree whose root node is the QueryEqualToExpression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>Uri based on the given expression.</returns>
        public IList<ODataUriSegment> Visit(QueryEqualToExpression expression)
        {
            throw new TaupoNotSupportedException("Handled by separate visitor");
        }

        /// <summary>
        /// Visits a QueryExpression tree whose root node is the QueryFunctionImportCallExpression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>Uri based on the given expression.</returns>
        public IList<ODataUriSegment> Visit(QueryFunctionImportCallExpression expression)
        {
            throw new TaupoNotSupportedException("Not supported");
        }

        /// <summary>
        /// Visits a QueryExpression tree whose root node is the QueryFunctionParameterReferenceExpression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>Uri based on the given expression.</returns>
        public IList<ODataUriSegment> Visit(QueryFunctionParameterReferenceExpression expression)
        {
            throw new TaupoNotSupportedException("This operation is not supported in URI.");
        }

        /// <summary>
        /// Visits a QueryExpression tree whose root node is the QueryGreaterThanExpression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>Uri based on the given expression.</returns>
        public IList<ODataUriSegment> Visit(QueryGreaterThanExpression expression)
        {
            throw new TaupoNotSupportedException("Handled by separate visitor");
        }

        /// <summary>
        /// Visits a QueryExpression tree whose root node is the QueryGreaterThanOrEqualToExpression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>Uri based on the given expression.</returns>
        public IList<ODataUriSegment> Visit(QueryGreaterThanOrEqualToExpression expression)
        {
            throw new TaupoNotSupportedException("Handled by separate visitor");
        }

        /// <summary>
        /// Visits a QueryExpression tree whose root node is the QueryIsNotNullExpression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>Uri based on the given expression.</returns>
        public IList<ODataUriSegment> Visit(QueryIsNotNullExpression expression)
        {
            throw new TaupoNotSupportedException("Handled by separate visitor");
        }

        /// <summary>
        /// Visits a QueryExpression tree whose root node is the QueryIsNullExpression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>Uri based on the given expression.</returns>
        public IList<ODataUriSegment> Visit(QueryIsNullExpression expression)
        {
            throw new TaupoNotSupportedException("Handled by separate visitor");
        }

        /// <summary>
        /// Visits a QueryExpression tree whose root node is the QueryIsOfExpression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>Uri based on the given expression.</returns>
        public IList<ODataUriSegment> Visit(QueryIsOfExpression expression)
        {
            throw new TaupoNotSupportedException("Handled by separate visitor");
        }

        /// <summary>
        /// Visits a QueryExpression tree whose root node is the QueryLessThanExpression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>Uri based on the given expression.</returns>
        public IList<ODataUriSegment> Visit(QueryLessThanExpression expression)
        {
            throw new TaupoNotSupportedException("Handled by separate visitor");
        }

        /// <summary>
        /// Visits a QueryExpression tree whose root node is the QueryLessThanOrEqualToExpression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>Uri based on the given expression.</returns>
        public IList<ODataUriSegment> Visit(QueryLessThanOrEqualToExpression expression)
        {
            throw new TaupoNotSupportedException("Handled by separate visitor");
        }

        /// <summary>
        /// Visits a QueryExpression tree whose root node is the QueryModuloExpression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>Uri based on the given expression.</returns>
        public IList<ODataUriSegment> Visit(QueryModuloExpression expression)
        {
            throw new TaupoNotSupportedException("Handled by separate visitor");
        }

        /// <summary>
        /// Visits a QueryExpression tree whose root node is the QueryMultiplyExpression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>Uri based on the given expression.</returns>
        public IList<ODataUriSegment> Visit(QueryMultiplyExpression expression)
        {
            throw new TaupoNotSupportedException("Handled by separate visitor");
        }

        /// <summary>
        /// Visits a QueryExpression tree whose root node is the QueryNotEqualToExpression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>Uri based on the given expression.</returns>
        public IList<ODataUriSegment> Visit(QueryNotEqualToExpression expression)
        {
            throw new TaupoNotSupportedException("Handled by separate visitor");
        }

        /// <summary>
        /// Visits a QueryExpression tree whose root node is the QueryNotExpression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>Uri based on the given expression.</returns>
        public IList<ODataUriSegment> Visit(QueryNotExpression expression)
        {
            throw new TaupoNotSupportedException("Handled by separate visitor");
        }

        /// <summary>
        /// Visits a QueryExpression tree whose root node is the QueryNullExpression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>Uri based on the given expression.</returns>
        public IList<ODataUriSegment> Visit(QueryNullExpression expression)
        {
            throw new TaupoNotSupportedException("Handled by separate visitor");
        }

        /// <summary>
        /// Visits a QueryExpression tree whose root node is the QueryOrExpression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>Uri based on the given expression.</returns>
        public IList<ODataUriSegment> Visit(QueryOfTypeExpression expression)
        {
            var segments = expression.Source.Accept(this);
            var queryEntityType = expression.TypeToOperateAgainst as QueryEntityType;
            ExceptionUtilities.CheckObjectNotNull(queryEntityType, "Astoria only supports of type against an EntityType");
            segments.Add(ODataUriBuilder.EntityType(queryEntityType.EntityType));
            return segments;
        }

        /// <summary>
        /// Visits a QueryExpression tree whose root node is the QueryOrExpression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>Uri based on the given expression.</returns>
        public IList<ODataUriSegment> Visit(QueryOrExpression expression)
        {
            throw new TaupoNotSupportedException("Handled by separate visitor");
        }

        /// <summary>
        /// Visits a QueryExpression tree whose root node is the QueryPropertyExpression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>Uri based on the given expression.</returns>
        public IList<ODataUriSegment> Visit(QueryPropertyExpression expression)
        {
            var segments = expression.Instance.Accept(this);

            var type = expression.Instance.GetExpressionTypeOrElementType() as QueryStructuralType;
            ExceptionUtilities.CheckObjectNotNull(type, "Expression instance type was not a structural type. Instance: {0}", expression.Instance.ToString());

            StructuralType structuralType;
            var entityType = type as QueryEntityType;
            if (entityType != null)
            {
                structuralType = entityType.EntityType;
            }
            else
            {
                var complexType = type as QueryComplexType;
                ExceptionUtilities.CheckObjectNotNull(complexType, "Structural type was neither an entity type nor a complex type");
                structuralType = complexType.ComplexType;
            }

            if (IsPropertyNamedStream(structuralType, expression.Name))
            {
                segments.Add(ODataUriBuilder.NamedStream(structuralType, expression.Name));
            }
            else
            {
                segments.Add(ODataUriBuilder.Property(structuralType, expression.Name));
            }

            return segments;
        }

        /// <summary>
        /// Visits a QueryExpression tree whose root node is the LinqQueryRootExpression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>Uri based on the given expression.</returns>
        public IList<ODataUriSegment> Visit(QueryRootExpression expression)
        {
            var collectionType = expression.ExpressionType as QueryCollectionType;
            ExceptionUtilities.CheckObjectNotNull(collectionType, "Root query must be an entity set");
            var entityType = collectionType.ElementType as QueryEntityType;
            ExceptionUtilities.CheckObjectNotNull(entityType, "Root query must be an entity set");

            return new List<ODataUriSegment>() { ODataUriBuilder.EntitySet(entityType.EntitySet) };
        }

        /// <summary>
        /// Visits a QueryExpression tree whose root node is the QuerySubtractExpression.
        /// </summary>
        /// <param name="expression">The root node of the expression tree being visited.</param>
        /// <returns>Uri based on the given expression.</returns>
        public IList<ODataUriSegment> Visit(QuerySubtractExpression expression)
        {
            throw new TaupoNotSupportedException("Handled by separate visitor");
        }

        /// <summary>
        /// Returns true or false if a given property name is a named stream on the entity type
        /// </summary>
        /// <param name="type">The metadata for the type</param>
        /// <param name="propertyName">The property name</param>
        /// <returns>returns true if the property is a named stream</returns>
        internal static bool IsPropertyNamedStream(StructuralType type, string propertyName)
        {
            if (type.Properties.Any(p => p.Name.Equals(propertyName) && p.IsStream()))
            {
                return true;
            }
            else
            {
                // The given type may be derived so the property could exist on the base type for the given type
                EntityType et = type as EntityType;
                if ((et != null) && (et.BaseType != null))
                {
                    return IsPropertyNamedStream(et.BaseType, propertyName);
                }
                else
                {
                    return false;
                }
            }
        }

        private IList<ODataUriSegment> VisitServiceOperation(QueryCustomFunctionCallExpression expression)
        {
            IList<FunctionParameter> parameters = expression.Function.Parameters;
            for (int i = 0; i < expression.Arguments.Count; ++i)
            {
                var argumentValue = expression.Arguments.ElementAt(i) as QueryConstantExpression;
                ExceptionUtilities.CheckObjectNotNull(argumentValue, "Non-constant argument values are not supported");

                // Null parameters are not included in service operation call URIs
                if (!argumentValue.ScalarValue.IsNull)
                {
                    string argumentName = parameters[i].Name;
                    this.uriInProgress.CustomQueryOptions.Add(argumentName, Uri.EscapeDataString(this.LiteralConverter.SerializePrimitive(argumentValue.ScalarValue.Value)));
                }
            }

            return new List<ODataUriSegment> { new FunctionSegment(expression.Function) };
        }

        private IList<ODataUriSegment> VisitAction(ServiceOperationAnnotation annotation, QueryCustomFunctionCallExpression expression)
        {
            List<ODataUriSegment> segments = new List<ODataUriSegment>();
            
            bool bindingTypeIsOpen = false;

            // If action is marked as binding then evaluate first parameter
            if (annotation.BindingKind.IsBound())
            {
                var initialArgument = expression.Arguments.ElementAt(0);
                segments.AddRange(initialArgument.Accept(this));
                
                bindingTypeIsOpen = expression.Function.IsActionBoundOnOpenType();
            }

            var functionSegment = ODataUriBuilder.ServiceOperation(expression.Function);
            if (bindingTypeIsOpen)
            {
                functionSegment = ODataUriBuilder.ServiceOperation(expression.Function, expression.Function.Model.GetDefaultEntityContainer());
            }

            segments.Add(functionSegment);
            var nonboundArguments = expression.GetNonBoundFunctionArgments();
            if (expression.Function.IsFunction() && nonboundArguments.Count > 0)
            {
                var parameterValues = expression.Function.ConvertActionArgumentsToTypedValues(annotation, nonboundArguments);
                segments.Add(new ParametersExpressionSegment(parameterValues));
            }
            
            return segments;
        }

        private void AddToSegmentPathCollection(ODataUriSegmentPathCollection pathCollection, QueryType expressionType, string toAdd)
        {
            var type = expressionType as QueryEntityType;
            if (type == null)
            {
                var collectionType = expressionType as QueryCollectionType;
                if (collectionType != null)
                {
                    type = collectionType.ElementType as QueryEntityType;
                }
            }

            ExceptionUtilities.CheckObjectNotNull(type, "Expression type was not an entity type or an entity collection type. Expression type: {0}", expressionType);

            foreach (var segmentPath in ODataUriParser.ParseSegmentPathCollection(type.EntityType, toAdd))
            {
                pathCollection.Add(segmentPath);
            }
        }
    }
}
