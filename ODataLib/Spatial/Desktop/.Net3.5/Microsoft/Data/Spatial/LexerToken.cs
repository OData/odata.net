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
