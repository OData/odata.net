//---------------------------------------------------------------------
// <copyright file="AggregateTransformationNode.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.UriParser.Aggregation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using ODataErrorStrings = Microsoft.OData.Strings;

    /// <summary>
    /// Node representing a aggregate transformation.
    /// </summary>
    public sealed class AggregateTransformationNode : TransformationNode
    {
        private readonly IEnumerable<AggregateExpressionBase> expressions;

        /// <summary>
        /// Create a AggregateTransformationNode.
        /// </summary>
        /// <param name="expressions">A list of <see cref="AggregateExpressionBase"/>.</param>
        public AggregateTransformationNode(IEnumerable<AggregateExpressionBase> expressions)
        {
            ExceptionUtils.CheckArgumentNotNull(expressions, "expressions");

            this.expressions = expressions;
        }

        /// <summary>
        /// Property that returns a list of all <see cref="AggregateExpressionBase"/>s of this transformation node.
        /// </summary>
        public IEnumerable<AggregateExpressionBase> AggregateExpressions
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
