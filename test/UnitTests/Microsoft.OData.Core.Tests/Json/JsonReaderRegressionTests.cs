//---------------------------------------------------------------------
// <copyright file="JsonReaderRegressionTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.OData.Core;
using Microsoft.OData.Json;
using Xunit;

namespace Microsoft.OData.Tests.Json
{
    public class JsonReaderRegressionTests
    {
        #region Constants

        private const string LargeNumberAsString = "3.1415926535897932384626433832795028841971693993751058209749445" +
                "92307816406286208998628034825342117067982148086513282306647093844609550582231725359408128" +
                "48111745028410270193852110555964462294895493038196442881097566593344612847564823378678316" +
                "52712019091456485669234603486104543266482133936072602491412737245870066063155881748815209" +
                "20962829254091715364367892590360011330530548820466521384146951941511609433057270365759591" +
                "95309218611738193261179310511854807446237996274956735188575272489122793818301194912983367" +
                "33624406566430860213949463952247371907021798609437027705392171762931767523846748184676694" +
                "05132000568127145263560827785771342757789609173637178721468440901224953430146549585371050" +
                "79227968925892354201995611212902196086403441815981362977477130996051870721134999999837297" +
                "80499510597317328160963185950244594553469083026425223082533446850352619311881710100031378" +
                "38752886587533208381420617177669147303598253490428755468731159562863882353787593751957781" +
                "8577805321712268066130019278766111959092164201989380952572010654858632788";

        private const string RichEntityPayload = "{\"@odata.context\":\"http://tempuri.org/$metadata#SuperEntities/$entity\"," +
            "\"Id\":1," +
            "\"BooleanProperty\":true," +
            "\"Int32Property\":13," +
            "\"SingleProperty\":3.142," +
            "\"Int16Property\":7," +
            "\"Int64Property\":6078747774547," +
            "\"DoubleProperty\":3.14159265359," +
            "\"DecimalProperty\":7654321," +
            "\"GuidProperty\":\"00000017-003b-003b-0001-020304050607\"," +
            "\"DateTimeOffsetProperty\":\"1970-12-31T23:59:59Z\"," +
            "\"TimeSpanProperty\":\"PT23H59M59S\"," +
            "\"ByteProperty\":1,\"SignedByteProperty\":9," +
            "\"StringProperty\":\"foo\"," +
            "\"DateProperty\":\"1970-01-01\"," +
            "\"TimeOfDayProperty\":\"23:59:59.0000000\"," +
            "\"ColorProperty\":\"Black\"," +
            "\"GeographyPointProperty\":{\"type\":\"Point\",\"coordinates\":[22.2,22.2],\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:4326\"}}}," +
            "\"GeometryPointProperty\":{\"type\":\"Point\",\"coordinates\":[7.0,13.0],\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:0\"}}}," +
            "\"BooleanCollectionProperty\":[true,false]," +
            "\"Int32CollectionProperty\":[13,31]," +
            "\"SingleCollectionProperty\":[3.142,241.3]," +
            "\"Int16CollectionProperty\":[7,11]," +
            "\"Int64CollectionProperty\":[6078747774547,7454777478706]," +
            "\"DoubleCollectionProperty\":[3.14159265359,95356295141.3]," +
            "\"DecimalCollectionProperty\":[7654321,1234567]," +
            "\"GuidCollectionProperty\":[\"00000017-003b-003b-0001-020304050607\",\"0000000b-001d-001d-0706-050403020100\"]," +
            "\"DateTimeOffsetCollectionProperty\":[\"1970-12-31T23:59:59Z\",\"1858-11-17T11:29:29Z\"]," +
            "\"TimeSpanCollectionProperty\":[\"PT23H59M59S\",\"PT11H29M29S\"]," +
            "\"ByteCollectionProperty\":[1,9]," +
            "\"SignedByteCollectionProperty\":[9,1]," +
            "\"StringCollectionProperty\":[\"foo\",\"bar\"]," +
            "\"DateCollectionProperty\":[\"1970-12-31\",\"1858-11-17\"]," +
            "\"TimeOfDayCollectionProperty\":[\"23:59:59.0000000\",\"11:29:29.0000000\"]," +
            "\"ColorCollectionProperty\":[\"Black\",\"White\"]," +
            "\"GeographyPointCollectionProperty\":[" +
            "{\"type\":\"Point\",\"coordinates\":[22.2,22.2],\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:4326\"}}}," +
            "{\"type\":\"Point\",\"coordinates\":[11.6,11.9],\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:4326\"}}}]," +
            "\"GeometryPointCollectionProperty\":[" +
            "{\"type\":\"Point\",\"coordinates\":[7.0,13.0],\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:0\"}}}," +
            "{\"type\":\"Point\",\"coordinates\":[13.0,7.0],\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:0\"}}}]," +
            "\"DynamicPrimitiveProperty@odata.type\":\"#Edm.Double\"," +
            "\"DynamicPrimitiveProperty\":3.14159265359," +
            "\"DynamicSpatialProperty@odata.type\":\"#Edm.GeographyPoint\"," +
            "\"DynamicSpatialProperty\":{\"type\":\"Point\",\"coordinates\":[11.1,11.1],\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:4326\"}}}," +
            "\"DynamicNullProperty@odata.type\":\"#Edm.String\"," +
            "\"DynamicNullProperty\":null," +
            "\"DynamicStringValueProperty@odata.type\":\"#Edm.String\"," +
            "\"DynamicStringValueProperty\":\"The quick brown fox jumps over the lazy dog\"," +
            "\"CoordinateProperty\":{\"Longitude\":47.64229583688,\"Latitude\":-122.13694393057}," +
            "\"EntityProperty\":{\"@odata.id\":\"http://tempuri.org/Customers(1)\",\"Id\":1,\"Name\":\"Customer 1\"}," +
            "\"CoordinateCollectionProperty\":[" +
            "{\"Longitude\":47.64229583688,\"Latitude\":-122.13694393057}," +
            "{\"Longitude\":-1.25873495895,\"Latitude\":36.80558172342}]," +
            "\"EntityCollectionProperty\":[" +
            "{\"@odata.id\":\"http://tempuri.org/Customers(1)\",\"Id\":1,\"Name\":\"Customer 1\"}," +
            "{\"@odata.id\":\"http://tempuri.org/Customers(2)\",\"Id\":2,\"Name\":\"Customer 2\"}]," +
            "\"DynamicComplexProperty\":{\"@odata.type\":\"#NS.Coordinate\",\"Longitude\":47.64229583688,\"Latitude\":-122.13694393057}," +
            "\"DynamicComplexCollectionProperty@odata.type\":\"#Collection(NS.Coordinate)\"," +
            "\"DynamicComplexCollectionProperty\":[" +
            "{\"Longitude\":47.64229583688,\"Latitude\":-122.13694393057}," +
            "{\"Longitude\":-1.25873495895,\"Latitude\":36.80558172342}]}";

        #endregion Constants

        public enum ReaderSourceKind
        {
            Buffered,   // StringReader (baseline)
            Chunked     // ChunkedStringReader (forced refills / async completion)
        }

        private static IEnumerable<object[]> ExpandWithReaderKinds(TheoryData<string> theoryData)
        {
            foreach (var dataRow in theoryData)
            {
                var payload = dataRow[0];
                yield return new object[] { payload, ReaderSourceKind.Buffered }; // baseline buffered StringReader
                yield return new object[] { payload, ReaderSourceKind.Chunked }; // chunked/refill ChunkedStringReader
            }
        }

        private static TextReader CreateTextReader(string payload, ReaderSourceKind readerKind, int chunkSize = 128) =>
            readerKind == ReaderSourceKind.Buffered ? new StringReader(payload) : new ChunkedStringReader(payload, chunkSize);

