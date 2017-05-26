//---------------------------------------------------------------------
// <copyright file="LexerToken.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Spatial
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
