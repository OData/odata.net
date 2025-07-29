using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.OData.Serializer.V3.Json.Writers;

internal class ODataJsonEnumerableWriter<TCollection, TElement> : ODataJsonWriter<TCollection>
    where TCollection : IEnumerable<TElement>
{
    public override async ValueTask Write(TCollection value, ODataJsonWriterState state)
    {
        foreach (var item in value)
        {
            await state.WriteValue(item);
        }
    }
}