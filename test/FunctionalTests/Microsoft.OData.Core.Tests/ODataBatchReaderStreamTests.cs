//---------------------------------------------------------------------
// <copyright file="ODataBatchReaderStreamTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.IO;
using System.Text;
using FluentAssertions;
using Xunit;
using ErrorStrings = Microsoft.OData.Core.Strings;

namespace Microsoft.OData.Core.Tests
{
    public class ODataBatchReaderStreamTests
    {
        [Fact]
        public void ReadFirstNonEmptyLineShouldThrowOnEndOfInput()
        {
            Action readAtEndOfInput = () => CreateBatchReaderStream("").ReadFirstNonEmptyLine();
            readAtEndOfInput.ShouldThrow<ODataException>().WithMessage(ErrorStrings.ODataBatchReaderStream_UnexpectedEndOfInput);
        }

        [Fact]
        public void ReadFirstNonEmptyLineShouldSkipEmptyLines()
        {
            const string input = @"


First non-empty line";
            CreateBatchReaderStream(input).ReadFirstNonEmptyLine().Should().Be("First non-empty line");
        }

        [Fact]
        public void ReadFirstNonEmptyLineShouldNotIncludeTrailingCRLF()
        {
            const string input = "First non-empty line\r\n";
            CreateBatchReaderStream(input).ReadFirstNonEmptyLine().Should().Be("First non-empty line");
        }

        [Fact]
        public void ReadFirstNonEmptyLineShouldNotIncludeTrailingLF()
        {
            const string input = "First non-empty line\n";
            CreateBatchReaderStream(input).ReadFirstNonEmptyLine().Should().Be("First non-empty line");
        }

        [Fact]
        public void ReadFirstNonEmptyLineShouldNotIncludeTrailingCR()
        {
            const string input = "First non-empty line\r";
            CreateBatchReaderStream(input).ReadFirstNonEmptyLine().Should().Be("First non-empty line");
        }

        [Fact]
        public void ReadFirstNonEmptyLineShouldNotIncludeSecondLine()
        {
            const string input = @"First non-empty line
Second line";
            CreateBatchReaderStream(input).ReadFirstNonEmptyLine().Should().Be("First non-empty line");
        }

        private static ODataBatchReaderStream CreateBatchReaderStream(string inputString)
        {
            var underlyingStream = new MemoryStream(Encoding.UTF8.GetBytes(inputString));
            var inputContext = new ODataRawInputContext(
                ODataFormat.Batch, 
                underlyingStream, 
                Encoding.UTF8, 
                new ODataMessageReaderSettings(),
                false, 
                true, 
                null, 
                null, 
                ODataPayloadKind.Batch);
            var batchStream = new ODataBatchReaderStream(inputContext, "batch_862fb28e-dc50-4af1-aad5-9608647761d1", Encoding.UTF8);
            return batchStream;
        }
    }
}
