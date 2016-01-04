//---------------------------------------------------------------------
// <copyright file="FilterTransformationNode.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Core.UriParser.Extensions.Semantic
{
    using TreeNodeKinds;
    using Microsoft.OData.Core.UriParser.Semantic;

    /// <summary>
    /// Node representing a filter transformation.
    /// </summary>
    public sealed class FilterTransformationNode : TransformationNode
    {
        private readonly FilterClause filterClause;

        /// <summary>
        /// Create a FilterTransformationNode.
        /// </summary>
        /// <param name="filterClause">A <see cref="FilterClause"/> representing the metadata bound filter expression.</param>
        public FilterTransformationNode(FilterClause filterClause)
        {
            ExceptionUtils.CheckArgumentNotNull(filterClause, "filterClause");

            this.filterClause = filterClause;
        }

        /// <summary>
        /// Gets the <see cref="FilterClause"/> representing the metadata bound filter expression.
        /// </summary>
        public FilterClause FilterClause
        {
            get
            {
                return this.filterClause;
            }
        }

        /// <summary>
        /// Gets the kind of the transformation node.
        /// </summary>
        public override TransformationNodeKind Kind
        {
            get
            {
                return TransformationNodeKind.Filter;
            }
        }
    }
}
