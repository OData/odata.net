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

namespace Microsoft.Data.Spatial
{
    using System;

    /// <summary>
    /// Text Lexer Token
    /// </summary>
    internal class LexerToken
    {
        /// <summary>
        /// The Token Text
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Token Type
        /// </summary>
        public int Type { get; set; }

        /// <summary>
        /// Test whether this token matches the input criterion
        /// </summary>
        /// <param name="targetType">The target type</param>
        /// <param name="targetText">The target text, or null</param>
        /// <param name="comparison">The StringComparison</param>
        /// <returns>True if this token matches the input criterion</returns>
        public bool MatchToken(int targetType, String targetText, StringComparison comparison)
        {
            return (this.Type == targetType) && (String.IsNullOrEmpty(targetText) || this.Text.Equals(targetText, comparison));
        }

        /// <summary>
        /// String representation of this token
        /// </summary>
        /// <returns>String representation of this token</returns>
        public override string ToString()
        {
            return "Type:[" + this.Type + "] Text:[" + this.Text + "]";
        }
    }
}
