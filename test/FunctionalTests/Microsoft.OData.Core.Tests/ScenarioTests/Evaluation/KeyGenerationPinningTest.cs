//---------------------------------------------------------------------
// <copyright file="KeyGenerationPinningTest.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Xml;
using FluentAssertions;
using Microsoft.OData.Core.Evaluation;
using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Library;
using Microsoft.OData.Edm.Library.Values;
using Microsoft.OData.Edm.Values;
using Xunit;

namespace Microsoft.OData.Core.Tests.ScenarioTests.Evaluation
{
    public class KeyGenerationPinningTest
    {
        [Fact]
        public void DoublePinning()
        {
            var keyValues = new object[] { 1.0D, 1D, -1.0D, -1D, 1e15D, 1e-15D, -1e15D, -1e-15D, 1.23D, -1.23D, 1.23E123D, -1.23E123D, 1.23E-123D, double.PositiveInfinity, double.NegativeInfinity, double.NaN };

            const string expected = @"(1.0)
(1.0)
(-1.0)
(-1.0)
(1E%2B15)
(1E-15)
(-1E%2B15)
(-1E-15)
(1.23)
(-1.23)
(1.23E%2B123)
(-1.23E%2B123)
(1.23E-123)
(INF)
(-INF)
(NaN)
(prop0=1.0,prop1=1.0,prop2=-1.0,prop3=-1.0,prop4=1E%2B15,prop5=1E-15,prop6=-1E%2B15,prop7=-1E-15,prop8=1.23,prop9=-1.23,prop10=1.23E%2B123,prop11=-1.23E%2B123,prop12=1.23E-123,prop13=INF,prop14=-INF,prop15=NaN)
";

            RunPinningTest(expected, keyValues);
        }

        [Fact]
        public void FloatPinning()
        {
            var keyValues = new object[] { 1.0F, 1F, -1.0F, -1F, 1e10F, 1e-10F, -1e10F, -1e-10F, 1.23F, -1.23F, 1.23E30F, -1.23E30F, 1.23E-30F, float.PositiveInfinity, float.NegativeInfinity, float.NaN };

            const string expected = @"(1)
(1)
(-1)
(-1)
(1E%2B10)
(1E-10)
(-1E%2B10)
(-1E-10)
(1.23)
(-1.23)
(1.23E%2B30)
(-1.23E%2B30)
(1.23E-30)
(INF)
(-INF)
(NaN)
(prop0=1,prop1=1,prop2=-1,prop3=-1,prop4=1E%2B10,prop5=1E-10,prop6=-1E%2B10,prop7=-1E-10,prop8=1.23,prop9=-1.23,prop10=1.23E%2B30,prop11=-1.23E%2B30,prop12=1.23E-30,prop13=INF,prop14=-INF,prop15=NaN)
";

            RunPinningTest(expected, keyValues);
        }

        [Fact]
        public void OtherTypesPinning()
        {
            var keyValues = new object[] { 1, "abc", "abc pqr", new byte[0], new byte[] { 1, 2 } };

            const string expected = @"(1)
('abc')
('abc%20pqr')
(binary'')
(binary'AQI%3D')
(prop0=1,prop1='abc',prop2='abc%20pqr',prop3=binary'',prop4=binary'AQI%3D')
";

            RunPinningTest(expected, keyValues);
        }

