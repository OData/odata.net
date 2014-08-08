//   OData .NET Libraries ver. 5.6.2
//   Copyright (c) Microsoft Corporation. All rights reserved.
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

namespace System.Data.Services
{
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
        /// <param name="expandPaths">A collection of <see cref="T:System.Data.Services.ExpandSegmentCollection" /> paths to expand. </param>
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
