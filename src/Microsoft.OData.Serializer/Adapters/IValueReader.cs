using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.OData.Serializer;

public interface IValueReader<TCustomState>
{
    int GetInt32(ODataReaderState<TCustomState> state);
    bool GetBoolean(ODataReaderState<TCustomState> state);
    string? GetString(ODataReaderState<TCustomState> state);
}
