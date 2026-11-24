//---------------------------------------------------------------------
// <copyright file="ODataBatchReaderStreamTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.OData.Core;
using Microsoft.OData.MultipartMixed;
using Xunit;

namespace Microsoft.OData.Tests
{
    public class ODataBatchReaderStreamTests
    {
        private const string BatchBoundary = "batch_862fb28e-dc50-4af1-aad5-9608647761d1";

        [Fact]
        public void ReadFirstNonEmptyLineShouldThrowOnEndOfInput()
        {
            Action readAtEndOfInput = () => CreateBatchReaderStream("").ReadFirstNonEmptyLine();
            readAtEndOfInput.Throws<ODataException>(SRResources.ODataBatchReaderStream_UnexpectedEndOfInput);
        }

        [Fact]
        public void ReadFirstNonEmptyLineShouldSkipEmptyLines()
        {
            const string input = @"


First non-empty line";
            Assert.Equal("First non-empty line", CreateBatchReaderStream(input).ReadFirstNonEmptyLine());
        }

        [Fact]
        public void ReadFirstNonEmptyLineShouldNotIncludeTrailingCRLF()
        {
            const string input = "First non-empty line\r\n";
            Assert.Equal("First non-empty line", CreateBatchReaderStream(input).ReadFirstNonEmptyLine());
        }

        [Fact]
        public void ReadFirstNonEmptyLineShouldNotIncludeTrailingLF()
        {
            const string input = "First non-empty line\n";
            Assert.Equal("First non-empty line", CreateBatchReaderStream(input).ReadFirstNonEmptyLine());
        }

        [Fact]
        public void ReadFirstNonEmptyLineShouldNotIncludeTrailingCR()
        {
            const string input = "First non-empty line\r";
            Assert.Equal("First non-empty line", CreateBatchReaderStream(input).ReadFirstNonEmptyLine());
        }

        [Fact]
        public void ReadFirstNonEmptyLineShouldNotIncludeSecondLine()
        {
            const string input = @"First non-empty line
Second line";
            Assert.Equal("First non-empty line", CreateBatchReaderStream(input).ReadFirstNonEmptyLine());
        }

        [Theory]
        [InlineData(1999)]
        [InlineData(2000)]
        [InlineData(2001)]
        [InlineData(7999)]
        [InlineData(8000)]
        [InlineData(8001)]
        [InlineData(15999)]
        [InlineData(16000)]
        [InlineData(16001)]
        public void ReadFirstNonEmptyLinePreservesLongLinesAndFollowingLine(int length)
        {
            string line = new string('a', length);
            foreach (string lineEnding in new[] { "\r", "\n", "\r\n" })
            {
                var stream = CreateBatchReaderStream(line + lineEnding + "Next line\r\n");

                Assert.Equal(line, stream.ReadFirstNonEmptyLine());
                Assert.Equal("Next line", stream.ReadFirstNonEmptyLine());
            }

            Assert.Equal(line, CreateBatchReaderStream(line).ReadFirstNonEmptyLine());
        }

        [Fact]
        public void ReadFirstNonEmptyLinePreservesMultibyteCharactersAcrossRefills()
        {
            string line = new string('a', 7999) + new string('\u00e9', 16000);
            var stream = CreateBatchReaderStream(line + "\r\nNext line\r\n");

            Assert.Equal(line, stream.ReadFirstNonEmptyLine());
            Assert.Equal("Next line", stream.ReadFirstNonEmptyLine());
        }

        [Theory]
        [InlineData("")]
        [InlineData("\r")]
        [InlineData("\n")]
        [InlineData("\r\n")]
        public void ReadFirstNonEmptyLineAllocatesLinearSpaceForLongLines(string lineEnding)
        {
            const int length = 1024 * 1024;
            string line = new string('a', length);
            var stream = CreateBatchReaderStream(line + lineEnding);
            CreateBatchReaderStream("Warmup\r\n").ReadFirstNonEmptyLine();

            long before = GC.GetAllocatedBytesForCurrentThread();
            string result = stream.ReadFirstNonEmptyLine();
            long allocated = GC.GetAllocatedBytesForCurrentThread() - before;

            Assert.Equal(line, result);
            Assert.InRange(allocated, 0L, 8L * length);
        }

        [Theory]
        [InlineData("")]
        [InlineData("\r\n")]
        public void ProcessPartHeaderRejectsLongMalformedHeaderWithLinearAllocations(string lineEnding)
        {
            const int length = 1024 * 1024;
            string header = new string('a', length);
            var stream = CreateBatchReaderStream("--" + BatchBoundary + "\r\n" + header + lineEnding);
            Assert.True(stream.SkipToBoundary(out bool isEndBoundary, out bool isParentBoundary));
            Assert.False(isEndBoundary);
            Assert.False(isParentBoundary);

            long before = GC.GetAllocatedBytesForCurrentThread();
            var exception = Assert.Throws<ODataException>(() => stream.ProcessPartHeader(out _));
            long allocated = GC.GetAllocatedBytesForCurrentThread() - before;

            Assert.Equal(Error.Format(SRResources.ODataBatchReaderStream_InvalidHeaderSpecified, header), exception.Message);
            Assert.InRange(allocated, 0L, 16L * length);
        }

        private static ODataMultipartMixedBatchReaderStream CreateBatchReaderStream(string inputString)
        {
            string boundary = BatchBoundary;
            var messageInfo = new ODataMessageInfo
            {
                Encoding = Encoding.UTF8,
                IsResponse = false,
                IsAsync = false,
                MessageStream = new MemoryStream(Encoding.UTF8.GetBytes(inputString)),
                MediaType = new ODataMediaType(MimeConstants.MimeMultipartType, MimeConstants.MimeMixedSubType, new KeyValuePair<string,string>(ODataConstants.HttpMultipartBoundary, boundary))
            };
            var inputContext = new ODataMultipartMixedBatchInputContext(
                ODataFormat.Batch,
                messageInfo,
                new ODataMessageReaderSettings());
            var batchStream = new ODataMultipartMixedBatchReaderStream(inputContext, boundary, Encoding.UTF8);
            return batchStream;
        }
    }
}
