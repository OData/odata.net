using Microsoft.OData.Serializer.V3.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.OData.Serializer.V3.Adapters;

// TODO: perhaps this should be an interface instead of abstract class.
// Abstract class makes it easier to add common functionality or optional methods
// like support for annotations, etc. Not sure if this is where we'll add that support though.
// TODO: internal because this is not yet supported.
internal abstract class ODataAsyncPropertyWriter<TDeclaringType, TCustomState>
{
    public abstract ValueTask WriteValueAsync(
        TDeclaringType resource,
        IStreamValueWriter<TCustomState> writer,
        ODataWriterState<TCustomState> state);
}
