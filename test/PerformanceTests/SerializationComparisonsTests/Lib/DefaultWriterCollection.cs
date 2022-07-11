using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using Microsoft.OData.Edm;

namespace ExperimentsLib
{
    /// <summary>
    /// Creates the default collection of <see cref="IPayloadWriter{T}"/>s used for testing.
    /// </summary>
    public static class DefaultWriterCollection
    {
        const int BufferSize = 84000;

        /// <summary>
        /// Creates a collection of <see cref="IPayloadWriter{Customer}"/>s for testing.
        /// </summary>
        /// <returns>A collection of <see cref="IPayloadWriter{Customer}"/>.</returns>
        public static WriterCollection<IEnumerable<Customer>> Create()
        {
            IEdmModel model = DataModel.GetEdmModel();
            model.MarkAsImmutable();
            WriterCollection<IEnumerable<Customer>> servers = new WriterCollection<IEnumerable<Customer>>();

            servers.AddWriters(
                ("JsonSerializer", "utf-8", new JsonSerializerPayloadWriter()),

                ("Utf8JsonWriter-Direct-NoValidation", "utf-8", new Utf8JsonWriterDirectPayloadWriter(
                    stream => new Utf8JsonWriter(stream, new JsonWriterOptions { SkipValidation = true }))),
                ("Utf8JsonWriter-Direct-ArrayPool-NoValidation", "utf-8", new Utf8JsonWriterDirectPayloadWriterWithArrayPool(
                    bufferWriter => new Utf8JsonWriter(bufferWriter, new JsonWriterOptions { SkipValidation = true }))),

                ("NoOpWriter-Direct", "utf-8", new ODataJsonWriterDirectPayloadWriter(
                    stream => new NoopJsonWriter())),

                ("ODataUtf8JsonWriter-Direct", "utf-8", new ODataJsonWriterDirectPayloadWriter(
                    stream => stream.CreateODataUtf8JsonWriter())),
                ("ODataUtf8JsonWriter-Direct-Utf16", "utf-16", new ODataJsonWriterDirectPayloadWriter(
                    stream => stream.CreateODataUtf8JsonWriter(Encoding.Unicode))),

                ("ODataJsonWriter-Direct", "utf-8", new ODataJsonWriterDirectPayloadWriter(
                    stream => stream.CreateODataJsonWriter())),
                ("ODataJsonWriter-Direct-Utf16", "utf-16", new ODataJsonWriterDirectPayloadWriter(
                    stream => stream.CreateODataJsonWriter(Encoding.Unicode))),
                ("ODataJsonWriter-Direct-Buffered", "utf-8", new ODataJsonWriterDirectPayloadWriter(
                    stream => new BufferedStream(stream, BufferSize).CreateODataJsonWriter())),

                ("ODataJsonWriter-Direct-Async", "utf-8", new ODataJsonWriterAsyncDirectPayloadWriter(
                    stream => stream.CreateODataJsonWriterAsync())),

                ("ODataUtf8JsonWriter", "utf-8", new ODataJsonWriterPayloadWriter(
                    stream => stream.CreateODataUtf8JsonWriter())),
                ("ODataUtf8JsonWriter-Utf16", "utf-16", new ODataJsonWriterPayloadWriter(
                    stream => stream.CreateODataUtf8JsonWriter(Encoding.Unicode))),

                ("ODataJsonWriter-Utf8", "utf-8", new ODataJsonWriterPayloadWriter(
                    stream => stream.CreateODataJsonWriter())),
                ("ODataJsonWriter-Utf16", "utf-16", new ODataJsonWriterPayloadWriter(
                    stream => stream.CreateODataJsonWriter(Encoding.Unicode))),
                ("ODataJsonWriter-Utf8-Buffered", "utf-8", new ODataJsonWriterPayloadWriter(
                    stream => new BufferedStream(stream, BufferSize).CreateODataJsonWriter())),

                ("ODataJsonWriter-Utf8-Async", "utf-8", new ODataJsonWriterAsyncPayloadWriter(
                    stream => stream.CreateODataJsonWriterAsync())),
                ("ODataJsonWriter-Utf16-Async", "utf-16", new ODataJsonWriterAsyncPayloadWriter(
                    stream => stream.CreateODataJsonWriterAsync(Encoding.Unicode))),
                ("ODataJsonWriter-Utf8-Buffered-Async", "utf-8", new ODataJsonWriterAsyncPayloadWriter(
                    stream => new BufferedStream(stream, BufferSize).CreateODataJsonWriterAsync())),

                ("NoOpWriter", "utf-8", new ODataJsonWriterPayloadWriter(
                    stream => new NoopJsonWriter())),
                ("NoOpWriter-Direct", "utf-8", new ODataJsonWriterDirectPayloadWriter(
                    stream => new NoopJsonWriter())),
                ("NoOpWriter-Async", "utf-8", new ODataJsonWriterPayloadWriter(stream => new NoopJsonWriter())),

                ("ODataMessageWriter-Utf8", "utf-8", new ODataMessageWriterPayloadWriter(model, stream => stream.CreateUtf8Message())),
                ("ODataMessageWriter-Utf16", "utf-16", new ODataMessageWriterPayloadWriter(model, stream => stream.CreateUtf16Message())),
                ("ODataMessageWriter-Utf8-Buffered", "utf-8", new ODataMessageWriterPayloadWriter(model, stream => new BufferedStream(stream, BufferSize).CreateUtf8Message())),
                ("ODataMessageWriter-Utf8-NoValidation", "utf-8", new ODataMessageWriterPayloadWriter(model, stream => stream.CreateUtf8Message(), enableValidation: false)),
                ("ODataMessageWriter-NoOp", "utf-8", new ODataMessageWriterPayloadWriter(model, stream => stream.CreateNoopMessage())),
                ("ODataMessageWriter-Utf8JsonWriter", "utf8", new ODataMessageWriterPayloadWriter(model, stream => stream.CreateUtf8JsonWriterMessage())),
                ("ODataMessageWriter-Utf8JsonWriter-NoValidation", "utf8", new ODataMessageWriterPayloadWriter(model, stream => stream.CreateUtf8JsonWriterMessage(), enableValidation: false)),

                ("ODataMessageWriter-Utf8-Async", "utf-8", new ODataMessageWriterAsyncPayloadWriter(model, stream => stream.CreateUtf8Message())),
                ("ODataMessageWriter-Utf16-Async", "utf-16", new ODataMessageWriterAsyncPayloadWriter(model, stream => stream.CreateUtf16Message())),
                ("ODataMessageWriter-Utf8-NoValidation-Async", "utf-8", new ODataMessageWriterAsyncPayloadWriter(model, stream => stream.CreateUtf8Message(), enableValidation: false)),
                ("ODataMessageWriter-NoOp-Async", "utf-8", new ODataMessageWriterAsyncPayloadWriter(model, stream => stream.CreateNoopMessage()))
                );

            return servers;
        }
    }
}
