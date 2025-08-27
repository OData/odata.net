using Microsoft.OData.Serializer.V3.Adapters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.OData.Serializer.V3.Json.Writers;

internal class CustomInstanceAnnotationWriter<TCustomState> : IAnnotationWriter<TCustomState>
{
    public static readonly CustomInstanceAnnotationWriter<TCustomState> Instance = new();

    public void WriteAnnotation<TValue>(ReadOnlySpan<char> name, TValue value, ODataWriterState<TCustomState> state)
    {
        // TODO: prefix annotation withs @ and ensure name includes . but does not start with odata.
        var jsonWriter = state.JsonWriter;
        JsonMetadataHelpers.WriteCustomAnnotationName(jsonWriter, name);
        bool completed = state.WriteValue(value);
        if (!completed)
        {
            throw new InvalidOperationException("Resumable annotation writes are not supported.");
        }
    }

    public void WriteAnnotation<TValue>(ReadOnlySpan<byte> name, TValue value, ODataWriterState<TCustomState> state)
    {
        var jsonWriter = state.JsonWriter;
        JsonMetadataHelpers.WriteCustomAnnotationName(jsonWriter, name);
        bool completed = state.WriteValue(value);
        if (!completed)
        {
            throw new InvalidOperationException("Resumable annotation writes are not supported.");
        }
    }
}
