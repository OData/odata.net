using Microsoft.OData.Serializer.V3.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.OData.Serializer.V3.Adapters;

public interface IPropertyWriter<TCustomState>
{
    bool WriteProperty<T>(ReadOnlySpan<char> propertyName, T value, ODataWriterState<TCustomState> state);
}
