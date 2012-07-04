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

namespace Microsoft.Data.OData.Query
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using Microsoft.Data.Edm;
    #endregion Namespaces

    /// <summary>
    /// The root node of a query. Holds the query itself plus additional metadata about the query.
    /// </summary>
#if INTERNAL_DROP
    internal sealed class QueryDescriptorQueryNode : QueryNode
#else
    public sealed class QueryDescriptorQueryNode : QueryNode
#endif
    {
        /// <summary>
        /// The default setting for the max depth.
        /// </summary>
        private const int DefaultMaxDepth = 800;

        /// <summary>
        /// The kind of the query node.
        /// </summary>
        public override QueryNodeKind Kind
        {
            get
            {
                return QueryNodeKind.QueryDescriptor;
            }
        }

        /// <summary>
        /// The query tree.
        /// </summary>
        public QueryNode Query
        {
            get;
            set;
        }

        /// <summary>
        /// The custom query options.
        /// </summary>
        public IEnumerable<QueryNode> CustomQueryOptions
        {
            get;
            set;
        }

        /// <summary>
        /// Parses the <paramref name="queryUri"/> and binds the query to the metadata provided
        /// then returns a new instance of <see cref="QueryDescriptorQueryNode"/>
        /// describing the query specified by the uri.
        /// </summary>
        /// <param name="queryUri">The absolute URI which holds the query to parse. This must be a path relative to the <paramref name="serviceBaseUri"/>.</param>
        /// <param name="serviceBaseUri">The base URI of the service.</param>
        /// <param name="model">The model to use for binding.</param>
        /// <returns>A new instance of <see cref="QueryDescriptorQueryNode"/> which represents the query specified in the <paramref name="queryUri"/>.</returns>
        public static QueryDescriptorQueryNode ParseUri(Uri queryUri, Uri serviceBaseUri, IEdmModel model)
        {
            return ParseUri(queryUri, serviceBaseUri, model, DefaultMaxDepth);
        }

        /// <summary>
        /// Parses the <paramref name="queryUri"/> and binds the query to the metadata provided
        /// then returns a new instance of <see cref="QueryDescriptorQueryNode"/>
        /// describing the query specified by the uri.
        /// </summary>
        /// <param name="queryUri">The absolute URI which holds the query to parse. This must be a path relative to the <paramref name="serviceBaseUri"/>.</param>
        /// <param name="serviceBaseUri">The base URI of the service.</param>
        /// <param name="model">The model to use for binding.</param>
        /// <param name="maxDepth">The maximum depth of any single query part. Security setting to guard against DoS attacks causing stack overflows and such.</param>
        /// <returns>A new instance of <see cref="QueryDescriptorQueryNode"/> which represents the query specified in the <paramref name="queryUri"/>.</returns>
        public static QueryDescriptorQueryNode ParseUri(Uri queryUri, Uri serviceBaseUri, IEdmModel model, int maxDepth)
        {
            ExceptionUtils.CheckArgumentNotNull(model, "model");

            QueryDescriptorQueryToken queryDescriptorQueryToken = QueryDescriptorQueryToken.ParseUri(queryUri, serviceBaseUri, maxDepth);
            MetadataBinder metadataBinder = new MetadataBinder(model);
            return metadataBinder.BindQuery(queryDescriptorQueryToken);
        }
    }
}
