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
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Linq;
    using System.Spatial;
    using System.Text;
    using System.Xml;
    using Microsoft.Data.OData.Query.SyntacticAst;
    using ODataErrorStrings = Microsoft.Data.OData.Strings;
    #endregion Namespaces

    /// <summary>
    /// URI builder that constructes a <see cref="Uri"/> from the parsed query tokens.
    /// </summary>
    internal sealed class ODataUriBuilder
    {
        /// <summary>
        /// The query token to write to Uri.
        /// </summary>
        private readonly SyntacticTree query;

        /// <summary>
        /// The string builder to write the query token to.
        /// </summary>
        private readonly StringBuilder builder = new StringBuilder();

        /// <summary>
        /// Create a new Uri builder for the given token.
        /// </summary>
        /// <param name="query">The token to write out as Uri.</param>
        private ODataUriBuilder(SyntacticTree query)
        {
            this.query = query;
        }

        /// <summary>
        /// Create a URI for the given queryDescriptor given the base service URI.
        /// </summary>
        /// <param name="baseUri">The base service URI.</param>
        /// <param name="queryDescriptor">The query descriptor to create the result URI from.</param>
        /// <returns>An absolute URI that base on the baseUri and represent the queryDescriptor.</returns>
        public static Uri CreateUri(Uri baseUri, SyntacticTree queryDescriptor)
        {
            ExceptionUtils.CheckArgumentNotNull(baseUri, "baseUri");
            ExceptionUtils.CheckArgumentNotNull(queryDescriptor, "queryDescriptor");

            ODataUriBuilder odataUriBuilder = new ODataUriBuilder(queryDescriptor);
            string uriPart = odataUriBuilder.Build();
            if (uriPart.StartsWith(ExpressionConstants.SymbolQueryStart, StringComparison.Ordinal))
            {
                UriBuilder uriBuilder = new UriBuilder(baseUri);
                uriBuilder.Query = uriPart;
                return uriBuilder.Uri;
            }

            return new Uri(baseUri, new Uri(uriPart, UriKind.RelativeOrAbsolute));
        }

        /// <summary>
        /// Get the Uri string representation of the given CLR object literal.
        /// </summary>
        /// <param name="clrLiteral">The object to return as literal.</param>
        /// <returns>Uri string represent if is a CLR literal.  Throw exception if not.</returns>
        [SuppressMessage("Microsoft.Design", "CA1055:UriReturnValuesShouldNotBeStrings", Justification = "Method is intended to return the Uri literal form as string.")]
        public static string GetUriRepresentation(object clrLiteral)
        {
            StringBuilder builder = new StringBuilder();
            WriteClrLiteral(builder, clrLiteral);
            return builder.ToString();
        }

        /// <summary>
        /// Write the descriptor token as URI part to this builder.
        /// </summary>
        /// <param name="queryDescriptor">To write as URI part.</param>
        public void WriteQueryDescriptor(SyntacticTree queryDescriptor)
        {
            ExceptionUtils.CheckArgumentNotNull(queryDescriptor, "queryDescriptor");

            this.WritePath(queryDescriptor.Path);

            bool writeQueryPrefix = true;
            if (queryDescriptor.Filter != null)
            {
                this.WriteQueryPrefixOrSeparator(writeQueryPrefix);
                writeQueryPrefix = false;

                this.builder.Append(UriQueryConstants.FilterQueryOption);
                this.builder.Append(ExpressionConstants.SymbolEqual);
                this.WriteQuery(queryDescriptor.Filter);
            }

            if (queryDescriptor.Select != null && queryDescriptor.Select.Properties.Count() > 0)
            {
                this.WriteQueryPrefixOrSeparator(writeQueryPrefix);
                writeQueryPrefix = false;

                this.WriteSelect(queryDescriptor.Select);
            }

            if (queryDescriptor.Expand != null && queryDescriptor.Expand.ExpandTerms.Count() > 0)
            {
                this.WriteQueryPrefixOrSeparator(writeQueryPrefix);
                writeQueryPrefix = false;

                this.WriteExpand(queryDescriptor.Expand);
            }

            if (queryDescriptor.OrderByTokens.Count() > 0)
            {
                this.WriteQueryPrefixOrSeparator(writeQueryPrefix);
                writeQueryPrefix = false;

                this.builder.Append(UriQueryConstants.OrderByQueryOption);
                this.builder.Append(ExpressionConstants.SymbolEqual);

                this.WriteOrderBys(queryDescriptor.OrderByTokens);
            }

            foreach (CustomQueryOptionToken queryOption in queryDescriptor.QueryOptions)
            {
                this.WriteQueryPrefixOrSeparator(writeQueryPrefix);
                writeQueryPrefix = false;

                this.WriteQueryOption(queryOption);
            }

            if (queryDescriptor.Top != null)
            {
                this.WriteQueryPrefixOrSeparator(writeQueryPrefix);
                writeQueryPrefix = false;

                this.builder.Append(UriQueryConstants.TopQueryOption);
                this.builder.Append(ExpressionConstants.SymbolEqual);
                this.builder.Append(queryDescriptor.Top);
            }

            if (queryDescriptor.Skip != null)
            {
                this.WriteQueryPrefixOrSeparator(writeQueryPrefix);
                writeQueryPrefix = false;

                this.builder.Append(UriQueryConstants.SkipQueryOption);
                this.builder.Append(ExpressionConstants.SymbolEqual);
                this.builder.Append(queryDescriptor.Skip);
            }

            if (queryDescriptor.Format != null)
            {
                this.WriteQueryPrefixOrSeparator(writeQueryPrefix);
                writeQueryPrefix = false;

                this.builder.Append(UriQueryConstants.FormatQueryOption);
                this.builder.Append(ExpressionConstants.SymbolEqual);
                this.builder.Append(queryDescriptor.Format);
            }

            if (queryDescriptor.InlineCount.HasValue)
            {
                this.WriteQueryPrefixOrSeparator(writeQueryPrefix);
                writeQueryPrefix = false;

                this.builder.Append(UriQueryConstants.InlineCountQueryOption);
                this.builder.Append(ExpressionConstants.SymbolEqual);
                this.builder.Append(queryDescriptor.InlineCount.Value.ToText());
            }
        }

        /// <summary>
        /// Append the given text to this builder.
        /// </summary>
        /// <param name="text">The text to append.</param>
        internal void Append(string text)
        {
            DebugUtils.CheckNoExternalCallers(); 
            this.builder.Append(text);
        }

        /// <summary>
        /// Write the query token as URI part to this builder.
        /// </summary>
        /// <param name="queryPart">To write as URI part.</param>
        internal void WriteQuery(QueryToken queryPart)
        {
            DebugUtils.CheckNoExternalCallers(); 
            ExceptionUtils.CheckArgumentNotNull(queryPart, "query");

            switch (queryPart.Kind)
            {
                case QueryTokenKind.BinaryOperator:
                    this.WriteBinary((BinaryOperatorToken)queryPart);
                    break;

                case QueryTokenKind.FunctionCall:
                    this.WriteFunctionCall((FunctionCallToken)queryPart);
                    break;

                case QueryTokenKind.Literal:
                    this.WriteLiteral((LiteralToken)queryPart);
                    break;

                case QueryTokenKind.EndPath:
                    this.WritePropertyAccess((EndPathToken)queryPart);
                    break;
                case QueryTokenKind.InnerPath:
                    this.WriteNavigationProperty((InnerPathToken)queryPart);
                    break;
                case QueryTokenKind.Star:
                    this.WriteStar((StarToken)queryPart);
                    break;

                case QueryTokenKind.UnaryOperator:
                    this.WriteUnary((UnaryOperatorToken)queryPart);
                    break;

                case QueryTokenKind.OrderBy:
                    this.WriteOrderBy((OrderByToken)queryPart);
                    break;

                case QueryTokenKind.CustomQueryOption:
                    this.WriteQueryOption((CustomQueryOptionToken)queryPart);
                    break;

                case QueryTokenKind.Select:
                    this.WriteSelect((SelectToken)queryPart);
                    break;

                default:
                    ODataUriBuilderUtils.NotSupportedQueryTokenKind(queryPart.Kind);
                    break;
            }
        }

        /// <summary>
        /// Write the Uri string representation of the given CLR object literal to the given builder.
        /// </summary>
        /// <param name="builder">The <see cref="StringBuilder"/> to write the <paramref name="clrLiteral"/> to.</param>
        /// <param name="clrLiteral">The object to write as literal.</param>
        [SuppressMessage("DataWeb.Usage", "AC0018:SystemUriEscapeDataStringRule", Justification = "Single quote delimiters are not part of the string being escaped.")]
        private static void WriteClrLiteral(StringBuilder builder, object clrLiteral)
        {
            if (clrLiteral == null)
            {
                builder.Append(ExpressionConstants.KeywordNull);
                return;
            }

            TypeCode code = PlatformHelper.GetTypeCode(clrLiteral.GetType());
            switch (code)
            {
                case TypeCode.Byte:
                    builder.Append(((byte)clrLiteral).ToString(ODataUriBuilderUtils.IntegerFormat, CultureInfo.InvariantCulture));
                    return;

                case TypeCode.Boolean:
                    builder.Append((bool)clrLiteral ? ExpressionConstants.KeywordTrue : ExpressionConstants.KeywordFalse);
                    return;

                case TypeCode.DateTime:
                    builder.Append(ExpressionConstants.LiteralPrefixDateTime);
                    builder.Append(ExpressionConstants.SymbolSingleQuote);
                    builder.Append(((DateTime)clrLiteral).ToString(ODataUriBuilderUtils.DateTimeFormat, CultureInfo.InvariantCulture));
                    builder.Append(ExpressionConstants.SymbolSingleQuote);
                    return;

                case TypeCode.Decimal:
                    builder.Append(((Decimal)clrLiteral).ToString(ODataUriBuilderUtils.DecimalFormatInfo));
                    builder.Append(ExpressionConstants.LiteralSuffixDecimal);
                    return;

                case TypeCode.Double:
                    builder.Append(((Double)clrLiteral).ToString(ODataUriBuilderUtils.DoubleFormat, ODataUriBuilderUtils.DoubleFormatInfo));
                    return;

                case TypeCode.Int16:
                    builder.Append(((short)clrLiteral).ToString(ODataUriBuilderUtils.IntegerFormat, CultureInfo.InvariantCulture));
                    return;

                case TypeCode.Int32:
                    builder.Append(((Int32)clrLiteral).ToString(ODataUriBuilderUtils.IntegerFormat, CultureInfo.InvariantCulture));
                    return;

                case TypeCode.Int64:
                    builder.Append(((Int64)clrLiteral).ToString(ODataUriBuilderUtils.IntegerFormat, CultureInfo.InvariantCulture));
                    builder.Append(ExpressionConstants.LiteralSuffixInt64);
                    return;

                case TypeCode.SByte:
                    builder.Append(((SByte)clrLiteral).ToString(ODataUriBuilderUtils.IntegerFormat, CultureInfo.InvariantCulture));
                    return;

                case TypeCode.Single:
                    builder.Append(((float)clrLiteral).ToString(ODataUriBuilderUtils.FloatFormat, CultureInfo.InvariantCulture));
                    builder.Append(ExpressionConstants.LiteralSuffixSingle);
                    return;

                case TypeCode.String:
                    builder.Append(ExpressionConstants.SymbolSingleQuote);
                    builder.Append(Uri.EscapeDataString(ODataUriBuilderUtils.Escape(clrLiteral.ToString())));
                    builder.Append(ExpressionConstants.SymbolSingleQuote);
                    return;

                default:
                    break;
            }

            if (clrLiteral is DateTimeOffset)
            {
                builder.Append(ExpressionConstants.LiteralPrefixDateTimeOffset);
                builder.Append(ExpressionConstants.SymbolSingleQuote);
                builder.Append(((DateTimeOffset)clrLiteral).ToString(ODataUriBuilderUtils.DateTimeOffsetFormat, CultureInfo.InvariantCulture));
                builder.Append(ExpressionConstants.SymbolSingleQuote);
                return;
            }

            if (clrLiteral is TimeSpan)
            {
                builder.Append(ExpressionConstants.LiteralPrefixTime);
                builder.Append(ExpressionConstants.SymbolSingleQuote);
                builder.Append(XmlConvert.ToString((TimeSpan)clrLiteral));
                builder.Append(ExpressionConstants.SymbolSingleQuote);
                return;
            }

            if (clrLiteral is Guid)
            {
                builder.Append(ExpressionConstants.LiteralPrefixGuid);
                builder.Append(ExpressionConstants.SymbolSingleQuote);
                builder.Append(((Guid)clrLiteral).ToString(ODataUriBuilderUtils.IntegerFormat));
                builder.Append(ExpressionConstants.SymbolSingleQuote);
                return;
            }

            byte[] bytes = clrLiteral as byte[];
            if (bytes != null)
            {
                builder.Append(ExpressionConstants.LiteralPrefixBinary);
                builder.Append(ExpressionConstants.SymbolSingleQuote);
                foreach (byte @byte in bytes)
                {
                    builder.Append(@byte.ToString(ODataUriBuilderUtils.BinaryFormat, CultureInfo.InvariantCulture));
                }

                builder.Append(ExpressionConstants.SymbolSingleQuote);
                return;
            }

            Geography geography = clrLiteral as Geography;
            if (geography != null)
            {
                builder.Append(ExpressionConstants.LiteralPrefixGeography);
                builder.Append(ExpressionConstants.SymbolSingleQuote);

                builder.Append(LiteralUtils.ToWellKnownText(geography));

                builder.Append(ExpressionConstants.SymbolSingleQuote);
                return;
            }

            Geometry geometry = clrLiteral as Geometry;
            if (geometry != null)
            {
                builder.Append(ExpressionConstants.LiteralPrefixGeometry);
                builder.Append(ExpressionConstants.SymbolSingleQuote);

                builder.Append(LiteralUtils.ToWellKnownText(geometry));

                builder.Append(ExpressionConstants.SymbolSingleQuote);
                return;
            }

            ODataUriBuilderUtils.NotSupported(clrLiteral.GetType());
        }

        /// <summary>
        /// Build the queryToken as Uri string part.
        /// </summary>
        /// <returns>The Uri part representing the queryToken.</returns>
        private string Build()
        {
            this.WriteQueryDescriptor(this.query);
            return this.builder.ToString();
        }

        /// <summary>
        /// Write the binary token as URI part to this builder.
        /// </summary>
        /// <param name="binary">To write as URI part.</param>
        private void WriteBinary(BinaryOperatorToken binary)
        {
            ExceptionUtils.CheckArgumentNotNull(binary, "binary");

            BinaryOperatorUriBuilder writer = new BinaryOperatorUriBuilder(this);
            writer.Write(binary);
        }

        /// <summary>
        /// Write the function call token as URI part to this builder.
        /// </summary>
        /// <param name="functionToken">To write as URI part.</param>
        private void WriteFunctionCall(FunctionCallToken functionToken)
        {
            ExceptionUtils.CheckArgumentNotNull(functionToken, "functionToken");

            this.builder.Append(functionToken.Name);
            this.builder.Append(ExpressionConstants.SymbolOpenParen);

            bool needComma = false;
            foreach (QueryToken parameter in functionToken.Arguments)
            {
                if (needComma)
                {
                    this.builder.Append(ExpressionConstants.SymbolComma);
                }
                else
                {
                    needComma = true;
                }

                this.WriteQuery(parameter);
            }

            this.builder.Append(ExpressionConstants.SymbolClosedParen);
        }

        /// <summary>
        /// Writes a path to this builder.
        /// </summary>
        /// <param name="path">Array of segments.</param>
        private void WritePath(IEnumerable<string> path)
        {
            bool first = true;
            foreach (var s in path)
            {
                if (!first)
                {
                    this.builder.Append("/");
                }

                this.builder.Append(s);
                first = false;
            }
        }

        /// <summary>
        /// Write the literal token as URI part to this builder.
        /// </summary>
        /// <param name="literal">To write as URI part.</param>
        private void WriteLiteral(LiteralToken literal)
        {
            ExceptionUtils.CheckArgumentNotNull(literal, "literal");

            WriteClrLiteral(this.builder, literal.Value);
        }

        /// <summary>
        /// Write the orderby tokens as URI part to this builder.
        /// </summary>
        /// <param name="orderBys">To write as URI part.</param>
        private void WriteOrderBys(IEnumerable<OrderByToken> orderBys)
        {
            ExceptionUtils.CheckArgumentNotNull(orderBys, "orderBys");

            bool needComma = false;
            foreach (OrderByToken orderBy in orderBys)
            {
                if (needComma)
                {
                    this.builder.Append(ExpressionConstants.SymbolComma);
                }

                this.WriteOrderBy(orderBy);
                needComma = true;
            }
        }

        /// <summary>
        /// Write the orderby token as URI part to this builder.
        /// </summary>
        /// <param name="orderBy">To write as URI part.</param>
        private void WriteOrderBy(OrderByToken orderBy)
        {
            ExceptionUtils.CheckArgumentNotNull(orderBy, "orderBy");

            this.WriteQuery(orderBy.Expression);
            if (orderBy.Direction == OrderByDirection.Descending)
            {
                this.builder.Append(ExpressionConstants.SymbolEscapedSpace);
                this.builder.Append(ExpressionConstants.KeywordDescending);
            }
        }

        /// <summary>
        /// Write out a PathSegmentToken
        /// </summary>
        /// <param name="segmentToken">the pathSegmentToken to write.</param>
        private void WritePathSegment(PathSegmentToken segmentToken)
        {
            NonSystemToken nonSystemToken = segmentToken as NonSystemToken;
            if (nonSystemToken != null)
            {
                // If this desriptor have no path, it is a service-document Url, so just quit
                if (string.IsNullOrEmpty(nonSystemToken.Identifier))
                {
                    return;
                }

                if (nonSystemToken.NextToken != null)
                {
                    this.WritePathSegment(nonSystemToken.NextToken);
                    this.builder.Append(ExpressionConstants.SymbolForwardSlash);
                }

                this.builder.Append(nonSystemToken.Identifier);

                if (nonSystemToken.NamedValues != null)
                {
                    this.builder.Append(ExpressionConstants.SymbolOpenParen);

                    bool needComma = false;
                    foreach (NamedValue nv in nonSystemToken.NamedValues)
                    {
                        if (needComma)
                        {
                            this.builder.Append(ExpressionConstants.SymbolComma);
                        }

                        this.builder.Append(nv.Name);
                        this.builder.Append(ExpressionConstants.SymbolEqual);
                        this.WriteLiteral(nv.Value);
                        needComma = true;
                    }

                    this.builder.Append(ExpressionConstants.SymbolClosedParen);
                }
            }
        }

        /// <summary>
        /// Write the property access token as URI part to this builder.
        /// </summary>
        /// <param name="endPath">To write as URI part.</param>
        private void WritePropertyAccess(EndPathToken endPath)
        {
            ExceptionUtils.CheckArgumentNotNull(endPath, "endPath");

            if (endPath.NextToken != null)
            {
                this.WriteQuery(endPath.NextToken);
                this.builder.Append(ExpressionConstants.SymbolForwardSlash);
            }

            this.builder.Append(endPath.Identifier);
        }

        /// <summary>
        /// Write the navigation property token as URI part to this builder.
        /// </summary>
        /// <param name="navigation">To write as URI part.</param>
        private void WriteNavigationProperty(InnerPathToken navigation)
        {
            ExceptionUtils.CheckArgumentNotNull(navigation, "navigation");

            if (navigation.NextToken != null)
            {
                this.WriteQuery(navigation.NextToken);
                this.builder.Append(ExpressionConstants.SymbolForwardSlash);
            }

            this.builder.Append(navigation.Identifier);
        }

        /// <summary>
        /// Write the given queryOption as Uri part.
        /// </summary>
        /// <param name="queryOption">To write as URI part.</param>
        private void WriteQueryOption(CustomQueryOptionToken queryOption)
        {
            ExceptionUtils.CheckArgumentNotNull(queryOption, "queryOption");

            this.builder.Append(queryOption.Name);
            this.builder.Append(ExpressionConstants.SymbolEqual);
            this.builder.Append(queryOption.Value);
        }

        /// <summary>
        /// Write ? or &amp; depending on whether it is the start of the whole query or query part.
        /// </summary>
        /// <param name="writeQueryPrefix">True if start of whole query, false if not.  
        /// This is set to false after this method is called.</param>
        private void WriteQueryPrefixOrSeparator(bool writeQueryPrefix)
        {
            if (writeQueryPrefix)
            {
                this.builder.Append(ExpressionConstants.SymbolQueryStart);
            }
            else 
            {
                this.builder.Append(ExpressionConstants.SymbolQueryConcatenate);
            }
        }

        /// <summary>
        /// Write the select token as URI part to this builder.
        /// </summary>
        /// <param name="selectToken">To write as URI part.</param>
        private void WriteSelect(SelectToken selectToken)
        {
            ExceptionUtils.CheckArgumentNotNull(selectToken, "SelectToken");

            this.builder.Append(UriQueryConstants.SelectQueryOption);
            this.builder.Append(ExpressionConstants.SymbolEqual);

            bool needComma = false;
            foreach (PathSegmentToken property in selectToken.Properties)
            {
                if (needComma)
                {
                    this.builder.Append(ExpressionConstants.SymbolComma);
                }

                this.WritePathSegment(property);
                needComma = true;
            }
        }

        /// <summary>
        /// Write the expand token as URI part to this builder.
        /// </summary>
        /// <param name="expand">To write as URI part.</param>
        private void WriteExpand(ExpandToken expand)
        {
            ExceptionUtils.CheckArgumentNotNull(expand, "expandQueryToken");

            this.builder.Append(UriQueryConstants.ExpandQueryOption);
            this.builder.Append(ExpressionConstants.SymbolEqual);

            bool needComma = false;
            foreach (ExpandTermToken expandTerm in expand.ExpandTerms)
            {
                if (needComma)
                {
                    this.builder.Append(ExpressionConstants.SymbolComma);
                }

                this.WritePathSegment(expandTerm.PathToNavProp);
                needComma = true;
            }
        }

        /// <summary>
        /// Write the star token as URI part to this builder.
        /// </summary>
        /// <param name="star">To write as URI part.</param>
        private void WriteStar(StarToken star)
        {
            ExceptionUtils.CheckArgumentNotNull(star, "star");

            if (star.NextToken != null)
            {
                this.WriteQuery(star.NextToken);
                this.builder.Append(ExpressionConstants.SymbolForwardSlash);
            }

            this.builder.Append(UriQueryConstants.Star);
        }

        /// <summary>
        /// Write the unary token as URI part to this builder.
        /// </summary>
        /// <param name="unary">To write as URI part.</param>
        private void WriteUnary(UnaryOperatorToken unary)
        {
            ExceptionUtils.CheckArgumentNotNull(unary, "unary");

            switch (unary.OperatorKind)
            {
                case UnaryOperatorKind.Negate:
                    this.builder.Append(ExpressionConstants.SymbolNegate);
                    break;

                case UnaryOperatorKind.Not:
                    this.builder.Append(ExpressionConstants.KeywordNot);
                    this.builder.Append(ExpressionConstants.SymbolEscapedSpace);
                    break;

                default:
                    throw new ODataException(ODataErrorStrings.General_InternalError(InternalErrorCodes.ODataUriBuilder_WriteUnary_UnreachableCodePath));
            }

            this.WriteQuery(unary.Operand);
        }
    }
}
