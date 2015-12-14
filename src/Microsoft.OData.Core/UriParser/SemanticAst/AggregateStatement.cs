//---------------------------------------------------------------------
// <copyright file="AggregateStatement.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------
// OData v4 Aggregation Extensions.
namespace Microsoft.OData.Core.UriParser.Semantic
{
    using TreeNodeKinds;
    using Microsoft.OData.Edm;

    public sealed class AggregateStatement
    {
        public AggregateStatement(SingleValueNode expression, AggregationVerb withVerb, SingleValuePropertyAccessNode from, string asAlias, IEdmTypeReference typeReference)
        {
            ExceptionUtils.CheckArgumentNotNull(expression, "expression");
            // OK for from to be null
            ExceptionUtils.CheckArgumentNotNull(asAlias, "asAlias");
            ExceptionUtils.CheckArgumentNotNull(typeReference, "typeReference");

            this._expression = expression;
            this._withVerb = withVerb;
            this._from = from;
            this._asAlias = asAlias;
            this._typeReference = typeReference;
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
