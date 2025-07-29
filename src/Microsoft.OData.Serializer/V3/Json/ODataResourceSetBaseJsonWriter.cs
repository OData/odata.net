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

    protected virtual ValueTask WritePreValueMetadata(TCollection value, ODataJsonWriterState state)
    {
        return ValueTask.CompletedTask;
    }

    protected abstract ValueTask WriteElements(TCollection value, ODataJsonWriterState state);
}
