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

    public ValueTask WriteAnnotation<TValue>(ReadOnlySpan<char> name, TValue value, ODataJsonWriterState<TCustomState> state)
    {
        // TODO: prefix annotation withs @ and ensure name includes . but does not start with odata.
        var jsonWriter = state.JsonWriter;
        JsonMetadataHelpers.WriteCustomAnnotationName(jsonWriter, name);
        return state.WriteValue(value);
    }

    public ValueTask WriteAnnotation<TValue>(ReadOnlySpan<byte> name, TValue value, ODataJsonWriterState<TCustomState> state)
    {
        var jsonWriter = state.JsonWriter;
        JsonMetadataHelpers.WriteCustomAnnotationName(jsonWriter, name);
        return state.WriteValue(value);
    }
}
