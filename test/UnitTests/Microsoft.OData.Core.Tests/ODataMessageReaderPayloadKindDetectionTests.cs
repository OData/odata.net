//---------------------------------------------------------------------
// <copyright file="ODataMessageReaderPayloadKindDetectionTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OData.Edm;
using Xunit;

namespace Microsoft.OData.Tests
{
    public class ODataMessageReaderPayloadKindDetectionTests
    {
        private const string CustomMediaType = "application/custom";

        [Fact]
        public async Task DetectPayloadKindAsync_ReturnsEmpty_WhenContentTypeMapsToNoKinds()
        {
            // Arrange: media type resolver returns no formats (empty list)
            var mediaTypeResolver = new TestMediaTypeResolver(CustomMediaType, Array.Empty<ODataMediaTypeFormat>());
            var serviceProvider = BuildServiceProvider(mediaTypeResolver);
            var responseMessage = new TestResponseMessage(CustomMediaType, serviceProvider);
            var messageReader = CreateMessageReader(responseMessage, serviceProvider.GetService<ODataMessageReaderSettings>());

            // Act
            var payloadKindDetectionResults = await DetectPayloadKindAsync(messageReader);

            // Assert
            Assert.Empty(payloadKindDetectionResults);
        }

        [Fact]
        public async Task DetectPayloadKindAsync_SinglePayloadKind()
        {
            // Arrange
            var format = new TestFormat("Test", new[] { ODataPayloadKind.Resource });
            var mediaTypeResolver = new TestMediaTypeResolver(CustomMediaType,
                new[] { MakeMediaTypeFormat(CustomMediaType, format) });
            var serviceProvider = BuildServiceProvider(mediaTypeResolver);
            var responseMessage = new TestResponseMessage(CustomMediaType, serviceProvider);
            var messageReader = CreateMessageReader(responseMessage, serviceProvider.GetRequiredService<ODataMessageReaderSettings>());

            // Act
            var payloadKindDetectionResults = await DetectPayloadKindAsync(messageReader);

            // Assert
            Assert.Equal(ODataPayloadKind.Resource, Assert.Single(payloadKindDetectionResults).PayloadKind);
        }

        [Fact]
        public async Task DetectPayloadKindAsync_MultiplePayloadKinds_SortsAscending()
        {
            // Arrange
            var format = new TestFormat("Test", new[] { ODataPayloadKind.Resource, ODataPayloadKind.Property, ODataPayloadKind.ResourceSet });
            var mediaTypeResolver = new TestMediaTypeResolver(CustomMediaType,
                new[] { MakeMediaTypeFormat(CustomMediaType, format) });
            var serviceProvider = BuildServiceProvider(mediaTypeResolver);
            var responseMessage = new TestResponseMessage(CustomMediaType, serviceProvider);
            var messageReader = CreateMessageReader(responseMessage, serviceProvider.GetRequiredService<ODataMessageReaderSettings>());

            // Act
            var results = await DetectPayloadKindAsync(messageReader);

            // Assert
            var payloadKindDetectionResults = results.Select(r => r.PayloadKind).ToArray();
            var sortedPayloadKinds = payloadKindDetectionResults.OrderBy(k => k).ToArray();
            Assert.True(
                sortedPayloadKinds.SequenceEqual(payloadKindDetectionResults),
                "Payload kinds should be sorted ascending for stable order.");
            Assert.Equal(3, payloadKindDetectionResults.Length);
            Assert.Contains(ODataPayloadKind.Property, payloadKindDetectionResults);
            Assert.Contains(ODataPayloadKind.ResourceSet, payloadKindDetectionResults);
            Assert.Contains(ODataPayloadKind.Resource, payloadKindDetectionResults);
        }

