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
    #endregion Namespaces

    /// <summary>
    /// Lexical token representing a segment query token that is a keyword ($metadata, $count, $value)
    /// </summary>
#if INTERNAL_DROP
    internal sealed class KeywordSegmentQueryToken : SegmentQueryToken
#else
    public sealed class KeywordSegmentQueryToken : SegmentQueryToken
#endif
    {
        /// <summary>
        /// The keyword kind of the segment.
        /// </summary>
        private readonly KeywordKind keyword;

        /// <summary>
        /// Create a new StarQueryToken given the parent (if any).
        /// </summary>
        /// <param name="keyword">The keyword kind of the segment.</param>
        /// <param name="parent">The parent segment, or null if this is the root segment.</param>
        public KeywordSegmentQueryToken(KeywordKind keyword, SegmentQueryToken parent)
            : base(QueryTokenUtils.GetNameFromKeywordKind(keyword), parent, null)
        {
            this.keyword = keyword;
        }

        /// <summary>
        /// The kind of the query token.
        /// </summary>
        public override QueryTokenKind Kind
        {
            get { return QueryTokenKind.KeywordSegment; }
        }

        /// <summary>
        /// The keyword kind of the segment.
        /// </summary>
        public KeywordKind Keyword
        {
            get { return this.keyword; }
        }
    }
}
