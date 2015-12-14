//---------------------------------------------------------------------
// <copyright file="GroupByTransformationNode.cs" company="Microsoft">
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
    public sealed class GroupByTransformationNode : TransformationNode
    {
        public GroupByTransformationNode(IList<GroupByPropertyNode> groupingProperties, IEdmTypeReference groupingItemType, TransformationNode childTransformation, IEdmTypeReference itemType, CollectionNode source)
        {
            ExceptionUtils.CheckArgumentNotNull(groupingProperties, "groupingProperties");
            ExceptionUtils.CheckArgumentNotNull(groupingItemType, "groupingItemType");
            // OK for childTransformation to be null
            ExceptionUtils.CheckArgumentNotNull(itemType, "itemType");
            // OK for source to be null

            this._groupingProperties = groupingProperties;
            this._groupingItemType = groupingItemType;
            this._childTransformation = childTransformation;
            this._itemType = itemType;
            this._source = source;
        }

        private readonly IEnumerable<GroupByPropertyNode> _groupingProperties;

        public IEnumerable<GroupByPropertyNode> GroupingProperties
        {
            get
            {
                return _groupingProperties;
            }
        }

        private readonly TransformationNode _childTransformation;

        public TransformationNode ChildTransformation
        {
            get
            {
                return _childTransformation;
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

        public override TransformationNodeKind Kind
        {
            get
            {
                return TransformationNodeKind.GroupBy;
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
        public override IEdmTypeReference ItemType
        {
            get
            {

                return this._itemType;
            }
        }
    }
}
