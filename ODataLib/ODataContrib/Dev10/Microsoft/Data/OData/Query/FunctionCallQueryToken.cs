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
    /// Lexical token representing a function call.
    /// </summary>
#if INTERNAL_DROP
    internal sealed class FunctionCallQueryToken : QueryToken
#else
    public sealed class FunctionCallQueryToken : QueryToken
#endif
    {
        /// <summary>
        /// The name of the function to call.
        /// </summary>
        private readonly string name;

        /// <summary>
        /// The arguments for the function.
        /// </summary>
        private readonly IEnumerable<QueryToken> arguments;

        /// <summary>
        /// Create a new FunctionCallQueryToken given name and arguments.
        /// </summary>
        /// <param name="name">The name of the function to call.</param>
        /// <param name="arguments">The arguments for the function.</param>
        public FunctionCallQueryToken(string name, IEnumerable<QueryToken> arguments)
        {
            ExceptionUtils.CheckArgumentStringNotNullOrEmpty(name, "name");

            this.name = name;
            this.arguments = new ReadOnlyEnumerable<QueryToken>(arguments ?? QueryToken.EmptyTokens);
        }

        /// <summary>
        /// The kind of the query token.
        /// </summary>
        public override QueryTokenKind Kind
        {
            get { return QueryTokenKind.FunctionCall; }
        }

        /// <summary>
        /// The name of the function to call.
        /// </summary>
        public string Name
        {
            get { return this.name; }
        }

        /// <summary>
        /// The arguments for the function.
        /// </summary>
        public IEnumerable<QueryToken> Arguments
        {
            get { return this.arguments; }
        }
    }
}
