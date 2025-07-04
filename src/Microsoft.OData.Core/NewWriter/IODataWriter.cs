using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.OData.Core.NewWriter;

internal interface IODataWriter<T> : IODataWriter
{
    ValueTask WriteAsync(T value, ODataWriterState state);
}

internal interface IODataWriter
{
    ValueTask WriteAsync(object value, ODataWriterState state);
}