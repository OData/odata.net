//---------------------------------------------------------------------
// <copyright file="MessageWriterConcurrencyTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.IO;
using System.Threading.Tasks;
using System;
using Xunit;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using Microsoft.OData.Tests;

namespace Microsoft.OData.Core.Tests
{
    public class MessageWriterConcurrencyTests
    {
        /// <summary>
        /// Verifies that concurrent message writer does not interleave execution and isolates the underlying streams.
        /// </summary>
        /// <returns>A task for the asynchronous test</returns>
        [Fact]
        public async Task VerifyConcurrentResultsAreIsolatedAsync()
        {
            ServiceCollection services = new();
            services.AddDefaultODataServices();
            ServiceProvider serviceProvider = services.BuildServiceProvider();

            var content1 = string.Concat(Enumerable.Repeat('A', 1000_000));
            var content2 = string.Concat(Enumerable.Repeat('B', 1000_000));
            for (int i = 0; i < 1000; i++)
            {
                var values = await Task.WhenAll([WritePayloadAsync(content1, serviceProvider), WritePayloadAsync(content2, serviceProvider)]);
                Assert.Equal(content1.Length, values[0].Length);
                Assert.Equal(content2.Length, values[1].Length);

                Assert.Equal(content1, values[0]);
                Assert.Equal(content2, values[1]);
            }
        }


        /// <summary>
        /// A helper function that writes to a stream using the message writer and returns the content that is present in the stream.
        /// </summary>
        /// <param name="content">String content to write.</param>
        /// <param name="serviceProvider">A service provider with the default configurations.</param>
        /// <returns>A task that resolves to the string present in the output stream.</returns>
        private static async Task<string> WritePayloadAsync(string content, IServiceProvider serviceProvider)
        {
            await using Stream outputStream = new MemoryStream();

            var message = new InMemoryMessage
            {
                Stream = outputStream,
                ServiceProvider = serviceProvider
            };

            var responseMessage = new ODataResponseMessage(message, writing: true, enableMessageStreamDisposal: true, maxMessageSize: -1);
            await using ODataMessageWriter writer = new ODataMessageWriter(responseMessage);

            await Task.Yield();

            await writer.WriteValueAsync(content);

            outputStream.Position = 0;
            using var reader = new StreamReader(outputStream);

            await Task.Yield();

            string written = await reader.ReadToEndAsync();
            await writer.DisposeAsync();
            return written;
        }
    }
}
