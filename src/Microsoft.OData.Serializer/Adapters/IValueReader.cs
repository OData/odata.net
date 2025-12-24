using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.OData.Serializer;

public interface IValueReader<TCustomState>
{
    bool ReadValue<T>(ODataReaderState<TCustomState> state, out T value);
}
