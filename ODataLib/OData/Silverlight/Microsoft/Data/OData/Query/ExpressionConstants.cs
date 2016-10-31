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
    /// <summary>This type provides constants used in URI query expressions.</summary>
    internal static class ExpressionConstants
    {
        /// <summary>"$it" keyword for expressions.</summary>
        internal const string It = "$it";

        /// <summary>"add" keyword for expressions.</summary>
        internal const string KeywordAdd = "add";

        /// <summary>"and" keyword for expressions.</summary>
        internal const string KeywordAnd = "and";

        /// <summary>"asc" keyword for expressions.</summary>
        internal const string KeywordAscending = "asc";

        /// <summary>"desc" keyword for expressions.</summary>
        internal const string KeywordDescending = "desc";

        /// <summary>"div" keyword for expressions.</summary>
        internal const string KeywordDivide = "div";

        /// <summary>"mod" keyword for expressions.</summary>
        internal const string KeywordModulo = "mod";

        /// <summary>"mul" keyword for expressions.</summary>
        internal const string KeywordMultiply = "mul";

        /// <summary>"not" keyword for expressions.</summary>
        internal const string KeywordNot = "not";

        /// <summary>"or" keyword for expressions.</summary>
        internal const string KeywordOr = "or";

        /// <summary>"sub" keyword for expressions.</summary>
        internal const string KeywordSub = "sub";

        /// <summary>'-' constant to represent an negate unary operator.</summary>
        internal const string SymbolNegate = "-";

        /// <summary>'=' constant to represent an assignment in name=value.</summary>
        internal const string SymbolEqual = "=";

        /// <summary>',' constant to represent an value list separator.</summary>
        internal const string SymbolComma = ",";

        /// <summary>'/' constant to represent the forward slash used in a query.</summary>
        internal const string SymbolForwardSlash = "/";

        /// <summary>'(' constant to represent an open parenthesis.</summary>
        internal const string SymbolOpenParen = "(";

        /// <summary>')' constant to represent an closed parenthesis.</summary>
        internal const string SymbolClosedParen = ")";

        /// <summary>'?' constant to represent the start of the query part.</summary>
        internal const string SymbolQueryStart = "?";

        /// <summary>'&amp;' constant to represent the concatenation of query parts.</summary>
        internal const string SymbolQueryConcatenate = "&";

        /// <summary>'\'' constant to represent a single quote as prefix/suffix for literals.</summary>
        internal const string SymbolSingleQuote = "'";

        /// <summary>"''" constant to represent a single-quote escape character in a string literal.</summary>
        internal const string SymbolSingleQuoteEscaped = "''";

        /// <summary>" " constant to represent a space character in a Uri query part.</summary>
        internal const string SymbolEscapedSpace = "%20";

        /// <summary>The 'allpages' value for the '$inlinecount' query option</summary>
        internal const string InlineCountAllPages = "allpages";

        /// <summary>The 'none' value for the '$inlinecount' query option</summary>
        internal const string InlineCountNone = "none";

        /// <summary>"eq" keyword for expressions.</summary>
        internal const string KeywordEqual = "eq";

        /// <summary>"false" keyword for expressions.</summary>
        internal const string KeywordFalse = "false";

        /// <summary>"gt" keyword for expressions.</summary>
        internal const string KeywordGreaterThan = "gt";

        /// <summary>"ge" keyword for expressions.</summary>
        internal const string KeywordGreaterThanOrEqual = "ge";

        /// <summary>"lt" keyword for expressions.</summary>
        internal const string KeywordLessThan = "lt";

        /// <summary>"le" keyword for expressions.</summary>
        internal const string KeywordLessThanOrEqual = "le";

        /// <summary>"ne" keyword for expressions.</summary>
        internal const string KeywordNotEqual = "ne";

        /// <summary>"null" keyword for expressions.</summary>
        internal const string KeywordNull = "null";

        /// <summary>"true" keyword for expressions.</summary>
        internal const string KeywordTrue = "true";

        /// <summary> "cast" function </summary>
        internal const string UnboundFunctionCast = "cast";

        /// <summary> "isof function </summary>
        internal const string UnboundFunctionIsOf = "isof";

        /// <summary> Spatial length function </summary>
        internal const string UnboundFunctionLength = "geo.length";

        /// <summary> Spatial intersects function. </summary>
        internal const string UnboundFunctionIntersects = "geo.intersects";

        /// <summary>"INF" literal used to represent infinity.</summary>
        internal const string InfinityLiteral = "INF";

        /// <summary>"NaN" literal used to represent not-a-number values.</summary>
        internal const string NaNLiteral = "NaN";

        /// <summary>'datetime' constant prefixed to datetime literals.</summary>
        internal const string LiteralPrefixDateTime = "datetime";

        /// <summary>'datetimeoffset' constant prefixed to datetimeoffset literals.</summary>
        internal const string LiteralPrefixDateTimeOffset = "datetimeoffset";

        /// <summary>'time' constant prefixed to time literals.</summary>
        internal const string LiteralPrefixTime = "time";

        /// <summary>'geometry' constant prefixed to geometry literals.</summary>
        internal const string LiteralPrefixGeometry = "geometry";

        /// <summary>'geography' constant prefixed to geography literals.</summary>
        internal const string LiteralPrefixGeography = "geography";

        /// <summary>'guid' constant prefixed to guid literals.</summary>
        internal const string LiteralPrefixGuid = "guid";

        /// <summary>'X': Prefix to binary type string representation.</summary>
        internal const string LiteralPrefixShortBinary = "X";

        /// <summary>'binary' constant prefixed to binary literals.</summary>
        internal const string LiteralPrefixBinary = "binary";

        /// <summary>'L': Suffix for long (int64) type's string representation</summary>
        internal const string LiteralSuffixInt64 = "L";

        /// <summary>'f': Suffix for float (single) type's string representation</summary>
        internal const string LiteralSuffixSingle = "f";

        /// <summary>'D': Suffix for double (Real) type's string representation</summary>
        internal const string LiteralSuffixDouble = "D";

        /// <summary>'M': Suffix for decimal type's string representation</summary>
        internal const string LiteralSuffixDecimal = "M";

        /// <summary>'datetime' constant prefixed to datetime literals.</summary>
        internal const string LiteralSingleQuote = "'";

        /// <summary> the filter query option </summary>
        internal const string QueryOptionFilter = "$filter";

        /// <summary> the orderby query option </summary>
        internal const string QueryOptionOrderby = "$orderby";

        /// <summary> the top query option </summary>
        internal const string QueryOptionTop = "$top";

        /// <summary> the skip query option </summary>
        internal const string QueryOptionSkip = "$skip";

        /// <summary> the inlinecount query option </summary>
        internal const string QueryOptionInlineCount = "$inlinecount";

        /// <summary> the select query option </summary>
        internal const string QueryOptionSelect = "$select";

        /// <summary> the expand query option </summary>
        internal const string QueryOptionExpand = "$expand";
    }
}
