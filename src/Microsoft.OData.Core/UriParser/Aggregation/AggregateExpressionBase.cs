//---------------------------------------------------------------------
// <copyright file="AggregateExpressionBase.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.UriParser.Aggregation
{
    /// <summary>
    /// Enumeration of the possible types of aggregations.
    /// </summary>
    public enum AggregateExpressionKind
    {
        /// <summary>Value used to treat non recognized aggregations.</summary>
        None = 0,

        /// <summary>Aggregation of a single value property.</summary>
        PropertyAggregate = 1,

        /// <summary>Aggregation of a entity set property.</summary>
        EntitySetAggregate = 2
    }

    /// <summary>
    /// A aggregate expression representing a aggregation transformation.
    /// </summary>
    public abstract class AggregateExpressionBase
    {
        /// <summary>Base constructor for concrete subclasses use for convenience.</summary>
        /// <param name="kind">The <see cref="AggregateExpressionKind"/> of the expression.</param>
        /// <param name="alias">Alias of the resulting aggregated value.</param>
        protected AggregateExpressionBase(AggregateExpressionKind kind, string alias)
        {
            AggregateKind = kind;
            Alias = alias;
        }

        /// <summary>Returns the <see cref="AggregateExpressionKind"/> of the expression.</summary>
        public AggregateExpressionKind AggregateKind { get; private set; }

        /// <summary>Returns the alias of the expression.</summary>
        public string Alias { get; private set; }
    }
}
