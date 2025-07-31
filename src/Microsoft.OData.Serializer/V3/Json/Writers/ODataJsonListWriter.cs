using Microsoft.OData.Serializer.V3.Adapters;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Microsoft.OData.Serializer.V3.Json.Writers;

[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "This class is instantiated via reflection.")]
internal class ODataJsonListWriter<TCollection, TElement, TCustomState> : ODataResourceSetBaseJsonWriter<TCollection, TElement, TCustomState>
    where TCollection : IList<TElement>
{
    public ODataJsonListWriter(ODataResourceTypeInfo<TCollection, TCustomState>? typeInfo = null)
        : base(typeInfo)
    {
    }
    protected override async ValueTask WriteElements(TCollection value, ODataJsonWriterState<TCustomState> state)
    {
        for (int i = 0; i < value.Count; i++)
        {
            await state.WriteValue(value[i]);
        }
    }
}
