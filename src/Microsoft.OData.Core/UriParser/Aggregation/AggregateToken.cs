//---------------------------------------------------------------------
// <copyright file="AggregateToken.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

#if ASTORIA_CLIENT
namespace Microsoft.OData.Client.ALinq.UriParser
#else
namespace Microsoft.OData.Core.UriParser.Aggregation
#endif
{
    using System.Collections.Generic;
    using Microsoft.OData.Core.UriParser.TreeNodeKinds;
    using Microsoft.OData.Core.UriParser.Visitors;
    using Microsoft.OData.Core.UriParser.Syntactic;

    internal sealed class AggregateToken : ApplyTransformationToken
    {
        private readonly IEnumerable<AggregateExpressionToken> expressions;

        public AggregateToken(IEnumerable<AggregateExpressionToken> expressions)
        {
            ExceptionUtils.CheckArgumentNotNull(expressions, "expressions");
            this.expressions = expressions;
        }

        public override QueryTokenKind Kind
        {
            get { return QueryTokenKind.Aggregate; }
        }

        public IEnumerable<AggregateExpressionToken> Expressions
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