        public static TheoryData<string> NullLiteralBufferBoundaryData => new()
        {
            // A: null fully inside initial 2040-char buffer
            { "{\"IgnoreProp\":\"" + new string('s', 2007) + "\",\"NullProp\":null}" },
            // B: buffer ends just after 'n'
            { "{\"IgnoreProp\":\"" + new string('s', 2011) + "\",\"NullProp\":null}" },
            // C: buffer ends just after ':' (null entirely requires second read)
            { "{\"IgnoreProp\":\"" + new string('s', 2012) + "\",\"NullProp\":null}" },
        };

        public static TheoryData<string> BooleanLiteralBufferBoundaryData => new()
        {
            // A: boolean fully inside initial 2040-char buffer
            { "{\"IgnoreProp\":\"" + new string('s', 2007) + "\",\"BooleanProp\":true}" },
            // B: buffer ends just after 't'
            { "{\"IgnoreProp\":\"" + new string('s', 2008) + "\",\"BooleanProp\":true}" },
            // C: buffer ends just after ':' (boolean entirely requires second read)
            { "{\"IgnoreProp\":\"" + new string('s', 2009) + "\",\"BooleanProp\":true}" },
        };

        public static TheoryData<string> NumberLiteralBufferBoundaryData => new()
        {
            // A: number fully inside initial 2040-char buffer
            { "{\"IgnoreProp\":\"" + new string('s', 1992) + "\",\"NumberProp\":3.142857142857143}" },
            // B: buffer ends just after '3'
            { "{\"IgnoreProp\":\"" + new string('s', 2009) + "\",\"NumberProp\":3.142857142857143}" },
            // C: buffer ends just after ':' (number entirely requires second read)
            { "{\"IgnoreProp\":\"" + new string('s', 2010) + "\",\"NumberProp\":3.142857142857143}" },
        };

        public static TheoryData<string> StringLiteralBufferBoundaryData => new()
        {
            // A: string fully inside initial 2040-char buffer
            { "{\"IgnoreProp\":\"" + new string('s', 1964) + "\",\"StringProp\":\"The quick brown fox jumps over the lazy dog\"}" },
            // B: buffer ends just after '\"T'
            { "{\"IgnoreProp\":\"" + new string('s', 2008) + "\",\"StringProp\":\"The quick brown fox jumps over the lazy dog\"}" },
            // C: buffer ends just after ':' (string entirely requires second read)
            { "{\"IgnoreProp\":\"" + new string('s', 2010) + "\",\"StringProp\":\"The quick brown fox jumps over the lazy dog\"}" },
        };

        public static TheoryData<string> StringLiteralWithEscapeBufferBoundaryData => new()
        {
            // A: string with escape fully inside initial 2040-char buffer
            { "{\"IgnoreProp\":\"" + new string('s', 1960) + "\",\"StringProp\":\"The quick brown fox jumps over the lazy dog\\r\\n\"}" },
            // B: buffer ends just after 'dog\\'
            { "{\"IgnoreProp\":\"" + new string('s', 1965) + "\",\"StringProp\":\"The quick brown fox jumps over the lazy dog\\r\\n\"}" },
            // C: buffer ends just after 'dog\\r'
            { "{\"IgnoreProp\":\"" + new string('s', 1964) + "\",\"StringProp\":\"The quick brown fox jumps over the lazy dog\\r\\n\"}" },
        };

        public static TheoryData<string> WhitespaceBufferBoundaryData => new()
        {
            // A: whitespace fully inside initial 2040-char buffer
            { "{\"IgnoreProp\":\"" + new string('s', 1939) + "\",\"WhitespacePrecedingValueProp\":       \"The quick brown fox jumps over the lazy dog\"}" },
            // B: buffer ends just after ':'
            { "{\"IgnoreProp\":\"" + new string('s', 1992) + "\",\"WhitespacePrecedingValueProp\":       \"The quick brown fox jumps over the lazy dog\"}" },
            // C: buffer ends just after ': '
            { "{\"IgnoreProp\":\"" + new string('s', 1991) + "\",\"WhitespacePrecedingValueProp\":       \"The quick brown fox jumps over the lazy dog\"}" },
            // C: buffer ends just after ':       '
            { "{\"IgnoreProp\":\"" + new string('s', 1985) + "\",\"WhitespacePrecedingValueProp\":       \"The quick brown fox jumps over the lazy dog\"}" },
            // C: buffer ends just after ':       \"'
            { "{\"IgnoreProp\":\"" + new string('s', 1984) + "\",\"WhitespacePrecedingValueProp\":       \"The quick brown fox jumps over the lazy dog\"}" },
        };

        public static TheoryData<string> PropertyBoundaryData => new()
        {
            // A: quoted property name fully inside initial 2040-char buffer
            { "{\"Pro" + new string('o', 2029) + "p\":13}" },
            // B: buffer ends just before 'p'
            { "{\"Pro" + new string('o', 2035) + "p\":13}" },
            // A: unquoted property name fully inside initial 2040-char buffer
            { "{Pro" + new string('o', 2031) + "p:13}" },
            // B: buffer ends just before 'p'
            { "{Pro" + new string('o', 2036) + "p:13}" },
        };

        // Wrapped datasets including both source patterns reader kinds
        public static IEnumerable<object[]> NullLiteralBufferBoundaryData_WithReaderKinds =>
            ExpandWithReaderKinds(NullLiteralBufferBoundaryData);

        public static IEnumerable<object[]> BooleanLiteralBufferBoundaryData_WithReaderKinds =>
            ExpandWithReaderKinds(BooleanLiteralBufferBoundaryData);

        public static IEnumerable<object[]> NumberLiteralBufferBoundaryData_WithReaderKinds =>
            ExpandWithReaderKinds(NumberLiteralBufferBoundaryData);

        public static IEnumerable<object[]> StringLiteralBufferBoundaryData_WithReaderKinds =>
            ExpandWithReaderKinds(StringLiteralBufferBoundaryData);

        public static IEnumerable<object[]> StringLiteralWithEscapeBufferBoundaryData_WithReaderKinds =>
            ExpandWithReaderKinds(StringLiteralWithEscapeBufferBoundaryData);

        public static IEnumerable<object[]> WhitespaceBufferBoundaryData_WithReaderKinds =>
            ExpandWithReaderKinds(WhitespaceBufferBoundaryData);

        public static IEnumerable<object[]> PropertyBoundaryData_WithReaderKinds =>
            ExpandWithReaderKinds(PropertyBoundaryData);

        public static IEnumerable<object[]> RichEntityPayloadData()
        {
            var whitespaceBeforeCommaPayload = RichEntityPayload.Replace("\"@odata.context\":", "\"@odata.context\"" + new string(' ', 2048) + ":");
            var whitespaceAfterCommaPayload = RichEntityPayload.Replace("\"@odata.context\":", "\"@odata.context\":" + new string(' ', 2048));

            yield return new object[] { RichEntityPayload, ReaderSourceKind.Buffered };
            yield return new object[] { RichEntityPayload, ReaderSourceKind.Chunked };
            yield return new object[] { whitespaceBeforeCommaPayload, ReaderSourceKind.Buffered };
            yield return new object[] { whitespaceBeforeCommaPayload, ReaderSourceKind.Chunked };
            yield return new object[] { whitespaceAfterCommaPayload, ReaderSourceKind.Buffered };
            yield return new object[] { whitespaceAfterCommaPayload, ReaderSourceKind.Chunked };
        }

        [Theory]
        [MemberData(nameof(NullLiteralBufferBoundaryData_WithReaderKinds))]
        public async Task Read_NullLiteral_BufferBoundaryAsync(string payload, ReaderSourceKind sourceKind)
        {
            var textReader = CreateTextReader(payload, sourceKind);
            using (var reader = new JsonReader(textReader, isIeee754Compatible: false))
            {
                await reader.ReadAsync(); // {
                await reader.ReadAsync(); // IgnoreProp
                Assert.Equal("IgnoreProp", await reader.GetValueAsync());
                await reader.ReadAsync(); // IgnoreProp value
                Assert.Equal(new string('s', payload.Length - 33), await reader.GetValueAsync());
                await reader.ReadAsync(); // NullProp
                Assert.Equal("NullProp", await reader.GetValueAsync());
                await reader.ReadAsync(); // NullProp value
                Assert.Null(await reader.GetValueAsync());

                if (sourceKind == ReaderSourceKind.Chunked)
                {
                    Assert.True(((ChunkedStringReader)textReader).ObservedAsyncCompletion);
                }
            }
        }

