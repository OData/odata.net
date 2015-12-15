//---------------------------------------------------------------------
// <copyright file="FilterTransformationNode.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Core.UriParser.Extensions.Semantic
{
    using TreeNodeKinds;
    using Microsoft.OData.Core.UriParser.Semantic;

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

        public override TransformationNodeKind Kind
        {
            get
            {
                return TransformationNodeKind.Filter;
            }
        }
    }
}
