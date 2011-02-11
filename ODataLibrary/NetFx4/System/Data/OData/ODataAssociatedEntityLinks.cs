//   Copyright 2011 Microsoft Corporation
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

namespace System.Data.OData
{
    #region Namespaces.
    using System.Diagnostics;
    using System.Collections.Generic;
    #endregion Namespaces.

    /// <summary>
    /// Represents a collection of associated entity links (the result of a $links query).
    /// Might include an inline count and a next link.
    /// </summary>
#if INTERNAL_DROP
    internal class ODataAssociatedEntityLinks : ODataAnnotatable
#else
    public class ODataAssociatedEntityLinks : ODataAnnotatable
#endif
    {
        /// <summary>
        /// Represents the optional inline count of the $links collection.
        /// </summary>
        public long? InlineCount
        {
            get;
            set;
        }

        /// <summary>
        /// Represents the optional next link of the $links collection.
        /// </summary>
        public Uri NextLink
        {
            get;
            set;
        }

        /// <summary>
        /// An enumerable of <see cref="ODataAssociatedEntityLink"/> instances representing the 
        /// links of the associated entities.
        /// </summary>
        /// <remarks>These links should be usable to retrieve or modify the associated entities.</remarks>
        public IEnumerable<ODataAssociatedEntityLink> Links
        {
            get;
            set;
        }
    }
}
