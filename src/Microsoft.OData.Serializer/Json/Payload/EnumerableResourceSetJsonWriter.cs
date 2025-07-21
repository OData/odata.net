namespace Microsoft.OData.Serializer.Json;

public class EnumerableResourceSetJsonWriter<TCollection, TElement> :
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