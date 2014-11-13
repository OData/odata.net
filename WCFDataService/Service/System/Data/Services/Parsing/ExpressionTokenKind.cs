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

namespace System.Data.Services.Parsing
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

        /// <summary>Time literal.</summary>
        TimeLiteral,

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
