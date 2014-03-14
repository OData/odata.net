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

namespace Microsoft.Data.OData.Query.SyntacticAst
{
    #region Namespaces

    using System.Diagnostics.CodeAnalysis;
    using Microsoft.Data.OData;
    using Microsoft.Data.OData.Query.SemanticAst;

    #endregion

    /// <summary>
    /// Base class for all lexical tokens of OData query.
    /// </summary>
    internal abstract class QueryToken : ODataAnnotatable
    {
        /// <summary>
        /// Empty list of arguments.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2105:ArrayFieldsShouldNotBeReadOnly",
            Justification = "Modeled after Type.EmptyTypes")] public static readonly QueryToken[] EmptyTokens =
                new QueryToken[0];

        /// <summary>
        /// The kind of the query token.
        /// </summary>
        public abstract QueryTokenKind Kind { get; }

        /// <summary>
        /// Accept a <see cref="ISyntacticTreeVisitor{T}"/> to walk a tree of <see cref="QueryToken"/>s.
        /// </summary>
        /// <typeparam name="T">Type that the visitor will return after visiting this token.</typeparam>
        /// <param name="visitor">An implementation of the visitor interface.</param>
        /// <returns>An object whose type is determined by the type parameter of the visitor.</returns>
        public abstract T Accept<T>(ISyntacticTreeVisitor<T> visitor);
    }
}
