//---------------------------------------------------------------------
// <copyright file="AggregateTransformationNode.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Core.UriParser.Aggregation
{
    using System.Collections.Generic;

    /// <summary>
    /// Node representing a aggregate transformation.
    /// </summary>
    public sealed class AggregateTransformationNode : TransformationNode
    {
        private readonly IEnumerable<AggregateExpression> expressions;

        /// <summary>
        /// Create a AggregateTransformationNode.
        /// </summary>
        /// <param name="expressions">A list of <see cref="AggregateExpression"/>.</param>
        public AggregateTransformationNode(IEnumerable<AggregateExpression> expressions)
        {
            ExceptionUtils.CheckArgumentNotNull(expressions, "expressions");

            this.expressions = expressions;
        }

        /// <summary>
        /// Gets the list of <see cref="AggregateExpression"/>.
        /// </summary>
        public IEnumerable<AggregateExpression> Expressions
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
                return TransformationNodeKind.Aggregate;
            }
        }
    }
}
