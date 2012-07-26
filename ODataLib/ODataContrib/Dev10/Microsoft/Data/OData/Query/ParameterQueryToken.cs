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
    /// Lexical token representing the parameter for an Any/All query.
    /// </summary>
    public class ParameterQueryToken : QueryToken
    {
        /// <summary>
        /// The name of the Any/All parameter.
        /// </summary>
        private readonly string visitor;

        /// <summary>
        /// The parent of the Any/All query
        /// </summary>
        private readonly NavigationPropertyToken parent;

        /// <summary>
        /// Create a new ParameterQueryToken given the parent and visitor if any
        /// </summary>
        /// <param name="parent">The parent of the Any/All query.</param>
        /// <param name="visitor">The name of the visitor for the Any/All query.</param>
        public ParameterQueryToken(NavigationPropertyToken parent, string visitor)
        {
            ExceptionUtils.CheckArgumentNotNull(visitor, "visitor");

            this.parent = parent;
            this.visitor = visitor;
        }

        /// <summary>
        /// The kind of the query token.
        /// </summary>
        public override QueryTokenKind Kind
        {
            get { return QueryTokenKind.Parameter; }
        }

        /// <summary>
        /// The parent of the Any/All query.
        /// </summary>
        public NavigationPropertyToken Parent
        {
            get { return this.parent; }
        }

        /// <summary>
        /// The name of the Any/All parameter.
        /// </summary>
        public string Visitor
        {
            get { return this.visitor; }
        }
    }
}
