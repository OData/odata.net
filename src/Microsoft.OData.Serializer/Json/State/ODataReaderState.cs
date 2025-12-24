using Microsoft.OData.Edm;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Microsoft.OData.Serializer;

public class ODataReaderState<TCustomState>
{
    private ODataJsonWriterProvider<TCustomState> converters;
    private IEdmModel model;

    internal ODataReaderState(
        Stream inputStream,
        IEdmModel model,
        ODataJsonWriterProvider<TCustomState> converters)
    {
        this.converters = converters;
        this.model = model;
        // TODO: Quick hack for POC purposes. Need to implement proper buffered reading.
        var memStream = new MemoryStream();
        inputStream.CopyTo(memStream);
        memStream.Position = 0;
        buffer = memStream.ToArray();
    }

    ReadOnlyMemory<byte> buffer;
    private JsonReaderState? JsonReaderState { get; set; }

    // TODO: this is a quick hack to help ensure the reader state is updated
    // after each scoped use of the reader, since we can't store the reader itself in a field.
    // This is bad design, error prone and probably bad for perf. So should definitely rethink it.
    internal Utf8JsonReader GetJsonReader()
    {
        // TODO: setting isFinalBlock to true because buffering not yet supported
        var reader = JsonReaderState.HasValue ? new Utf8JsonReader(buffer.Span, isFinalBlock: true, JsonReaderState.Value)
               : new Utf8JsonReader(buffer.Span);
        return reader;
    }

    internal void SaveJsonReaderState(ref Utf8JsonReader reader)
    {
        JsonReaderState = reader.CurrentState;
        long bytesConsumed = reader.BytesConsumed;
        buffer = buffer.Slice((int)bytesConsumed);
    }

    internal bool ReadValue<T>(out T value)
    {
        var converter = converters.GetWriter<T>(model);
        return converter.Read(this, out value);
    }
}
