using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.OData.Serializer.V3.IO;

internal interface IBufferedReader<T> : IAsyncDisposable where T : struct
{
    bool TryRead(out ReadResult<T> result);
    ValueTask<ReadResult<T>> ReadAsync();

    void AdvanceTo(in SequencePosition consumed);
    void AdvanceTo(in SequencePosition consumed, in SequencePosition examined);
}
