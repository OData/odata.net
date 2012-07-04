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
    /// Lexical token representing a query option.
    /// </summary>
#if INTERNAL_DROP
    internal sealed class QueryOptionQueryToken : QueryToken
#else
    public sealed class QueryOptionQueryToken : QueryToken
#endif
    {
        /// <summary>
        /// The name of the query option.
        /// </summary>
        private readonly string name;

        /// <summary>
        /// The value of the query option.
        /// </summary>
        private readonly string value;

        /// <summary>
        /// Create a new QueryOptionQueryToken given name and value.
        /// </summary>
        /// <param name="name">The name of the query option.</param>
        /// <param name="value">The value of the query option.</param>
        public QueryOptionQueryToken(string name, string value)
        {
            this.name = name;
            this.value = value;
        }

        /// <summary>
        /// The kind of the query token.
        /// </summary>
        public override QueryTokenKind Kind
        {
            get { return QueryTokenKind.QueryOption; }
        }

        /// <summary>
        /// The name of the query option.
        /// </summary>
        public string Name
        {
            get { return this.name; }
        }

        /// <summary>
        /// The value of the query option.
        /// </summary>
        public string Value
        {
            get { return this.value; }
        }
    }
}
