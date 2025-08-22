using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.OData.Serializer.V3.IO;

internal readonly struct ReadResult<T>(ReadOnlySequence<T> buffer, bool isCompleted)
{
    public ReadOnlySequence<T> Buffer { get; } = buffer;
    public bool IsCompleted { get; } = isCompleted;
}
