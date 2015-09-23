//---------------------------------------------------------------------
// <copyright file="AggregateNode2.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------
// OData v4 Aggregation Extensions.
namespace Microsoft.OData.Core.UriParser.Semantic
{
    using TreeNodeKinds;
    using Microsoft.OData.Edm;
    using System.Collections.Generic;

    public sealed class AggregateNode : QueryNode
    {
        public AggregateNode(IEnumerable<AggregateStatementNode> statements, IEdmTypeReference typeReference)
        {
            ExceptionUtils.CheckArgumentNotNull(statements, "statements");
            ExceptionUtils.CheckArgumentNotNull(typeReference, "typeReference");

            this._statements = statements;
            this._typeReference = typeReference;        
        }

        private readonly IEnumerable<AggregateStatementNode> _statements;

        public IEnumerable<AggregateStatementNode> Statements
        {
            get {
                return _statements;
            }
        }

        public override QueryNodeKind Kind
        {
            get
            {
                return QueryNodeKind.Aggregate;
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
