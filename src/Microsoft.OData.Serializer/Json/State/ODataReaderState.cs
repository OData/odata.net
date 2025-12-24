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
    internal ODataReaderState(Stream inputStream)
    {
        // TODO: Quick hack for POC purposes. Need to implement proper buffered reading.
        var memStream = new MemoryStream();
        inputStream.CopyTo(memStream);
        buffer = memStream.ToArray();
    }

    ReadOnlyMemory<byte> buffer;
    internal JsonReaderState JsonReaderState { get; set; }

    // TODO: this is a quick hack to help ensure the reader state is updated
    // after each scoped use of the reader, since we can't store the reader itself in a field.
    // This is bad design, error prone and probably bad for perf. So should definitely rethink it.
    internal JsonReaderScope GetJsonReaderScope()
    {
        var reader = new Utf8JsonReader(buffer.Span, isFinalBlock: true, state: JsonReaderState);
        return new JsonReaderScope(reader, this);
    }

    internal ref struct JsonReaderScope(Utf8JsonReader reader, ODataReaderState<TCustomState> state)
    {
        public readonly Utf8JsonReader Reader = reader;
        public readonly void Dispose()
        {
            state.JsonReaderState = Reader.CurrentState;
        }
    }
}
