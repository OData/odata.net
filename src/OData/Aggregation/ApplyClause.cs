using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.OData.Core.Aggregation;

namespace Microsoft.OData.Core.UriParser.Semantic
{
    /// <summary>
    /// The result of aggregation query parsing
    /// </summary>
    public class ApplyClause
    {
        /// <summary>
        /// Contains a list of aggregation Transformations
        /// </summary>
        public List<Tuple<string,AggregationTransformationBase>> Transformations { get; set; }

    }
}
