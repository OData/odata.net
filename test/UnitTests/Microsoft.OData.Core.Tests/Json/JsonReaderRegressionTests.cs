//---------------------------------------------------------------------
// <copyright file="JsonReaderRegressionTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
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
    }
}
