using Microsoft.OData.Edm;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.OData.Core.NewWriter2;

internal class EnumerableResourceSetJsonWriter<TCollection, TElement> :
    IODataWriter<ODataJsonWriterContext, ODataJsonWriterStack, TCollection>
    where TCollection : IEnumerable<TElement>
{
    public async ValueTask WriteAsync(TCollection value, ODataJsonWriterStack state, ODataJsonWriterContext context)
    {
        if (state.IsTopLevel())
        {
            context.JsonWriter.WriteStartObject();

            var metadataWriter = context.MetadataWriterProvider.GetMetadataWriter<TCollection>(context, state);

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

        // TODO: Looping over an IEnumerable requires us to allocate an enumerator.
        // We should probably a have a layer of abstraction that allows us to bypass
        // the allocation. The only thing we need to bypass is the loop.
        // We should have a "WriteElementsAsync" method that abstracts away the loop
        // so custom-implementors can optimize the loop without having to implement
        // IEnumerable<T>.
        foreach (var item in value)
        {
            var frame = new ODataJsonWriterStackFrame()
            {
                SelectExpandClause = state.Current.SelectExpandClause,
                EdmType = state.Current.EdmType.AsElementType(),
            };

            state.Push(frame);

            var resourceWriter = context.GetValueWriter<TElement>(state);
            await resourceWriter.WriteAsync(item, state, context);

            state.Pop();
        }

        context.JsonWriter.WriteEndArray();

        if (state.IsTopLevel())
        {
            context.JsonWriter.WriteEndObject();
        }
    }
}

internal class ODataResourceSetEnumerableJsonWriter<TElement> : EnumerableResourceSetJsonWriter<IEnumerable<TElement>, TElement>
{
}    