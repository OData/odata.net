//---------------------------------------------------------------------
// <copyright file="AggregateStatementToken.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

#if ASTORIA_CLIENT
namespace Microsoft.OData.Client.ALinq.UriParser
#else
namespace Microsoft.OData.Core.UriParser.Extensions.Syntactic
#endif
{
    using Microsoft.OData.Core.UriParser.Extensions;
    using Microsoft.OData.Core.UriParser.TreeNodeKinds;
    using Microsoft.OData.Core.UriParser.Visitors;
    using Microsoft.OData.Core.UriParser.Syntactic;

    internal sealed class AggregateStatementToken : QueryToken
    {
        private readonly QueryToken expression;

        private readonly AggregationVerb withVerb;

        private readonly string alias;

        public AggregateStatementToken(QueryToken expression, AggregationVerb withVerb, string alias)
        {
            ExceptionUtils.CheckArgumentNotNull(expression, "expression");
            ExceptionUtils.CheckArgumentNotNull(alias, "alias");

            this.expression = expression;
            this.withVerb = withVerb;
            this.alias = alias;
        }
       
        public override QueryTokenKind Kind
        {
            get { return QueryTokenKind.AggregateStatement; }
        }

        public AggregationVerb WithVerb
        {
            get { return this.withVerb; }
        }

        public QueryToken Expression
        {
            get { return this.expression; }
        }

        public string AsAlias
        {
            get { return this.alias; }
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