        [Theory]
        [MemberData(nameof(BooleanLiteralBufferBoundaryData_WithReaderKinds))]
        public async Task Read_BooleanLiteral_BufferBoundaryAsync(string payload, ReaderSourceKind sourceKind)
        {
            var textReader = CreateTextReader(payload, sourceKind);
            using (var reader = new JsonReader(textReader, isIeee754Compatible: false))
            {
                await reader.ReadAsync(); // {
                await reader.ReadAsync(); // IgnoreProp
                Assert.Equal("IgnoreProp", await reader.GetValueAsync());
                await reader.ReadAsync(); // IgnoreProp value
                Assert.Equal(new string('s', payload.Length - 36), await reader.GetValueAsync());
                await reader.ReadAsync(); // BooleanProp
                Assert.Equal("BooleanProp", await reader.GetValueAsync());
                await reader.ReadAsync(); // BooleanProp value
                Assert.Equal(true, await reader.GetValueAsync());

                if (sourceKind == ReaderSourceKind.Chunked)
                {
                    Assert.True(((ChunkedStringReader)textReader).ObservedAsyncCompletion);
                }
            }
        }

        [Theory]
        [MemberData(nameof(NumberLiteralBufferBoundaryData_WithReaderKinds))]
        public async Task Read_NumberLiteral_BufferBoundaryAsync(string payload, ReaderSourceKind sourceKind)
        {
            var textReader = CreateTextReader(payload, sourceKind);
            using (var reader = new JsonReader(textReader, isIeee754Compatible: false))
            {
                await reader.ReadAsync(); // {
                await reader.ReadAsync(); // IgnoreProp
                Assert.Equal("IgnoreProp", await reader.GetValueAsync());
                await reader.ReadAsync(); // IgnoreProp value
                Assert.Equal(new string('s', payload.Length - 48), await reader.GetValueAsync());
                await reader.ReadAsync(); // NumberProp
                Assert.Equal("NumberProp", await reader.GetValueAsync());
                await reader.ReadAsync(); // NumberProp value
                Assert.Equal(3.142857142857143m, await reader.GetValueAsync());

                if (sourceKind == ReaderSourceKind.Chunked)
                {
                    Assert.True(((ChunkedStringReader)textReader).ObservedAsyncCompletion);
                }
            }
        }

        [Theory]
        [MemberData(nameof(StringLiteralBufferBoundaryData_WithReaderKinds))]
        public async Task Read_StringLiteral_BufferBoundaryAsync(string payload, ReaderSourceKind sourceKind)
        {
            var textReader = CreateTextReader(payload, sourceKind);
            using (var reader = new JsonReader(textReader, isIeee754Compatible: false))
            {
                await reader.ReadAsync(); // {
                await reader.ReadAsync(); // IgnoreProp
                Assert.Equal("IgnoreProp", await reader.GetValueAsync());
                await reader.ReadAsync(); // IgnoreProp value
                Assert.Equal(new string('s', payload.Length - 76), await reader.GetValueAsync());
                await reader.ReadAsync(); // StringProp
                Assert.Equal("StringProp", await reader.GetValueAsync());
                await reader.ReadAsync(); // StringProp value
                Assert.Equal("The quick brown fox jumps over the lazy dog", await reader.GetValueAsync());

                if (sourceKind == ReaderSourceKind.Chunked)
                {
                    Assert.True(((ChunkedStringReader)textReader).ObservedAsyncCompletion);
                }
            }
        }

        [Theory]
        [MemberData(nameof(StringLiteralWithEscapeBufferBoundaryData_WithReaderKinds))]
        public async Task Read_StringLiteralWithEscape_BufferBoundaryAsync(string payload, ReaderSourceKind sourceKind)
        {
            var textReader = CreateTextReader(payload, sourceKind);
            using (var reader = new JsonReader(textReader, isIeee754Compatible: false))
            {
                await reader.ReadAsync(); // {
                await reader.ReadAsync(); // IgnoreProp
                Assert.Equal("IgnoreProp", await reader.GetValueAsync());
                await reader.ReadAsync(); // IgnoreProp value
                Assert.Equal(new string('s', payload.Length - 80), await reader.GetValueAsync());
                await reader.ReadAsync(); // StringProp
                Assert.Equal("StringProp", await reader.GetValueAsync());
                await reader.ReadAsync(); // StringProp value
                Assert.Equal("The quick brown fox jumps over the lazy dog\r\n", await reader.GetValueAsync());

                if (sourceKind == ReaderSourceKind.Chunked)
                {
                    Assert.True(((ChunkedStringReader)textReader).ObservedAsyncCompletion);
                }
            }
        }

        [Theory]
        [MemberData(nameof(WhitespaceBufferBoundaryData_WithReaderKinds))]
        public async Task Read_Whitespace_BufferBoundaryAsync(string payload, ReaderSourceKind sourceKind)
        {
            var textReader = CreateTextReader(payload, sourceKind);
            using (var reader = new JsonReader(textReader, isIeee754Compatible: false))
            {
                await reader.ReadAsync(); // {
                await reader.ReadAsync(); // IgnoreProp
                Assert.Equal("IgnoreProp", await reader.GetValueAsync());
                await reader.ReadAsync(); // IgnoreProp value
                Assert.Equal(new string('s', payload.Length - 101), await reader.GetValueAsync());
                await reader.ReadAsync(); // WhitespacePrecedingValueProp
                Assert.Equal("WhitespacePrecedingValueProp", await reader.GetValueAsync());
                await reader.ReadAsync(); // WhitespacePrecedingValueProp value
                Assert.Equal("The quick brown fox jumps over the lazy dog", await reader.GetValueAsync());

                if (sourceKind == ReaderSourceKind.Chunked)
                {
                    Assert.True(((ChunkedStringReader)textReader).ObservedAsyncCompletion);
                }
            }
        }

        [Theory]
        [MemberData(nameof(PropertyBoundaryData_WithReaderKinds))]
        public async Task Read_PropertyBoundary_BufferBoundaryAsync(string payload, ReaderSourceKind sourceKind)
        {
            var textReader = CreateTextReader(payload, sourceKind);
            using (var reader = new JsonReader(textReader, isIeee754Compatible: false))
            {
                await reader.ReadAsync(); // {
                await reader.ReadAsync(); // Prop
                Assert.Equal(
                    "Pro" + new string('o', payload.Length - (payload.StartsWith("{\"Pro") ? 11 : 9)) + "p",
                    await reader.GetValueAsync());
                await reader.ReadAsync(); // Prop value
                Assert.Equal(13, await reader.GetValueAsync());

                if (sourceKind == ReaderSourceKind.Chunked)
                {
                    Assert.True(((ChunkedStringReader)textReader).ObservedAsyncCompletion);
                }
            }
        }

        [Fact]
        public async Task Read_NullProperties_ChunkedSourceAsync()
        {
            var payload = "{" + BuildNullPropertiesPayload(3072, out int propertyCount) + "}";
            var textReader = new ChunkedStringReader(payload, chunkSize: 128);

            using (var reader = new JsonReader(textReader, isIeee754Compatible: false))
            {
                await reader.ReadAsync(); // {
                int i = 0;
                while (i < propertyCount)
                {
                    await reader.ReadAsync(); // Property name
                    Assert.Equal($"Property{i + 1}", await reader.GetValueAsync());
                    await reader.ReadAsync(); // Property value
                    Assert.Null(await reader.GetValueAsync());
                    i++;
                }
            }

            Assert.True(textReader.ObservedAsyncCompletion);
        }

