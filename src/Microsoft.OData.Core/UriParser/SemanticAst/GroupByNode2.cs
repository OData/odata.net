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
    public sealed class GroupByNode2 : SingleValueNode
    {
        public GroupByNode2(IEnumerable<SingleValuePropertyAccessNode> groupingProperties, AggregateNode2 aggregate, IEdmTypeReference typeReference, CollectionNode source)
        {
            ExceptionUtils.CheckArgumentNotNull(groupingProperties, "groupingProperties");
            // OK for aggregate to be null
            ExceptionUtils.CheckArgumentNotNull(typeReference, "typeReference");
            // OK for source to be null

            this._groupingProperties = groupingProperties;
            this._aggregate = aggregate;
            this._typeReference = typeReference;
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

        private readonly AggregateNode2 _aggregate;

        public AggregateNode2 Aggregate
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

        internal override InternalQueryNodeKind InternalKind
        {
            get
            {
                return InternalQueryNodeKind.GroupBy;
            }
        }

        private readonly IEdmTypeReference _typeReference;

        public override IEdmTypeReference TypeReference
        {
            get
            {

                return this._typeReference;
            }
        }
    }
}
