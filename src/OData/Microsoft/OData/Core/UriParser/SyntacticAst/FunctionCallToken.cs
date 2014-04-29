//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

#if ASTORIA_CLIENT
namespace Microsoft.OData.Client.ALinq.UriParser
#else
namespace Microsoft.OData.Core.UriParser.Syntactic
#endif
{
    #region Namespaces

    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.OData.Core.UriParser.TreeNodeKinds;
    using Microsoft.OData.Core.UriParser.Visitors;

    #endregion Namespaces

    /// <summary>
    /// Lexical token representing a function call.
    /// </summary>
    internal sealed class FunctionCallToken : QueryToken
    {
        /// <summary>
        /// The name of the function to call.
        /// </summary>
        private readonly string name;

        /// <summary>
        /// The arguments for the function.
        /// </summary>
        private readonly IEnumerable<FunctionParameterToken> arguments;

        /// <summary>
        /// the source token for this function call
        /// </summary>
        private readonly QueryToken source;

        /// <summary>
        /// Create a new FunctionCallToken using the given function name and argument values.
        /// </summary>
        /// <param name="name">The name of the function to call.</param>
        /// <param name="argumentValues">The argument values for the function.</param>
        public FunctionCallToken(string name, IEnumerable<QueryToken> argumentValues)
        {
            ExceptionUtils.CheckArgumentStringNotNullOrEmpty(name, "name");

            this.name = name;
            this.arguments = argumentValues == null ? 
                new ReadOnlyEnumerableForUriParser<FunctionParameterToken>(FunctionParameterToken.EmptyParameterList) : 
                new ReadOnlyEnumerableForUriParser<FunctionParameterToken>(argumentValues.Select(v => new FunctionParameterToken(null, v)));
            this.source = null;
        }

        /// <summary>
        /// Create a new FunctionCallToken using the given function name and parameter tokens.
        /// </summary>
        /// <param name="name">The name of the function to call.</param>
        /// <param name="arguments">The arguments for the function.</param>
        /// <param name="source">The syntactically bound parent of this function</param>
        public FunctionCallToken(string name, IEnumerable<FunctionParameterToken> arguments, QueryToken source)
        {
            ExceptionUtils.CheckArgumentStringNotNullOrEmpty(name, "name");

            this.name = name;
            this.arguments = new ReadOnlyEnumerableForUriParser<FunctionParameterToken>(arguments ?? FunctionParameterToken.EmptyParameterList);
            this.source = source;
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
        public IEnumerable<FunctionParameterToken> Arguments
        {
            get { return this.arguments; }
        }

        /// <summary>
        /// The syntactically bound parent of this function.
        /// </summary>
        public QueryToken Source
        {
            get { return this.source; }
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
