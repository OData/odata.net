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
    /// <summary>
    /// Lexical token representing a type segment.
    /// </summary>
    public sealed class CastToken : QueryToken
    {
        /// <summary>
        /// The name of the type segment.
        /// </summary>
        private readonly string name;

        /// <summary>
        /// The namespace of the type segment.
        /// </summary>
        private readonly string space;

        /// <summary>
        /// The parent segment.
        /// </summary>
        private readonly QueryToken parent;

        /// <summary>
        /// Create a TypeSegmentQueryToken given the name and the parent (if any)
        /// </summary>
        /// <param name="space">The namespace of the type segment.</param>
        /// <param name="name">The name of the type segment.</param>
        /// <param name="parent">The parent segment.</param>
        public CastToken(string space, string name, QueryToken parent)
        {
            ExceptionUtils.CheckArgumentStringNotNullOrEmpty(name, "name");
            ExceptionUtils.CheckArgumentStringNotNullOrEmpty(space, "ns");

            this.space = space;
            this.name = name;
            this.parent = parent;
        }

        /// <summary>
        /// The kind of the query token.
        /// </summary>
        public override QueryTokenKind Kind
        {
            get { return QueryTokenKind.Cast; }
        }

        /// <summary>
        /// The name of the type.
        /// </summary>
        public string SegmentName
        {
            get { return this.name; }
        }

        /// <summary>
        /// The parent.
        /// </summary>
        public QueryToken Source
        {
            get { return this.parent; }
        }

        /// <summary>
        /// The name of the namespace.
        /// </summary>
        public string SegmentSpace
        {
            get { return this.space; }
        }
    }
}
