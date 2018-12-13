//---------------------------------------------------------------------
// <copyright file="ExpandTransformationNode.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.UriParser.Aggregation
{
    using Microsoft.OData.UriParser;

    /// <summary>
    /// Node representing a expand transformation.
    /// </summary>
    public sealed class ExpandTransformationNode : TransformationNode
    {
        private readonly SelectExpandClause expandClause;

        /// <summary>
        /// Create a ExpandTransformationNode.
        /// </summary>
        /// <param name="expandClause">A <see cref="SelectExpandClause"/> representing the metadata bound expand expression.</param>
        public ExpandTransformationNode(SelectExpandClause expandClause)
        {
            ExceptionUtils.CheckArgumentNotNull(expandClause, "expandClause");

            this.expandClause = expandClause;
        }

        /// <summary>
        /// Gets the <see cref="SelectExpandClause"/> representing the metadata bound expand expression.
        /// </summary>
        public SelectExpandClause ExpandClause
        {
            get
            {
                return this.expandClause;
            }
        }

        /// <summary>
        /// Gets the kind of the transformation node.
        /// </summary>
        public override TransformationNodeKind Kind
        {
            get
            {
                return TransformationNodeKind.Expand;
            }
        }
    }
}
