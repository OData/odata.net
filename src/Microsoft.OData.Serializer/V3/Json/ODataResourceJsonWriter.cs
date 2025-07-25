using Microsoft.OData.Serializer.Core;
using Microsoft.OData.Serializer.V3.Adapters;
using Microsoft.OData.Serializer.V3.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.OData.Serializer.V3.Json;

internal class ODataResourceJsonWriter<T> : ODataWriter<T, ODataJsonWriterState>
{
    public override bool Write(T value, ODataJsonWriterState state)
    {
        ODataResourceTypeInfo<T> resourceInfo = state.GetResourceInfo(value);
        
        // This makes the following assumptions:
        // - all properties defined in the resourceInfo should be written
        // - the properties are written in the order they are defined in the resource info
        // - each property to be written is available (e.g. could be null or missing)
        foreach (var propertyInfo in resourceInfo.Properties)
        {
            // add this property to the state, including current index
            bool propertyWritten = propertyInfo.WriteValue(value, state);
            
            if (!propertyWritten)
            {
                // assume the property has stored enough state to resume writing later
                return false;
            }

            if (state.ShouldFlush())
            {
                return false;
            }

            // if async source needs more data, we should return false as well
        }
    }
}