        [Fact]
        public async Task Read_NumberProperties_ChunkedSourceAsync()
        {
            var payload = "{" + BuildNumberPropertiesPayload(3072, out int propertyCount) + "}";
            var textReader = new ChunkedStringReader(payload, chunkSize: 128);

            using (var reader = new JsonReader(textReader, isIeee754Compatible: false))
            {
                await reader.ReadAsync(); // {
                int i = 0;
                while (i < propertyCount)
                {
                    await reader.ReadAsync(); // Property name
                    Assert.Equal($"Property{i + 1}", await reader.GetValueAsync());
                    await reader.ReadAsync(); // Property value
                    Assert.Equal(i + 1, await reader.GetValueAsync());
                    i++;
                }
            }

            Assert.True(textReader.ObservedAsyncCompletion);
        }

        [Fact]
        public async Task Read_BooleanProperties_ChunkedSourceAsync()
        {
            var payload = "{" + BuildBooleanPropertiesPayload(3072, out int propertyCount) + "}";
            var textReader = new ChunkedStringReader(payload, chunkSize: 128);

            using (var reader = new JsonReader(textReader, isIeee754Compatible: false))
            {
                await reader.ReadAsync(); // {
                int i = 0;
                while (i < propertyCount)
                {
                    await reader.ReadAsync(); // Property name
                    Assert.Equal($"Property{i + 1}", await reader.GetValueAsync());
                    await reader.ReadAsync(); // Property value
                    Assert.Equal((i + 1) % 2 == 0, await reader.GetValueAsync());
                    i++;
                }
            }

            Assert.True(textReader.ObservedAsyncCompletion);
        }

        [Fact]
        public async Task Read_StringProperties_ChunkedSourceAsync()
        {
            var payload = "{" + BuildStringPropertiesPayload(3072, out int propertyCount) + "}";
            var textReader = new ChunkedStringReader(payload, chunkSize: 128);

            using (var reader = new JsonReader(textReader, isIeee754Compatible: false))
            {
                await reader.ReadAsync(); // {
                int i = 0;
                while (i < propertyCount)
                {
                    await reader.ReadAsync(); // Property name
                    Assert.Equal($"Property{i + 1}", await reader.GetValueAsync());
                    await reader.ReadAsync(); // Property value
                    Assert.Equal($"String{i + 1}", await reader.GetValueAsync());
                    i++;
                }
            }

            Assert.True(textReader.ObservedAsyncCompletion);
        }

        [Fact]
        public async Task Read_LargeNumberLiteral_ChunkedSourceAsync()
        {
            // Long number ensures multiple refills occur triggering slow path
            var payload = "{\"NumberProp\":" + LargeNumberAsString + "}";
            var textReader = new ChunkedStringReader(payload, chunkSize: 128);

            using (var reader = new JsonReader(textReader, isIeee754Compatible: false))
            {
                await reader.ReadAsync(); // {
                await reader.ReadAsync(); // NumberProp
                Assert.Equal("NumberProp", await reader.GetValueAsync());
                await reader.ReadAsync(); // NumberProp value
                Assert.True(3.14m <= Assert.IsType<decimal>(await reader.GetValueAsync()));
            }

            Assert.True(textReader.ObservedAsyncCompletion);
        }

        [Fact]
        public async Task Read_LargeNumberLiteral_EOF_ChunkedSourceAsync()
        {
            var payload = "{\"NumberProp\":" + LargeNumberAsString;
            var textReader = new ChunkedStringReader(payload, chunkSize: 128);

            using (var reader = new JsonReader(textReader, isIeee754Compatible: false))
            {
                await reader.ReadAsync(); // {
                await reader.ReadAsync(); // NumberProp
                Assert.Equal("NumberProp", await reader.GetValueAsync());
                await reader.ReadAsync(); // NumberProp value
                Assert.True(3.14m <= Assert.IsType<decimal>(await reader.GetValueAsync()));
            }

            Assert.True(textReader.ObservedAsyncCompletion);
        }

        [Fact]
        public async Task Read_Array_FirstElementAfterLargeWhitespace_ChunkedSourceAsync()
        {
            var payload = "{\"ArrayProp\":[" + new string(' ', 2026) + "13,7]}";
            var textReader = new ChunkedStringReader(payload, chunkSize: 128);

            using (var reader = new JsonReader(textReader, isIeee754Compatible: false))
            {
                await reader.ReadAsync(); // {
                await reader.ReadAsync(); // ArrayProp
                Assert.Equal("ArrayProp", await reader.GetValueAsync());
                await reader.ReadAsync(); // [
                Assert.Equal(JsonNodeType.StartArray, reader.NodeType);
                await reader.ReadAsync(); // Item 1
                Assert.Equal(13, await reader.GetValueAsync());
                await reader.ReadAsync(); // Item 2
                Assert.Equal(7, await reader.GetValueAsync());
                await reader.ReadAsync(); // ]
                Assert.Equal(JsonNodeType.EndArray, reader.NodeType);
                await reader.ReadAsync(); // }
            }

            Assert.True(textReader.ObservedAsyncCompletion);
        }

        [Theory]
        [InlineData(ReaderSourceKind.Chunked)]
        [InlineData(ReaderSourceKind.Buffered)]
        public async Task Read_EscapedStringLiteralAsync(ReaderSourceKind sourceKind)
        {
            var escapedSegments = new[]
            {
                "\r", "\n", "\t", "\b", "\f", "\\", "\"", "\'", "\u00A0", "\uD834\uDD1E"
            };

            // Large payload helps us ensure we hit slow paths
            var payload = "{" + BuildEscapedStringPropertiesPayload(16384, out int propertyCount) + "}";
            var textReader = CreateTextReader(payload, sourceKind, chunkSize: 128);

            using (var reader = new JsonReader(textReader, isIeee754Compatible: false))
            {
                await reader.ReadAsync(); // {
                int i = 0;

                while (i < propertyCount)
                {
                    var escapedSegment = escapedSegments[i % 10];

                    await reader.ReadAsync(); // Property name
                    Assert.Equal($"Property{i + 1}", await reader.GetValueAsync());
                    await reader.ReadAsync(); // Property value
                    var propertyValue = await reader.GetValueAsync();
                    Assert.Equal($"String{escapedSegment}{i + 1}", await reader.GetValueAsync());
                    i++;
                }
            }

            if (sourceKind == ReaderSourceKind.Chunked)
            {
                Assert.True(((ChunkedStringReader)textReader).ObservedAsyncCompletion);
            }
        }

