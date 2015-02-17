//---------------------------------------------------------------------
// <copyright file="QueryBasedNextLinkGenerator.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.ResponseVerification
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.Test.Taupo.Astoria.Common;
    using Microsoft.Test.Taupo.Astoria.Contracts.LinqToAstoria;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Astoria.Contracts.ResponseVerification;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Execution;
    using Microsoft.Test.Taupo.Query;
    using Microsoft.Test.Taupo.Query.Common;
    using Microsoft.Test.Taupo.Query.Contracts;
    using Microsoft.Test.Taupo.Query.Contracts.Linq;
    using Microsoft.Test.Taupo.Query.Contracts.Linq.Expressions;

    /// <summary>
    /// Default implementation of the next-link generator which uses query expressions
    /// </summary>
    [ImplementationName(typeof(INextLinkExpectationGenerator), "QueryBased")]
    public class QueryBasedNextLinkGenerator : INextLinkExpectationGenerator
    {
        /// <summary>
        /// Gets or sets the URI to string converter.
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public IODataUriToStringConverter UriToStringConverter { get; set; }

        /// <summary>
        /// Gets or sets the expression evaluator.
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public ILinqToAstoriaExpressionEvaluator ExpressionEvaluator { get; set; }

        /// <summary>
        /// Gets or sets the literal converter.
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public IODataLiteralConverter LiteralConverter { get; set; }

        /// <summary>
        /// Generates the expected next link for a top-level feed
        /// </summary>
        /// <param name="requestUri">The request URI.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="lastEntityValue">The last entity value.</param>
        /// <returns>
        /// The expected next link
        /// </returns>
        public string GenerateNextLink(ODataUri requestUri, int pageSize, QueryStructuralValue lastEntityValue)
        {
            ExceptionUtilities.CheckArgumentNotNull(requestUri, "requestUri");
            ExceptionUtilities.CheckArgumentNotNull(lastEntityValue, "lastEntityValue");
            
            var queryBasedUri = requestUri as QueryBasedODataUri;
            ExceptionUtilities.CheckObjectNotNull(queryBasedUri, "Only uris which were generated from queries are supported");
            
            var skipTokenValues = new List<object>();
            foreach (var keySelector in queryBasedUri.OrderByExpressions.SelectMany(o => o.KeySelectors))
            {
                skipTokenValues.Add(this.EvaluateLambdaForEntity(lastEntityValue, keySelector));
            }

            foreach (var keyProperty in lastEntityValue.Type.Properties.Where(p => p.IsPrimaryKey))
            {
                skipTokenValues.Add(lastEntityValue.GetScalarValue(keyProperty.Name).Value);
            }

            var skiptoken = this.BuildSkipTokenFromValues(skipTokenValues);

            // copy request uri segments exactly
            var nextLinkUri = new ODataUri(requestUri.Segments);

            // copy over $orderby, $filter, $expand, $select, and $inlinecount
            nextLinkUri.OrderBy = ModifyQueryOptionToMatchProduct(requestUri.OrderBy);
            nextLinkUri.Filter = ModifyQueryOptionToMatchProduct(requestUri.Filter);
            nextLinkUri.ExpandSegments = requestUri.ExpandSegments;
            nextLinkUri.SelectSegments = requestUri.SelectSegments;

            string inlineCountValue;
            if (requestUri.TryGetInlineCountValue(out inlineCountValue))
            {
                nextLinkUri.InlineCount = inlineCountValue;
            }

            // add the $skiptoken generated above
            nextLinkUri.SkipToken = skiptoken;

            // generate a new $top value
            if (requestUri.Top.HasValue)
            {
                var top = requestUri.Top.Value - pageSize;
                if (top > 0)
                {
                    nextLinkUri.Top = top;
                }
            }

            if (requestUri.IsServiceOperation())
            {
                var functionSegment = requestUri.Segments.OfType<FunctionSegment>().Last();
                foreach (var paramName in functionSegment.Function.Parameters.Select(p => p.Name))
                {
                    string paramValue;
                    if (requestUri.CustomQueryOptions.TryGetValue(paramName, out paramValue))
                    {
                        nextLinkUri.CustomQueryOptions[paramName] = paramValue;
                    }
                }
            }

            return this.UriToStringConverter.ConvertToString(nextLinkUri);
        }

        /// <summary>
        /// Generates the next link for an expanded feed.
        /// </summary>
        /// <param name="containingEntity">The containing entity.</param>
        /// <param name="navigation">The expanded navigation property.</param>
        /// <param name="lastEntityValue">The last entity value.</param>
        /// <returns>
        /// The expected next link
        /// </returns>
        public string GenerateExpandedNextLink(EntityInstance containingEntity, NavigationPropertyInstance navigation, QueryStructuralValue lastEntityValue)
        {
            ExceptionUtilities.CheckArgumentNotNull(containingEntity, "containingEntity");
            ExceptionUtilities.CheckArgumentNotNull(navigation, "navigation");
            ExceptionUtilities.CheckArgumentNotNull(lastEntityValue, "lastEntityValue");

            var navigationUriString = ((ExpandedLink)navigation.Value).UriString;
            if (string.IsNullOrEmpty(navigationUriString))
            {
                navigationUriString = UriHelpers.ConcatenateUriSegments(containingEntity.EditLink, navigation.Name);
            }

            var skipTokenValues = new List<object>();
            foreach (var keyProperty in lastEntityValue.Type.Properties.Where(p => p.IsPrimaryKey))
            {
                skipTokenValues.Add(lastEntityValue.GetScalarValue(keyProperty.Name).Value);
            }

            var skiptoken = this.BuildSkipTokenFromValues(skipTokenValues);

            var nextLinkUri = new ODataUri(new UnrecognizedSegment(navigationUriString));
            nextLinkUri.SkipToken = skiptoken;

            return this.UriToStringConverter.ConvertToString(nextLinkUri);
        }

        internal static string ModifyQueryOptionToMatchProduct(string queryOption)
        {
            if (queryOption == null)
            {
                return null;
            }

            // when the test framework builds comparison and other operators, it does not automatically escape spaces
            return queryOption.Replace(" ", "%20");

            ////StringBuilder builder = new StringBuilder();
            ////bool inString = false;
            ////for (int i = 0; i < queryOption.Length; i++)
            ////{
            ////    if (queryOption[i] == ' ')
            ////    {
            ////        builder.Append("%20");
            ////        continue;
            ////    }

            ////    if (queryOption[i] == '\'')
            ////    {
            ////        inString = !inString;
            ////    }
            ////    else if (!inString)
            ////    {
            ////        if ((i + 2) < queryOption.Length)
            ////        {
            ////            if (queryOption.Substring(i, 3) == "%2B")
            ////            {
            ////                builder.Append('+');
            ////                i += 2;
            ////                continue;
            ////            }
            ////        }
            ////    }

            ////    builder.Append(queryOption[i]);
            ////}

            ////return builder.ToString();
        }

        /// <summary>
        /// Evaluates the given lambda as if it were called on the given entity. Used for evaluating orderby expressions for the last result in a feed.
        /// </summary>
        /// <param name="entity">The entity to evaluate the lambda for.</param>
        /// <param name="lamda">The lamda to evaluate.</param>
        /// <returns>The result of evaluating the lamda for the given entity.</returns>
        private object EvaluateLambdaForEntity(QueryStructuralValue entity, LinqLambdaExpression lamda)
        {
            var expression = CommonQueryBuilder.Root("fakeSet", entity.Type.CreateCollectionType()).Select(lamda).Single();

            var fakeQueryDataSet = new QueryDataSet();
            fakeQueryDataSet.RootQueryData["fakeSet"] = entity.Type.CreateCollectionType().CreateCollectionWithValues(new[] { entity });

            QueryValue evaluated;
            using (this.ExpressionEvaluator.WithTemporaryDataSet(fakeQueryDataSet))
            {
                evaluated = this.ExpressionEvaluator.Evaluate(expression);
            }

            return ((QueryScalarValue)evaluated).Value;
        }

        private string BuildSkipTokenFromValues(IEnumerable<object> skipTokenValues)
        {
            return string.Join(",", skipTokenValues.Select(v => this.LiteralConverter.SerializePrimitive(v, true)).Select(l => Uri.EscapeDataString(l)).ToArray());
        }
    }
}
