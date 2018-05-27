//---------------------------------------------------------------------
// <copyright file="ClientKeyGenerationPinningTest.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.TDD.Tests.Client
{
    using System;
#if !(NETCOREAPP1_0 || NETCOREAPP2_0)
    using System.Data.Linq;
#endif
    using System.Linq;
    using System.Text;
    using System.Xml;
    using System.Xml.Linq;
    using FluentAssertions;
    using Microsoft.OData.Client;
    using Microsoft.OData.Edm;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ClientKeyGenerationPinningTest
    {
        [TestMethod]
        public void ClientDoublePinning()
        {
            var keyValues = new object[] { 1.0D, 1D, 1e10D, -1.0D, -1D, -1e10D, -1e-10D, 1.23D, -1.23D, 1.23E123D, -1.23E123D, 1.23E-123D, double.PositiveInfinity, double.NegativeInfinity };

            const string expected = @"(1.0)
(1.0)
(10000000000.0)
(-1)
(-1)
(-10000000000)
(-1E-10)
(1.23)
(-1.23)
(1.23E%2B123)
(-1.23E%2B123)
(1.23E-123)
(INF)
(-INF)
(prop=1.0,prop=1.0,prop=10000000000.0,prop=-1,prop=-1,prop=-10000000000,prop=-1E-10,prop=1.23,prop=-1.23,prop=1.23E%2B123,prop=-1.23E%2B123,prop=1.23E-123,prop=INF,prop=-INF)
";

            RunClientPinningTest(expected, keyValues);
        }

        [TestMethod]
        public void ClientFloatPinning()
        {
            var keyValues = new object[] { 1.0F, 1F, 1e10F, -1.0F, -1F, -1e10F, -1e-10F, 1.23F, -1.23F, 1.23E30F, -1.23E30F, 1.23E-30F, float.PositiveInfinity, float.NegativeInfinity };

            const string expected = @"(1)
(1)
(1E%2B10)
(-1)
(-1)
(-1E%2B10)
(-1E-10)
(1.23)
(-1.23)
(1.23E%2B30)
(-1.23E%2B30)
(1.23E-30)
(INF)
(-INF)
(prop=1,prop=1,prop=1E%2B10,prop=-1,prop=-1,prop=-1E%2B10,prop=-1E-10,prop=1.23,prop=-1.23,prop=1.23E%2B30,prop=-1.23E%2B30,prop=1.23E-30,prop=INF,prop=-INF)
";

            RunClientPinningTest(expected, keyValues);
        }

        [TestMethod]
        public void ClientOtherTypesPinning()
        {
#if (NETCOREAPP1_0 || NETCOREAPP2_0)
            var keyValues = new object[] { 1, "abc", "abc pqr", new byte[0], new byte[] { 1, 2 }, new XElement("Fake") };

            const string expected = @"(1)
('abc')
('abc%20pqr')
(binary'')
(binary'AQI%3D')
('%3CFake%20%2F%3E')
(prop=1,prop='abc',prop='abc%20pqr',prop=binary'',prop=binary'AQI%3D',prop='%3CFake%20%2F%3E')
";
#else
            var keyValues = new object[] { 1, "abc", "abc pqr", new byte[0], new byte[] { 1, 2 }, new XElement("Fake"), new Binary(new byte[0]), new Binary(new byte[] { 1, 2 }) };

            const string expected = @"(1)
('abc')
('abc%20pqr')
(binary'')
(binary'AQI%3D')
('%3CFake%20%2F%3E')
(binary'')
(binary'AQI%3D')
(prop=1,prop='abc',prop='abc%20pqr',prop=binary'',prop=binary'AQI%3D',prop='%3CFake%20%2F%3E',prop=binary'',prop=binary'AQI%3D')
";
#endif

            RunClientPinningTest(expected, keyValues);
        }

        [TestMethod]
        public void ClientGiantPinningTest()
        {
            StringBuilder builder = new StringBuilder();
            RunClientPinningTest(builder, true, false);
            RunClientPinningTest(builder, 0, 1, 255);
            RunClientPinningTest(builder, 0, 1, -1, sbyte.MaxValue, sbyte.MinValue);
            RunClientPinningTest(builder, Decimal.MaxValue, Decimal.MinValue, Decimal.One, Decimal.Zero, Decimal.MinValue, Decimal.MaxValue);
            RunClientPinningTest(builder, 0, 1, -0.1, Double.Epsilon, Double.MaxValue, Double.MinValue, Double.NegativeInfinity, Double.PositiveInfinity, Double.NaN, 7E-06, 9e+09, 9E+16); /*last 2 cases are values with no periods in them*/
            RunClientPinningTest(builder, 0, 1, -0.1, Single.Epsilon, Single.MaxValue, Single.MinValue, Single.NegativeInfinity, Single.PositiveInfinity, Single.NaN, 7E-06f, 9E+09f); /*last 2 cases are values with no periods in them*/
            RunClientPinningTest(builder, 0, 1, -1, Int16.MaxValue, Int16.MinValue);
            RunClientPinningTest(builder, 0, 1, -1, Int32.MaxValue, Int32.MinValue);
            RunClientPinningTest(builder, 0, 1, -1, Int64.MaxValue, Int64.MinValue);
            RunClientPinningTest(builder, new byte[0], new byte[] { 0 }, new byte[] { 0, 1, byte.MinValue, byte.MaxValue }, new byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 });
            RunClientPinningTest(builder, DateTimeOffset.MaxValue, DateTimeOffset.MinValue, XmlConvert.ToDateTimeOffset("2012-11-16T10:54:13.5422534-08:00"), XmlConvert.ToDateTimeOffset("2012-11-16T18:54:13.5422534Z"));
            RunClientPinningTest(builder, TimeSpan.MaxValue, TimeSpan.MinValue, TimeSpan.FromDays(1.5));
            RunClientPinningTest(builder, Date.MaxValue, Date.MinValue, new Date(2014, 9, 25));
            RunClientPinningTest(builder, TimeOfDay.MaxValue, TimeOfDay.MinValue, new TimeOfDay(12, 9, 25, 900));
            RunClientPinningTest(builder, Guid.Empty, Guid.Parse("b467459e-1eb5-4598-8a63-2c40c6a2590c"));
#if !(NETCOREAPP1_0 || NETCOREAPP2_0)
            RunClientPinningTest(builder, new Binary(new byte[0]), new Binary(new byte[] { 1, 2, byte.MaxValue }));
#endif
            RunClientPinningTest(builder, XElement.Parse("<xelement>content<nested><!--comment--></nested> </xelement>"));
            RunClientPinningTest(builder, "", "  \t \r\n", ".,();", "\r\n", "\r\n\r\n\r\n\r\n", "\r", "\n", "\n\r", "a\x0302e\x0327\x0627\x0654\x0655", "a surrogate pair: \xd800\xdc00", "left to right \x05d0\x05d1 \x05ea\x05e9 english", "\x1\x2\x3\x4\x5\x20");

#if (NETCOREAPP1_0 || NETCOREAPP2_0)
            const string expected = @"(true)
(false)
(prop=true,prop=false)

(0)
(1)
(255)
(prop=0,prop=1,prop=255)

(0)
(1)
(-1)
(127)
(-128)
(prop=0,prop=1,prop=-1,prop=127,prop=-128)

(79228162514264337593543950335)
(-79228162514264337593543950335)
(1)
(0)
(-79228162514264337593543950335)
(79228162514264337593543950335)
(prop=79228162514264337593543950335,prop=-79228162514264337593543950335,prop=1,prop=0,prop=-79228162514264337593543950335,prop=79228162514264337593543950335)

(0)
(1)
(-0.1)
(4.94065645841247E-324)
(1.7976931348623157E%2B308)
(-1.7976931348623157E%2B308)
(-INF)
(INF)
(NaN)
(7E-06)
(9000000000.0)
(9E%2B16)
(prop=0,prop=1,prop=-0.1,prop=4.94065645841247E-324,prop=1.7976931348623157E%2B308,prop=-1.7976931348623157E%2B308,prop=-INF,prop=INF,prop=NaN,prop=7E-06,prop=9000000000.0,prop=9E%2B16)

(0)
(1)
(-0.1)
(1.401298E-45)
(3.40282347E%2B38)
(-3.40282347E%2B38)
(-INF)
(INF)
(NaN)
(7E-06)
(8.999999E%2B09)
(prop=0,prop=1,prop=-0.1,prop=1.401298E-45,prop=3.40282347E%2B38,prop=-3.40282347E%2B38,prop=-INF,prop=INF,prop=NaN,prop=7E-06,prop=8.999999E%2B09)

(0)
(1)
(-1)
(32767)
(-32768)
(prop=0,prop=1,prop=-1,prop=32767,prop=-32768)

(0)
(1)
(-1)
(2147483647)
(-2147483648)
(prop=0,prop=1,prop=-1,prop=2147483647,prop=-2147483648)

(0)
(1)
(-1)
(9223372036854775807)
(-9223372036854775808)
(prop=0,prop=1,prop=-1,prop=9223372036854775807,prop=-9223372036854775808)

(binary'')
(binary'AA%3D%3D')
(binary'AAEA%2Fw%3D%3D')
(binary'AAECAwQFBgcICQoLDA0ODxA%3D')
(prop=binary'',prop=binary'AA%3D%3D',prop=binary'AAEA%2Fw%3D%3D',prop=binary'AAECAwQFBgcICQoLDA0ODxA%3D')

(9999-12-31T23%3A59%3A59.9999999Z)
(0001-01-01T00%3A00%3A00Z)
(2012-11-16T10%3A54%3A13.5422534-08%3A00)
(2012-11-16T18%3A54%3A13.5422534Z)
(prop=9999-12-31T23%3A59%3A59.9999999Z,prop=0001-01-01T00%3A00%3A00Z,prop=2012-11-16T10%3A54%3A13.5422534-08%3A00,prop=2012-11-16T18%3A54%3A13.5422534Z)

(duration'P10675199DT2H48M5.4775807S')
(duration'-P10675199DT2H48M5.4775808S')
(duration'P1DT12H')
(prop=duration'P10675199DT2H48M5.4775807S',prop=duration'-P10675199DT2H48M5.4775808S',prop=duration'P1DT12H')

(9999-12-31)
(0001-01-01)
(2014-09-25)
(prop=9999-12-31,prop=0001-01-01,prop=2014-09-25)

(23%3A59%3A59.9999999)
(00%3A00%3A00.0000000)
(12%3A09%3A25.9000000)
(prop=23%3A59%3A59.9999999,prop=00%3A00%3A00.0000000,prop=12%3A09%3A25.9000000)

(00000000-0000-0000-0000-000000000000)
(b467459e-1eb5-4598-8a63-2c40c6a2590c)
(prop=00000000-0000-0000-0000-000000000000,prop=b467459e-1eb5-4598-8a63-2c40c6a2590c)

('%3Cxelement%3Econtent%3Cnested%3E%3C%21--comment--%3E%3C%2Fnested%3E%3C%2Fxelement%3E')
('%3Cxelement%3Econtent%3Cnested%3E%3C%21--comment--%3E%3C%2Fnested%3E%3C%2Fxelement%3E')

('')
('%20%20%09%20%0D%0A')
('.%2C%28%29%3B')
('%0D%0A')
('%0D%0A%0D%0A%0D%0A%0D%0A')
('%0D')
('%0A')
('%0A%0D')
('a%CC%82e%CC%A7%D8%A7%D9%94%D9%95')
('a%20surrogate%20pair%3A%20%F0%90%80%80')
('left%20to%20right%20%D7%90%D7%91%20%D7%AA%D7%A9%20english')
('%01%02%03%04%05%20')
(prop='',prop='%20%20%09%20%0D%0A',prop='.%2C%28%29%3B',prop='%0D%0A',prop='%0D%0A%0D%0A%0D%0A%0D%0A',prop='%0D',prop='%0A',prop='%0A%0D',prop='a%CC%82e%CC%A7%D8%A7%D9%94%D9%95',prop='a%20surrogate%20pair%3A%20%F0%90%80%80',prop='left%20to%20right%20%D7%90%D7%91%20%D7%AA%D7%A9%20english',prop='%01%02%03%04%05%20')
";
#else
            const string expected = @"(true)
(false)
(prop=true,prop=false)

(0)
(1)
(255)
(prop=0,prop=1,prop=255)

(0)
(1)
(-1)
(127)
(-128)
(prop=0,prop=1,prop=-1,prop=127,prop=-128)

(79228162514264337593543950335)
(-79228162514264337593543950335)
(1)
(0)
(-79228162514264337593543950335)
(79228162514264337593543950335)
(prop=79228162514264337593543950335,prop=-79228162514264337593543950335,prop=1,prop=0,prop=-79228162514264337593543950335,prop=79228162514264337593543950335)

(0)
(1)
(-0.1)
(4.94065645841247E-324)
(1.7976931348623157E%2B308)
(-1.7976931348623157E%2B308)
(-INF)
(INF)
(NaN)
(7E-06)
(9000000000.0)
(9E%2B16)
(prop=0,prop=1,prop=-0.1,prop=4.94065645841247E-324,prop=1.7976931348623157E%2B308,prop=-1.7976931348623157E%2B308,prop=-INF,prop=INF,prop=NaN,prop=7E-06,prop=9000000000.0,prop=9E%2B16)

(0)
(1)
(-0.1)
(1.401298E-45)
(3.40282347E%2B38)
(-3.40282347E%2B38)
(-INF)
(INF)
(NaN)
(7E-06)
(8.999999E%2B09)
(prop=0,prop=1,prop=-0.1,prop=1.401298E-45,prop=3.40282347E%2B38,prop=-3.40282347E%2B38,prop=-INF,prop=INF,prop=NaN,prop=7E-06,prop=8.999999E%2B09)

(0)
(1)
(-1)
(32767)
(-32768)
(prop=0,prop=1,prop=-1,prop=32767,prop=-32768)

(0)
(1)
(-1)
(2147483647)
(-2147483648)
(prop=0,prop=1,prop=-1,prop=2147483647,prop=-2147483648)

(0)
(1)
(-1)
(9223372036854775807)
(-9223372036854775808)
(prop=0,prop=1,prop=-1,prop=9223372036854775807,prop=-9223372036854775808)

(binary'')
(binary'AA%3D%3D')
(binary'AAEA%2Fw%3D%3D')
(binary'AAECAwQFBgcICQoLDA0ODxA%3D')
(prop=binary'',prop=binary'AA%3D%3D',prop=binary'AAEA%2Fw%3D%3D',prop=binary'AAECAwQFBgcICQoLDA0ODxA%3D')

(9999-12-31T23%3A59%3A59.9999999Z)
(0001-01-01T00%3A00%3A00Z)
(2012-11-16T10%3A54%3A13.5422534-08%3A00)
(2012-11-16T18%3A54%3A13.5422534Z)
(prop=9999-12-31T23%3A59%3A59.9999999Z,prop=0001-01-01T00%3A00%3A00Z,prop=2012-11-16T10%3A54%3A13.5422534-08%3A00,prop=2012-11-16T18%3A54%3A13.5422534Z)

(duration'P10675199DT2H48M5.4775807S')
(duration'-P10675199DT2H48M5.4775808S')
(duration'P1DT12H')
(prop=duration'P10675199DT2H48M5.4775807S',prop=duration'-P10675199DT2H48M5.4775808S',prop=duration'P1DT12H')

(9999-12-31)
(0001-01-01)
(2014-09-25)
(prop=9999-12-31,prop=0001-01-01,prop=2014-09-25)

(23%3A59%3A59.9999999)
(00%3A00%3A00.0000000)
(12%3A09%3A25.9000000)
(prop=23%3A59%3A59.9999999,prop=00%3A00%3A00.0000000,prop=12%3A09%3A25.9000000)

(00000000-0000-0000-0000-000000000000)
(b467459e-1eb5-4598-8a63-2c40c6a2590c)
(prop=00000000-0000-0000-0000-000000000000,prop=b467459e-1eb5-4598-8a63-2c40c6a2590c)

(binary'')
(binary'AQL%2F')
(prop=binary'',prop=binary'AQL%2F')

('%3Cxelement%3Econtent%3Cnested%3E%3C%21--comment--%3E%3C%2Fnested%3E%3C%2Fxelement%3E')
('%3Cxelement%3Econtent%3Cnested%3E%3C%21--comment--%3E%3C%2Fnested%3E%3C%2Fxelement%3E')

('')
('%20%20%09%20%0D%0A')
('.%2C%28%29%3B')
('%0D%0A')
('%0D%0A%0D%0A%0D%0A%0D%0A')
('%0D')
('%0A')
('%0A%0D')
('a%CC%82e%CC%A7%D8%A7%D9%94%D9%95')
('a%20surrogate%20pair%3A%20%F0%90%80%80')
('left%20to%20right%20%D7%90%D7%91%20%D7%AA%D7%A9%20english')
('%01%02%03%04%05%20')
(prop='',prop='%20%20%09%20%0D%0A',prop='.%2C%28%29%3B',prop='%0D%0A',prop='%0D%0A%0D%0A%0D%0A%0D%0A',prop='%0D',prop='%0A',prop='%0A%0D',prop='a%CC%82e%CC%A7%D8%A7%D9%94%D9%95',prop='a%20surrogate%20pair%3A%20%F0%90%80%80',prop='left%20to%20right%20%D7%90%D7%91%20%D7%AA%D7%A9%20english',prop='%01%02%03%04%05%20')
";
#endif

            var actual = builder.ToString().Replace("!", "%21").Replace("()", "%28%29");
            actual.Should().Be(expected);
        }

        [TestMethod]
        public void ClientErrorPinningTest()
        {
            Action withNull = () => DataServiceUrlKeyDelimiter.Parentheses.AppendKeyExpression(new object[1], k => "foo", k => null, new StringBuilder());
            withNull.ShouldThrow<InvalidOperationException>().WithMessage("The serialized resource has a null value in key member 'foo'. Null values are not supported in key members.");

            Action withUnknownType = () => DataServiceUrlKeyDelimiter.Parentheses.AppendKeyExpression(new object[1], k => "foo", k => this, new StringBuilder());
            withUnknownType.ShouldThrow<InvalidOperationException>().WithMessage("Unable to convert value '" + this.GetType().FullName + "' into a key string for a URI.");
        }

        private static void RunClientPinningTest(string expected, params object[] keyValues)
        {
            StringBuilder builder = new StringBuilder();
            RunClientPinningTest(builder, keyValues);

            var actual = builder.ToString();
            actual.Should().Be(expected);
        }

        private static void RunClientPinningTest(StringBuilder builder, params object[] keyValues)
        {
            var compositeKey = keyValues.Select(k => new { Name = "prop", Value = k }).ToArray();
            var allKeys = keyValues.Select(k => new[] { new { Name = "prop", Value = k } }).Concat(new[] { compositeKey });

            foreach (var key in allKeys)
            {
                if (builder.Length > 0)
                {
                    builder.AppendLine();
                }

                DataServiceUrlKeyDelimiter.Parentheses.AppendKeyExpression(key, k => k.Name, k => k.Value, builder);
            }

            builder.AppendLine();
        }
    }
}
