using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.OData.Serializer;

internal class DefaultValueWriter<TCustomState> : IValueWriter<TCustomState>
{
    public static readonly DefaultValueWriter<TCustomState> Instance = new();

    public bool WriteValue<T>(T value, ODataWriterState<TCustomState> state)
    {
        return state.WriteValue(value);
    }
}
