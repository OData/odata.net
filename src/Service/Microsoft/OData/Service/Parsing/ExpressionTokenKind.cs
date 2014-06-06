//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Service.Parsing
{
    /// <summary>Enumeration values for token kinds.</summary>
    internal enum ExpressionTokenKind
    {
        /// <summary>Unknown.</summary>
        Unknown,

        /// <summary>End of text.</summary>
        End,

        /// <summary>'=' - equality character.</summary>
        Equal,

        /// <summary>Identifier.</summary>
        Identifier,

        /// <summary>NullLiteral.</summary>
        NullLiteral,

        /// <summary>BooleanLiteral.</summary>
        BooleanLiteral,

        /// <summary>StringLiteral.</summary>
        StringLiteral,

        /// <summary>IntegerLiteral.</summary>
        IntegerLiteral,

        /// <summary>Int64 literal.</summary>
        Int64Literal,

        /// <summary>Single literal.</summary>
        SingleLiteral,

        /// <summary>DateTime literal.</summary>
        DateTimeLiteral,

        /// <summary>Decimal literal.</summary>
        DecimalLiteral,

        /// <summary>Double literal.</summary>
        DoubleLiteral,

        /// <summary>GUID literal.</summary>
        GuidLiteral,

        /// <summary>Binary literal.</summary>
        BinaryLiteral,

        /// <summary>DateTimeOffset literal.</summary>
        DateTimeOffsetLiteral,

        /// <summary>Duration literal.</summary>
        DurationLiteral,

        /// <summary>Exclamation.</summary>
        Exclamation,

        /// <summary>OpenParen.</summary>
        OpenParen,

        /// <summary>CloseParen.</summary>
        CloseParen,

        /// <summary>Comma.</summary>
        Comma,

        /// <summary>Minus.</summary>
        Minus,

        /// <summary>Slash.</summary>
        Slash,

        /// <summary>Question.</summary>
        Question,

        /// <summary>Dot.</summary>
        Dot,

        /// <summary>Star.</summary>
        Star,

        /// <summary>Colon.</summary>
        Colon,

        /// <summary>Semicolon</summary>
        Semicolon,

        /// <summary>Spatial Literal</summary>
        GeographylLiteral,

        /// <summary>Geometry Literal</summary>
        GeometryLiteral,

        /// <summary>Whitespace</summary>
        WhiteSpace
    }
}
