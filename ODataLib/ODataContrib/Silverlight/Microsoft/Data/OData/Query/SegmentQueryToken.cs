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

namespace Microsoft.Data.OData.Query
{
    #region Namespaces
    using System.Collections.Generic;
    #endregion Namespaces

    /// <summary>
    /// Lexical token representing a single segment in the query path.
    /// </summary>
#if INTERNAL_DROP
    internal class SegmentQueryToken : QueryToken
#else
    public class SegmentQueryToken : QueryToken
#endif
    {
        /// <summary>
        /// The name of the segment, the identifier.
        /// </summary>
        private readonly string name;

        /// <summary>
        /// The parent segment, or null if this is the root segment.
        /// </summary>
        private readonly SegmentQueryToken parent;

        /// <summary>
        /// The named values in the key lookup for this segment.
        /// If the segment has no key lookup, then this property is null.
        /// If the segment has empty key lookup (), then this property is an empty collection.
        /// </summary>
        private readonly IEnumerable<NamedValue> namedValues;

        /// <summary>
        /// Create a new SegmentQueryToken given the name and parent and namedValues if any
        /// </summary>
        /// <param name="name">The name of the segment, the identifier.</param>
        /// <param name="parent">The parent segment, or null if this is the root segment.</param>
        /// <param name="namedValues">The named values in the key lookup for this segment.</param>
        public SegmentQueryToken(string name, SegmentQueryToken parent, IEnumerable<NamedValue> namedValues)
        {
            // We allow an "empty" name segment so we can create one for the case of a service-document URL (which has no path)
            ExceptionUtils.CheckArgumentNotNull(name, "name");

            this.name = name;
            this.parent = parent;
            this.namedValues = namedValues == null ? null : new ReadOnlyEnumerable<NamedValue>(namedValues);
        }

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
            get { return this.name; }
        }

        /// <summary>
        /// The parent segment, or null if this is the root segment.
        /// </summary>
        public SegmentQueryToken Parent
        {
            get { return this.parent; }
        }

        /// <summary>
        /// The named values in the key lookup for this segment.
        /// If the segment has no key lookup, then this property is null.
        /// If the segment has empty key lookup (), then this property is an empty collection.
        /// </summary>
        public IEnumerable<NamedValue> NamedValues
        {
            get { return this.namedValues; }
        }
    }
}
