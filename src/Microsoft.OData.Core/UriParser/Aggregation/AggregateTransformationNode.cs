//---------------------------------------------------------------------
// <copyright file="AggregateTransformationNode.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.UriParser.Aggregation
{
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
        /// Property that only return <see cref="AggregateExpression"/>s.
        /// Exists for backward compatibility.
        /// </summary>
        public IEnumerable<AggregateExpression> Expressions
        {
            get
            {
                return expressions.Select(x => GetAggregateExpressionOrThrow(x));
            }
        }

        /// <summary>
        /// Property that returns a list of all <see cref="AggregateExpressionBase"/>s of this tranformation node.
        /// Should be used over <see cref="Expressions"/> property.
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

        /// <summary>
        /// Tries to get a <see cref="AggregateExpression"/> from it's base class.
        /// If it fails, throws an <see cref="ODataException"/>
        /// </summary>
        /// <param name="expression">The object that is going to be cast.</param>
        /// <returns>Returns a <see cref="AggregateExpression"/> or throws an <see cref="ODataException"/>.</returns>
        private static AggregateExpression GetAggregateExpressionOrThrow(AggregateExpressionBase expression)
        {
            AggregateExpression aggregateExpression = expression as AggregateExpression;
            if (aggregateExpression != null)
            {
                return aggregateExpression;
            }

            throw new ODataException(Strings.AggregateTransformationNode_UnsupportedAggregateExpressions());
        }
    }
}
