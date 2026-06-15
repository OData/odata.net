//---------------------------------------------------------------------
// <copyright file="WebUtilTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Microsoft.OData.Client.Tests
{
    public class WebUtilTests
    {
        [Fact]
        public async Task CopyStreamAsync_CopiesAllBytes()
        {
            // Arrange
            byte[] data = Encoding.UTF8.GetBytes("Hello, OData!");
            using var input = new MemoryStream(data);
            using var output = new MemoryStream();
            byte[] buffer = new byte[WebUtil.DefaultBufferSizeForStreamCopy];

            // Act
            long bytesCopied = await WebUtil.CopyStreamAsync(input, output, buffer);

            // Assert
            Assert.Equal(data.Length, bytesCopied);
            Assert.Equal(data, output.ToArray());
        }

        [Fact]
        public async Task CopyStreamAsync_WithSmallBuffer_CopiesAllBytes()
        {
            // Arrange — buffer smaller than the data to exercise multiple read iterations
            byte[] data = Encoding.UTF8.GetBytes("Hello, OData!");
            using var input = new MemoryStream(data);
            using var output = new MemoryStream();
            byte[] buffer = new byte[4];

            // Act
            long bytesCopied = await WebUtil.CopyStreamAsync(input, output, buffer);

            // Assert
            Assert.Equal(data.Length, bytesCopied);
            Assert.Equal(data, output.ToArray());
        }

        [Fact]
        public async Task CopyStreamAsync_WithEmptyInput_ReturnsZero()
        {
            // Arrange
            using var input = new MemoryStream(new byte[0]);
            using var output = new MemoryStream();
            byte[] buffer = new byte[WebUtil.DefaultBufferSizeForStreamCopy];

            // Act
            long bytesCopied = await WebUtil.CopyStreamAsync(input, output, buffer);

            // Assert
            Assert.Equal(0, bytesCopied);
            Assert.Equal(0, output.Length);
        }

        [Fact]
        public async Task CopyStreamAsync_WhenCancelled_ThrowsOperationCanceledException()
        {
            // Arrange
            byte[] data = new byte[1024];
            using var input = new MemoryStream(data);
            using var output = new MemoryStream();
            byte[] buffer = new byte[4]; // small buffer to ensure multiple iterations
            using var cts = new CancellationTokenSource();
            cts.Cancel();

            // Act & Assert
            await Assert.ThrowsAnyAsync<OperationCanceledException>(
                () => WebUtil.CopyStreamAsync(input, output, buffer, cts.Token));
        }

        [Fact]
        public void CopyStream_CopiesAllBytes()
        {
            // Arrange
            byte[] data = Encoding.UTF8.GetBytes("Hello, OData!");
            using var input = new MemoryStream(data);
            using var output = new MemoryStream();
            byte[] buffer = new byte[WebUtil.DefaultBufferSizeForStreamCopy];

            // Act
            long bytesCopied = WebUtil.CopyStream(input, output, buffer);

            // Assert
            Assert.Equal(data.Length, bytesCopied);
            Assert.Equal(data, output.ToArray());
        }

        [Fact]
        public void CopyStream_WithSmallBuffer_CopiesAllBytes()
        {
            // Arrange — buffer smaller than the data to exercise multiple read iterations
            byte[] data = Encoding.UTF8.GetBytes("Hello, OData!");
            using var input = new MemoryStream(data);
            using var output = new MemoryStream();
            byte[] buffer = new byte[4];

            // Act
            long bytesCopied = WebUtil.CopyStream(input, output, buffer);

            // Assert
            Assert.Equal(data.Length, bytesCopied);
            Assert.Equal(data, output.ToArray());
        }
    }
}
