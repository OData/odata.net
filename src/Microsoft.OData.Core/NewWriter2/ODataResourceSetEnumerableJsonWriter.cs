using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.OData.Core.NewWriter2;

internal class ODataResourceSetEnumerableJsonWriter<TColl, TElement> :
    IODataWriter<ODataJsonWriterContext, ODataJsonWriterStack, TColl>
    where TColl : IEnumerable<TElement>
{
    public async ValueTask WriteAsync(TColl value, ODataJsonWriterStack state, ODataJsonWriterContext context)
    {
        if (state.IsTopLevel())
        {
            context.JsonWriter.WriteStartObject();

            var metadataWriter = context.MetadataWriterProvider.GetMetadataWriter<TColl>(context, state);

            // TODO: We should probably expose a ShouldWriteXXX method for metadata to give
            // users an easy way to control whether certain metadata should be written
            if (context.MetadataLevel >= ODataMetadataLevel.Minimal)
            {
                await metadataWriter.WriteContextUrlAsync(value, state, context);
            }
            

            if (context.ODataUri.QueryCount.HasValue
                && context.ODataUri.QueryCount.Value)
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
            var resourceWriter = context.ResourceWriterProvider.GetResourceWriter<TElement>(context, state);
            await resourceWriter.WriteAsync(item, state, context);
        }

        context.JsonWriter.WriteEndArray();

        if (state.IsTopLevel())
        {
            context.JsonWriter.WriteEndObject();
        }
    }
}

internal class ODataResourceSetEnumerableJsonWriter<TElement> : ODataResourceSetEnumerableJsonWriter<IEnumerable<TElement>, TElement>
{
}    