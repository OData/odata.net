using Microsoft.OData.Serializer.V3.Adapters;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.OData.Serializer.V3.Json.Writers;

[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "This class is instantiated via reflection.")]
internal class ODataJsonEnumerableWriter<TCollection, TElement> : ODataResourceSetBaseJsonWriter<TCollection, TElement>
    where TCollection : IEnumerable<TElement>
{
    public ODataJsonEnumerableWriter(ODataResourceTypeInfo<TCollection>? typeInfo = null)
        : base(typeInfo)
    {
    }

    protected override async ValueTask WriteElements(TCollection value, ODataJsonWriterState state)
    {
        foreach (var item in value)
        {
            await state.WriteValue(item);
        }
    }
}