using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Microsoft.OData.Serializer.V3.Json.Writers;

internal class ODataJsonReadOnlyListWriter<TCollection, TElement> : ODataResourceSetBaseJsonWriter<TCollection, TElement>
    where TCollection : IReadOnlyList<TElement>
{
    protected override async ValueTask WriteElements(TCollection value, ODataJsonWriterState state)
    {
        for (int i = 0; i < value.Count; i++)
        {
            await state.WriteValue(value[i]);
        }
    }
}
