//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Core.UriParser
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using Microsoft.OData.Core.UriParser.Parsers;
    using Microsoft.OData.Core.UriParser.Semantic;
    using Microsoft.OData.Core.UriParser.Syntactic;
    using Microsoft.OData.Core.UriParser.TreeNodeKinds;

    #endregion Namespaces

    /// <summary>
    /// Lexical token representing the entire query.
    /// </summary>
    internal sealed class SyntacticTree
    {
        /// <summary>
        /// The default setting for the max depth.
        /// </summary>
        private const int DefaultMaxDepth = 800;

        /// <summary>
        /// The parameter alias strings for other syntctic parts, can be null.
        /// </summary>
        private readonly IDictionary<string, string> parameterAliases;

        /// <summary>
        /// The path for the query.
        /// </summary>
        private readonly ICollection<string> path;

        /// <summary>
        /// The filter for the query. If the property is null, there's no filter for this query.
        /// </summary>
        private readonly QueryToken filter;

        /// <summary>
        /// Enumeration of order by tokens. The order by operations must be applied in the order in which
        /// they are listed in this enumeration.
        /// </summary>
        private readonly IEnumerable<OrderByToken> orderByTokens;

        /// <summary>
        /// The select for the query. If the property is null, there's no select for this query.
        /// </summary>
        private readonly SelectToken select;

        /// <summary>
        /// The expand for the query. If the property is null, there's no expand for this query.
        /// </summary>
        private readonly ExpandToken expand;

        /// <summary>
        /// The number of entities to skip in the result.
        /// </summary>
        private readonly int? skip;

        /// <summary>
        /// The (maximum) number of entities to include in the result.
        /// </summary>
        private readonly int? top;

        /// <summary>
        /// The format for the query.
        /// </summary>
        private readonly string format;

        /// <summary>
        /// query count in the response of the query.
        /// </summary>
        private readonly bool? queryCount;

        /// <summary>
        /// The query options for the query; these include non-system query options starting with '$', 
        /// service operation arguments and custom query options.
        /// </summary>
        private readonly IEnumerable<CustomQueryOptionToken> queryOptions;

        /// <summary>
        /// Create a new SyntacticTree given its parts as arguments.
        /// </summary>
        /// <param name="parameterAliases">The parameter alias strings for other syntctic parts, can be null.</param>
        /// <param name="path">The path for the query. Must not be null.</param>
        /// <param name="filter">The filter for the query. If the property is null, there's no filter for this query.</param>
        /// <param name="orderByTokens">Enumeration of order by tokens.</param>
        /// <param name="select">The select for the query. If the property is null, there's no select for this query.</param>
        /// <param name="expand">The expansions for the query. If the property is null, there are no expandsion for this query.</param>
        /// <param name="skip">The number of entities to skip in the result.</param>
        /// <param name="top">The (maximum) number of entities to include in the result.</param>
        /// <param name="queryCount">Query count in the response of the query.</param>
        /// <param name="format">The format for the query.</param>
        /// <param name="queryOptions">The query options for the query.</param>
        public SyntacticTree(
            IDictionary<string, string> parameterAliases,
            ICollection<string> path,
            QueryToken filter,
            IEnumerable<OrderByToken> orderByTokens,
            SelectToken select,
            ExpandToken expand,
            int? skip,
            int? top,
            bool? queryCount,
            string format,
            IEnumerable<CustomQueryOptionToken> queryOptions)
        {
            ExceptionUtils.CheckArgumentNotNull(path, "path");
            this.parameterAliases = parameterAliases;
            this.path = path;
            this.filter = filter;
            this.orderByTokens = new ReadOnlyEnumerableForUriParser<OrderByToken>(orderByTokens ?? new OrderByToken[0]);
            this.select = select;
            this.expand = expand;
            this.skip = skip;
            this.top = top;
            this.queryCount = queryCount;
            this.format = format;
            this.queryOptions = new ReadOnlyEnumerableForUriParser<CustomQueryOptionToken>(queryOptions ?? new CustomQueryOptionToken[0]);
        }

        /// <summary>
        /// The parameter alias strings for other syntctic parts, can be null.
        /// </summary>
        public IDictionary<string, string> ParameterAliases
        {
            get { return this.parameterAliases; }
        }

        /// <summary>
        /// The path for the query.
        /// </summary>
        public ICollection<string> Path
        {
            get { return this.path; }
        }

        /// <summary>
        /// The filter for the query. If the property is null, there's no filter for this query.
        /// </summary>
        public QueryToken Filter
        {
            get { return this.filter; }
        }

        /// <summary>
        /// Enumeration of order by tokens. The order by operations must be applied in the order in which
        /// they are listed in this enumeration.
        /// </summary>
        public IEnumerable<OrderByToken> OrderByTokens
        {
            get { return this.orderByTokens; }
        }

        /// <summary>
        /// The select for the query. If the property is null, there's no select for this query.
        /// </summary>
        public SelectToken Select
        {
            get { return this.select; }
        }

        /// <summary>
        /// The expand for the query. If the property is null, there's no expand for this query.
        /// </summary>
        public ExpandToken Expand
        {
            get { return this.expand; }
        }

        /// <summary>
        /// The number of entities to skip in the result.
        /// </summary>
        public int? Skip
        {
            get { return this.skip; }
        }

        /// <summary>
        /// The (maximum) number of entities to include in the result.
        /// </summary>
        public int? Top
        {
            get { return this.top; }
        }

        /// <summary>
        /// The format for the query.
        /// </summary>
        public string Format
        {
            get { return this.format; }
        }

        /// <summary>
        /// query count in the response of the query.
        /// </summary>
        public bool? QueryCount
        {
            get { return this.queryCount; }
        }

        /// <summary>
        /// The query options for the query; these include non-system query options starting with '$', 
        /// service operation arguments and custom query options.
        /// </summary>
        public IEnumerable<CustomQueryOptionToken> QueryOptions
        {
            get { return this.queryOptions; }
        }

        /// <summary>
        /// Parses the <paramref name="queryUri"/> and returns a new instance of <see cref="SyntacticTree"/>
        /// describing the query specified by the uri.
        /// </summary>
        /// <param name="queryUri">The absolute URI which holds the query to parse. This must be a path relative to the <paramref name="serviceBaseUri"/>.</param>
        /// <param name="serviceBaseUri">The base URI of the service.</param>
        /// <returns>A new instance of <see cref="SyntacticTree"/> which represents the query specified in the <paramref name="queryUri"/>.</returns>
        public static SyntacticTree ParseUri(Uri queryUri, Uri serviceBaseUri)
        {
            return ParseUri(queryUri, serviceBaseUri, DefaultMaxDepth);
        }

        /// <summary>
        /// Parses the <paramref name="queryUri"/> and returns a new instance of <see cref="SyntacticTree"/>
        /// describing the query specified by the uri.
        /// </summary>
        /// <param name="queryUri">The absolute URI which holds the query to parse. This must be a path relative to the <paramref name="serviceBaseUri"/>.</param>
        /// <param name="serviceBaseUri">The base URI of the service.</param>
        /// <param name="maxDepth">The maximum depth of any single query part. Security setting to guard against DoS attacks causing stack overflows and such.</param>
        /// <returns>A new instance of <see cref="SyntacticTree"/> which represents the query specified in the <paramref name="queryUri"/>.</returns>
        public static SyntacticTree ParseUri(Uri queryUri, Uri serviceBaseUri, int maxDepth)
        {
            ExceptionUtils.CheckArgumentNotNull(queryUri, "queryUri");
            if (!queryUri.IsAbsoluteUri)
            {
                throw new ArgumentException(Strings.SyntacticTree_UriMustBeAbsolute(queryUri), "queryUri");
            }

            ExceptionUtils.CheckArgumentNotNull(serviceBaseUri, "serviceBaseUri");
            if (!serviceBaseUri.IsAbsoluteUri)
            {
                throw new ArgumentException(Strings.SyntacticTree_UriMustBeAbsolute(serviceBaseUri), "serviceBaseUri");
            }

            if (maxDepth <= 0)
            {
                throw new ArgumentException(Strings.SyntacticTree_MaxDepthInvalid, "maxDepth");
            }


            UriPathParser pathParser = new UriPathParser(maxDepth);
            var path = pathParser.ParsePathIntoSegments(queryUri, serviceBaseUri);

            List<CustomQueryOptionToken> queryOptions = UriUtils.ParseQueryOptions(queryUri);
            IDictionary<string, string> parameterAliases = queryOptions.GetParameterAliases();

            QueryToken filter = null;
            string filterQuery = queryOptions.GetQueryOptionValueAndRemove(UriQueryConstants.FilterQueryOption);
            if (filterQuery != null)
            {
                UriQueryExpressionParser expressionParser = new UriQueryExpressionParser(maxDepth);
                filter = expressionParser.ParseFilter(filterQuery);
            }

            IEnumerable<OrderByToken> orderByTokens = null;
            string orderByQuery = queryOptions.GetQueryOptionValueAndRemove(UriQueryConstants.OrderByQueryOption);
            if (orderByQuery != null)
            {
                UriQueryExpressionParser expressionParser = new UriQueryExpressionParser(maxDepth);
                orderByTokens = expressionParser.ParseOrderBy(orderByQuery);
            }

            SelectToken select = null;
            string selectQuery = queryOptions.GetQueryOptionValueAndRemove(UriQueryConstants.SelectQueryOption);
            if (selectQuery != null)
            {
                SelectExpandParser selectParser = new SelectExpandParser(selectQuery, ODataUriParserSettings.DefaultSelectExpandLimit);
                select = selectParser.ParseSelect();
            }

            ExpandToken expand = null;
            string expandQuery = queryOptions.GetQueryOptionValueAndRemove(UriQueryConstants.ExpandQueryOption);
            if (expandQuery != null)
            {
                SelectExpandParser expandParser = new SelectExpandParser(expandQuery, ODataUriParserSettings.DefaultSelectExpandLimit);
                expand = expandParser.ParseExpand();
            }

            int? skip = null;
            string skipQuery = queryOptions.GetQueryOptionValueAndRemove(UriQueryConstants.SkipQueryOption);
            if (skipQuery != null)
            {
                int skipValue;
                if (!UriPrimitiveTypeParser.TryUriStringToNonNegativeInteger(skipQuery, out skipValue))
                {
                    throw new ODataException(Strings.SyntacticTree_InvalidSkipQueryOptionValue(skipQuery));
                }

                skip = skipValue;
            }

            int? top = null;
            string topQuery = queryOptions.GetQueryOptionValueAndRemove(UriQueryConstants.TopQueryOption);
            if (topQuery != null)
            {
                int topValue;
                if (!UriPrimitiveTypeParser.TryUriStringToNonNegativeInteger(topQuery, out topValue))
                {
                    throw new ODataException(Strings.SyntacticTree_InvalidTopQueryOptionValue(topQuery));
                }

                top = topValue;
            }

            string countQuery = queryOptions.GetQueryOptionValueAndRemove(UriQueryConstants.CountQueryOption);
            bool? count = QueryTokenUtils.ParseQueryCount(countQuery);

            string format = queryOptions.GetQueryOptionValueAndRemove(UriQueryConstants.FormatQueryOption);

            return new SyntacticTree(
                parameterAliases,
                path,
                filter,
                orderByTokens,
                select,
                expand,
                skip,
                top,
                count,
                format,
                queryOptions.Count == 0 ? null : new ReadOnlyCollection<CustomQueryOptionToken>(queryOptions));
        }
    }
}
