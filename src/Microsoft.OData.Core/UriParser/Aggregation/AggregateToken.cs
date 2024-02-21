//---------------------------------------------------------------------
// <copyright file="AggregateToken.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

#if ODATA_CLIENT
namespace Microsoft.OData.Client.ALinq.UriParser
#else
namespace Microsoft.OData.UriParser.Aggregation
#endif
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Query token representing an Aggregate token.
    /// </summary>
    public sealed class AggregateToken : ApplyTransformationToken
    {
        private readonly IEnumerable<AggregateTokenBase> expressions;

        /// <summary>
        /// Create an AggregateTransformationToken.
        /// </summary>
        /// <param name="expressions">The aggregate expressions.</param>
        public AggregateToken(IEnumerable<AggregateTokenBase> expressions)
        {
            ExceptionUtils.CheckArgumentNotNull(expressions, "expressions");
            this.expressions = expressions;
        }

        /// <summary>
        /// Gets the kind of this token.
        /// </summary>
        public override QueryTokenKind Kind
        {
            get { return QueryTokenKind.Aggregate; }
        }

        /// <summary>
        /// Gets the expressions of this token.
        /// </summary>
        public IEnumerable<AggregateTokenBase> AggregateExpressions
        {
            get
            {
                return expressions;
            }
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
