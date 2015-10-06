//---------------------------------------------------------------------
// <copyright file="CollectionOpenPropertyAccessNode.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------
// OData v4 Aggregation Extensions.

using Microsoft.OData.Edm;
using System.Collections.Generic;
using System;
using Microsoft.OData.Core.UriParser.TreeNodeKinds;

namespace Microsoft.OData.Core.UriParser.Semantic
{
    public sealed class GroupByNode : QueryNode
    {
        public GroupByNode(IEnumerable<SingleValuePropertyAccessNode> groupingProperties, IEdmTypeReference groupingItemType, AggregateNode aggregate, IEdmTypeReference itemType, CollectionNode source)
        {
            ExceptionUtils.CheckArgumentNotNull(groupingProperties, "groupingProperties");
            ExceptionUtils.CheckArgumentNotNull(groupingItemType, "groupingItemType");
            // OK for aggregate to be null
            ExceptionUtils.CheckArgumentNotNull(itemType, "itemType");
            // OK for source to be null

            this._groupingProperties = groupingProperties;
            this._groupingItemType = groupingItemType;
            this._aggregate = aggregate;
            this._itemType = itemType;
            this._source = source;
        }

        private readonly IEnumerable<SingleValuePropertyAccessNode> _groupingProperties;

        public IEnumerable<SingleValuePropertyAccessNode> GroupingProperties
        {
            get
            {
                return _groupingProperties;
            }
        }

        private readonly AggregateNode _aggregate;

        public AggregateNode Aggregate
        {
            get
            {
                return _aggregate;
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
                return QueryNodeKind.GroupBy;
            }
        }

        private readonly IEdmTypeReference _groupingItemType;

        /// <summary>
        /// Gets the type resulting from the grouping properties.
        /// </summary>
        public IEdmTypeReference GroupingItemType
        {
            get
            {

                return this._groupingItemType;
            }
        }

        private readonly IEdmTypeReference _itemType;

        /// <summary>
        /// Gets the type resulting from the grouping properties and aggregate.
        /// </summary>
        public IEdmTypeReference ItemType
        {
            get
            {

                return this._itemType;
            }
        }
    }
}
