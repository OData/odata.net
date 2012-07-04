//   Copyright 2011 Microsoft Corporation
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
    #endregion Namespaces

    /// <summary>
    /// URI builder that constructes a <see cref="Uri"/> from the parsed query tokens.
    /// </summary>
    public class ODataUriBuilder
    {
        /// <summary>
        /// The query token to write to Uri.
        /// </summary>
        private readonly QueryToken queryToken;

        /// <summary>
        /// The string builder to write the query token to.
        /// </summary>
        private readonly StringBuilder builder = new StringBuilder();

        /// <summary>
        /// Create a new Uri builder for the given token.
        /// </summary>
        /// <param name="queryToken">The token to write out as Uri.</param>
        protected ODataUriBuilder(QueryToken queryToken)
        {
            this.queryToken = queryToken;
        }

        /// <summary>
        /// The query token to write to Uri.
        /// </summary>
        protected StringBuilder Builder 
        { 
            get { return this.builder; } 
        }

        /// <summary>
        /// The string builder to write the query token to.
        /// </summary>
        protected QueryToken QueryToken 
        { 
            get { return this.queryToken; } 
        }

        /// <summary>
        /// Create a URI for the given queryDescriptor given the base service URI.
        /// </summary>
        /// <param name="baseUri">The base service URI.</param>
        /// <param name="queryDescriptor">The query descriptor to create the result URI from.</param>
        /// <returns>An absolute URI that base on the baseUri and represent the queryDescriptor.</returns>
        public static Uri CreateUri(Uri baseUri, QueryDescriptorQueryToken queryDescriptor)
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
        public virtual void WriteQueryDescriptor(QueryDescriptorQueryToken queryDescriptor)
        {
            ExceptionUtils.CheckArgumentNotNull(queryDescriptor, "queryDescriptor");

            this.WriteQuery(queryDescriptor.Path);

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

            if (queryDescriptor.Expand != null && queryDescriptor.Expand.Properties.Count() > 0)
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

            foreach (QueryOptionQueryToken queryOption in queryDescriptor.QueryOptions)
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
            this.builder.Append(text);
        }

        /// <summary>
        /// Write the query token as URI part to this builder.
        /// </summary>
        /// <param name="query">To write as URI part.</param>
        protected internal virtual void WriteQuery(QueryToken query)
        {
            ExceptionUtils.CheckArgumentNotNull(query, "query");

            switch (query.Kind)
            {
                case QueryTokenKind.BinaryOperator:
                    this.WriteBinary((BinaryOperatorQueryToken)query);
                    break;

                case QueryTokenKind.FunctionCall:
                    this.WriteFunctionCall((FunctionCallQueryToken)query);
                    break;

                case QueryTokenKind.Literal:
                    this.WriteLiteral((LiteralQueryToken)query);
                    break;

                case QueryTokenKind.PropertyAccess:
                    this.WritePropertyAccess((PropertyAccessQueryToken)query);
                    break;

                case QueryTokenKind.Star:
                    this.WriteStar((StarQueryToken)query);
                    break;

                case QueryTokenKind.UnaryOperator:
                    this.WriteUnary((UnaryOperatorQueryToken)query);
                    break;

                case QueryTokenKind.QueryDescriptor:
                    this.WriteQueryDescriptor((QueryDescriptorQueryToken)query);
                    break;

                case QueryTokenKind.KeywordSegment:
                case QueryTokenKind.Segment:
                    this.WriteSegment((SegmentQueryToken)query);
                    break;

                case QueryTokenKind.OrderBy:
                    this.WriteOrderBy((OrderByQueryToken)query);
                    break;

                case QueryTokenKind.QueryOption:
                    this.WriteQueryOption((QueryOptionQueryToken)query);
                    break;

                case QueryTokenKind.Select:
                    this.WriteSelect((SelectQueryToken)query);
                    break;

                case QueryTokenKind.Extension:
                default:
                    ODataUriBuilderUtils.NotSupported(query.Kind);
                    break;
            }
        }

        /// <summary>
        /// Build the queryToken as Uri string part.
        /// </summary>
        /// <returns>The Uri part representing the queryToken.</returns>
        protected virtual string Build()
        {
            this.WriteQuery(this.queryToken);
            return this.builder.ToString();
        }

        /// <summary>
        /// Write the binary token as URI part to this builder.
        /// </summary>
        /// <param name="binary">To write as URI part.</param>
        protected virtual void WriteBinary(BinaryOperatorQueryToken binary)
        {
            ExceptionUtils.CheckArgumentNotNull(binary, "binary");

            BinaryOperatorUriBuilder writer = new BinaryOperatorUriBuilder(this);
            writer.Write(binary);
        }

        /// <summary>
        /// Write the function call token as URI part to this builder.
        /// </summary>
        /// <param name="functionQueryToken">To write as URI part.</param>
        protected virtual void WriteFunctionCall(FunctionCallQueryToken functionQueryToken)
        {
            ExceptionUtils.CheckArgumentNotNull(functionQueryToken, "functionQueryToken");

            this.builder.Append(functionQueryToken.Name);
            this.builder.Append(ExpressionConstants.SymbolOpenParen);

            bool needComma = false;
            foreach (QueryToken parameter in functionQueryToken.Arguments)
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
        /// Write the literal token as URI part to this builder.
        /// </summary>
        /// <param name="literal">To write as URI part.</param>
        protected virtual void WriteLiteral(LiteralQueryToken literal)
        {
            ExceptionUtils.CheckArgumentNotNull(literal, "literal");

            WriteClrLiteral(this.builder, literal.Value);
        }

        /// <summary>
        /// Write the orderby tokens as URI part to this builder.
        /// </summary>
        /// <param name="orderBys">To write as URI part.</param>
        protected virtual void WriteOrderBys(IEnumerable<OrderByQueryToken> orderBys)
        {
            ExceptionUtils.CheckArgumentNotNull(orderBys, "orderBys");

            bool needComma = false;
            foreach (OrderByQueryToken orderBy in orderBys)
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
        protected virtual void WriteOrderBy(OrderByQueryToken orderBy)
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
        /// Write the descriptor path token as URI part to this builder.
        /// </summary>
        /// <param name="segment">To write as URI part.</param>
        protected virtual void WriteSegment(SegmentQueryToken segment)
        {
            ExceptionUtils.CheckArgumentNotNull(segment, "segment");

            // If this desriptor have no path, it is a service-document Url, so just quit
            if (string.IsNullOrEmpty(segment.Name))
            {
                return;
            }

            if (segment.Parent != null)
            {
                this.WriteSegment(segment.Parent);
                this.builder.Append(ExpressionConstants.SymbolForwardSlash);
            }

            this.builder.Append(segment.Name);

            if (segment.NamedValues != null)
            {
                this.builder.Append(ExpressionConstants.SymbolOpenParen);

                bool needComma = false;
                foreach (NamedValue nv in segment.NamedValues)
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

        /// <summary>
        /// Write the propert access token as URI part to this builder.
        /// </summary>
        /// <param name="propertyAccess">To write as URI part.</param>
        protected virtual void WritePropertyAccess(PropertyAccessQueryToken propertyAccess)
        {
            ExceptionUtils.CheckArgumentNotNull(propertyAccess, "propertyAccess");

            if (propertyAccess.Parent != null)
            {
                this.WriteQuery(propertyAccess.Parent);
                this.builder.Append(ExpressionConstants.SymbolForwardSlash);
            }

            this.builder.Append(propertyAccess.Name);
        }

        /// <summary>
        /// Write the given queryOption as Uri part.
        /// </summary>
        /// <param name="queryOption">To write as URI part.</param>
        protected virtual void WriteQueryOption(QueryOptionQueryToken queryOption)
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
        protected virtual void WriteQueryPrefixOrSeparator(bool writeQueryPrefix)
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
        /// <param name="selectQueryToken">To write as URI part.</param>
        protected virtual void WriteSelect(SelectQueryToken selectQueryToken)
        {
            ExceptionUtils.CheckArgumentNotNull(selectQueryToken, "selectQueryToken");

            this.builder.Append(UriQueryConstants.SelectQueryOption);
            this.builder.Append(ExpressionConstants.SymbolEqual);

            bool needComma = false;
            foreach (QueryToken property in selectQueryToken.Properties)
            {
                if (needComma)
                {
                    this.builder.Append(ExpressionConstants.SymbolComma);
                }

                this.WriteQuery(property);
                needComma = true;
            }
        }

        /// <summary>
        /// Write the expand token as URI part to this builder.
        /// </summary>
        /// <param name="expand">To write as URI part.</param>
        protected virtual void WriteExpand(ExpandQueryToken expand)
        {
            ExceptionUtils.CheckArgumentNotNull(expand, "expandQueryToken");

            this.builder.Append(UriQueryConstants.ExpandQueryOption);
            this.builder.Append(ExpressionConstants.SymbolEqual);

            bool needComma = false;
            foreach (QueryToken property in expand.Properties)
            {
                if (needComma)
                {
                    this.builder.Append(ExpressionConstants.SymbolComma);
                }

                this.WriteQuery(property);
                needComma = true;
            }
        }

        /// <summary>
        /// Write the star token as URI part to this builder.
        /// </summary>
        /// <param name="star">To write as URI part.</param>
        protected virtual void WriteStar(StarQueryToken star)
        {
            ExceptionUtils.CheckArgumentNotNull(star, "star");

            if (star.Parent != null)
            {
                this.WriteQuery(star.Parent);
                this.builder.Append(ExpressionConstants.SymbolForwardSlash);
            }

            this.builder.Append(UriQueryConstants.Star);
        }

        /// <summary>
        /// Write the unary token as URI part to this builder.
        /// </summary>
        /// <param name="unary">To write as URI part.</param>
        protected virtual void WriteUnary(UnaryOperatorQueryToken unary)
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
                    throw new ODataException(Strings.General_InternalError(InternalErrorCodes.ODataUriBuilder_WriteUnary_UnreachableCodePath));
            }

            this.WriteQuery(unary.Operand);
        }

        /// <summary>
        /// Write the Uri string representation of the given CLR object literal to the given builder.
        /// </summary>
        /// <param name="builder">The <see cref="StringBuilder"/> to write the <paramref name="clrLiteral"/> to.</param>
        /// <param name="clrLiteral">The object to write as literal.</param>
        private static void WriteClrLiteral(StringBuilder builder, object clrLiteral)
        {
            if (clrLiteral == null)
            {
                builder.Append(ExpressionConstants.KeywordNull);
                return;
            }

            TypeCode code = Type.GetTypeCode(clrLiteral.GetType());
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
    }
}
