//---------------------------------------------------------------------
// <copyright file="AggregateStatementToken.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------
// OData v4 Aggregation Extensions.

#if ASTORIA_CLIENT
namespace Microsoft.OData.Client.ALinq.UriParser
#else
namespace Microsoft.OData.Core.UriParser.Syntactic
#endif
{
    using Microsoft.OData.Core.UriParser;
    using Microsoft.OData.Core.UriParser.TreeNodeKinds;
    using Microsoft.OData.Core.UriParser.Visitors;

    internal sealed class AggregateStatementToken : QueryToken
    {
        public AggregateStatementToken(QueryToken expression, AggregationVerb withVerb, string alias)
        {
            ExceptionUtils.CheckArgumentNotNull(expression, "expression");
            ExceptionUtils.CheckArgumentNotNull(alias, "alias");

            this._expression = expression;
            this._withVerb = withVerb;
            this._asAlias = alias;
        }
       
        public override QueryTokenKind Kind
        {
            get { return QueryTokenKind.AggregateStatement; }
        }

        private readonly AggregationVerb _withVerb;

        public AggregationVerb WithVerb
        {
            get { return this._withVerb; }
        }

        private readonly QueryToken _expression;

        public QueryToken Expression
        {
            get { return this._expression; }
        }

        private readonly string _asAlias;

        public string AsAlias
        {
            get { return this._asAlias; }
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
