using Microsoft.OData.Serializer.V3.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.OData.Serializer.V3.Adapters;

public interface IEtagWriter<TCustomState>
{
    void WriteEtag(ReadOnlySpan<char> etag, ODataJsonWriterState<TCustomState> state);
    void WriteEtag(ReadOnlySpan<byte> etag, ODataJsonWriterState<TCustomState> state);
}
