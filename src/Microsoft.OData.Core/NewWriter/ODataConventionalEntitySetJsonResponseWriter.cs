using Microsoft.OData.Edm;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Microsoft.OData.Core.NewWriter;

internal class ODataConventionalEntitySetJsonResponseWriter<TPayload>(
    Utf8JsonWriter jsonWriter,
    IODataWriter<TPayload> resourceWriter) : IODataWriter<IEnumerable<TPayload>>
{
    public async ValueTask WriteAsync(IEnumerable<TPayload> values, ODataWriterState state)
    {
        jsonWriter.WriteStartObject();
        jsonWriter.WritePropertyName("@odata.context");
        jsonWriter.WriteStringValue("contextUrl");

        jsonWriter.WritePropertyName("value");
        jsonWriter.WriteStartArray();

        var resourceState = new ODataWriterState
        {
            WriterContext = state.WriterContext,
            EdmType = state.EdmType.AsElementType(),
            SelectAndExpand = state.WriterContext.SelectExpandClause
        };

        foreach (var value in values)
        {
            
            await resourceWriter.WriteAsync(value, resourceState);
        }

        jsonWriter.WriteEndArray();
        jsonWriter.WriteEndObject();
    }
}
