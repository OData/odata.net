using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.OData.Serializer.V3.Utils;

internal static class ShortLivedArrayHelpers
{
    const int StackAllocCharThreshold = 256;

    public static void WriteCharArray<TState>(int maxLength, TState state, SpanAction<char, TState> writeAction)
    {
        char[]? allocatedArray = null;
        Span<char> buffer = maxLength <= StackAllocCharThreshold ?
            stackalloc char[maxLength]
            : allocatedArray = ArrayPool<char>.Shared.Rent(maxLength);

        writeAction(buffer[..maxLength], state);

        if (allocatedArray != null)
        {
            ArrayPool<char>.Shared.Return(allocatedArray);
        }
    }
}
