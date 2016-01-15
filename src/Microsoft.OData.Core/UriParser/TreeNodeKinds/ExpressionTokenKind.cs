//---------------------------------------------------------------------
// <copyright file="ExpressionTokenKind.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Core.UriParser.TreeNodeKinds
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

        /// <summary>Duration literal.</summary>
        DurationLiteral = 12,

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

        /// <summary>Quoted</summary>
        QuotedLiteral = 32,

        /// <summary>Date literal.</summary>
        DateLiteral = 33,

        /// <summary>TimeOfDay literal.</summary>
        TimeOfDayLiteral = 34,

        /// <summary>CustomType Literal.</summary>
        CustomTypeLiteral = 35
    }
}
