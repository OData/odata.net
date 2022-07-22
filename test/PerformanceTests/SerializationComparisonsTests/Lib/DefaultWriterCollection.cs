//---------------------------------------------------------------------
// <copyright file="DefaultWriterCollection.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.Generic;
using System.Text.Json;
using Microsoft.OData.Edm;

namespace ExperimentsLib
{
    /// <summary>
    /// Creates the default collection of <see cref="IPayloadWriter{T}"/> instances used for testing.
    /// </summary>
    public static class DefaultWriterCollection
    {
        /// <summary>
        /// Creates a collection of <see cref="IPayloadWriter{Customer}"/> instances for testing.
        /// </summary>
        /// <returns>A collection of <see cref="IPayloadWriter{Customer}"/> instances.</returns>
        public static WriterCollection<IEnumerable<Customer>> Create()
        {
            IEdmModel model = DataModel.GetEdmModel();
            model.MarkAsImmutable();
            WriterCollection<IEnumerable<Customer>> writers = new WriterCollection<IEnumerable<Customer>>();

            writers.AddWriters(
                ("JsonSerializer", new JsonSerializerPayloadWriter()),

                ("Utf8JsonWriter-Direct-ArrayPool-NoValidation", new Utf8JsonWriterDirectPayloadWriterWithArrayPool(
                    bufferWriter => new Utf8JsonWriter(bufferWriter, new JsonWriterOptions { SkipValidation = true }))),

                ("NoOpWriter-Direct", new ODataJsonWriterDirectPayloadWriter(
                    stream => new NoopJsonWriter())),

                ("ODataUtf8JsonWriter-Direct", new ODataJsonWriterDirectPayloadWriter(
                    stream => stream.CreateODataUtf8JsonWriter())),

                ("ODataJsonWriter-Direct", new ODataJsonWriterDirectPayloadWriter(
                    stream => stream.CreateODataJsonWriter())),
                ("ODataJsonWriter-Direct-Async", new ODataJsonWriterAsyncDirectPayloadWriter(
                    stream => stream.CreateODataJsonWriterAsync())),

                ("ODataMessageWriter", new ODataMessageWriterPayloadWriter(model, stream => stream.CreateJsonWriterMessage())),
                ("ODataMessageWriter-Utf16", new ODataMessageWriterPayloadWriter(model, stream => stream.CreateJsonWriterMessage("UTF-16"))),
                ("ODataMessageWriter-NoValidation", new ODataMessageWriterPayloadWriter(model, stream => stream.CreateJsonWriterMessage(), enableValidation: false)),
                ("ODataMessageWriter-NoOp", new ODataMessageWriterPayloadWriter(model, stream => stream.CreateNoopMessage())),

                ("ODataMessageWriter-Utf8JsonWriter", new ODataMessageWriterPayloadWriter(model, stream => stream.CreateUtf8JsonWriterMessage())),
                ("ODataMessageWriter-Utf8JsonWriter-Utf16", new ODataMessageWriterPayloadWriter(model, stream => stream.CreateUtf8JsonWriterMessage("UTF-16"))),
                ("ODataMessageWriter-Utf8JsonWriter-NoValidation", new ODataMessageWriterPayloadWriter(model, stream => stream.CreateUtf8JsonWriterMessage(), enableValidation: false)),

                ("ODataMessageWriter-Async", new ODataMessageWriterAsyncPayloadWriter(model, stream => stream.CreateJsonWriterMessage())),
                ("ODataMessageWriter-NoValidation-Async", new ODataMessageWriterAsyncPayloadWriter(model, stream => stream.CreateJsonWriterMessage(), enableValidation: false)),
                ("ODataMessageWriter-NoOp-Async", new ODataMessageWriterAsyncPayloadWriter(model, stream => stream.CreateNoopMessage()))
                );

            return writers;
        }
    }
}
