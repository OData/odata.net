using Microsoft.OData.Serializer.V3.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.OData.Serializer.V3.Adapters;

internal interface IStreamValueWriter<TCustomState>
{
    void WriteValue<T>(T value, ODataJsonWriterState<TCustomState> state);
    void WriteStringSegment(ReadOnlySpan<char> value, ODataJsonWriterState<TCustomState> state);
    void WriteStringSegment(ReadOnlySpan<byte> value, ODataJsonWriterState<TCustomState> state);
    void WriteBinarySegment(ReadOnlySpan<byte> value, ODataJsonWriterState<TCustomState> state);

    ValueTask FlushAsync(ODataJsonWriterState<TCustomState> state);
    ValueTask FlushIfFull(ODataJsonWriterState<TCustomState> state);
}
