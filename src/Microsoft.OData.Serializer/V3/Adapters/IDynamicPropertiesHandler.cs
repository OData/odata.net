using Microsoft.OData.Serializer.V3.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.OData.Serializer.V3.Adapters;

internal interface IDynamicPropertiesHandler<TCustomState>
{
    ValueTask WriteDynamicProperties(object dynamicProperties, IDynamicPropertyWriter<TCustomState> writer, ODataJsonWriterState<TCustomState> state);
}
