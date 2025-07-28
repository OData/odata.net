using Microsoft.OData.Serializer.V3.Adapters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Microsoft.OData.Serializer.V3.Json;

public sealed class ODataJsonWriterState(ODataSerializerOptions options)
{
    internal ODataSerializerOptions Options => options;
    internal Utf8JsonWriter JsonWriter { get; init; }

    internal bool ShouldFlush()
    {
        return JsonWriter.BytesPending > 0.9 * this.Options.BufferSize;
    }
}
