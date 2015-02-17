//---------------------------------------------------------------------
// <copyright file="ExpandSegmentCollection.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

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
