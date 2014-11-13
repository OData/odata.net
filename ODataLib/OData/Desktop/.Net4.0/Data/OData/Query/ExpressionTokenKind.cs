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

namespace Microsoft.Data.OData.Query
{
    /// <summary>Enumeration values for token kinds.</summary>
    internal enum ExpressionTokenKind
    {
        /// <summary>Unknown.</summary>
        Unknown = 0,

        /// <summary>End of text.</summary>
        End = 1,

        /// <summary>'=' - equality character.</summary>
        Equal = 2,

        /// <summary>Identifier.</summary>
        Identifier = 3,

        /// <summary>NullLiteral.</summary>
        NullLiteral = 4,

        /// <summary>BooleanLiteral.</summary>
        BooleanLiteral = 5,

        /// <summary>StringLiteral.</summary>
        StringLiteral = 6,

        /// <summary>IntegerLiteral.</summary>
        IntegerLiteral = 7,

        /// <summary>Int64 literal.</summary>
        Int64Literal = 8,

        /// <summary>Single literal.</summary>
        SingleLiteral = 9,

        /// <summary>DateTime literal.</summary>
        DateTimeLiteral = 10,

        /// <summary>DateTimeOffset literal.</summary>
        DateTimeOffsetLiteral = 11,

        /// <summary>Time literal.</summary>
        TimeLiteral = 12,

        /// <summary>Decimal literal.</summary>
        DecimalLiteral = 13,

        /// <summary>Double literal.</summary>
        DoubleLiteral = 14,

        /// <summary>GUID literal.</summary>
        GuidLiteral = 15,

        /// <summary>Binary literal.</summary>
        BinaryLiteral = 16,

        /// <summary>Geography literal.</summary>
        GeographyLiteral = 17,

        /// <summary>Geometry literal.</summary>
        GeometryLiteral = 18,

        /// <summary>Exclamation.</summary>
        Exclamation = 19,

        /// <summary>OpenParen.</summary>
        OpenParen = 20,

        /// <summary>CloseParen.</summary>
        CloseParen = 21,

        /// <summary>Comma.</summary>
        Comma = 22,
        
        /// <summary>Colon.</summary>
        Colon = 23,

        /// <summary>Minus.</summary>
        Minus = 24,

        /// <summary>Slash.</summary>
       Slash = 25,

        /// <summary>Question.</summary>
        Question = 26,

        /// <summary>Dot.</summary>
        Dot = 27,

        /// <summary>Star.</summary>
        Star = 28,

        /// <summary>SemiColon</summary>
        SemiColon = 29,

        /// <summary>ParameterAlias</summary>
        ParameterAlias = 30,

        /// <summary>A Brace BracketedExpression is an expression within brackets or braces. It contains a JSON object or array.</summary>
        BracketedExpression = 31,
    }
}
