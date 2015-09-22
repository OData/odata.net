//---------------------------------------------------------------------
// <copyright file="AggregateStatementNode2.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------
// OData v4 Aggregation Extensions.
namespace Microsoft.OData.Core.UriParser.Semantic
{
    using TreeNodeKinds;
    using Microsoft.OData.Edm;

    public sealed class AggregateStatementNode2 : QueryNode
    {
        public AggregateStatementNode2(SingleValueNode expression, AggregationVerb withVerb, SingleValuePropertyAccessNode from, string asAlias, IEdmTypeReference typeReference, CollectionNode source)
        {
            ExceptionUtils.CheckArgumentNotNull(expression, "expression");
            // OK for from to be null
            ExceptionUtils.CheckArgumentNotNull(asAlias, "asAlias");
            ExceptionUtils.CheckArgumentNotNull(typeReference, "typeReference");
            // OK for source to be null


            this._expression = expression;
            this._withVerb = withVerb;
            this._from = from;
            this._asAlias = asAlias;
            this._typeReference = typeReference;
            this._source = source;
        }

        private readonly SingleValueNode _expression;

        public SingleValueNode Expression
        {
            get {
                return _expression;
            }
        }

        private readonly AggregationVerb _withVerb;

        public AggregationVerb WithVerb
        {
            get
            {
                return _withVerb;
            }
        }

        private readonly string _asAlias;

        public string AsAlias
        {
            get
            {
                return _asAlias;
            }
        }

        private readonly SingleValuePropertyAccessNode _from;

        public SingleValuePropertyAccessNode From
        {
            get
            {
                return _from;
            }
        }

        private readonly CollectionNode _source;

        public CollectionNode Source
        {
            get
            {
                return _source;
            }
        }

        public override QueryNodeKind Kind
        {
            get
            {
                return QueryNodeKind.AggregateStatement;
            }
        }

        private readonly IEdmTypeReference _typeReference;

        public IEdmTypeReference TypeReference
        {
            get
            {
                
                return this._typeReference;
            }
        }

    }
}
