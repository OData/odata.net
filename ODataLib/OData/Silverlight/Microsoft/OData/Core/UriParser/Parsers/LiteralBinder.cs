//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Core.UriParser.Parsers
{
    using Microsoft.OData.Core.UriParser.Semantic;
    using Microsoft.OData.Core.UriParser.Syntactic;

    /// <summary>
    /// Class that knows how to bind literal values.
    /// </summary>
    internal sealed class LiteralBinder
    {
        /// <summary>
        /// Binds a literal value to a ConstantNode
        /// </summary>
        /// <param name="literalToken">Literal token to bind.</param>
        /// <returns>Bound query node.</returns>
        internal static ConstantNode BindLiteral(LiteralToken literalToken)
        {
            ExceptionUtils.CheckArgumentNotNull(literalToken, "literalToken");

            if (!string.IsNullOrEmpty(literalToken.OriginalText))
            {
                return new ConstantNode(literalToken.Value, literalToken.OriginalText);
            }

            return new ConstantNode(literalToken.Value);
        }
    }
}
