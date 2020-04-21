//---------------------------------------------------------------------
// <copyright file="WellKnownTextSqlReader.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Spatial
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.IO;
    using System.Text;
    using System.Xml;

    /// <summary>
    /// Reader for Extended Well Known Text, Case sensitive
    /// example:
    /// SRID=1234;POINT(10.0 20.0 NULL 30.0)
    /// </summary>
    internal class WellKnownTextSqlReader : SpatialReader<TextReader>
    {
        /// <summary>
        /// restricts the reader to allow only two dimensions.
        /// </summary>
        private bool allowOnlyTwoDimensions;

        /// <summary>
        /// Creates a reader that that will send messages to the destination during read.
        /// </summary>
        /// <param name="destination">The instance to message to during read.</param>
        public WellKnownTextSqlReader(SpatialPipeline destination)
            : this(destination, false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WellKnownTextSqlReader"/> class.
        /// </summary>
        /// <param name="destination">The destination.</param>
        /// <param name="allowOnlyTwoDimensions">if set to <c>true</c> allows only two dimensions.</param>
        public WellKnownTextSqlReader(SpatialPipeline destination, bool allowOnlyTwoDimensions)
            : base(destination)
        {
            this.allowOnlyTwoDimensions = allowOnlyTwoDimensions;
        }

        /// <summary>
        /// Parses some serialized format that represents a geography value, passing the result down the pipeline.
        /// </summary>
        /// <param name = "input">TextReader instance to read from.</param>
        protected override void ReadGeographyImplementation(TextReader input)
        {
            // Geography in WKT has lat/long reversed, should be y (long), x (lat), z, m
            new Parser(input, new TypeWashedToGeographyLongLatPipeline(this.Destination), allowOnlyTwoDimensions).Read();
        }

        /// <summary>
        /// Parses some serialized format that represents a geometry value, passing the result down the pipeline.
        /// </summary>
        /// <param name = "input">TextReader instance to read from.</param>
        protected override void ReadGeometryImplementation(TextReader input)
        {
            new Parser(input, new TypeWashedToGeometryPipeline(this.Destination), allowOnlyTwoDimensions).Read();
        }

        /// <summary>
        /// This class parses the text and calls the pipeline based on what is parsed
        /// </summary>
        private class Parser
        {
            /// <summary>
            /// restricts the parser to allow only two dimensions.
            /// </summary>
            private readonly bool allowOnlyTwoDimensions;

            /// <summary>
            /// Text lexer
            /// </summary>
            private readonly TextLexerBase lexer;

            /// <summary>
            /// Output pipeline
            /// </summary>
            private readonly TypeWashedPipeline pipeline;

            /// <summary>
            /// Creates a parser with the given reader and pipeline
            /// </summary>
            /// <param name="reader">The reader that is the source of what is parsed.</param>
            /// <param name="pipeline">The pipeline to be called as the parser recognizes tokens.</param>
            /// <param name="allowOnlyTwoDimensions">if set to <c>true</c> allows only two dimensions.</param>
            public Parser(TextReader reader, TypeWashedPipeline pipeline, bool allowOnlyTwoDimensions)
            {
                this.lexer = new WellKnownTextLexer(reader);
                this.pipeline = pipeline;
                this.allowOnlyTwoDimensions = allowOnlyTwoDimensions;
            }

            /// <summary>
            /// Read WellKnownText into an instance of Geography
            /// </summary>
            public void Read()
            {
                this.ParseSRID();
                this.ParseTaggedText();
            }

            /// <summary>
            /// Test whether the current token matches the expected token
            /// </summary>
            /// <param name="type">The expected token type</param>
            /// <param name="text">The expected token text</param>
            /// <returns>True if the two tokens match</returns>
            private bool IsTokenMatch(WellKnownTextTokenType type, String text)
            {
                return this.lexer.CurrentToken.MatchToken((int)type, text, StringComparison.OrdinalIgnoreCase);
            }

            /// <summary>
            /// Move the lexer to the next non-whitespace token
            /// </summary>
            /// <returns>True if the lexer gets a new token</returns>
            private bool NextToken()
            {
                while (this.lexer.Next())
                {
                    if (!this.lexer.CurrentToken.MatchToken((int)WellKnownTextTokenType.WhiteSpace, String.Empty, StringComparison.Ordinal))
                    {
                        return true;
                    }
                }

                return false;
            }

            /// <summary>
            /// Parse Collection Text
            /// </summary>
            private void ParseCollectionText()
            {
                // <geometrycollection text> ::= <empty set> | <left paren> <geometry tagged text> {<comma> <geometry tagged text>}* <right paren>
                if (!this.ReadEmptySet())
                {
                    this.ReadToken(WellKnownTextTokenType.LeftParen, null);
                    this.ParseTaggedText();
                    while (this.ReadOptionalToken(WellKnownTextTokenType.Comma, null))
                    {
                        this.ParseTaggedText();
                    }

                    this.ReadToken(WellKnownTextTokenType.RightParen, null);
                }
            }

            /// <summary>
            /// Parse a LineString text
            /// </summary>
            private void ParseLineStringText()
            {
                // <linestring text> ::= <empty set> | <left paren> <point> {<comma> <point>}* <right paren>
                if (!this.ReadEmptySet())
                {
                    this.ReadToken(WellKnownTextTokenType.LeftParen, null);
                    this.ParsePoint(true);
                    while (this.ReadOptionalToken(WellKnownTextTokenType.Comma, null))
                    {
                        this.ParsePoint(false);
                    }

                    this.ReadToken(WellKnownTextTokenType.RightParen, null);
                    this.pipeline.EndFigure();
                }
            }

            /// <summary>
            /// Parse a Multi* text
            /// </summary>
            /// <param name="innerType">The inner spatial type</param>
            /// <param name="innerReader">The inner reader</param>
            private void ParseMultiGeoText(SpatialType innerType, Action innerReader)
            {
                // <multipoint text> ::= <empty set> | <left paren> <point text> {<comma> <point text>}* <right paren>
                // <multilinestring text> ::= <empty set> | <left paren> <linestring text> {<comma> <linestring text>}* <right paren>
                // <multipolygon text> ::= <empty set> | <left paren> <polygon text> {<comma> <polygon text>}* <right paren>
                if (!this.ReadEmptySet())
                {
                    this.ReadToken(WellKnownTextTokenType.LeftParen, null);
                    this.pipeline.BeginGeo(innerType);
                    innerReader();
                    this.pipeline.EndGeo();

                    while (this.ReadOptionalToken(WellKnownTextTokenType.Comma, null))
                    {
                        this.pipeline.BeginGeo(innerType);
                        innerReader();
                        this.pipeline.EndGeo();
                    }

                    this.ReadToken(WellKnownTextTokenType.RightParen, null);
                }
            }

            /// <summary>
            /// Parse Point Representation
            /// </summary>
            /// <param name="firstFigure">Whether this is the first point in the figure</param>
            private void ParsePoint(bool firstFigure)
            {
                double x = this.ReadDouble();
                double y = this.ReadDouble();
                double? z;
                double? m;

                if (this.TryReadOptionalNullableDouble(out z) && allowOnlyTwoDimensions)
                {
                    throw new FormatException(Strings.WellKnownText_TooManyDimensions);
                }

                if (this.TryReadOptionalNullableDouble(out m) && allowOnlyTwoDimensions)
                {
                    throw new FormatException(Strings.WellKnownText_TooManyDimensions);
                }

                if (firstFigure)
                {
                    this.pipeline.BeginFigure(x, y, z, m);
                }
                else
                {
                    this.pipeline.LineTo(x, y, z, m);
                }
            }

            /// <summary>
            /// Parse a point text
            /// </summary>
            private void ParsePointText()
            {
                // <point text> ::= <empty set> | <left paren> <point> <right paren>
                if (!this.ReadEmptySet())
                {
                    this.ReadToken(WellKnownTextTokenType.LeftParen, null);
                    this.ParsePoint(true);
                    this.ReadToken(WellKnownTextTokenType.RightParen, null);
                    this.pipeline.EndFigure();
                }
            }

            /// <summary>
            /// Parse a Polygon text
            /// </summary>
            private void ParsePolygonText()
            {
                // <polygon text> ::= <empty set> | <left paren> <linestring text> {<comma> <linestring text>}* <right paren>
                if (!this.ReadEmptySet())
                {
                    this.ReadToken(WellKnownTextTokenType.LeftParen, null);
                    this.ParseLineStringText();
                    while (this.ReadOptionalToken(WellKnownTextTokenType.Comma, null))
                    {
                        this.ParseLineStringText();
                    }

                    this.ReadToken(WellKnownTextTokenType.RightParen, null);
                }
            }

            /// <summary>
            /// Parse an instance of SRID
            /// </summary>
            private void ParseSRID()
            {
                // <SRID> := SRID <EQUALS> <INTEGER>;
                if (this.ReadOptionalToken(WellKnownTextTokenType.Text, WellKnownTextConstants.WktSrid))
                {
                    this.ReadToken(WellKnownTextTokenType.Equals, null);
                    this.pipeline.SetCoordinateSystem(this.ReadInteger());
                    this.ReadToken(WellKnownTextTokenType.Semicolon, null);
                }
                else
                {
                    this.pipeline.SetCoordinateSystem(null);
                }
            }

            /// <summary>
            /// Parse Tagged Text
            /// </summary>
            private void ParseTaggedText()
            {
                // <geometry tagged text> ::= <point tagged text> |
                //                            <linestring tagged text> |
                //                            <polygon tagged text> |
                //                            <multipoint tagged text> |
                //                            <multilinestring tagged text> |
                //                            <multipolygon tagged text> |
                //                            <geometrycollection tagged text>
                // <point tagged text> ::= point <point text>
                // <linestring tagged text> ::= linestring <linestring text>
                // <polygon tagged text> ::= polygon <polygon text>
                // <multipoint tagged text> ::= multipoint <multipoint text>
                // <multilinestring tagged text> ::= multilinestring <multilinestring text>
                // <multipolygon tagged text> ::= multipolygon <multipolygon text>
                // <geometrycollection tagged text> ::= geometrycollection <geometrycollection text>
                if (!this.NextToken())
                {
                    throw new FormatException(Strings.WellKnownText_UnknownTaggedText(String.Empty));
                }

                switch (this.lexer.CurrentToken.Text.ToUpperInvariant())
                {
                    case WellKnownTextConstants.WktPoint:
                        this.pipeline.BeginGeo(SpatialType.Point);
                        this.ParsePointText();
                        this.pipeline.EndGeo();
                        break;
                    case WellKnownTextConstants.WktLineString:
                        this.pipeline.BeginGeo(SpatialType.LineString);
                        this.ParseLineStringText();
                        this.pipeline.EndGeo();
                        break;
                    case WellKnownTextConstants.WktPolygon:
                        this.pipeline.BeginGeo(SpatialType.Polygon);
                        this.ParsePolygonText();
                        this.pipeline.EndGeo();
                        break;
                    case WellKnownTextConstants.WktMultiPoint:
                        this.pipeline.BeginGeo(SpatialType.MultiPoint);
                        this.ParseMultiGeoText(SpatialType.Point, this.ParsePointText);
                        this.pipeline.EndGeo();
                        break;
                    case WellKnownTextConstants.WktMultiLineString:
                        this.pipeline.BeginGeo(SpatialType.MultiLineString);
                        this.ParseMultiGeoText(SpatialType.LineString, this.ParseLineStringText);
                        this.pipeline.EndGeo();
                        break;
                    case WellKnownTextConstants.WktMultiPolygon:
                        this.pipeline.BeginGeo(SpatialType.MultiPolygon);
                        this.ParseMultiGeoText(SpatialType.Polygon, this.ParsePolygonText);
                        this.pipeline.EndGeo();
                        break;
                    case WellKnownTextConstants.WktCollection:
                        this.pipeline.BeginGeo(SpatialType.Collection);
                        this.ParseCollectionText();
                        this.pipeline.EndGeo();
                        break;
                    case WellKnownTextConstants.WktFullGlobe:
                        this.pipeline.BeginGeo(SpatialType.FullGlobe);
                        this.pipeline.EndGeo();
                        break;
                    default:
                        throw new FormatException(Strings.WellKnownText_UnknownTaggedText(this.lexer.CurrentToken.Text));
                }
            }

            /// <summary>
            /// Read a double literal
            /// </summary>
            /// <returns>The read double</returns>
            private double ReadDouble()
            {
                // <double> := <NUM> {.<NUM>}
                var numberText = new StringBuilder();
                this.ReadToken(WellKnownTextTokenType.Number, null);
                numberText.Append(this.lexer.CurrentToken.Text);
                if (this.ReadOptionalToken(WellKnownTextTokenType.Period, null))
                {
                    numberText.Append(WellKnownTextConstants.WktPeriod);
                    this.ReadToken(WellKnownTextTokenType.Number, null);
                    numberText.Append(this.lexer.CurrentToken.Text);
                }

                return Double.Parse(numberText.ToString(), CultureInfo.InvariantCulture);
            }

            /// <summary>
            /// Check to see if the content is EMPTY
            /// </summary>
            /// <returns>True if the content is declared as EMPTY</returns>
            private bool ReadEmptySet()
            {
                // <empty set> ::= EMPTY
                return this.ReadOptionalToken(WellKnownTextTokenType.Text, WellKnownTextConstants.WktEmpty);
            }

            /// <summary>
            /// Read an integer literal
            /// </summary>
            /// <returns>The read integer</returns>
            private Int32 ReadInteger()
            {
                this.ReadToken(WellKnownTextTokenType.Number, null);
                return XmlConvert.ToInt32(this.lexer.CurrentToken.Text);
            }

            /// <summary>
            /// Read an optional double literal
            /// </summary>
            /// <param name="value">The value that was read.</param>
            /// <returns>true if a value was read, otherwise returns false</returns>
            private bool TryReadOptionalNullableDouble(out double? value)
            {
                // <double> := <NUM> {.<NUM>}
                var numberText = new StringBuilder();

                if (this.ReadOptionalToken(WellKnownTextTokenType.Number, null))
                {
                    numberText.Append(this.lexer.CurrentToken.Text);
                    if (this.ReadOptionalToken(WellKnownTextTokenType.Period, null))
                    {
                        numberText.Append(WellKnownTextConstants.WktPeriod);
                        this.ReadToken(WellKnownTextTokenType.Number, null);
                        numberText.Append(this.lexer.CurrentToken.Text);
                    }

                    value = Double.Parse(numberText.ToString(), CultureInfo.InvariantCulture);
                    return true;
                }

                value = null;
                return this.ReadOptionalToken(WellKnownTextTokenType.Text, WellKnownTextConstants.WktNull);
            }

            /// <summary>
            /// Read an optional token. If the read token matches the expected optional token, then consume it.
            /// </summary>
            /// <param name="expectedTokenType">The expected token type</param>
            /// <param name="expectedTokenText">The expected token text, or null</param>
            /// <returns>True if the optional token matches the next token in stream</returns>
            private bool ReadOptionalToken(WellKnownTextTokenType expectedTokenType, String expectedTokenText)
            {
                LexerToken token;
                while (this.lexer.Peek(out token))
                {
                    if (token.MatchToken((int)WellKnownTextTokenType.WhiteSpace, null, StringComparison.OrdinalIgnoreCase))
                    {
                        this.lexer.Next();
                    }
                    else if (token.MatchToken((int)expectedTokenType, expectedTokenText, StringComparison.OrdinalIgnoreCase))
                    {
                        this.lexer.Next();
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }

                return false;
            }

            /// <summary>
            /// Read and consume a token from the lexer, throw if the read token does not match the expected token
            /// </summary>
            /// <param name="type">The expected token type</param>
            /// <param name="text">The expected token text</param>
            private void ReadToken(WellKnownTextTokenType type, String text)
            {
                if (!(this.NextToken() && this.IsTokenMatch(type, text)))
                {
                    throw new FormatException(Strings.WellKnownText_UnexpectedToken(type, text, this.lexer.CurrentToken));
                }
            }
        }
    }
}
