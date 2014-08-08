//   OData .NET Libraries ver. 5.6.2
//   Copyright (c) Microsoft Corporation. All rights reserved.
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

namespace Microsoft.Data.OData.Query.SyntacticAst
{
    using Microsoft.Data.OData.Query.SemanticAst;

    #region Namespaces
    #endregion Namespaces

    /// <summary>
    /// Lexical token representing the parameter for an Any/All query.
    /// </summary>
    internal sealed class RangeVariableToken : QueryToken
    {
        /// <summary>
        /// The name of the Any/All parameter.
        /// </summary>
        private readonly string name;

        /// <summary>
        /// Create a new RangeVariableToken
        /// </summary>
        /// <param name="name">The name of the visitor for the Any/All query.</param>
        public RangeVariableToken(string name)
        {
            ExceptionUtils.CheckArgumentNotNull(name, "visitor");

            this.name = name;
        }

        /// <summary>
        /// The kind of the query token.
        /// </summary>
        public override QueryTokenKind Kind
        {
            get { return QueryTokenKind.RangeVariable; }
        }

        /// <summary>
        /// The name of the parameter.
        /// </summary>
        public string Name
        {
            get { return this.name; }
        }

        /// <summary>
        /// Accept a <see cref="ISyntacticTreeVisitor{T}"/> to walk a tree of <see cref="QueryToken"/>s.
        /// </summary>
        /// <typeparam name="T">Type that the visitor will return after visiting this token.</typeparam>
        /// <param name="visitor">An implementation of the visitor interface.</param>
        /// <returns>An object whose type is determined by the type parameter of the visitor.</returns>
        public override T Accept<T>(ISyntacticTreeVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }
    }
}
