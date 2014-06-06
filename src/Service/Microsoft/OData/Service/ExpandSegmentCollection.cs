//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Service
{
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Provides a description of a path in an $expand query option
    /// for a WCF Data Service.
    /// </summary>
    public class ExpandSegmentCollection : List<ExpandSegment>
    {
        /// <summary>Creates a collection of expand segments for a query.</summary>
        public ExpandSegmentCollection()
        {
        }

        /// <summary>Initializes a new collection of expand segments that is empty and has the specified initial capacity.</summary>
        /// <param name="capacity">The number of expand segments that the new collection can initially store.</param>
        public ExpandSegmentCollection(int capacity) : base(capacity)
        {
        }

        /// <summary>Boolean value that indicates whether segments to be expanded include a filter clause.</summary>
        /// <returns>Boolean value that indicates whether segments to be expanded include a filter clause. </returns>
        public bool HasFilter
        {
            get { return this.Any(segment => segment.HasFilter); }
        }
    }
}
