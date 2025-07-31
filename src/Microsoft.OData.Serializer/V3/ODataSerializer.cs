using Microsoft.OData.Core.NewWriter2;
using Microsoft.OData.Edm;
using Microsoft.OData.Serializer.V3.Json;
using Microsoft.OData.Serializer.V3.Json.State;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Microsoft.OData.Serializer.V3;

public static class ODataSerializer
{
    public static ValueTask WriteAsync<T>(T value, Stream stream, ODataUri uri, IEdmModel model, ODataSerializerOptions options)
    {
        return WriteAsync<T, DefaultState>(value, stream, uri, model, options);
    }

    public static async ValueTask WriteAsync<T, TCustomState>(T value, Stream stream, ODataUri uri, IEdmModel model, ODataSerializerOptions<TCustomState> options)
    {
        // this is rough structure of what we expect the writer to do
        // based on payload kind, determine the appropirate state, context and underlying writer.

        // init state
        var jsonWriter = new Utf8JsonWriter(stream);
        var writerProvider = new ODataJsonWriterProvider<TCustomState>(options);
        var state = new ODataJsonWriterState<TCustomState>(options, writerProvider, jsonWriter)
        {
            ODataUri = uri,
            PayloadKind = ODataPayloadKind.ResourceSet
        };
        // get writer
        var writer = writerProvider.GetWriter<T>(); // should we pass the value as well?

        await writer.Write(value, state);

        await jsonWriter.FlushAsync();


        // TODO: brainstorming re-entrancy support
        //bool isDone = false;
        //do
        //{
        //    // write value
        //    isDone = writer.Write(value, state);
        //    // might also need to fetch more data from value (e.g. if source is a stream or IAsyncEnumerable)

        //    if (!isDone)
        //    {
        //        if (typeof(IAsyncSource).IsAssignableFrom(typeof(T));
        //        {
        //            IAsyncSource source = (IAsyncSource)value;
        //            if (source.NeedsMoreData)
        //            {
        //                await source.AdvanceAsync();
        //            }
        //        }

        //        if (state.ShouldFlush())
        //        {
        //            // if not done, we might need to flush the stream or do some async operation
        //            // to continue writing later.
        //            await stream.FlushAsync();
        //        } 
        //    }
        //} while (!isDone);
    }
}
