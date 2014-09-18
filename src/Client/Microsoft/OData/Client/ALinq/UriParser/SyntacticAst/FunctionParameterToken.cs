//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation. All rights reserved.  
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at

//       http://www.apache.org/licenses/LICENSE-2.0

//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

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
