using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.OData.Core.UriParser.Semantic;

namespace Microsoft.OData.Core.UriParser.Semantic 
{
    public class ApplyFilterClause  : AggregationTransformationBase
    {
        public FilterClause Filter { get; set; }

        public string RawQueryString { get; set; }
    }
}
