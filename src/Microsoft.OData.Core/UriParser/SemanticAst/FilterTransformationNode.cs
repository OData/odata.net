//---------------------------------------------------------------------
// <copyright file="FilterTransformationNode.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------
// OData v4 Aggregation Extensions.
namespace Microsoft.OData.Core.UriParser.Semantic
{
    using TreeNodeKinds;
    using Microsoft.OData.Edm;
    using System.Collections.Generic;

    public sealed class FilterTransformationNode : TransformationNode
    {
        public FilterTransformationNode(FilterClause filterClause)
        {
            ExceptionUtils.CheckArgumentNotNull(filterClause, "filterClause");

            this._filterClause = filterClause;
        }

        private FilterClause _filterClause;

        public FilterClause FilterClause
        {
            get
            {
                return this._filterClause;
            }
        }

        /// <summary>
        /// Gets the type of item returned by this clause.
        /// </summary>
        public override IEdmTypeReference ItemType
        {
            get
            {
                return this.FilterClause.RangeVariable.TypeReference;
            }
        }

        public override TransformationNodeKind Kind
        {
            get
            {
                return TransformationNodeKind.Filter;
            }
        }
    }
}
