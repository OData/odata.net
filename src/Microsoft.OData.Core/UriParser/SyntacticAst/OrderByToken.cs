//---------------------------------------------------------------------
// <copyright file="OrderByToken.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.OData.UriParser;

#if ODATA_CLIENT
namespace Microsoft.OData.Client.ALinq.UriParser
#else
namespace Microsoft.OData.UriParser
#endif
{
    using System;

    /// <summary>
    /// Lexical token representing an order by operation.
    /// </summary>
    public sealed class OrderByToken : QueryToken
    {
        /// <summary>
        /// The direction of the ordering.
        /// </summary>
        private readonly OrderByDirection direction;

        /// <summary>
        /// The expression according to which to order the results.
        /// </summary>
        private readonly QueryToken expression;

        /// <summary>
        /// Create a new OrderByToken given the expression and direction
        /// </summary>
        /// <param name="expression">The expression according to which to order the results.</param>
        /// <param name="direction">The direction of the ordering.</param>
        public OrderByToken(QueryToken expression, OrderByDirection direction)
        {
            ExceptionUtils.CheckArgumentNotNull(expression, "expression");

            this.expression = expression;
            this.direction = direction;
        }

        /// <summary>
        /// The kind of the query token.
        /// </summary>
        public override QueryTokenKind Kind
        {
            get { return QueryTokenKind.OrderBy; }
        }

        /// <summary>
        /// The direction of the ordering.
        /// </summary>
        public OrderByDirection Direction
        {
            get { return this.direction; }
        }

        /// <summary>
        /// The expression according to which to order the results.
        /// </summary>
        public QueryToken Expression
        {
            get { return this.expression; }
        }

        /// <summary>
        /// Accept a <see cref="ISyntacticTreeVisitor{T}"/> to walk a tree of <see cref="QueryToken"/>s.
        /// </summary>
        /// <typeparam name="T">Type that the visitor will return after visiting this token.</typeparam>
        /// <param name="visitor">An implementation of the visitor interface.</param>
        /// <returns>An object whose type is determined by the type parameter of the visitor.</returns>
        public override T Accept<T>(ISyntacticTreeVisitor<T> visitor)
        {
            throw new NotImplementedException();
        }
    }
}