//---------------------------------------------------------------------
// <copyright file="AggregateTransformationNode.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------
// OData v4 Aggregation Extensions.
namespace Microsoft.OData.Core.UriParser.Semantic
{
    using TreeNodeKinds;
    using Microsoft.OData.Edm;
    using System.Collections.Generic;

      public sealed class AggregateTransformationNode : TransformationNode
    {
        public AggregateTransformationNode(IEnumerable<AggregateStatement> statements, IEdmTypeReference itemType)
        {
            ExceptionUtils.CheckArgumentNotNull(statements, "statements");
            ExceptionUtils.CheckArgumentNotNull(itemType, "itemType");

            this._statements = statements;
            this._itemType = itemType;
        }

        private readonly IEnumerable<AggregateStatement> _statements;

        public IEnumerable<AggregateStatement> Statements
        {
            get {
                return _statements;
            }
        }

        public override TransformationNodeKind Kind
        {
            get
            {
                return TransformationNodeKind.Aggregate;
            }
        }

        private readonly IEdmTypeReference _itemType;

        public override IEdmTypeReference ItemType
        {
            get
            {

                return this._itemType;
            }
        }
    }
}
