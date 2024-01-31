//---------------------------------------------------------------------
// <copyright file="DefaultWriterCollection.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.Generic;
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

                ("ODataMessageWriter", new ODataMessageWriterPayloadWriter(model, stream => stream.CreateJsonWriterMessage())),
                ("ODataMessageWriter-NoValidation", new ODataMessageWriterPayloadWriter(model, stream => stream.CreateJsonWriterMessage(), enableValidation: false)),

                ("ODataMessageWriter-Utf8JsonWriter", new ODataMessageWriterPayloadWriter(model,
                    stream => stream.CreateUtf8JsonWriterMessage())),
                ("ODataMessageWriter-Utf8JsonWriter-NoValidation", new ODataMessageWriterPayloadWriter(model,
                    stream => stream.CreateUtf8JsonWriterMessage(), enableValidation: false)),

                ("ODataMessageWriter-Async", new ODataMessageWriterAsyncPayloadWriter(model,
                    stream => stream.CreateJsonWriterMessage())),
                ("ODataMessageWriter-NoValidation-Async", new ODataMessageWriterAsyncPayloadWriter(model,
                    stream => stream.CreateJsonWriterMessage(), enableValidation: false)),

                ("ODataMessageWriter-Utf8JsonWriter-Async", new ODataMessageWriterAsyncPayloadWriter(model,
                    stream => stream.CreateUtf8JsonWriterMessage())),
                ("ODataMessageWriter-Utf8JsonWriter-NoValidation-Async", new ODataMessageWriterAsyncPayloadWriter(model,
                    stream => stream.CreateUtf8JsonWriterMessage(), enableValidation: false)));

            return writers;
        }
    }
}
