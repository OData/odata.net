//---------------------------------------------------------------------
// <copyright file="IFunctionCallParser.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.UriParser
{
    /// <summary>
    /// Interface for a class that can parse an identifier as a function and return a representative QueryToken.
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
        /// <param name="result">QueryToken representing this function.</param>
        /// <returns>True if the parsing was successful.</returns>
        bool TryParseIdentifierAsFunction(QueryToken parent, out QueryToken result);
    }
}