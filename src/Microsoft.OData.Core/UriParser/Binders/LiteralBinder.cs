//---------------------------------------------------------------------
// <copyright file="LiteralBinder.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.UriParser
{
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
                if (literalToken.ExpectedEdmTypeReference != null)
                {
                    return new ConstantNode(literalToken.Value, literalToken.OriginalText, literalToken.ExpectedEdmTypeReference);
                }

                return new ConstantNode(literalToken.Value, literalToken.OriginalText);
            }

            return new ConstantNode(literalToken.Value);
        }
    }
}