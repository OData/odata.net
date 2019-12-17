//---------------------------------------------------------------------
// <copyright file="WellKnownTextTokenType.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Spatial
{
    /// <summary>
    /// WellKnownText Lexer Token Type
    /// </summary>
    internal enum WellKnownTextTokenType : int
    {
        /// <summary>
        /// A-Z only support upper case text. i.e., POINT() instead of Point() or point()
        /// </summary>
        Text = 0,

        /// <summary>
        /// character '='
        /// </summary>
        Equals,

        /// <summary>
        /// characters '0' to '9'
        /// </summary>
        Number,

        /// <summary>
        /// character ';'
        /// </summary>
        Semicolon,

        /// <summary>
        /// character '('
        /// </summary>
        LeftParen,

        /// <summary>
        /// character ')'
        /// </summary>
        RightParen,

        /// <summary>
        /// character '.'
        /// </summary>
        Period,

        /// <summary>
        /// character ','
        /// </summary>
        Comma,

        /// <summary>
        /// character ' ', '\t'
        /// </summary>
        WhiteSpace
    }
}
