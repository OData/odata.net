//---------------------------------------------------------------------
// <copyright file="ODataBatchReaderStreamTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.OData.MultipartMixed;
using Xunit;
using ErrorStrings = Microsoft.OData.Strings;

namespace Microsoft.OData.Tests
{
    public class ODataBatchReaderStreamTests
    {
        [Fact]
        public void ReadFirstNonEmptyLineShouldThrowOnEndOfInput()
        {
            Action readAtEndOfInput = () => CreateBatchReaderStream("").ReadFirstNonEmptyLine();
            readAtEndOfInput.Throws<ODataException>(ErrorStrings.ODataBatchReaderStream_UnexpectedEndOfInput);
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

        private static ODataMultipartMixedBatchReaderStream CreateBatchReaderStream(string inputString)
        {
            string boundary = "batch_862fb28e-dc50-4af1-aad5-9608647761d1";
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