        [Theory]
        [MemberData(nameof(RichEntityPayloadData))]
        public async Task Read_RichEntityPayloadAsync(string payload, ReaderSourceKind sourceKind)
        {
            var textReader = CreateTextReader(payload, sourceKind, chunkSize: 128);

            using (var reader = new JsonReader(textReader, isIeee754Compatible: false))
            {
                await reader.ReadAsync(); // {
                Assert.Equal(JsonNodeType.StartObject, reader.NodeType);
                await ReadPropertyAsync(reader, "@odata.context", "http://tempuri.org/$metadata#SuperEntities/$entity");
                await ReadPropertyAsync(reader, "Id", 1);
                await ReadPropertyAsync(reader, "BooleanProperty", true);
                await ReadPropertyAsync(reader, "Int32Property", 13);
                await ReadPropertyAsync(reader, "SingleProperty", 3.142m); // Read as decimal
                await ReadPropertyAsync(reader, "Int16Property", 7); // Read as Int32
                await ReadPropertyAsync(reader, "Int64Property", 6078747774547m); // Read as decimal
                await ReadPropertyAsync(reader, "DoubleProperty", 3.14159265359m); // Read as decimal
                await ReadPropertyAsync(reader, "DecimalProperty", 7654321); // Read as Int32
                await ReadPropertyAsync(reader, "GuidProperty", "00000017-003b-003b-0001-020304050607"); // Read as string
                await ReadPropertyAsync(reader, "DateTimeOffsetProperty", "1970-12-31T23:59:59Z"); // Read as string
                await ReadPropertyAsync(reader, "TimeSpanProperty", "PT23H59M59S"); // Read as string
                await ReadPropertyAsync(reader, "ByteProperty", 1); // Read as Int32
                await ReadPropertyAsync(reader, "SignedByteProperty", 9); // Read as Int32
                await ReadPropertyAsync(reader, "StringProperty", "foo");
                await ReadPropertyAsync(reader, "DateProperty", "1970-01-01"); // Read as string
                await ReadPropertyAsync(reader, "TimeOfDayProperty", "23:59:59.0000000"); // Read as string
                await ReadPropertyAsync(reader, "ColorProperty", "Black");
                await ReadPropertyNameAsync(reader, "GeographyPointProperty");
                await ReadSpatialAsync(reader, 22.2m, 22.2m, "EPSG:4326");
                await ReadPropertyNameAsync(reader, "GeometryPointProperty");
                await ReadSpatialAsync(reader, 7.0m, 13.0m, "EPSG:0");
                await ReadPropertyNameAsync(reader, "BooleanCollectionProperty");
                await reader.ReadAsync(); // [
                Assert.Equal(JsonNodeType.StartArray, reader.NodeType);
                await ReadItemAsync(reader, true); // Item 1
                await ReadItemAsync(reader, false); // Item 2
                await reader.ReadAsync(); // ]
                Assert.Equal(JsonNodeType.EndArray, reader.NodeType);
                await ReadPropertyNameAsync(reader, "Int32CollectionProperty");
                await reader.ReadAsync(); // [
                Assert.Equal(JsonNodeType.StartArray, reader.NodeType);
                await ReadItemAsync(reader, 13); // Item 1
                await ReadItemAsync(reader, 31); // Item 2
                await reader.ReadAsync(); // ]
                Assert.Equal(JsonNodeType.EndArray, reader.NodeType);
                await ReadPropertyNameAsync(reader, "SingleCollectionProperty");
                await reader.ReadAsync(); // [
                Assert.Equal(JsonNodeType.StartArray, reader.NodeType);
                await ReadItemAsync(reader, 3.142m); // Item 1
                await ReadItemAsync(reader, 241.3m); // Item 2
                await reader.ReadAsync(); // ]
                Assert.Equal(JsonNodeType.EndArray, reader.NodeType);
                await ReadPropertyNameAsync(reader, "Int16CollectionProperty");
                await reader.ReadAsync(); // [
                Assert.Equal(JsonNodeType.StartArray, reader.NodeType);
                await ReadItemAsync(reader, 7); // Item 1
                await ReadItemAsync(reader, 11); // Item 2
                await reader.ReadAsync(); // ]
                Assert.Equal(JsonNodeType.EndArray, reader.NodeType);
                await ReadPropertyNameAsync(reader, "Int64CollectionProperty");
                await reader.ReadAsync(); // [
                Assert.Equal(JsonNodeType.StartArray, reader.NodeType);
                await ReadItemAsync(reader, 6078747774547m); // Item 1
                await ReadItemAsync(reader, 7454777478706m); // Item 2
                await reader.ReadAsync(); // ]
                Assert.Equal(JsonNodeType.EndArray, reader.NodeType);
                await ReadPropertyNameAsync(reader, "DoubleCollectionProperty");
                await reader.ReadAsync(); // [
                Assert.Equal(JsonNodeType.StartArray, reader.NodeType);
                await ReadItemAsync(reader, 3.14159265359m); // Item 1
                await ReadItemAsync(reader, 95356295141.3m); // Item 2
                await reader.ReadAsync(); // ]
                Assert.Equal(JsonNodeType.EndArray, reader.NodeType);
                await ReadPropertyNameAsync(reader, "DecimalCollectionProperty");
                await reader.ReadAsync(); // [
                Assert.Equal(JsonNodeType.StartArray, reader.NodeType);
                await ReadItemAsync(reader, 7654321); // Item 1
                await ReadItemAsync(reader, 1234567); // Item 2
                await reader.ReadAsync(); // ]
                Assert.Equal(JsonNodeType.EndArray, reader.NodeType);
                await ReadPropertyNameAsync(reader, "GuidCollectionProperty");
                await reader.ReadAsync(); // [
                Assert.Equal(JsonNodeType.StartArray, reader.NodeType);
                await ReadItemAsync(reader, "00000017-003b-003b-0001-020304050607"); // Item 1
                await ReadItemAsync(reader, "0000000b-001d-001d-0706-050403020100"); // Item 2
                await reader.ReadAsync(); // ]
                Assert.Equal(JsonNodeType.EndArray, reader.NodeType);
                await ReadPropertyNameAsync(reader, "DateTimeOffsetCollectionProperty");
                await reader.ReadAsync(); // [
                Assert.Equal(JsonNodeType.StartArray, reader.NodeType);
                await ReadItemAsync(reader, "1970-12-31T23:59:59Z"); // Item 1
                await ReadItemAsync(reader, "1858-11-17T11:29:29Z"); // Item 2
                await reader.ReadAsync(); // ]
                Assert.Equal(JsonNodeType.EndArray, reader.NodeType);
                await ReadPropertyNameAsync(reader, "TimeSpanCollectionProperty");
                await reader.ReadAsync(); // [
                Assert.Equal(JsonNodeType.StartArray, reader.NodeType);
                await ReadItemAsync(reader, "PT23H59M59S"); // Item 1
                await ReadItemAsync(reader, "PT11H29M29S"); // Item 2
                await reader.ReadAsync(); // ]
                Assert.Equal(JsonNodeType.EndArray, reader.NodeType);
                await ReadPropertyNameAsync(reader, "ByteCollectionProperty");
                await reader.ReadAsync(); // [
                Assert.Equal(JsonNodeType.StartArray, reader.NodeType);
                await ReadItemAsync(reader, 1); // Item 1
                await ReadItemAsync(reader, 9); // Item 2
                await reader.ReadAsync(); // ]
                Assert.Equal(JsonNodeType.EndArray, reader.NodeType);
                await ReadPropertyNameAsync(reader, "SignedByteCollectionProperty");
                await reader.ReadAsync(); // [
                Assert.Equal(JsonNodeType.StartArray, reader.NodeType);
                await ReadItemAsync(reader, 9); // Item 1
                await ReadItemAsync(reader, 1); // Item 2
                await reader.ReadAsync(); // ]
                Assert.Equal(JsonNodeType.EndArray, reader.NodeType);
                await ReadPropertyNameAsync(reader, "StringCollectionProperty");
                await reader.ReadAsync(); // [
                Assert.Equal(JsonNodeType.StartArray, reader.NodeType);
                await ReadItemAsync(reader, "foo"); // Item 1
                await ReadItemAsync(reader, "bar"); // Item 2
                await reader.ReadAsync(); // ]
                Assert.Equal(JsonNodeType.EndArray, reader.NodeType);
                await ReadPropertyNameAsync(reader, "DateCollectionProperty");
                await reader.ReadAsync(); // [
                Assert.Equal(JsonNodeType.StartArray, reader.NodeType);
                await ReadItemAsync(reader, "1970-12-31"); // Item 1
                await ReadItemAsync(reader, "1858-11-17"); // Item 2
                await reader.ReadAsync(); // ]
                Assert.Equal(JsonNodeType.EndArray, reader.NodeType);
                await ReadPropertyNameAsync(reader, "TimeOfDayCollectionProperty");
                await reader.ReadAsync(); // [
                Assert.Equal(JsonNodeType.StartArray, reader.NodeType);
                await ReadItemAsync(reader, "23:59:59.0000000"); // Item 1
                await ReadItemAsync(reader, "11:29:29.0000000"); // Item 2
                await reader.ReadAsync(); // ]
                Assert.Equal(JsonNodeType.EndArray, reader.NodeType);
                await ReadPropertyNameAsync(reader, "ColorCollectionProperty");
                await reader.ReadAsync(); // [
                Assert.Equal(JsonNodeType.StartArray, reader.NodeType);
                await ReadItemAsync(reader, "Black"); // Item 1
                await ReadItemAsync(reader, "White"); // Item 2
                await reader.ReadAsync(); // ]
                Assert.Equal(JsonNodeType.EndArray, reader.NodeType);
                await ReadPropertyNameAsync(reader, "GeographyPointCollectionProperty");
                await reader.ReadAsync(); // [
                Assert.Equal(JsonNodeType.StartArray, reader.NodeType);
                await ReadSpatialAsync(reader, 22.2m, 22.2m, "EPSG:4326");
                await ReadSpatialAsync(reader, 11.6m, 11.9m, "EPSG:4326");
                await reader.ReadAsync(); // ]
                Assert.Equal(JsonNodeType.EndArray, reader.NodeType);
                await ReadPropertyNameAsync(reader, "GeometryPointCollectionProperty");
                await reader.ReadAsync(); // [
                Assert.Equal(JsonNodeType.StartArray, reader.NodeType);
                await ReadSpatialAsync(reader, 7.0m, 13.0m, "EPSG:0");
                await ReadSpatialAsync(reader, 13.0m, 7.0m, "EPSG:0");
                await reader.ReadAsync(); // ]
                Assert.Equal(JsonNodeType.EndArray, reader.NodeType);
                await ReadPropertyAsync(reader, "DynamicPrimitiveProperty@odata.type", "#Edm.Double");
                await ReadPropertyAsync(reader, "DynamicPrimitiveProperty", 3.14159265359m);
                await ReadPropertyAsync(reader, "DynamicSpatialProperty@odata.type", "#Edm.GeographyPoint");
                await ReadPropertyNameAsync(reader, "DynamicSpatialProperty");
                await ReadSpatialAsync(reader, 11.1m, 11.1m, "EPSG:4326");
                await ReadPropertyAsync(reader, "DynamicNullProperty@odata.type", "#Edm.String");
                await ReadPropertyAsync(reader, "DynamicNullProperty", null);
                await ReadPropertyAsync(reader, "DynamicStringValueProperty@odata.type", "#Edm.String");
                await ReadPropertyAsync(reader, "DynamicStringValueProperty", "The quick brown fox jumps over the lazy dog");
                await ReadPropertyNameAsync(reader, "CoordinateProperty");
                await ReadComplexAsync(reader, 47.64229583688m, -122.13694393057m);
                await ReadPropertyNameAsync(reader, "EntityProperty");
                await AssertEntityAsync(reader, "http://tempuri.org/Customers(1)", 1, "Customer 1");
                await ReadPropertyNameAsync(reader, "CoordinateCollectionProperty");
                await reader.ReadAsync(); // [
                Assert.Equal(JsonNodeType.StartArray, reader.NodeType);
                await ReadComplexAsync(reader, 47.64229583688m, -122.13694393057m);
                await ReadComplexAsync(reader, -1.25873495895m, 36.80558172342m);
                await reader.ReadAsync(); // ]
                Assert.Equal(JsonNodeType.EndArray, reader.NodeType);
                await ReadPropertyNameAsync(reader, "EntityCollectionProperty");
                await reader.ReadAsync(); // [
                Assert.Equal(JsonNodeType.StartArray, reader.NodeType);
                await AssertEntityAsync(reader, "http://tempuri.org/Customers(1)", 1, "Customer 1");
                await AssertEntityAsync(reader, "http://tempuri.org/Customers(2)", 2, "Customer 2");
                await reader.ReadAsync(); // ]
                Assert.Equal(JsonNodeType.EndArray, reader.NodeType);
                await ReadPropertyNameAsync(reader, "DynamicComplexProperty");
                await reader.ReadAsync(); // {
                Assert.Equal(JsonNodeType.StartObject, reader.NodeType);
                await ReadPropertyAsync(reader, "@odata.type", "#NS.Coordinate");
                await ReadPropertyAsync(reader, "Longitude", 47.64229583688m);
                await ReadPropertyAsync(reader, "Latitude", -122.13694393057m);
                await reader.ReadAsync(); // }
                Assert.Equal(JsonNodeType.EndObject, reader.NodeType);
                await ReadPropertyAsync(reader, "DynamicComplexCollectionProperty@odata.type", "#Collection(NS.Coordinate)");
                await ReadPropertyNameAsync(reader, "DynamicComplexCollectionProperty");
                await reader.ReadAsync(); // [
                Assert.Equal(JsonNodeType.StartArray, reader.NodeType);
                await ReadComplexAsync(reader, 47.64229583688m, -122.13694393057m);
                await ReadComplexAsync(reader, -1.25873495895m, 36.80558172342m);
                await reader.ReadAsync(); // ]
                Assert.Equal(JsonNodeType.EndArray, reader.NodeType);
                await reader.ReadAsync(); // }
                Assert.Equal(JsonNodeType.EndObject, reader.NodeType);
            }

            if (sourceKind == ReaderSourceKind.Chunked)
            {
                Assert.True(((ChunkedStringReader)textReader).ObservedAsyncCompletion);
            }

            static async ValueTask ReadPropertyNameAsync(JsonReader readerParam, string propertyName)
            {
                await readerParam.ReadAsync(); // Property name
                Assert.Equal(propertyName, await readerParam.GetValueAsync());
            }

            static async ValueTask ReadPropertyAsync(JsonReader readerParam, string propertyName, object propertyValue)
            {
                await ReadPropertyNameAsync(readerParam, propertyName);
                await readerParam.ReadAsync(); // Property value
                Assert.Equal(propertyValue, await readerParam.GetValueAsync());
            }

            static async ValueTask ReadItemAsync(JsonReader readerParam, object itemValue)
            {
                await readerParam.ReadAsync(); // Item
                Assert.Equal(itemValue, await readerParam.GetValueAsync());
            }

            static async ValueTask ReadSpatialAsync(JsonReader readerParam, decimal longitudeOrX, decimal latitudeOrX, string epsg)
            {
                await readerParam.ReadAsync(); // {
                Assert.Equal(JsonNodeType.StartObject, readerParam.NodeType);
                await ReadPropertyAsync(readerParam, "type", "Point");
                await ReadPropertyNameAsync(readerParam, "coordinates");
                await readerParam.ReadAsync(); // [
                Assert.Equal(JsonNodeType.StartArray, readerParam.NodeType);
                await ReadItemAsync(readerParam, longitudeOrX); // Longitude or X
                await ReadItemAsync(readerParam, latitudeOrX); // Latitude or Y
                await readerParam.ReadAsync(); // ]
                Assert.Equal(JsonNodeType.EndArray, readerParam.NodeType);
                await ReadPropertyNameAsync(readerParam, "crs");
                await readerParam.ReadAsync(); // {
                Assert.Equal(JsonNodeType.StartObject, readerParam.NodeType);
                await ReadPropertyAsync(readerParam, "type", "name");
                await ReadPropertyNameAsync(readerParam, "properties");
                await readerParam.ReadAsync(); // {
                Assert.Equal(JsonNodeType.StartObject, readerParam.NodeType);
                await ReadPropertyAsync(readerParam, "name", epsg);
                await readerParam.ReadAsync(); //  }
                Assert.Equal(JsonNodeType.EndObject, readerParam.NodeType);
                await readerParam.ReadAsync(); //   }
                Assert.Equal(JsonNodeType.EndObject, readerParam.NodeType);
                await readerParam.ReadAsync(); //  }
                Assert.Equal(JsonNodeType.EndObject, readerParam.NodeType);
            }

            static async ValueTask ReadComplexAsync(JsonReader readerParam, decimal longitude, decimal latitude)
            {
                await readerParam.ReadAsync(); // {
                Assert.Equal(JsonNodeType.StartObject, readerParam.NodeType);
                await ReadPropertyAsync(readerParam, "Longitude", longitude);
                await ReadPropertyAsync(readerParam, "Latitude", latitude);
                await readerParam.ReadAsync(); // }
                Assert.Equal(JsonNodeType.EndObject, readerParam.NodeType);
            }

            static async ValueTask AssertEntityAsync(JsonReader readerParam, string odataId, int id, string name)
            {
                await readerParam.ReadAsync(); // {
                Assert.Equal(JsonNodeType.StartObject, readerParam.NodeType);
                await ReadPropertyAsync(readerParam, "@odata.id", odataId);
                await ReadPropertyAsync(readerParam, "Id", id);
                await ReadPropertyAsync(readerParam, "Name", name);
                await readerParam.ReadAsync(); // }
                Assert.Equal(JsonNodeType.EndObject, readerParam.NodeType);
            }
        }

