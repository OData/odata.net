using Microsoft.OData.Serializer.Json.State;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.OData.Serializer.Adapters;

public interface IEtagWriter<TCustomState>
{
    void WriteEtag(ReadOnlySpan<char> etag, ODataWriterState<TCustomState> state);
    void WriteEtag(ReadOnlySpan<byte> etag, ODataWriterState<TCustomState> state);
}
