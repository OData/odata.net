//---------------------------------------------------------------------
// <copyright file="AggregateCollectionExpression.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.UriParser.Aggregation
{
    using Microsoft.OData.Edm;

    /// <summary>
    /// Represents an aggregate expression that aggregates a collection value node.
    /// </summary>
    public sealed class AggregateCollectionExpression : AggregateExpressionBase
    {
        /// <summary>
        /// Create a AggregateCollectionExpression.
        /// </summary>
        /// <param name="expression">The aggregation expression.</param>
        /// <param name="methodDefinition">The <see cref="AggregationMethodDefinition"/>.</param>
        /// <param name="alias">The aggregation alias.</param>
        /// <param name="typeReference">The <see cref="IEdmTypeReference"/> of this aggregate expression.</param>
        public AggregateCollectionExpression(CollectionNode expression, AggregationMethodDefinition methodDefinition, string alias, IEdmTypeReference typeReference)
            : base(AggregateExpressionKind.CollectionPropertyAggregate, alias)
        {
            ExceptionUtils.CheckArgumentNotNull(expression, "expression");
            ExceptionUtils.CheckArgumentNotNull(methodDefinition, "methodDefinition");
            ExceptionUtils.CheckArgumentNotNull(alias, "alias");

            Expression = expression;
            MethodDefinition = methodDefinition;

            // TypeReference is null for dynamic properties ??
            TypeReference = typeReference;
        }

        /// <summary>
        /// Gets the aggregation expression.
        /// </summary>
        public CollectionNode Expression { get; }

        /// <summary>
        /// Gets the <see cref="AggregationMethod"/>.
        /// </summary>
        public AggregationMethod Method => MethodDefinition.MethodKind;

        /// <summary>
        /// Gets the <see cref="AggregationMethodDefinition"/>.
        /// </summary>
        public AggregationMethodDefinition MethodDefinition { get; }

        /// <summary>
        /// Gets the <see cref="IEdmTypeReference"/> of this aggregate expression.
        /// </summary>
        public IEdmTypeReference TypeReference { get; }
    }
}
