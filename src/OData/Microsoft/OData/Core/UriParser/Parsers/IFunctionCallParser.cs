//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Core.UriParser.Parsers
{
    using Microsoft.OData.Core.UriParser.Syntactic;

    /// <summary>
    /// Interface for a class that can parse an identifier as a function and return a representitive QueryToken.
    /// </summary>
    internal interface IFunctionCallParser
    {
        /// <summary>
        /// Reference to the lexer.
        /// </summary>
        ExpressionLexer Lexer { get; }

        /// <summary>
        /// Parses an identifier that represents a function.
        /// </summary>
        /// <param name="parent">the syntactically bound parent of this identifier.</param>
        /// <returns>QueryToken representing this function.</returns>
        QueryToken ParseIdentifierAsFunction(QueryToken parent);
    }
}
