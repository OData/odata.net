using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Microsoft.OData.Serializer.V3.Json.Writers;

[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "This class is instantiated via reflection.")]
internal class ODataJsonListWriter<TCollection, TElement> : ODataResourceSetBaseJsonWriter<TCollection, TElement>
    where TCollection : IList<TElement>
{
    protected override async ValueTask WriteElements(TCollection value, ODataJsonWriterState state)
    {
        for (int i = 0; i < value.Count; i++)
        {
            await state.WriteValue(value[i]);
        }
    }
}