        [Theory]
        [InlineData(ReaderSourceKind.Chunked, "\\", "\\1")]
        [InlineData(ReaderSourceKind.Chunked, "\\\"\\", "\\1")]
        [InlineData(ReaderSourceKind.Chunked, "\\'\\", "\\1")]
        [InlineData(ReaderSourceKind.Chunked, "\\u", "\\u149\"")]
        [InlineData(ReaderSourceKind.Buffered, "\\", "\\1")]
        [InlineData(ReaderSourceKind.Buffered, "\\\"\\", "\\1")]
        [InlineData(ReaderSourceKind.Buffered, "\\'\\", "\\1")]
        [InlineData(ReaderSourceKind.Buffered, "\\u", "\\u149\"")]
        public async Task Read_IncompleteEscapeSequence_ThrowsAsync(ReaderSourceKind sourceKind, string sequence, string fragment)
        {
            var escapedSegments = new[]
            {
                "\r", "\n", "\t", "\b", "\f", "\\", "\"", "\'", "\u00A0", "\uD834\uDD1E"
            };

            // Large payload helps us ensure we hit slow paths in escape sequence handling
            var escapedProperties = BuildEscapedStringPropertiesPayload(4096, out int propertyCount);
            var nextProperty = propertyCount + 1;
            escapedProperties += $",\"Property{nextProperty}\":\"String{sequence}{nextProperty}\"";
            var payload = "{" + escapedProperties + "}";
            var textReader = CreateTextReader(payload, sourceKind, chunkSize: 128);

            using (var reader = new JsonReader(textReader, isIeee754Compatible: false))
            {
                await reader.ReadAsync(); // {
                int i = 0;

                while (i < propertyCount)
                {
                    var escapedSegment = escapedSegments[i % 10];
                    
                    await reader.ReadAsync(); // Property name
                    Assert.Equal($"Property{i + 1}", await reader.GetValueAsync());
                    await reader.ReadAsync(); // Property value
                    var propertyValue = await reader.GetValueAsync();
                    Assert.Equal($"String{escapedSegment}{i + 1}", await reader.GetValueAsync());
                    i++;
                }

                // Read property with incomplete escape sequence
                await reader.ReadAsync(); // Property name
                Assert.Equal($"Property{nextProperty}", await reader.GetValueAsync());
                await reader.ReadAsync(); // Property value
                var exception = await Assert.ThrowsAsync<ODataException>(reader.GetValueAsync);

                Assert.Equal(
                    $"Invalid JSON. An unrecognized escape sequence '{fragment}' was found in a JSON string value.",
                    exception.Message);
            }

            if (sourceKind == ReaderSourceKind.Chunked)
            {
                Assert.True(((ChunkedStringReader)textReader).ObservedAsyncCompletion);
            }
        }

