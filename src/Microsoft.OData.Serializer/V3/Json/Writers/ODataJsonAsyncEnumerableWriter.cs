using Microsoft.OData.Serializer.V3.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.OData.Serializer.V3.Json.Writers;

internal class ODataJsonAsyncEnumerableWriter<TCustomState> :
    ODataJsonWriter<IBufferedReader<byte>, TCustomState>
{
    public override bool Write(
        IBufferedReader<byte> value,
        ODataJsonWriterState<TCustomState> state)
    {
        
    }
}
