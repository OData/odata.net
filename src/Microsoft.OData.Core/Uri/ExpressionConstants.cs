//---------------------------------------------------------------------
// <copyright file="ExpressionConstants.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData
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

        /// <summary>'.' constant to represent the value of a dot separator.</summary>
        internal const string SymbolDot = ".";

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

        /// <summary>"has" keyword for expressions.</summary>
        internal const string KeywordHas = "has";

        /// <summary>"null" keyword for expressions.</summary>
        internal const string KeywordNull = "null";

        /// <summary>"true" keyword for expressions.</summary>
        internal const string KeywordTrue = "true";

        /// <summary>"max" keyword for expressions.</summary>
        internal const string KeywordMax = "max";

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

        /// <summary>'duration' constant prefixed to duration literals.</summary>
        internal const string LiteralPrefixDuration = "duration";

        /// <summary>'geometry' constant prefixed to geometry literals.</summary>
        internal const string LiteralPrefixGeometry = "geometry";

        /// <summary>'geography' constant prefixed to geography literals.</summary>
        internal const string LiteralPrefixGeography = "geography";

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

        /// <summary> the count query option </summary>
        internal const string QueryOptionCount = "$count";

        /// <summary> the levels query option </summary>
        internal const string QueryOptionLevels = "$levels";

        /// <summary> the search query option</summary>
        internal const string QueryOptionSearch = "$search";

        /// <summary> the select query option </summary>
        internal const string QueryOptionSelect = "$select";

        /// <summary> the expand query option </summary>
        internal const string QueryOptionExpand = "$expand";

        /// <summary>"AND" keyword for search option.</summary>
        internal const string SearchKeywordAnd = "AND";

        /// <summary>"NOT" keyword for search option.</summary>
        internal const string SearchKeywordNot = "NOT";

        /// <summary>"OR" keyword for search option.</summary>
        internal const string SearchKeywordOr = "OR";

        /// <summary>"any" keyword for expressions.</summary>
        internal const string KeywordAny = "any";

        /// <summary>"all" keyword for expressions.</summary>
        internal const string KeywordAll = "all";

        /// <summary>the apply query option.</summary>
        internal const string QueryOptionApply = "$apply";

        /// <summary>"aggregate" keyword for $apply.</summary>
        internal const string KeywordAggregate = "aggregate";

        /// <summary>"filter" keyword for $apply.</summary>
        internal const string KeywordFilter = "filter";

        /// <summary>"groupby" keyword for $apply.</summary>
        internal const string KeywordGroupBy = "groupby";

        /// <summary>"compute" keyword for $apply.</summary>
        internal const string KeywordCompute = "compute";

        /// <summary>"sum" keyword for expressions.</summary>
        internal const string KeywordSum = "sum";

        /// <summary>"min" keyword for expressions.</summary>
        internal const string KeywordMin = "min";

        /// <summary>"average" keyword for expressions.</summary>
        internal const string KeywordAverage = "average";

        /// <summary>"countdistinct" keyword for expressions.</summary>
        internal const string KeywordCountDistinct = "countdistinct";

        /// <summary>"as" keyword for alias expressions.</summary>
        internal const string KeywordAs = "as";

        /// <summary>"with" keyword for aggregate verb expressions.</summary>
        internal const string KeywordWith = "with";

        /// <summary>the compute query option.</summary>
        internal const string QueryOptionCompute = "$compute";
    }
}
