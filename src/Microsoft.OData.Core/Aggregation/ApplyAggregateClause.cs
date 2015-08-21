using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.OData.Core.Aggregation;
using Microsoft.OData.Core.UriParser.Semantic;

namespace Microsoft.OData.Core.UriParser.Semantic
{
    /// <summary>
    /// The result of parsing a $apply=aggregate query option.
    /// </summary>
    public class ApplyAggregateClause : AggregationTransformationBase
    {
        /// <summary>
        /// Gets the name of the AggregatableProperty
        /// </summary>
        public string AggregatableProperty { get; set; }

        /// <summary>
        /// Gets the name of the Aggregation Method
        /// </summary>
        public string AggregationMethod { get; set; }

        /// <summary>
        /// Gets the alias
        /// </summary>
        public string Alias { get; set; }

        /// <summary>
        /// Gets a projection Expression that defines access to the property to aggregate
        /// </summary>
        public ExpressionClause AggregatablePropertyExpression { get; set; }
    }
}
