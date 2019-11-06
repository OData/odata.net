//---------------------------------------------------------------------
// <copyright file="SelectTermToken.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

#if ODATA_CLIENT
namespace Microsoft.OData.Client.ALinq.UriParser
#else
namespace Microsoft.OData.UriParser
#endif
{
    #region Namespaces
    using System.Collections.Generic;
    #endregion Namespaces

    /// <summary>
    /// Lexical token representing a select operation.
    /// </summary>
    public sealed class SelectTermToken : SelectExpandTermToken
    {
        /// <summary>
        /// Initializes a new instance of  <see cref="SelectTermToken"/> class.
        /// </summary>
        /// <param name="pathToProperty">the path to the property for this select term</param>
        public SelectTermToken(PathSegmentToken pathToProperty)
            : this(pathToProperty, null)
        {
        }

        /// <summary>
        /// Create an select term using only the property and its subexpand/select
        /// </summary>
        /// <param name="pathToProperty">the path to the property for this select term</param>
        /// <param name="selectOption">the sub select for this token</param>
        public SelectTermToken(PathSegmentToken pathToProperty, SelectToken selectOption)
            : this(pathToProperty, null, null, null, null, null, null, selectOption, null)
        {
        }

        /// <summary>
        /// Create a select term using only the property and its supporting query options.
        /// </summary>
        /// <param name="pathToProperty">the path to the property for this select term</param>
        /// <param name="filterOption">the filter option for this select term</param>
        /// <param name="orderByOptions">the orderby options for this select term</param>
        /// <param name="topOption">the top option for this select term</param>
        /// <param name="skipOption">the skip option for this select term</param>
        /// <param name="countQueryOption">the query count option for this select term</param>
        /// <param name="searchOption">the search option for this select term</param>
        /// <param name="selectOption">the select option for this select term</param>
        /// <param name="computeOption">the compute option for this select term</param>
        public SelectTermToken(PathSegmentToken pathToProperty,
            QueryToken filterOption, IEnumerable<OrderByToken> orderByOptions, long? topOption, long? skipOption, bool? countQueryOption, QueryToken searchOption, SelectToken selectOption, ComputeToken computeOption)
            : base(pathToProperty, filterOption, orderByOptions, topOption, skipOption, countQueryOption, searchOption, selectOption, computeOption)
        {
        }

        /// <summary>
        /// Gets the kind of this expand term.
        /// </summary>
        public override QueryTokenKind Kind
        {
            get { return QueryTokenKind.SelectTerm; }
        }

        /// <summary>
        /// Implement the visitor for this Token
        /// </summary>
        /// <typeparam name="T">The type to return</typeparam>
        /// <param name="visitor">A tree visitor that will visit this node.</param>
        /// <returns>Determined by the return type of the visitor.</returns>
        public override T Accept<T>(ISyntacticTreeVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }
    }
}
