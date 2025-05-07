using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.OData.Core.NewWriter;

internal interface IODataWriter<T>
{
    ValueTask WriteAsync(T value, ODataWriterState state);
}
