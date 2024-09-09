//---------------------------------------------------------------------
// <copyright file="MessageWriterConcurrencyTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System;
using Xunit;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;

namespace Microsoft.OData.Core.Tests
{
    public class MessageWriterConcurrencyTests
    {
        /// <summary>
        /// Verifies that concurrent message writer does not interleave execution and isolates the underlying streams.
        /// </summary>
        /// <returns>A task for the asyncronous test</returns>

        [Fact]
        public async Task VerifyConcurrentResultsAreConsistentAsync()
        {
            ServiceCollection services = new();
            services.AddDefaultODataServices();
            ServiceProvider serviceProvider = services.BuildServiceProvider();

            await Task.CompletedTask;
            var content1 = string.Concat(Enumerable.Repeat('A', 1000_000));
            var content2 = string.Concat(Enumerable.Repeat('B', 1000_000));
            for (int i = 0; i < 1000; i++)
            {
                var values = await Task.WhenAll([WritePayload(content1, serviceProvider), WritePayload(content2, serviceProvider)]);
                Assert.Equal(content1.Length, values[0].Length);
                Assert.Equal(content2.Length, values[1].Length);

                Assert.Equal(content1, values[0]);
                Assert.Equal(content2, values[1]);
            }
        }


        /// <summary>
        /// A helper function that writes to a strem using the message writer and returns the content that is present in the stream.
        /// </summary>
        /// <param name="content">String content to write.</param>
        /// <param name="serviceProvider">A service provider with the default configurations.</param>
        /// <returns>A task that resolves to the string present in the output stream.</returns>
        private async Task<string> WritePayload(string content, IServiceProvider serviceProvider)
        {
            using Stream outputStream = new MemoryStream();

            var message = new ODataMessage(outputStream, serviceProvider);
            await using ODataMessageWriter writer = new ODataMessageWriter(message);
            await Task.Yield();

            await writer.WriteValueAsync(content);

            outputStream.Position = 0;
            using var reader = new StreamReader(outputStream);
            await Task.Yield();
            string writen = await reader.ReadToEndAsync();
            await writer.DisposeAsync();
            return writen;
        }


        class ODataMessage : IODataResponseMessage, IODataResponseMessageAsync, IServiceCollectionProvider
        {
            private Dictionary<string, string> _headers = new();
            private Stream _outputStream;
            public ODataMessage(Stream outputStream, IServiceProvider serviceProvider)
            {
                this.ServiceProvider = serviceProvider;
                _outputStream = outputStream;
            }
            public IEnumerable<KeyValuePair<string, string>> Headers => _headers;

            public int StatusCode { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

            public IServiceProvider ServiceProvider { get; private set; }

            public string GetHeader(string headerName)
            {
                if (_headers.TryGetValue(headerName, out var value))
                {
                    return value;
                }

                return null;
            }

            public Stream GetStream()
            {
                return _outputStream;
            }

            public Task<Stream> GetStreamAsync()
            {
                return Task.FromResult(_outputStream);
            }

            public void SetHeader(string headerName, string headerValue)
            {
                _headers[headerName] = headerValue;
            }
        }
    }
}
