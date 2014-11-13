//   OData .NET Libraries ver. 5.6.3
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 
//   MIT License
//   Permission is hereby granted, free of charge, to any person obtaining a copy of
//   this software and associated documentation files (the "Software"), to deal in
//   the Software without restriction, including without limitation the rights to use,
//   copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the
//   Software, and to permit persons to whom the Software is furnished to do so,
//   subject to the following conditions:

//   The above copyright notice and this permission notice shall be included in all
//   copies or substantial portions of the Software.

//   THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
//   FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
//   COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
//   IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
//   CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

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
