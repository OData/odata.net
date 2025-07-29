using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.OData.Serializer.Json;

[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "This class is instantiated via reflection.")]
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
