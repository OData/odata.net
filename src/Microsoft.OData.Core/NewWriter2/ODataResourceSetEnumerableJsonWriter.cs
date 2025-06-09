using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.OData.Core.NewWriter2;

internal class ODataResourceSetEnumerableJsonWriter<T> : IODataWriter<ODataJsonWriterContext, ODataJsonWriterStack, IEnumerable<T>>
{
    public async ValueTask WriteAsync(IEnumerable<T> value, ODataJsonWriterStack state, ODataJsonWriterContext context)
    {
        if (state.IsTopLevel())
        {
            context.JsonWriter.WriteStartObject();

            // TODO: write context url and annotations
            var metadataWriter = context.MetadataWriterProvider.GetMetadataWriter<IEnumerable<T>>(context, state);
            await metadataWriter.WriteContextUrlAsync(value, state, context);

            if (state.IsTopLevel()
                && context.ODataUri.QueryCount.HasValue
                && context.ODataUri.QueryCount.Value
                && context.MetadataLevel >= ODataMetadataLevel.Minimal)
            {
                await metadataWriter.WriteCountPropertyAsync(value, state, context);
            }

            await metadataWriter.WriteNextLinkPropertyAsync(value, state, context);

            context.JsonWriter.WritePropertyName("value");
        }

        // TODO: if this is the value of a property, we should write annotations for the property
        // before the array start

        context.JsonWriter.WriteStartArray();

        foreach (var item in value)
        {
            var resourceWriter = context.ResourceWriterProvider.GetResourceWriter<T>(context, state);
            await resourceWriter.WriteAsync(item, state, context);
        }

        context.JsonWriter.WriteEndArray();

        if (state.IsTopLevel())
        {
            context.JsonWriter.WriteEndObject();
        }
    }
}
