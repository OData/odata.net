//---------------------------------------------------------------------
// <copyright file="LiteralBinder.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.UriParser
{
    using System.Diagnostics;

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
        internal static QueryNode BindLiteral(LiteralToken literalToken)
        {
            ExceptionUtils.CheckArgumentNotNull(literalToken, "literalToken");

            if (!string.IsNullOrEmpty(literalToken.OriginalText))
            {
                if (literalToken.ExpectedEdmTypeReference != null)
                {
                    OData.Edm.IEdmCollectionTypeReference collectionReference =
                        literalToken.ExpectedEdmTypeReference as OData.Edm.IEdmCollectionTypeReference;
                    if (collectionReference != null && literalToken.OriginalText[0] == '(')
                    {
                        // CollectionConstantNode currently supports only parenthesis-based literals.
                        // See https://github.com/OData/odata.net/issues/1164. When we are ready to move bracket-based
                        // literals to CollectionConstantNode, simply remove the '(' check in the if-statement.
                        ODataCollectionValue collectionValue = literalToken.Value as ODataCollectionValue;
                        if (collectionValue != null)
                        {
                            return new CollectionConstantNode(collectionValue.Items, literalToken.OriginalText, collectionReference);
                        }

                        Debug.Assert(collectionValue != null,
                            "Literal token creates ODataCollectionValue. If this is not the case anymore, update the code path.");
                    }

                    return new ConstantNode(literalToken.Value, literalToken.OriginalText, literalToken.ExpectedEdmTypeReference);
                }

                return new ConstantNode(literalToken.Value, literalToken.OriginalText);
            }

            return new ConstantNode(literalToken.Value);
        }
    }
}