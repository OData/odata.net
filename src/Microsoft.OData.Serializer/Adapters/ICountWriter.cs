using Microsoft.OData.Serializer.Json.State;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.OData.Serializer.Adapters;

public interface ICountWriter<TCustomState>
{
    void WriteCount(long count, ODataWriterState<TCustomState> state);
}
