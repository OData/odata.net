using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.OData.Serializer.Utils;

internal static class ShortLivedArrayHelpers
{
    const int StackAllocCharThreshold = 256;

    public static void CreateCharArray<TState>(int maxLength, in TState state, SpanActionWithRefArg<char, TState> writeAction)
    {
        char[]? allocatedArray = null;
        Span<char> buffer = maxLength <= StackAllocCharThreshold ?
            stackalloc char[maxLength]
            : allocatedArray = ArrayPool<char>.Shared.Rent(maxLength);

        writeAction(buffer[..maxLength], in state);

        if (allocatedArray != null)
        {
            ArrayPool<char>.Shared.Return(allocatedArray);
        }
    }
}

// TODO: using this custom delegate instead of System.Buffers.SpanAction<T, TArg>
// so that I can pass the custom state by ref using in keyword since
// we mostly use tuples with multiple properties to pass the state.
// TODO: should do some benchmarks to see whether this actually improves things.
delegate void SpanActionWithRefArg<T, TArg>(Span<T> span, in TArg arg); // TODO: allow ref struct in .NET 10