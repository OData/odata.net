//---------------------------------------------------------------------
// <copyright file="EntitySetAggregateToken.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;

#if ODATA_CLIENT
namespace Microsoft.OData.Client.ALinq.UriParser
#else
namespace Microsoft.OData.UriParser.Aggregation
#endif
{
    public sealed class EntitySetAggregateToken : AggregateTokenBase
    {
        private readonly QueryToken entitySet;
        private readonly IEnumerable<AggregateTokenBase> expressions;

        public EntitySetAggregateToken(QueryToken entitySet, IEnumerable<AggregateTokenBase> expressions)
        {
            ExceptionUtils.CheckArgumentNotNull(expressions, "expressions");
            this.expressions = expressions;
            this.entitySet = entitySet;
        }

        public override QueryTokenKind Kind
        {
            get { return QueryTokenKind.EntitySetAggregateExpression; }
        }

        public IEnumerable<AggregateTokenBase> Expressions
        {
            get
            {
                return expressions;
            }
        }

        public QueryToken EntitySet
        {
            get
            {
                return entitySet;
            }
        }

        /// <summary>
        /// Merges two <see cref="EntitySetAggregateToken"/> that have the same entity set into one.
        /// If the parameters do not share the the entity set, an exception is thrown.
        /// </summary>
        /// <param name="token1">First token that is going to be merged.</param>
        /// <param name="token2">Second token that is going to be merged.</param>
        /// <returns>
        /// Returns a token with the same entitySet as the parameters and with expressions from both objects.
        /// </returns>
        public static EntitySetAggregateToken Merge(EntitySetAggregateToken token1, EntitySetAggregateToken token2)
        {
            if (token1 == null)
            {
                return token2;
            }

            if (token2 == null)
            {
                return token1;
            }

            ExceptionUtils.Equals(token1.entitySet, token2.entitySet);
            return new EntitySetAggregateToken(token1.entitySet, token1.expressions.Concat(token2.expressions));
        }

        /// <summary>
        /// Returns the path to access the entity set.
        /// </summary>
        /// <returns>Returns a <see cref="string"/> that contains the path to access the entity set.</returns>
        public string Path()
        {
            List<string> properties = new List<string>();
            QueryToken token = entitySet;
            var pathToken = token as PathToken;

            while (pathToken != null)
            {
                properties.Add(pathToken.Identifier);
                pathToken = pathToken.NextToken as PathToken;
            }

            properties.Reverse();
            return String.Join("/", properties.ToArray());
        }

        /// <summary>
        /// Accept a <see cref="ISyntacticTreeVisitor{T}"/> to walk a tree of <see cref="QueryToken"/>s.
        /// </summary>
        /// <typeparam name="T">Type that the visitor will return after visiting this token.</typeparam>
        /// <param name="visitor">An implementation of the visitor interface.</param>
        /// <returns>An object whose type is determined by the type parameter of the visitor.</returns>
        public override T Accept<T>(ISyntacticTreeVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }
    }
}
