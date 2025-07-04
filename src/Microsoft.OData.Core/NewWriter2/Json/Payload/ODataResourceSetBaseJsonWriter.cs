using Microsoft.OData.Edm;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Microsoft.OData.Core.NewWriter2.Json.Payload;

internal abstract class ODataResourceSetBaseJsonWriter<TCollection, TElement> : IODataWriter<ODataJsonWriterContext, ODataJsonWriterStack, TCollection>
{
    public async ValueTask WriteAsync(TCollection value, ODataJsonWriterStack state, ODataJsonWriterContext context)
    {
        if (state.IsTopLevel())
        {
            context.JsonWriter.WriteStartObject();

            // TODO: consider virtual methods instead of handlers/providers
            var metadataWriter = context.GetMetadataWriter<TCollection>(state);

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
        else
        {
            // TODO: if this is the value of a property, we should write annotations for the property
            // before the array start. Should we write the annotations here in the parent property writer.
            // Since the parent property writer has already written the property name by the time we get here,
            // then we cannot write annotations here.
            // But this creates an issue. The annotations are written by different components depending
            // on whether this is a top-level write or not.
            // Perhaps this component should only be responsible for writing the array and the top-level
            // annotations moved to some parent component
        }

        context.JsonWriter.WriteStartArray();

        await WriteElements(value, state, context);

        context.JsonWriter.WriteEndArray();

        if (state.IsTopLevel())
        {
            context.JsonWriter.WriteEndObject();
        }
    }

    protected abstract ValueTask WriteElements(TCollection collection, ODataJsonWriterStack state, ODataJsonWriterContext context);

    protected virtual async ValueTask WriteElement(TElement element, ODataJsonWriterStack state, ODataJsonWriterContext context)
    {
        var frame = new ODataJsonWriterStackFrame()
        {
            SelectExpandClause = state.Current.SelectExpandClause,
            EdmType = state.Current.EdmType.AsElementType(),
        };

        state.Push(frame);

        var resourceWriter = context.GetValueWriter<TElement>(state);
        await resourceWriter.WriteAsync(element, state, context);

        state.Pop();
    }
}
