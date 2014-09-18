//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation. All rights reserved.  
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at

//       http://www.apache.org/licenses/LICENSE-2.0

//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

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
