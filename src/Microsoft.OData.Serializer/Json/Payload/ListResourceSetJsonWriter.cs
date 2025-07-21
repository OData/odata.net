using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.OData.Serializer.Json;

internal class ListResourceSetJsonWriter<TCollection, TElement> :
    ODataResourceSetBaseJsonWriter<TCollection, TElement>
    where TCollection : IReadOnlyList<TElement>
{
    protected override async ValueTask WriteElements(TCollection collection, ODataJsonWriterStack state, ODataJsonWriterContext context)
    {
        for (int i = 0; i < collection.Count; i++)
        {
            var element = collection[i];
            await WriteElement(element, state, context);
        }
    }
}
