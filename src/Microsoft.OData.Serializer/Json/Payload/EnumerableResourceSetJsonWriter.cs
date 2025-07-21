using Microsoft.OData.Core.NewWriter2.Json.Payload;
using Microsoft.OData.Edm;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.OData.Serializer.Json;

internal class EnumerableResourceSetJsonWriter<TCollection, TElement> :
    ODataResourceSetBaseJsonWriter<TCollection, TElement>
    where TCollection : IEnumerable<TElement>
{
    protected override async ValueTask WriteElements(TCollection collection, ODataJsonWriterStack state, ODataJsonWriterContext context)
    {
        foreach (var item in collection)
        {
            await WriteElement(item, state, context);
        }
    }
}