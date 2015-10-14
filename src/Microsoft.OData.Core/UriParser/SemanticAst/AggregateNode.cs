//---------------------------------------------------------------------
// <copyright file="AggregateNode.cs" company="Microsoft">
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
        public AggregateNode(IEnumerable<AggregateStatementNode> statements, IEdmTypeReference itemType)
        {
            ExceptionUtils.CheckArgumentNotNull(statements, "statements");
            ExceptionUtils.CheckArgumentNotNull(itemType, "itemType");

            this._statements = statements;
            this._itemType = itemType;
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

        private readonly IEdmTypeReference _itemType;

        public IEdmTypeReference ItemType
        {
            get
            {

                return this._itemType;
            }
        }
    }
}
