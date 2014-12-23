//   OData .NET Libraries ver. 6.9
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

#if ASTORIA_CLIENT
namespace Microsoft.OData.Client.ALinq.UriParser
#else
namespace Microsoft.OData.Core.UriParser.Syntactic
#endif
{
    using Microsoft.OData.Core.UriParser.Syntactic;
    using Microsoft.OData.Core.UriParser.TreeNodeKinds;
    using Microsoft.OData.Core.UriParser.Visitors;

    /// <summary>
    /// A token to represent a parameter to a function call.
    /// </summary>
    internal sealed class FunctionParameterToken : QueryToken
    {
        /// <summary>
        /// get an empty list of parameters
        /// </summary>
        public static FunctionParameterToken[] EmptyParameterList = new FunctionParameterToken[0];

        /// <summary>
        /// The name of the parameter
        /// </summary>
        private readonly string parameterName;
        
        /// <summary>
        /// The value of this parameter
        /// </summary>
        private readonly QueryToken valueToken;
        
        /// <summary>
        /// Creates a FunctionParameterToken
        /// </summary>
        /// <param name="parameterName">the name of this parameter</param>
        /// <param name="valueToken">the syntactically parsed value</param>
        public FunctionParameterToken(string parameterName, QueryToken valueToken)
        {
            this.parameterName = parameterName;
            this.valueToken = valueToken;
        }

        /// <summary>
        /// Gets the name of this parameter 
        /// </summary>
        public string ParameterName
        {
            get { return parameterName; }
        }

        /// <summary>
        /// Gets the syntactically parsed value of this token.
        /// </summary>
        public QueryToken ValueToken
        {
            get { return valueToken; }
        }

        /// <summary>
        /// Gets the kind of this token
        /// </summary>
        public override QueryTokenKind Kind
        {
            get { return QueryTokenKind.FunctionParameter; }
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
