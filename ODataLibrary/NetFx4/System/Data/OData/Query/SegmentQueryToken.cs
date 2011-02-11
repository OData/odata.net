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

namespace System.Data.OData.Query
{
    #region Namespaces.
    using System.Collections.Generic;
    #endregion Namespaces.

    /// <summary>
    /// Lexical token representing a single segment in the query path.
    /// </summary>
#if INTERNAL_DROP
    internal sealed class SegmentQueryToken : QueryToken
#else
    public sealed class SegmentQueryToken : QueryToken
#endif
    {
        /// <summary>
        /// The kind of the query token.
        /// </summary>
        public override QueryTokenKind Kind
        {
            get { return QueryTokenKind.Segment; }
        }

        /// <summary>
        /// The name of the segment, the identifier.
        /// </summary>
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// The parent segment, or null if this is the root segment.
        /// </summary>
        public SegmentQueryToken Parent
        {
            get;
            set;
        }

        /// <summary>
        /// The named values in the key lookup for this segment.
        /// If the segment has no key lookup, then this property is null.
        /// If the segment has empty key lookup (), then this property is an empty collection.
        /// </summary>
        public IEnumerable<NamedValue> NamedValues
        {
            get;
            set;
        }
    }
}
