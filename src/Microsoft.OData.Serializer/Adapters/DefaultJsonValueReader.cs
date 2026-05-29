using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.OData.Serializer;

internal class DefaultJsonValueReader<TCustomState> : IValueReader<TCustomState>
{
    public static DefaultJsonValueReader<TCustomState> Instance { get; } = new DefaultJsonValueReader<TCustomState>();
    public bool ReadValue<T>(ODataReaderState<TCustomState> state, out T value)
    {
        return state.ReadValue<T>(out value);
    }
}