        [Fact]
        public void GiantPinningTest()
        {
            StringBuilder builder = new StringBuilder();
            RunPinningTest(builder, true, false);
            RunPinningTest(builder, 0, 1, 255);
            RunPinningTest(builder, 0, 1, -1, sbyte.MaxValue, sbyte.MinValue);
            RunPinningTest(builder, Decimal.MaxValue, Decimal.MinValue, Decimal.One, Decimal.Zero, Decimal.MinValue, Decimal.MaxValue);
            RunPinningTest(builder, 0, 1, -1, Int16.MaxValue, Int16.MinValue);
            RunPinningTest(builder, 0, 1, -1, Int32.MaxValue, Int32.MinValue);
            RunPinningTest(builder, 0, 1, -1, Int64.MaxValue, Int64.MinValue);
            RunPinningTest(builder, new byte[0], new byte[] { 0 }, new byte[] { 0, 1, byte.MinValue, byte.MaxValue }, new byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 });
            RunPinningTest(builder, DateTimeOffset.MaxValue, DateTimeOffset.MinValue, XmlConvert.ToDateTimeOffset("2012-11-16T10:54:13.5422534-08:00"), XmlConvert.ToDateTimeOffset("2012-11-16T18:54:13.5422534Z"));
            RunPinningTest(builder, Date.MinValue, new Date(2014, 12, 31), Date.MaxValue);
            RunPinningTest(builder, TimeOfDay.MinValue, new TimeOfDay(12, 20, 4, 123), new TimeOfDay(TimeOfDay.MaxTickValue));
            RunPinningTest(builder, TimeSpan.MaxValue, TimeSpan.MinValue, TimeSpan.FromDays(1.5));
            RunPinningTest(builder, Guid.Empty, Guid.Parse("b467459e-1eb5-4598-8a63-2c40c6a2590c"));
            RunPinningTest(builder, "", "  \t \r\n", ".,();", "\r\n", "\r\n\r\n\r\n\r\n", "\r", "\n", "\n\r", "a\x0302e\x0327\x0627\x0654\x0655", "a surrogate pair: \xd800\xdc00", "left to right \x05d0\x05d1 \x05ea\x05e9 english", "\x1\x2\x3\x4\x5\x20");

            const string expected = @"(true)
(false)
(prop0=true,prop1=false)

(0)
(1)
(255)
(prop0=0,prop1=1,prop2=255)

(0)
(1)
(-1)
(127)
(-128)
(prop0=0,prop1=1,prop2=-1,prop3=127,prop4=-128)

(79228162514264337593543950335)
(-79228162514264337593543950335)
(1)
(0)
(-79228162514264337593543950335)
(79228162514264337593543950335)
(prop0=79228162514264337593543950335,prop1=-79228162514264337593543950335,prop2=1,prop3=0,prop4=-79228162514264337593543950335,prop5=79228162514264337593543950335)

(0)
(1)
(-1)
(32767)
(-32768)
(prop0=0,prop1=1,prop2=-1,prop3=32767,prop4=-32768)

(0)
(1)
(-1)
(2147483647)
(-2147483648)
(prop0=0,prop1=1,prop2=-1,prop3=2147483647,prop4=-2147483648)

(0)
(1)
(-1)
(9223372036854775807)
(-9223372036854775808)
(prop0=0,prop1=1,prop2=-1,prop3=9223372036854775807,prop4=-9223372036854775808)

(binary'')
(binary'AA%3D%3D')
(binary'AAEA%2Fw%3D%3D')
(binary'AAECAwQFBgcICQoLDA0ODxA%3D')
(prop0=binary'',prop1=binary'AA%3D%3D',prop2=binary'AAEA%2Fw%3D%3D',prop3=binary'AAECAwQFBgcICQoLDA0ODxA%3D')

(9999-12-31T23%3A59%3A59.9999999Z)
(0001-01-01T00%3A00%3A00Z)
(2012-11-16T10%3A54%3A13.5422534-08%3A00)
(2012-11-16T18%3A54%3A13.5422534Z)
(prop0=9999-12-31T23%3A59%3A59.9999999Z,prop1=0001-01-01T00%3A00%3A00Z,prop2=2012-11-16T10%3A54%3A13.5422534-08%3A00,prop3=2012-11-16T18%3A54%3A13.5422534Z)

(0001-01-01)
(2014-12-31)
(9999-12-31)
(prop0=0001-01-01,prop1=2014-12-31,prop2=9999-12-31)

(00%3A00%3A00.0000000)
(12%3A20%3A04.1230000)
(23%3A59%3A59.9999999)
(prop0=00%3A00%3A00.0000000,prop1=12%3A20%3A04.1230000,prop2=23%3A59%3A59.9999999)

(duration'P10675199DT2H48M5.4775807S')
(duration'-P10675199DT2H48M5.4775808S')
(duration'P1DT12H')
(prop0=duration'P10675199DT2H48M5.4775807S',prop1=duration'-P10675199DT2H48M5.4775808S',prop2=duration'P1DT12H')

(00000000-0000-0000-0000-000000000000)
(b467459e-1eb5-4598-8a63-2c40c6a2590c)
(prop0=00000000-0000-0000-0000-000000000000,prop1=b467459e-1eb5-4598-8a63-2c40c6a2590c)

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
(prop0='',prop1='%20%20%09%20%0D%0A',prop2='.%2C%28%29%3B',prop3='%0D%0A',prop4='%0D%0A%0D%0A%0D%0A%0D%0A',prop5='%0D',prop6='%0A',prop7='%0A%0D',prop8='a%CC%82e%CC%A7%D8%A7%D9%94%D9%95',prop9='a%20surrogate%20pair%3A%20%F0%90%80%80',prop10='left%20to%20right%20%D7%90%D7%91%20%D7%AA%D7%A9%20english',prop11='%01%02%03%04%05%20')
";

