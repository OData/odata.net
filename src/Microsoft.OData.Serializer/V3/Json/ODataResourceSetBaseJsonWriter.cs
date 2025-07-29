using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.OData.Serializer.V3.Json;

public abstract class ODataResourceSetBaseJsonWriter<TCollection, TElement> : ODataJsonWriter<TCollection>
{
    public override async ValueTask Write(TCollection value, ODataJsonWriterState state)
    {
        if (state.IsTopLevel())
        {
            state.JsonWriter.WriteStartObject();

            await WritePreValueMetadata(value, state);

            state.JsonWriter.WritePropertyName("value");
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

        state.JsonWriter.WriteStartArray();

        await WriteElements(value, state);

        state.JsonWriter.WriteEndArray();

        if (state.IsTopLevel())
        {
            state.JsonWriter.WriteEndObject();
        }
    }

    protected abstract ValueTask WriteElements(TCollection value, ODataJsonWriterState state);

    protected virtual async ValueTask WritePreValueMetadata(TCollection value, ODataJsonWriterState state)
    {
        // TODO: We should probably expose a ShouldWriteXXX method for metadata to give
        // users an easy way to control whether certain metadata should be written
        if (state.MetadataLevel >= ODataMetadataLevel.Minimal)
        {
            await WriteContextUrl(value, state);
        }

        //// TODO: should this condition be implemented by the WriteCountProperty method?
        //if (context.ODataUri.QueryCount.HasValue
        //    && context.ODataUri.QueryCount.Value)
        //{
        //    await WriteCountProperty(value, state, context);
        //}

        //await WriteNextLinkProperty(value, state, context);
    }

    protected virtual ValueTask WriteContextUrl(TCollection value, ODataJsonWriterState state)
    {
        if (state.PayloadKind == ODataPayloadKind.ResourceSet && state.IsTopLevel())
        {
            ContextUrlHelper.WriteContextUrlProperty(state.PayloadKind, state);
        }

        // TODO: nested context and other payload kinds

        return ValueTask.CompletedTask;
    }
}
