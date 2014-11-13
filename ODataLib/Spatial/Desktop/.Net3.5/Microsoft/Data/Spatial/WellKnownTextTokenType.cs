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

namespace Microsoft.Data.Spatial
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