        [Fact]
        public async Task DetectPayloadKindAsync_MissingContentTypeHeader_DefaultsToApplicationJson()
        {
            // Arrange
            // Empty payload length -> default to application/json
            var format = new TestFormat("Json", new[] { ODataPayloadKind.ServiceDocument });
            var mediaTypeResolver = new TestMediaTypeResolver("application/json",
                new[] { MakeMediaTypeFormat("application/json", format) });
            var serviceProvider = BuildServiceProvider(mediaTypeResolver);
            var responseMessage = new TestResponseMessage(null, serviceProvider, payload: string.Empty);
            var messageReader = CreateMessageReader(responseMessage, serviceProvider.GetRequiredService<ODataMessageReaderSettings>());

            // Act
            var payloadKindDetectionResults = await DetectPayloadKindAsync(messageReader);

            // Assert
            Assert.Equal(ODataPayloadKind.ServiceDocument, Assert.Single(payloadKindDetectionResults).PayloadKind);
        }

        [Fact]
        public async Task DetectPayloadKindAsync_SingleKind_IsRepeatableAndSafe()
        {
            // Arrange
            var format = new TestFormat("Test", new[] { ODataPayloadKind.Resource });
            var mediaTypeResolver = new TestMediaTypeResolver(CustomMediaType,
                new[] { MakeMediaTypeFormat(CustomMediaType, format) });
            var serviceProvider = BuildServiceProvider(mediaTypeResolver);
            var responseMessage = new TestResponseMessage(CustomMediaType, serviceProvider);
            var messageReader = CreateMessageReader(responseMessage, serviceProvider.GetRequiredService<ODataMessageReaderSettings>());

            // Act & Assert
            var firstPayloadKindDetectionResults = await DetectPayloadKindAsync(messageReader);
            Assert.Equal(ODataPayloadKind.Resource, Assert.Single(firstPayloadKindDetectionResults).PayloadKind);

            var secondPayloadKindDetectionResults = await DetectPayloadKindAsync(messageReader);
            Assert.Equal(ODataPayloadKind.Resource, Assert.Single(secondPayloadKindDetectionResults).PayloadKind);
        }

        [Fact]
        public async Task DetectPayloadKindAsync_ReturnsEmpty_WhenFormatReturnsNull()
        {
            // Arrange
            var format = new NullReturningFormat("Null");
            var mediaTypeResolver = new TestMediaTypeResolver(CustomMediaType,
                new[] { MakeMediaTypeFormat(CustomMediaType, format) });
            var serviceProvider = BuildServiceProvider(mediaTypeResolver);
            var responseMessage = new TestResponseMessage(CustomMediaType, serviceProvider);
            var messageReader = CreateMessageReader(responseMessage, serviceProvider.GetRequiredService<ODataMessageReaderSettings>());

            // Act
            var payloadKindDetectionResults = await DetectPayloadKindAsync(messageReader);

            // Assert
            Assert.Empty(payloadKindDetectionResults);
        }

        [Fact]
        public void DetectPayloadKind_ReturnsEmpty_WhenContentTypeMapsToNoKinds()
        {
            // Arrange: media type resolver returns no formats (empty list)
            var mediaTypeResolver = new TestMediaTypeResolver(CustomMediaType, Array.Empty<ODataMediaTypeFormat>());
            var serviceProvider = BuildServiceProvider(mediaTypeResolver);
            var responseMessage = new TestResponseMessage(CustomMediaType, serviceProvider);
            var messageReader = CreateMessageReader(responseMessage, serviceProvider.GetService<ODataMessageReaderSettings>());

            // Act
            var payloadKindDetectionResults = messageReader.DetectPayloadKind();

            // Assert
            Assert.Empty(payloadKindDetectionResults);
        }

