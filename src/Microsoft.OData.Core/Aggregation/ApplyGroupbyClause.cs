using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.OData.Core.Aggregation;
using Microsoft.OData.Core.UriParser.Semantic;

namespace Microsoft.OData.Core.UriParser.Semantic
{
    /// <summary>
    /// The result of parsing a $apply=groupby query option.
    /// </summary>
    public class ApplyGroupbyClause : AggregationTransformationBase
    {
        /// <summary>
        /// Gets a list of selected properties that define the key to group by
        /// </summary>
        public IEnumerable<string> SelectedStatements { get; set; }

        /// <summary>
        /// Gets a list of selected properties that define the key to group by as projection Expression that defines access to the property
        /// </summary>
        public List<ExpressionClause> SelectedPropertiesExpressions { get; set; }

        /// <summary>
        /// Gets an optional aggregation clause that defines aggregation on the grouped values
        /// </summary>
        public ApplyAggregateClause Aggregate { get; set; }   
    }
}
