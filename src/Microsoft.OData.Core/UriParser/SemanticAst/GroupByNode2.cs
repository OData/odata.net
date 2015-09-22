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
    public sealed class GroupByNode2 : QueryNode
    {
        public GroupByNode2(IEnumerable<SingleValuePropertyAccessNode> groupingProperties, IEdmTypeReference groupingTypeReference, AggregateNode2 aggregate, IEdmTypeReference typeReference, CollectionNode source)
        {
            ExceptionUtils.CheckArgumentNotNull(groupingProperties, "groupingProperties");
            ExceptionUtils.CheckArgumentNotNull(groupingTypeReference, "groupingTypeReference");
            // OK for aggregate to be null
            ExceptionUtils.CheckArgumentNotNull(typeReference, "typeReference");
            // OK for source to be null

            this._groupingProperties = groupingProperties;
            this._groupingTypeReference = groupingTypeReference;
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

        public override QueryNodeKind Kind
        {
            get
            {
                return QueryNodeKind.GroupBy;
            }
        }

        private readonly IEdmTypeReference _groupingTypeReference;

        /// <summary>
        /// Gets the type resulting from the grouping properties.
        /// </summary>
        public IEdmTypeReference GroupingTypeReference
        {
            get
            {

                return this._groupingTypeReference;
            }
        }

        private readonly IEdmTypeReference _typeReference;

        /// <summary>
        /// Gets the type resulting from the grouping properties and aggregate.
        /// </summary>
        public IEdmTypeReference TypeReference
        {
            get
            {

                return this._typeReference;
            }
        }
    }
}
