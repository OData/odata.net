//---------------------------------------------------------------------
// <copyright file="EntitySetAggregateExpression.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.UriParser.Aggregation
{
    using System.Collections.Generic;

    /// <summary>
    /// Represents an aggregate expression that aggregates an entity set.
    /// An example of a OData query that generates it can be found in example 59 of OData Extension for Data Aggregation Version 4.0.
    /// </summary>
    public sealed class EntitySetAggregateExpression : AggregateExpressionBase
    {
        private readonly CollectionNavigationNode expression;

        private readonly IEnumerable<AggregateExpressionBase> children;

        /// <summary>Constructor.</summary>
        /// <param name="expression">Navigation node used to create the expression.</param>
        /// <param name="children">Children of the expression.</param>
        public EntitySetAggregateExpression(CollectionNavigationNode expression, IEnumerable<AggregateExpressionBase> children)
            : base(AggregateExpressionKind.EntitySetAggregate, expression.NavigationProperty.Name)
        {
            this.expression = expression;
            this.children = children;
        }

        /// <summary>Returns the collection navigation node of this expression.</summary>
        public CollectionNavigationNode Expression
        {
            get
            {
                return this.expression;
            }
        }

        /// <summary>Returns the children expression of this expression.</summary>
        public IEnumerable<AggregateExpressionBase> Children
        {
            get
            {
                return this.children;
            }
        }
    }
}
