//   Copyright 2011 Microsoft Corporation
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

namespace System.Data.OData.Query
{
    #region Namespaces.
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    #endregion Namespaces.

    /// <summary>
    /// Lexical token representing the entire query.
    /// </summary>
#if INTERNAL_DROP
    internal sealed class QueryDescriptorQueryToken : QueryToken
#else
    public sealed class QueryDescriptorQueryToken : QueryToken
#endif
    {
        /// <summary>
        /// The default setting for the max depth.
        /// </summary>
        private const int DefaultMaxDepth = 800;

        /// <summary>
        /// The kind of the query token.
        /// </summary>
        public override QueryTokenKind Kind
        {
            get { return QueryTokenKind.QueryDescriptor; }
        }

        /// <summary>
        /// The path for the query.
        /// </summary>
        public QueryToken Path
        {
            get;
            set;
        }

        /// <summary>
        /// The filter for the query. If the property is null, there's no filter for this query.
        /// </summary>
        public QueryToken Filter
        {
            get;
            set;
        }

        /// <summary>
        /// Enumeration of order by tokens. The order by operations must be applied in the order in which
        /// they are listed in this enumeration.
        /// </summary>
        public IEnumerable<OrderByQueryToken> OrderByTokens
        {
            get;
            set;
        }

        /// <summary>
        /// The number of entities to skip in the result.
        /// </summary>
        public int? Skip
        {
            get;
            set;
        }

        /// <summary>
        /// The (maximum) number of entities to include in the result.
        /// </summary>
        public int? Top
        {
            get;
            set;
        }

        /// <summary>
        /// The query options for the query; these include non-system query options starting with '$', 
        /// service operation arguments and custom query options.
        /// </summary>
        public IEnumerable<QueryOptionQueryToken> QueryOptions
        {
            get;
            set;
        }

        /// <summary>
        /// Parses the <paramref name="queryUri"/> and returns a new instance of <see cref="QueryDescriptorQueryToken"/>
        /// describing the query specified by the uri.
        /// </summary>
        /// <param name="queryUri">The absolute URI which holds the query to parse. This must be a path relative to the <paramref name="serviceBaseUri"/>.</param>
        /// <param name="serviceBaseUri">The base URI of the service.</param>
        /// <returns>A new instance of <see cref="QueryDescriptorQueryToken"/> which represents the query specified in the <paramref name="queryUri"/>.</returns>
        public static QueryDescriptorQueryToken ParseUri(Uri queryUri, Uri serviceBaseUri)
        {
            return ParseUri(queryUri, serviceBaseUri, DefaultMaxDepth);
        }

        /// <summary>
        /// Parses the <paramref name="queryUri"/> and returns a new instance of <see cref="QueryDescriptorQueryToken"/>
        /// describing the query specified by the uri.
        /// </summary>
        /// <param name="queryUri">The absolute URI which holds the query to parse. This must be a path relative to the <paramref name="serviceBaseUri"/>.</param>
        /// <param name="serviceBaseUri">The base URI of the service.</param>
        /// <param name="maxDepth">The maximum depth of any single query part. Security setting to guard against DoS attacks causing stack overflows and such.</param>
        /// <returns>A new instance of <see cref="QueryDescriptorQueryToken"/> which represents the query specified in the <paramref name="queryUri"/>.</returns>
        public static QueryDescriptorQueryToken ParseUri(Uri queryUri, Uri serviceBaseUri, int maxDepth)
        {
            ExceptionUtils.CheckArgumentNotNull(queryUri, "queryUri");
            if (!queryUri.IsAbsoluteUri)
            {
                throw new ArgumentException(Strings.QueryDescriptorQueryToken_UriMustBeAbsolute(queryUri), "queryUri");
            }

            ExceptionUtils.CheckArgumentNotNull(serviceBaseUri, "serviceBaseUri");
            if (!serviceBaseUri.IsAbsoluteUri)
            {
                throw new ArgumentException(Strings.QueryDescriptorQueryToken_UriMustBeAbsolute(serviceBaseUri), "serviceBaseUri");
            }

            if (maxDepth <= 0)
            {
                throw new ArgumentException(Strings.QueryDescriptorQueryToken_MaxDepthInvalid, "maxDepth");
            }

            QueryDescriptorQueryToken queryDescriptor = new QueryDescriptorQueryToken();

            UriQueryPathParser pathParser = new UriQueryPathParser(maxDepth);
            queryDescriptor.Path = pathParser.ParseUri(queryUri, serviceBaseUri);

            List<QueryOptionQueryToken> queryOptions = HttpUtils.ParseQueryOptions(queryUri);

            string filter = queryOptions.GetQueryOptionValueAndRemove(UriQueryConstants.FilterQueryOption);
            if (filter != null)
            {
                UriQueryExpressionParser expressionParser = new UriQueryExpressionParser(maxDepth);
                queryDescriptor.Filter = expressionParser.ParseFilter(filter);
            }

            string orderBy = queryOptions.GetQueryOptionValueAndRemove(UriQueryConstants.OrderByQueryOption);
            if (orderBy != null)
            {
                UriQueryExpressionParser expressionParser = new UriQueryExpressionParser(maxDepth);
                queryDescriptor.OrderByTokens = expressionParser.ParseOrderBy(orderBy);
            }

            string skip = queryOptions.GetQueryOptionValueAndRemove(UriQueryConstants.SkipQueryOption);
            if (skip != null)
            {
                int skipValue;
                if (!UriPrimitiveTypeParser.TryUriStringToNonNegativeInteger(skip, out skipValue))
                {
                    throw new ODataException(Strings.QueryDescriptorQueryToken_InvalidSkipQueryOptionValue(skip));
                }

                queryDescriptor.Skip = skipValue;
            }

            string top = queryOptions.GetQueryOptionValueAndRemove(UriQueryConstants.TopQueryOption);
            if (top != null)
            {
                int topValue;
                if (!UriPrimitiveTypeParser.TryUriStringToNonNegativeInteger(top, out topValue))
                {
                    throw new ODataException(Strings.QueryDescriptorQueryToken_InvalidTopQueryOptionValue(top));
                }

                queryDescriptor.Top = topValue;
            }

            // the remaining query options are stored on the query descriptor
            queryDescriptor.QueryOptions = queryOptions.Count == 0 ? null : new ReadOnlyCollection<QueryOptionQueryToken>(queryOptions);

            return queryDescriptor;
        }
    }
}
