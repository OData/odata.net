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
    /// Lexical token representing an all-properties access.
    /// </summary>
#if INTERNAL_DROP
    internal sealed class StarQueryToken : QueryToken
#else
    public sealed class StarQueryToken : QueryToken
#endif
    {
        /// <summary>
        /// The parent token to access the property on.
        /// If this is null, then the property access has no parent. That usually means to access the property
        /// on the implicit parameter for the expression, the result on which the expression is being applied.
        /// </summary>
        private readonly QueryToken parent;

        /// <summary>
        /// Create a new StarQueryToken given the parent (if any).
        /// </summary>
        /// <param name="parent">The parent token to access the property on. Pass no if this property has no parent.</param>
        public StarQueryToken(QueryToken parent)
        {
            this.parent = parent;
        }

        /// <summary>
        /// The kind of the query token.
        /// </summary>
        public override QueryTokenKind Kind
        {
            get { return QueryTokenKind.Star; }
        }

        /// <summary>
        /// The parent token to access the property on.
        /// If this is null, then the property access has no parent. That usually means to access the property
        /// on the implicit parameter for the expression, the result on which the expression is being applied.
        /// </summary>
        public QueryToken Parent
        {
            get { return this.parent; }
        }
    }
}
