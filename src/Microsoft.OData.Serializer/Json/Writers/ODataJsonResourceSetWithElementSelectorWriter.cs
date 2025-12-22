using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.OData.Serializer;

internal class ODataJsonResourceSetWithElementSelectorWriter<TCollection, TCustomState>(ODataTypeInfo<TCollection, TCustomState> typeInfo)
    : ODataResourceSetBaseJsonWriter<TCollection, object, TCustomState>(typeInfo)
{
    protected override bool WriteElements(TCollection value, ODataWriterState<TCustomState> state)
    {
        Debug.Assert(TypeInfo != null, "typeInfo should not be null for ODataJsonResourceSetWithElementSelectorWriter.");
        Debug.Assert(TypeInfo.ElementSelector != null, "ElementSelector should not be null for ODataJsonResourceSetWithElementSelectorWriter.");

        return TypeInfo.ElementSelector.WriteElements(value, state);
    }
}