        [Fact]
        public void DetectPayloadKind_SinglePayloadKind()
        {
            // Arrange
            var format = new TestFormat("Test", new[] { ODataPayloadKind.Resource });
            var mediaTypeResolver = new TestMediaTypeResolver(CustomMediaType,
                new[] { MakeMediaTypeFormat(CustomMediaType, format) });
            var serviceProvider = BuildServiceProvider(mediaTypeResolver);
            var responseMessage = new TestResponseMessage(CustomMediaType, serviceProvider);
            var messageReader = CreateMessageReader(responseMessage, serviceProvider.GetRequiredService<ODataMessageReaderSettings>());

            // Act
            var payloadKindDetectionResults = messageReader.DetectPayloadKind();

            // Assert
            Assert.Equal(ODataPayloadKind.Resource, Assert.Single(payloadKindDetectionResults).PayloadKind);
        }

        [Fact]
        public void DetectPayloadKind_MultiplePayloadKinds_SortsAscending()
        {
            // Arrange
            var format = new TestFormat("Test", new[] { ODataPayloadKind.Resource, ODataPayloadKind.Property, ODataPayloadKind.ResourceSet });
            var mediaTypeResolver = new TestMediaTypeResolver(CustomMediaType,
                new[] { MakeMediaTypeFormat(CustomMediaType, format) });
            var serviceProvider = BuildServiceProvider(mediaTypeResolver);
            var responseMessage = new TestResponseMessage(CustomMediaType, serviceProvider);
            var messageReader = CreateMessageReader(responseMessage, serviceProvider.GetRequiredService<ODataMessageReaderSettings>());

            // Act
            var results = messageReader.DetectPayloadKind();

            // Assert
            var payloadKindDetectionResults = results.Select(r => r.PayloadKind).ToArray();
            var sortedPayloadKinds = payloadKindDetectionResults.OrderBy(k => k).ToArray();
            Assert.True(
                sortedPayloadKinds.SequenceEqual(payloadKindDetectionResults),
                "Payload kinds should be sorted ascending for stable order.");
            Assert.Equal(3, payloadKindDetectionResults.Length);
            Assert.Contains(ODataPayloadKind.Property, payloadKindDetectionResults);
            Assert.Contains(ODataPayloadKind.ResourceSet, payloadKindDetectionResults);
            Assert.Contains(ODataPayloadKind.Resource, payloadKindDetectionResults);
        }

        [Fact]
        public void DetectPayloadKind_MissingContentTypeHeader_DefaultsToApplicationJson()
        {
            // Arrange
            // Empty payload length -> default to application/json
            var format = new TestFormat("Json", new[] { ODataPayloadKind.ServiceDocument });
            var mediaTypeResolver = new TestMediaTypeResolver("application/json",
                new[] { MakeMediaTypeFormat("application/json", format) });
            var serviceProvider = BuildServiceProvider(mediaTypeResolver);
            var responseMessage = new TestResponseMessage(null, serviceProvider, payload: string.Empty);
            var messageReader = CreateMessageReader(responseMessage, serviceProvider.GetRequiredService<ODataMessageReaderSettings>());

            // Act
            var payloadKindDetectionResults = messageReader.DetectPayloadKind();

            // Assert
            Assert.Equal(ODataPayloadKind.ServiceDocument, Assert.Single(payloadKindDetectionResults).PayloadKind);
        }

        [Fact]
        public void DetectPayloadKind_SingleKind_IsRepeatableAndSafe()
        {
            // Arrange
            var format = new TestFormat("Test", new[] { ODataPayloadKind.Resource });
            var mediaTypeResolver = new TestMediaTypeResolver(CustomMediaType,
                new[] { MakeMediaTypeFormat(CustomMediaType, format) });
            var serviceProvider = BuildServiceProvider(mediaTypeResolver);
            var responseMessage = new TestResponseMessage(CustomMediaType, serviceProvider);
            var messageReader = CreateMessageReader(responseMessage, serviceProvider.GetRequiredService<ODataMessageReaderSettings>());

            // Act & Assert
            var firstPayloadKindDetectionResults = messageReader.DetectPayloadKind();
            Assert.Equal(ODataPayloadKind.Resource, Assert.Single(firstPayloadKindDetectionResults).PayloadKind);

            var secondPayloadKindDetectionResults = messageReader.DetectPayloadKind();
            Assert.Equal(ODataPayloadKind.Resource, Assert.Single(secondPayloadKindDetectionResults).PayloadKind);
        }

