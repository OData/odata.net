using Microsoft.OData.Serializer.V3.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.OData.Serializer.V3.Adapters;

public interface IODataIdWriter<TCustomState>
{
    void WriteId(ReadOnlySpan<char> id, ODataWriterState<TCustomState> state);
    void WriteId(ReadOnlySpan<byte> id, ODataWriterState<TCustomState> state);
    void WriteId(Uri id, ODataWriterState<TCustomState> state);
}
