using Microsoft.OData.Serializer.V3.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.OData.Serializer.V3.Adapters;

public interface INextLinkWriter<TCustomState>
{
    void WriteNextLink(ReadOnlySpan<char> nextLink, ODataJsonWriterState<TCustomState> state);
    void WriteNextLink(ReadOnlySpan<byte> nextLink, ODataJsonWriterState<TCustomState> state);
    void WriteNextLink(Uri nextLink, ODataJsonWriterState<TCustomState> state);
}
