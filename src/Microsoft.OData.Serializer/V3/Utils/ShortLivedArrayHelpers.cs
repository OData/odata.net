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

// TODO: this would allow us to pass the TArg as an in parameter, i.e. by ref
// and could be more efficient for passing large structs by ref instead
// of copies, since we're passing multiple values in a tuple in most scenarios.
// However, using parameter modifies in lambda functions is still in preview.
//delegate void SpanActionWithRefArg<T, TArg>(Span<T> span, in TArg arg);