//---------------------------------------------------------------------
// <copyright file="ComputeTransformationNode.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.UriParser.Aggregation
{
    using System.Collections.Generic;

    /// <summary>
    /// Node representing a compute expression.
    /// </summary>
    public sealed class ComputeTransformationNode : TransformationNode
    {
        private readonly IEnumerable<ComputeExpression> expressions;

        /// <summary>
        /// Create a ComputeTransformationNode.
        /// </summary>
        /// <param name="expressions">A list of <see cref="ComputeExpression"/>.</param>
        public ComputeTransformationNode(IEnumerable<ComputeExpression> expressions)
        {
            ExceptionUtils.CheckArgumentNotNull(expressions, "expressions");

            this.expressions = expressions;
        }

        /// <summary>
        /// Gets the list of <see cref="ComputeExpression"/>.
        /// </summary>
        public IEnumerable<ComputeExpression> Expressions
        {
            get
            {
                return expressions;
            }
        }

        /// <summary>
        /// Gets the kind of the transformation node.
        /// </summary>
        public override TransformationNodeKind Kind
        {
            get
            {
                return TransformationNodeKind.Compute;
            }
        }
    }
}