        [Fact]
        public void DetectPayloadKind_ReturnsEmpty_WhenFormatReturnsNull()
        {
            // Arrange
            var format = new NullReturningFormat("Null");
            var mediaTypeResolver = new TestMediaTypeResolver(CustomMediaType,
                new[] { MakeMediaTypeFormat(CustomMediaType, format) });
            var serviceProvider = BuildServiceProvider(mediaTypeResolver);
            var responseMessage = new TestResponseMessage(CustomMediaType, serviceProvider);
            var messageReader = CreateMessageReader(responseMessage, serviceProvider.GetRequiredService<ODataMessageReaderSettings>());

            // Act
            var payloadKindDetectionResults = messageReader.DetectPayloadKind();

            // Assert
            Assert.Empty(payloadKindDetectionResults);
        }

        #region Helper Methods

        private static ODataMessageReader CreateMessageReader(
            TestResponseMessage responseMessage,
            ODataMessageReaderSettings messageReaderSettings)
        {
            return new ODataMessageReader(responseMessage, messageReaderSettings, EdmCoreModel.Instance);
        }

        private static IServiceProvider BuildServiceProvider(ODataMediaTypeResolver resolver)
        {
            var services = new ServiceCollection();
            services.AddSingleton<IEdmModel>(EdmCoreModel.Instance);
            services.AddSingleton(resolver);
            services.AddSingleton(new ODataMessageReaderSettings());

            return services.BuildServiceProvider();
        }

        private static ODataMediaTypeFormat MakeMediaTypeFormat(string mediaType, ODataFormat format)
        {
            Debug.Assert(!string.IsNullOrWhiteSpace(mediaType), $"{mediaType} must be non-empty");
            Debug.Assert(mediaType.IndexOf(';') < 0, $"{mediaType} must not contain parameters");
            Debug.Assert(mediaType.IndexOf('/') > 0, $"{mediaType} must be of form type/subtype");
            Debug.Assert(format != null, $"{format} != null");

            var slashIndex = mediaType.IndexOf('/');

            string type = mediaType.Substring(0, slashIndex);
            string subType = mediaType.Substring(slashIndex + 1);

            var odataMediaType = new ODataMediaType(type, subType);
            return new ODataMediaTypeFormat(odataMediaType, format);
        }

        // Helper to run detection and materialize.
        private static async Task<ODataPayloadKindDetectionResult[]> DetectPayloadKindAsync(ODataMessageReader messageReader) =>
            (await messageReader.DetectPayloadKindAsync().ConfigureAwait(false)).ToArray();

        #endregion Helper Methods

        #region Helper Classes

        private sealed class TestFormat : ODataFormat
        {
            private readonly IEnumerable<ODataPayloadKind> payloadKinds;

            public string Name { get; }

            public TestFormat(string name, IEnumerable<ODataPayloadKind> payloadKinds)
            {
                Name = name;
                this.payloadKinds = payloadKinds?.ToArray() ?? Array.Empty<ODataPayloadKind>();
            }

            public override IEnumerable<ODataPayloadKind> DetectPayloadKind(ODataMessageInfo messageInfo, ODataMessageReaderSettings messageReaderSettings)
                => payloadKinds;

            public override Task<IEnumerable<ODataPayloadKind>> DetectPayloadKindAsync(ODataMessageInfo messageInfo, ODataMessageReaderSettings messageReaderSettings)
                => Task.FromResult(payloadKinds);

            public override ODataInputContext CreateInputContext(ODataMessageInfo messageInfo, ODataMessageReaderSettings messageReaderSettings)
                => throw new NotImplementedException();

