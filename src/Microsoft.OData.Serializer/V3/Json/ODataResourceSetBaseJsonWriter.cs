using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.OData.Serializer.V3.Json;

internal class ODataResourceSetBaseJsonWriter<TCollection, TElement> : ODataJsonWriter<TCollection>
{
    public override bool Write(TCollection value, ODataJsonWriterState state)
    {
        if (state.IsTopLevel())
        {
            context.JsonWriter.WriteStartObject();

            await WritePreValueMetadata(value, state, context);

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
}
