//---------------------------------------------------------------------
// <copyright file="LiteralConversionTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.TDD.Tests.Server
{
    using System;
    using System.Data.Linq;
    using Microsoft.OData.Service.Parsing;
    using System.IO;
    using System.Runtime.Serialization;
    using System.Text;
    using System.Xml;
    using System.Xml.Linq;
    using FluentAssertions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.OData.Service;
    using Microsoft.OData;

    /// <summary>
    /// Note: Several tests in this file were copied from older 'unit-tests' which used reflection.
    /// </summary>
    [TestClass]
    public class LiteralConversionTests
    {
        private static readonly LiteralFormatter DefaultLiteralFormatter = LiteralFormatter.ForKeys(false);
        private static readonly LiteralParser DefaultLiteralParser = LiteralParser.ForKeys(false);
        private static readonly LiteralFormatter KeyAsSegmentsLiteralFormatter = LiteralFormatter.ForKeys(true);
        private static readonly LiteralParser KeyAsSegmentsLiteralParser = LiteralParser.ForKeys(true);

        [TestMethod]
        public void ValuesShouldRoundTripForDefaultFormatting()
        {
            ValuesShouldRoundTrip(DefaultLiteralFormatter, DefaultLiteralParser);
        }

        [TestMethod]
        public void ValuesShouldRoundTripForKeysAsSegmentsFormatting()
        {
            ValuesShouldRoundTrip(KeyAsSegmentsLiteralFormatter, KeyAsSegmentsLiteralParser);
        }

        [TestMethod]
        public void DecimalShouldSupportExponentialNotation()
        {
            // we used to allow exponential format in decimal (NumberStyle.Float)
            // verify we are not breaking this:
            foreach (string expFormat in new[] { "1e8M", "+1e8M", "-1e09M", "+1.61e5M" })
            {
                object temp;
                Type targetType = typeof(Decimal);
                DefaultLiteralParser.TryParseLiteral(targetType, expFormat, out temp).Should().BeTrue();
            }
        }

        [TestMethod]
        public void BinaryParsingShouldReturnCorrectType()
        {
            object result;
            DefaultLiteralParser.TryParseLiteral(typeof(byte[]), "binary''", out result).Should().BeTrue();
            result.Should().BeAssignableTo<byte[]>();

            DefaultLiteralParser.TryParseLiteral(typeof(Binary), "binary''", out result).Should().BeTrue();
            result.Should().BeAssignableTo<Binary>();
        }

        [TestMethod]
        public void DefaultParserShouldUnwrapFormattedLiteralEncodedAsBinary()
        {
            Guid guid = Guid.NewGuid();
            string formattedGuid = DefaultLiteralFormatter.Format(guid);
            byte[] binaryEncodedGuid = Encoding.UTF8.GetBytes(formattedGuid);
            string formattedBinary = DefaultLiteralFormatter.Format(binaryEncodedGuid);

            object result;
            DefaultLiteralParser.TryParseLiteral(typeof(Guid), formattedBinary, out result).Should().BeTrue();
            result.Should().Be(guid);
        }

        [TestMethod]
        public void KeyAsSegmentsParserShouldNotUnwrapFormattedLiteralEncodedAsBinary()
        {
            Guid guid = Guid.NewGuid();
            string formattedGuid = KeyAsSegmentsLiteralFormatter.Format(guid);
            byte[] binaryEncodedGuid = Encoding.UTF8.GetBytes(formattedGuid);
            string formattedBinary = KeyAsSegmentsLiteralFormatter.Format(binaryEncodedGuid);

            object result;
            KeyAsSegmentsLiteralParser.TryParseLiteral(typeof(Guid), formattedBinary, out result).Should().BeFalse();
            KeyAsSegmentsLiteralParser.TryParseLiteral(typeof(byte[]), formattedBinary, out result).Should().BeTrue();
            result.As<byte[]>().Should().ContainInOrder(binaryEncodedGuid);
        }

        [TestMethod]
        public void NullableTypeShouldRoundTrip()
        {
            ValueShouldRoundTrip<bool?>(false, DefaultLiteralFormatter, DefaultLiteralParser);
            ValueShouldRoundTrip<bool?>(true, DefaultLiteralFormatter, DefaultLiteralParser);
        }

        [TestMethod]
        public void WebConvertIsKeyValueQuotedTest()
        {
            QuotesAreValid("''").Should().BeTrue();
            QuotesAreValid("'a'").Should().BeTrue();
            QuotesAreValid("'''a'''").Should().BeTrue();
            QuotesAreValid("'''''a'").Should().BeTrue();
            QuotesAreValid("'a''b'").Should().BeTrue();

            QuotesAreValid("noquotes").Should().BeFalse();
            QuotesAreValid("'").Should().BeFalse();
            QuotesAreValid("''a''").Should().BeFalse();
            QuotesAreValid("'a").Should().BeFalse();
            QuotesAreValid("b'").Should().BeFalse();
            QuotesAreValid("'a''").Should().BeFalse();
            QuotesAreValid("''a").Should().BeFalse();
        }

        [TestMethod]
        public void KeyAsSegmentFormatterShouldEscapeDollarSign()
        {
            KeyAsSegmentsLiteralFormatter.Format("$").Should().Be("%24%24");
            KeyAsSegmentsLiteralFormatter.Format("$foo").Should().Be("%24%24foo");
        }

        [TestMethod]
        public void KeyAsSegmentFormatterShouldFormatEnumValue()
        {
            KeyAsSegmentsLiteralFormatter.Format(new ODataEnumValue("Value1")).Should().Be("Value1");
            KeyAsSegmentsLiteralFormatter.Format(new ODataEnumValue("Value2", "Custom Type")).Should().Be("Value2");
        }

        [TestMethod]
        public void KeyAsSegmentParserShouldUnEscapeDollarSign()
        {
            object result;
            KeyAsSegmentsLiteralParser.TryParseLiteral(typeof(string), "$$foo", out result).Should().BeTrue();
            result.Should().Be("$foo");
        }

        private static bool QuotesAreValid(string text)
        {
            return WebConvert.TryRemoveQuotes(ref text);
        }

        private static void ValueShouldRoundTrip<T>(T value, LiteralFormatter formatter, LiteralParser parser)
        {
            var type = typeof(T);
            string formatted = formatter.Format(value);
            object result;
            string text = Uri.UnescapeDataString(formatted);
            parser.TryParseLiteral(type, text, out result).Should().BeTrue();

            // Since DateTime values get converted into 
            if (typeof(T) != typeof(DateTime))
            {
                result.Should().BeAssignableTo<T>();
            }
            if (type == typeof(byte[]))
            {
                result.As<byte[]>().Should().ContainInOrder(value as byte[]);
            }
            else if (type == typeof(XElement))
            {
                result.As<XElement>().Should().Be(value as XElement);
            }
            else
            {
                result.Should().Be(value);
            }
        }

        private static void ValuesShouldRoundTrip<T>(LiteralFormatter formatter, LiteralParser parser, params T[] values)
        {
            foreach (T value in values)
            {
                ValueShouldRoundTrip(value, formatter, parser);
            }
        }

        private static void ValuesShouldRoundTrip(LiteralFormatter formatter, LiteralParser parser)
        {
            // TODO: reduce coverage, this is overkill and mostly testing XmlConvert.
            ValuesShouldRoundTrip(formatter, parser, true, false);
            ValuesShouldRoundTrip(formatter, parser, 0, 1, 255);
            ValuesShouldRoundTrip(formatter, parser, 0, 1, -1, sbyte.MaxValue, sbyte.MinValue);
            ValuesShouldRoundTrip(formatter, parser, Decimal.MaxValue, Decimal.MinValue, Decimal.One, Decimal.Zero, Decimal.MinValue, Decimal.MaxValue);
            ValuesShouldRoundTrip(formatter, parser, 0, 1, -0.1, Double.Epsilon, Double.MaxValue, Double.MinValue, Double.NegativeInfinity, Double.PositiveInfinity, Double.NaN, 7E-06, 9e+09, 9E+16); /*last 2 cases are values with no periods in them*/
            ValuesShouldRoundTrip(formatter, parser, 0, 1, -0.1, Single.Epsilon, Single.MaxValue, Single.MinValue, Single.NegativeInfinity, Single.PositiveInfinity, Single.NaN, 7E-06f, 9E+09f); /*last 2 cases are values with no periods in them*/
            ValuesShouldRoundTrip(formatter, parser, 0, 1, -1, Int16.MaxValue, Int16.MinValue);
            ValuesShouldRoundTrip(formatter, parser, 0, 1, -1, Int32.MaxValue, Int32.MinValue);
            ValuesShouldRoundTrip(formatter, parser, 0, 1, -1, Int64.MaxValue, Int64.MinValue);
            ValuesShouldRoundTrip(formatter, parser, new byte[0], new byte[] { 0 }, new byte[] { 0, 1, byte.MinValue, byte.MaxValue }, new byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 });
            ValuesShouldRoundTrip(formatter, parser, DateTime.MaxValue, DateTime.MinValue, DateTime.Now, DateTime.Today, DateTime.UtcNow); // Kinds?
            ValuesShouldRoundTrip(formatter, parser, DateTimeOffset.MaxValue, DateTimeOffset.MinValue, DateTimeOffset.Now, DateTimeOffset.UtcNow);
            ValuesShouldRoundTrip(formatter, parser, TimeSpan.MaxValue, TimeSpan.MinValue, TimeSpan.FromDays(1.5));
            ValuesShouldRoundTrip(formatter, parser, Guid.Empty, Guid.NewGuid());
            ValuesShouldRoundTrip(formatter, parser, new Binary(new byte[0]), new Binary(new byte[] { 1, 2, byte.MaxValue }));
            ValuesShouldRoundTrip(formatter, parser, XElement.Parse("<xelement>content<nested><!--comment--></nested> </xelement>"));
            ValuesShouldRoundTrip(formatter, parser, "", "  \t \r\n", ".,();", "\r\n", "\r\n\r\n\r\n\r\n", "\r", "\n", "\n\r", "a\x0302e\x0327\x0627\x0654\x0655", "a surrogate pair: \xd800\xdc00", "left to right \x05d0\x05d1 \x05ea\x05e9 english", "\x1\x2\x3\x4\x5\x20");
        }
    }
}