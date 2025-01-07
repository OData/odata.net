//---------------------------------------------------------------------
// <copyright file="TranscodingWriteStreamTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.OData.Json;
using Xunit;

namespace Microsoft.OData.Tests.Json
{
    // Tests adapted from: https://github.com/dotnet/runtime/blob/main/src/libraries/System.Net.Http.Json/tests/UnitTests/TranscodingWriteStreamTests.cs
    public class TranscodingWriteStreamTests
    {
        private const int BufferSize = 4 * 1024;

        public static TheoryData WriteAsyncInputLatin =>
            GetLatinTextInput(BufferSize, BufferSize);

        public static TheoryData WriteAsyncInputUnicode =>
            GetUnicodeText(BufferSize);

        [Theory]
        [MemberData(nameof(WriteAsyncInputLatin))]
        [MemberData(nameof(WriteAsyncInputUnicode))]
        public Task WriteAsync_Works_WhenOutputIs_UTF32(string message)
        {
            Encoding targetEncoding = Encoding.UTF32;
            return WriteAsyncTest(Encoding.UTF8, targetEncoding, message);
        }

        [Theory]
        [MemberData(nameof(WriteAsyncInputLatin))]
        [MemberData(nameof(WriteAsyncInputUnicode))]
        public Task WriteAsync_Works_WhenOutputIs_Unicode(string message)
        {
            Encoding targetEncoding = Encoding.Unicode;
            return WriteAsyncTest(Encoding.UTF8, targetEncoding, message);
        }

        [Theory]
        [MemberData(nameof(WriteAsyncInputLatin))]
        public Task WriteAsync_Works_WhenOutputIs_WesternEuropeanEncoding(string message)
        {
            // Arrange
            Encoding targetEncoding = Encoding.GetEncoding(28591);
            return WriteAsyncTest(Encoding.UTF8, targetEncoding, message);
        }

        [Theory]
        [MemberData(nameof(WriteAsyncInputLatin))]
        public Task WriteAsync_Works_WhenOutputIs_ASCII(string message)
        {
            // Arrange
            Encoding targetEncoding = Encoding.ASCII;
            return WriteAsyncTest(Encoding.UTF8, targetEncoding, message);
        }

        private static async Task WriteAsyncTest(Encoding inputEncoding, Encoding targetEncoding, string message)
        {
            string expected = JavaScriptEncoder.Default.Encode(message);

            var memoryStream = new MemoryStream();
            var stream = new AsyncStream(memoryStream);

            await using var transcodingStream = new TranscodingWriteStream(stream, targetEncoding, inputEncoding);
            await transcodingStream.WriteAsync(inputEncoding.GetBytes(JavaScriptEncoder.Default.Encode(message)), default);
            await transcodingStream.FlushAsync();

            string actual = targetEncoding.GetString(memoryStream.ToArray());
            Assert.Equal(expected, actual, StringComparer.OrdinalIgnoreCase);
        }

        internal static TheoryData GetLatinTextInput(int maxCharBufferSize, int maxByteBufferSize)
        {
            return new TheoryData<string>
            {
                "Hello world",
                string.Join(string.Empty, Enumerable.Repeat("AB", 9000)),
                new string('A', count: maxByteBufferSize),
                new string('A', count: maxCharBufferSize),
                new string('A', count: maxByteBufferSize + 1),
                new string('A', count: maxCharBufferSize + 1),
            };
        }

        internal static TheoryData GetUnicodeText(int maxCharBufferSize)
        {
            return new TheoryData<string>
            {
                new string('\u00c6', count: 7),

                new string('A', count: maxCharBufferSize - 1) + '\u00c6',

                "Ab\u0100\u0101\u0102\u0103\u0104\u0105\u0106\u014a\u014b\u014c\u014d\u014e\u014f\u0150\u0151\u0152\u0153\u0154\u0155\u0156\u0157\u0158\u0159\u015a\u015f\u0160\u0161\u0162\u0163\u0164\u0165\u0166\u0167\u0168\u0169\u016a\u016b\u016c\u016d\u016e\u016f\u0170\u0171\u0172\u0173\u0174\u0175\u0176\u0177\u0178\u0179\u017a\u017b\u017c\u017d\u017e\u017fAbc",

               "Abc\u0b90\u0b92\u0b93\u0b94\u0b95\u0b99\u0b9a\u0b9c\u0b9e\u0b9f\u0ba3\u0ba4\u0ba8\u0ba9\u0baa\u0bae\u0baf\u0bb0\u0bb1\u0bb2\u0bb3\u0bb4\u0bb5\u0bb7\u0bb8\u0bb9",

               "\u2600\u2601\u2602\u2603\u2604\u2605\u2606\u2607\u2608\u2609\u260a\u260b\u260c\u260d\u260e\u260f\u2610\u2611\u2612\u2613\u261a\u261b\u261c\u261d\u261e\u261f\u2620\u2621\u2622\u2623\u2624\u2625\u2626\u2627\u2628\u2629\u262a\u262b\u262c\u262d\u262e\u262f\u2630\u2631\u2632\u2633\u2634\u2635\u2636\u2637\u2638",

                new string('\u00c6', count: 64 * 1024),

                new string('\u00c6', count: 64 * 1024 + 1),

               "ping\u00fcino",

                new string('\u0904', count: maxCharBufferSize + 1), // This uses 3 bytes to represent in UTF8
            };
        }
    }
}