        [Theory]
        [InlineData(ReaderSourceKind.Chunked, "\\", "\\")]
        [InlineData(ReaderSourceKind.Chunked, "\\\"\\", "\\")]
        [InlineData(ReaderSourceKind.Chunked, "\\'\\", "\\")]
        [InlineData(ReaderSourceKind.Chunked, "\\u", "\\u")]
        [InlineData(ReaderSourceKind.Chunked, "\\u00", "\\u00")]
        [InlineData(ReaderSourceKind.Buffered, "\\", "\\")]
        [InlineData(ReaderSourceKind.Buffered, "\\\"\\", "\\")]
        [InlineData(ReaderSourceKind.Buffered, "\\'\\", "\\")]
        [InlineData(ReaderSourceKind.Buffered, "\\u", "\\u")]
        [InlineData(ReaderSourceKind.Buffered, "\\u00", "\\u00")]
        public async Task Read_EofAtEndOfEscapeSequence_ThrowsAsync(ReaderSourceKind sourceKind, string sequence, string fragment)
        {
            var escapedSegments = new[]
            {
                "\r", "\n", "\t", "\b", "\f", "\\", "\"", "\'", "\u00A0", "\uD834\uDD1E"
            };

            // Large payload helps us ensure we hit slow paths in escape sequence handling
            var escapedProperties = BuildEscapedStringPropertiesPayload(4096, out int propertyCount);
            var nextProperty = propertyCount + 1;
            escapedProperties += $",\"Property{nextProperty}\":\"String{sequence}";
            var payload = "{" + escapedProperties;
            var textReader = CreateTextReader(payload, sourceKind, chunkSize: 128);

            using (var reader = new JsonReader(textReader, isIeee754Compatible: false))
            {
                await reader.ReadAsync(); // {
                int i = 0;

                while (i < propertyCount)
                {
                    var escapedSegment = escapedSegments[i % 10];
                    
                    await reader.ReadAsync(); // Property name
                    Assert.Equal($"Property{i + 1}", await reader.GetValueAsync());
                    await reader.ReadAsync(); // Property value
                    var propertyValue = await reader.GetValueAsync();
                    Assert.Equal($"String{escapedSegment}{i + 1}", await reader.GetValueAsync());
                    i++;
                }

                // Read property with incomplete escape sequence
                await reader.ReadAsync(); // Property name
                Assert.Equal($"Property{nextProperty}", await reader.GetValueAsync());
                await reader.ReadAsync(); // Property value
                var exception = await Assert.ThrowsAsync<ODataException>(reader.GetValueAsync);

                Assert.Equal(
                    $"Invalid JSON. An unrecognized escape sequence '{fragment}' was found in a JSON string value.",
                    exception.Message);
            }

            if (sourceKind == ReaderSourceKind.Chunked)
            {
                Assert.True(((ChunkedStringReader)textReader).ObservedAsyncCompletion);
            }
        }

        [Theory]
        [InlineData(ReaderSourceKind.Chunked)]
        [InlineData(ReaderSourceKind.Buffered)]
        public async Task Read_EofAtEndOfLongUnterminatedStringLiteral_ThrowsAsync(ReaderSourceKind sourceKind)
        {
            // Large payload helps us ensure we hit slow paths
            var payload = "{\"Property\":\"" + new string('s', 2048);
            var textReader = CreateTextReader(payload, sourceKind, chunkSize: 128);

            using (var reader = new JsonReader(textReader, isIeee754Compatible: false))
            {
                await reader.ReadAsync(); // {
                await reader.ReadAsync(); // Property name
                Assert.Equal($"Property", await reader.GetValueAsync());
                await reader.ReadAsync(); // Property value
                var exception = await Assert.ThrowsAsync<ODataException>(reader.GetValueAsync);

                Assert.Equal(
                    "Invalid JSON. Unexpected end of input reached while processing a JSON string value.",
                    exception.Message);
            }

            if (sourceKind == ReaderSourceKind.Chunked)
            {
                Assert.True(((ChunkedStringReader)textReader).ObservedAsyncCompletion);
            }
        }

