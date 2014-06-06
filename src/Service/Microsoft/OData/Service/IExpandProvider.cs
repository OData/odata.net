//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Service
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// This interface declares the methods required to support the $expand
    /// query option for a WCF Data Service.
    /// </summary>
    [Obsolete("The IExpandProvider interface is deprecated.")]
    public interface IExpandProvider
    {
        /// <summary>Applies expansions to the specified <paramref name="queryable" /> parameter.</summary>
        /// <returns>An <see cref="T:System.Collections.IEnumerable" /> object of the same type as the supplied <paramref name="queryable" /> object that includes the specified <paramref name="expandPaths" />.</returns>
        /// <param name="queryable">The <see cref="T:System.Linq.IQueryable`1" /> object to expand.</param>
        /// <param name="expandPaths">A collection of <see cref="T:Microsoft.OData.Service.ExpandSegmentCollection" /> paths to expand. </param>
        /// <remarks>
        /// This method may modify the <paramref name="expandPaths"/> to indicate which expansions
        /// are included.
        ///
        /// The returned <see cref="IEnumerable"/> may implement the <see cref="IExpandedResult"/>
        /// interface to provide enumerable objects for the expansions; otherwise, the expanded
        /// information is expected to be found directly in the enumerated objects.
        /// </remarks>
        IEnumerable ApplyExpansions(IQueryable queryable, ICollection<ExpandSegmentCollection> expandPaths);
    }
}
