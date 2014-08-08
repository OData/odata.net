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

namespace Microsoft.Data.OData.Query
{
    using Microsoft.Data.OData.Query.SemanticAst;
    using Microsoft.Data.OData.Query.SyntacticAst;

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
            DebugUtils.CheckNoExternalCallers(); 
            ExceptionUtils.CheckArgumentNotNull(literalToken, "literalToken");

            if (!string.IsNullOrEmpty(literalToken.OriginalText))
            {
                return new ConstantNode(literalToken.Value, literalToken.OriginalText);
            }

            return new ConstantNode(literalToken.Value);
        }
    }
}