            public override Task<ODataInputContext> CreateInputContextAsync(ODataMessageInfo messageInfo, ODataMessageReaderSettings messageReaderSettings)
                => throw new NotImplementedException();

            public override ODataOutputContext CreateOutputContext(ODataMessageInfo messageInfo, ODataMessageWriterSettings messageWriterSettings)
                => throw new NotImplementedException();

            public override Task<ODataOutputContext> CreateOutputContextAsync(ODataMessageInfo messageInfo, ODataMessageWriterSettings messageWriterSettings)
                => throw new NotImplementedException();
        }

        private sealed class NullReturningFormat : ODataFormat
        {
            public NullReturningFormat(string name)
            {
                Name = name;
            }

            public string Name { get; }

            public override IEnumerable<ODataPayloadKind> DetectPayloadKind(ODataMessageInfo messageInfo, ODataMessageReaderSettings settings)
                => null;

            public override Task<IEnumerable<ODataPayloadKind>> DetectPayloadKindAsync(ODataMessageInfo messageInfo, ODataMessageReaderSettings settings)
                => Task.FromResult<IEnumerable<ODataPayloadKind>>(null); // Trigger (payloadKinds is null) block.

            public override ODataInputContext CreateInputContext(ODataMessageInfo messageInfo, ODataMessageReaderSettings messageReaderSettings)
                => throw new NotImplementedException();

            public override Task<ODataInputContext> CreateInputContextAsync(ODataMessageInfo messageInfo, ODataMessageReaderSettings messageReaderSettings)
                => throw new NotImplementedException();

            public override ODataOutputContext CreateOutputContext(ODataMessageInfo messageInfo, ODataMessageWriterSettings messageWriterSettings)
                => throw new NotImplementedException();

            public override Task<ODataOutputContext> CreateOutputContextAsync(ODataMessageInfo messageInfo, ODataMessageWriterSettings messageWriterSettings)
                => throw new NotImplementedException();
        }

        private sealed class TestMediaTypeResolver : ODataMediaTypeResolver
        {
            private readonly string contentType;
            private readonly IList<ODataMediaTypeFormat> formats;

            public TestMediaTypeResolver(string contentType, IEnumerable<ODataMediaTypeFormat> formats)
            {
                this.contentType = contentType;
                this.formats = formats.ToList();
            }

            public override IEnumerable<ODataMediaTypeFormat> GetMediaTypeFormats(ODataPayloadKind payloadKind)
                => this.formats;
        }

        private sealed class TestResponseMessage : IODataResponseMessage, IODataResponseMessageAsync, IServiceCollectionProvider
        {
            private readonly MemoryStream stream;
            private readonly Dictionary<string, string> headers = new(StringComparer.OrdinalIgnoreCase);
            public IServiceProvider ServiceProvider { get; }

            public TestResponseMessage(
                string contentType,
                IServiceProvider serviceProvider,
                string payload = "{}")
            {
                ServiceProvider = serviceProvider;
                this.stream = new MemoryStream(Encoding.UTF8.GetBytes(payload));
                if (contentType != null)
                {
                    SetHeader(ODataConstants.ContentTypeHeader, contentType);
                }
                
                // Content-Length only matters if missing Content-Type and non-zero length; we keep zero/non-zero deterministic.
                SetHeader(ODataConstants.ContentLengthHeader, this.stream.Length.ToString());
            }

            public string GetHeader(string headerName) => this.headers.TryGetValue(headerName, out var v) ? v : null;
            public Stream GetStream() => new MemoryStream(this.stream.ToArray()); // Fresh copy for each call
            public Task<Stream> GetStreamAsync() => Task.FromResult(GetStream());
            public IEnumerable<KeyValuePair<string, string>> Headers => this.headers;
            public void SetHeader(string headerName, string headerValue) => this.headers[headerName] = headerValue;
            public int StatusCode { get; set; } = 200;
        }

        #endregion Helper Classes
    }
}
