using Microsoft.OData.Serializer.V3.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.OData.Serializer.V3.Adapters;

public interface IValueWriter<TCustomState>
{
    bool WriteValue<T>(T value, ODataJsonWriterState<TCustomState> state);
}
