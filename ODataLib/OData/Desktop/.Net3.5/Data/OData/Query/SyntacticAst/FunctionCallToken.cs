//   OData .NET Libraries ver. 5.6.3
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 
//   MIT License
//   Permission is hereby granted, free of charge, to any person obtaining a copy of
//   this software and associated documentation files (the "Software"), to deal in
//   the Software without restriction, including without limitation the rights to use,
//   copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the
//   Software, and to permit persons to whom the Software is furnished to do so,
//   subject to the following conditions:

//   The above copyright notice and this permission notice shall be included in all
//   copies or substantial portions of the Software.

//   THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
//   FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
//   COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
//   IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
//   CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

namespace Microsoft.Data.OData.Query.SyntacticAst
{
    #region Namespaces

    using System.Collections.Generic;
    using System.Linq;


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
