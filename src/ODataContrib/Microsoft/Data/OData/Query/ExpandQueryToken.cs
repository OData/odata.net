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
    /// Lexical token representing an expand operation.
    /// </summary>
#if INTERNAL_DROP
    internal sealed class ExpandQueryToken : QueryToken
#else
    public sealed class ExpandQueryToken : QueryToken
#endif
    {
        /// <summary>
        /// The properties according to which to expand in the results.
        /// </summary>
        private readonly IEnumerable<QueryToken> properties;

        /// <summary>
        /// Create a ExpandQueryToken given the property-accesses of the expand query.
        /// </summary>
        /// <param name="properties">The properties according to which to expand the results.</param>
        public ExpandQueryToken(IEnumerable<QueryToken> properties)
        {
            this.properties = new ReadOnlyEnumerable<QueryToken>(properties ?? QueryToken.EmptyTokens);
        }

        /// <summary>
        /// The kind of the query token.
        /// </summary>
        public override QueryTokenKind Kind
        {
            get { return QueryTokenKind.Expand; }
        }

        /// <summary>
        /// The properties according to which to expand in the results.
        /// </summary>
        public IEnumerable<QueryToken> Properties
        {
            get { return this.properties; }
        }
    }
}