        [Fact]
        public async Task Read_PropertyNameQuoted_MissingColon_ChunkedSource_ThrowsAsync()
        {
            var propertyName = "D" + new string('a', 128) + "ta";
            var payload = "{\"" + propertyName + "\"}";
            var textReader = new ChunkedStringReader(payload, chunkSize: 128);

            using (var reader = new JsonReader(textReader, isIeee754Compatible: false))
            {
                await reader.ReadAsync(); // {
                var exception = await Assert.ThrowsAsync<ODataException>(reader.ReadAsync); // Property name

                Assert.Equal(
                    Error.Format(SRResources.JsonReader_MissingColon, propertyName),
                    exception.Message);
            }
        }

        [Fact]
        public async Task Read_PropertyNameUnquoted_MissingColon_ChunkedSource_ThrowsAsync()
        {
            var propertyName = "D" + new string('a', 128) + "ta";
            var payload = "{" + propertyName + "}";
            var textReader = new ChunkedStringReader(payload, chunkSize: 128);

            using (var reader = new JsonReader(textReader, isIeee754Compatible: false))
            {
                await reader.ReadAsync(); // {
                var exception = await Assert.ThrowsAsync<ODataException>(reader.ReadAsync); // Property name

                Assert.Equal(
                    Error.Format(SRResources.JsonReader_MissingColon, propertyName),
                    exception.Message);
            }
        }

        [Fact]
        public async Task Read_BooleanLiteral_InvalidToken_ChunkedSource_ThrowsAsync()
        {
            var payload = "{\"D" + new string('a', 120) + "ta\":treu}";
            var textReader = new ChunkedStringReader(payload, chunkSize: 128);

            using (var reader = new JsonReader(textReader, isIeee754Compatible: false))
            {
                await reader.ReadAsync(); // {
                await reader.ReadAsync(); // Property name
                await reader.GetValueAsync();
                var exception = await Assert.ThrowsAsync<ODataException>(reader.ReadAsync); // Property value

                Assert.Equal(
                    Error.Format(SRResources.JsonReader_UnexpectedToken, "treu"),
                    exception.Message);
            }
        }

        [Fact]
        public async Task Read_NullLiteral_InvalidToken_ChunkedSource_ThrowsAsync()
        {
            var payload = "{\"IgnoreProp\":\"" + new string('s', 2019) + "\",\"NullProp\":nil}";
            using (var reader = new JsonReader(new ChunkedStringReader(payload, chunkSize: 128), isIeee754Compatible: false))
            {
                await reader.ReadAsync(); // {
                await reader.ReadAsync(); // IgnoreProp
                Assert.Equal("IgnoreProp", await reader.GetValueAsync());
                await reader.ReadAsync(); // IgnoreProp value
                await reader.GetValueAsync();
                await reader.ReadAsync(); // NullProp
                Assert.Equal("NullProp", await reader.GetValueAsync());
                await reader.ReadAsync(); // NullProp value
                var exception = await Assert.ThrowsAsync<ODataException>(reader.GetValueAsync);

                Assert.Equal(Error.Format(SRResources.JsonReader_UnexpectedToken, "nil"), exception.Message);
            }
        }

        [Fact]
        public async Task Read_StringLiteral_InvalidEscapeSequence_ChunkedSource_ThrowsAsync()
        {
            var payload = "{\"IgnoreProp\":\"" + new string('s', 1965) + "\",\"StringProp\":\"The quick brown fox jumps over the lazy dog\\";
            using (var reader = new JsonReader(new ChunkedStringReader(payload, chunkSize: 18), isIeee754Compatible: false))
            {
                await reader.ReadAsync(); // {
                await reader.ReadAsync(); // IgnoreProp
                Assert.Equal("IgnoreProp", await reader.GetValueAsync());
                await reader.ReadAsync(); // IgnoreProp value
                await reader.GetValueAsync();
                await reader.ReadAsync(); // StringProp
                Assert.Equal("StringProp", await reader.GetValueAsync());
                await reader.ReadAsync(); // StringProp value
                var exception = await Assert.ThrowsAsync<ODataException>(reader.GetValueAsync);

                Assert.Equal(
                    Error.Format(SRResources.JsonReader_UnrecognizedEscapeSequence, "\\"),
                    exception.Message);
            }
        }

        static string BuildNullPropertiesPayload(int lengthHint, out int propertyCount)
        {
            var builder = new StringBuilder(lengthHint + 64);
            int i = 1;
            bool first = true;
            while (builder.Length < lengthHint)
            {
                if (!first)
                {
                    builder.Append(',');
                }

                first = false;
                builder.Append("\"Property");
                builder.Append(i);
                builder.Append("\":null");
                i++;
            }

            propertyCount = i - 1;

            return builder.ToString();
        }

        static string BuildNumberPropertiesPayload(int lengthHint, out int propertyCount)
        {
            var builder = new StringBuilder(lengthHint + 64);
            int i = 1;
            bool first = true;
            while (builder.Length < lengthHint)
            {
                if (!first)
                {
                    builder.Append(',');
                }

                first = false;
                builder.Append("\"Property");
                builder.Append(i);
                builder.Append("\":");
                builder.Append(i);
                i++;
            }

            propertyCount = i - 1;

            return builder.ToString();
        }

        static string BuildBooleanPropertiesPayload(int lengthHint, out int propertyCount)
        {
            var builder = new StringBuilder(lengthHint + 64);
            int i = 1;
            bool first = true;
            while (builder.Length < lengthHint)
            {
                if (!first)
                {
                    builder.Append(',');
                }

                first = false;
                builder.Append("\"Property");
                builder.Append(i);
                builder.Append("\":");
                builder.Append(i % 2 == 0 ? "true" : "false");
                i++;
            }

            propertyCount = i - 1;

            return builder.ToString();
        }

        static string BuildStringPropertiesPayload(int lengthHint, out int propertyCount)
        {
            var builder = new StringBuilder(lengthHint + 64);
            int i = 1;
            bool first = true;
            while (builder.Length < lengthHint)
            {
                if (!first)
                {
                    builder.Append(',');
                }

                first = false;
                builder.Append("\"Property");
                builder.Append(i);
                builder.Append("\":");
                builder.Append($"\"String{i}\"");
                i++;
            }

            propertyCount = i - 1;

            return builder.ToString();
        }

        static string BuildEscapedStringPropertiesPayload(int lengthHint, out int propertyCount)
        {
            // Cycle of 10 escape kinds (index % 10):
            // 1:\r 2:\n 3:\t 4:\b 5:\f 6:\\ 7:\" 8:\' 9:\u00A0 0: \uD834\uDD1E (surrogate pair)
            var builder = new StringBuilder(lengthHint + 64);
            int i = 1;
            bool first = true;

            while (builder.Length < lengthHint)
            {
                if (!first)
                {
                    builder.Append(',');
                }
                first = false;

                builder.Append("\"Property");
                builder.Append(i);
                builder.Append("\":\"String");

                int mod = i % 10;
                switch (mod)
                {
                    case 1:
                        builder.Append("\\r");      // carriage return
                        break;
                    case 2:
                        builder.Append("\\n");      // newline
                        break;
                    case 3:
                        builder.Append("\\t");      // tab
                        break;
                    case 4:
                        builder.Append("\\b");      // backspace
                        break;
                    case 5:
                        builder.Append("\\f");      // form-feed
                        break;
                    case 6:
                        builder.Append("\\\\");     // backslash
                        break;
                    case 7:
                        builder.Append("\\\"");     // double quote
                        break;
                    case 8:
                        builder.Append("\\\'");     // single quote
                        break;
                    case 9:
                        builder.Append("\\u00A0");
                        break;
                    case 0:
                        builder.Append("\\uD834\\uDD1E");
                        break;
                }

                builder.Append(i);
                builder.Append('"');
                i++;
            }

            propertyCount = i - 1;
            return builder.ToString();
        }
    }
}