            var actual = builder.ToString();
            actual.Should().Be(expected);
        }

        [Fact]
        public void ErrorPinningTest()
        {
            //Action withNull = () => DataServiceUrlConventions.Default.AppendKeyExpression(new object[1], k => "foo", k => null, new StringBuilder());
            //withNull.ShouldThrow<InvalidOperationException>().WithMessage("The serialized resource has a null value in key member 'foo'. Null values are not supported in key members.");

            //Action withUnknownType = () => DataServiceUrlConventions.Default.AppendKeyExpression(new object[1], k => "foo", k => this, new StringBuilder());
            //withUnknownType.ShouldThrow<InvalidOperationException>().WithMessage("Unable to convert value 'AstoriaUnitTests.TDD.Tests.Client.KeyGenerationPinningTest' into a key string for a URI.");
        }

        private static void RunPinningTest(string expected, params object[] keyValues)
        {
            StringBuilder builder = new StringBuilder();
            RunPinningTest(builder, keyValues);

            var actual = builder.ToString();
            actual.Should().Be(expected);
        }

        private static void RunPinningTest(StringBuilder builder, params object[] keyValues)
        {
            var compositeKey = keyValues.Select((k, i) => new { Name = "prop" + i, Value = k }).ToArray();
            var allKeys = keyValues.Select(k => new[] { new { Name = "prop", Value = k } }).Concat(new[] { compositeKey });

            foreach (var key in allKeys)
            {
                if (builder.Length > 0)
                {
                    builder.AppendLine();
                }

                var properties = key.Select(p => new EdmPropertyValue(p.Name, EdmValueUtils.ConvertPrimitiveValue(p.Value, null).Value)).ToList();
                var entityType = new EdmEntityType("Fake", "Fake");
                entityType.AddKeys(properties.Select(p => new EdmStructuralProperty(entityType, p.Name, p.Value.Type)));
                var entity = new EdmStructuredValue(new EdmEntityTypeReference(entityType, false), properties);
                ODataConventionalUriBuilder uriBuilder = new ODataConventionalUriBuilder(new Uri("http://baseuri.org/"), UrlConvention.CreateWithExplicitValue(false));
                var entityInstanceUri = uriBuilder.BuildEntityInstanceUri(new Uri("http://baseuri.org/Customers"), new Collection<KeyValuePair<string, object>>(properties.Select(p => new KeyValuePair<string, object>(p.Name, ((IEdmPrimitiveValue)p.Value).ToClrValue())).ToList()), entity.Type.FullName());
                builder.Append(entityInstanceUri.OriginalString.Replace("http://baseuri.org/Customers", null).Replace("()", "%28%29"));
            }

            builder.AppendLine();
        }
    }
}