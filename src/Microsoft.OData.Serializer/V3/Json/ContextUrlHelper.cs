using System;
using System.Buffers;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.OData.Serializer.V3.Json;

internal static class ContextUrlHelper
{
    public static void WriteContextUrlProperty(ODataPayloadKind payloadKind, ODataJsonWriterState state)
    {
        switch (payloadKind)
        {
            case ODataPayloadKind.ResourceSet:
                WriteResourceSetContextUrl(state);
                break;

            default:
                // No context URL for other payload kinds
                throw new NotImplementedException();
        }
    }

    internal static void WriteResourceSetContextUrl(ODataJsonWriterState state)
    {
        if (state.ODataUri == null)
        {
            return;
        }

        var odataUri = state.ODataUri;
        var writer = state.JsonWriter;

        // TODO: This is a naive, incorrect, incomplete implementation of the context URL.
        // It's just a placeholder to show how it might be constructed.

        // TODO: in .NET 10 we can write consider Utf8JsonWriter.WriteStringValueSegment
        // to write the string in chunks

        var absoluteUri = odataUri.ServiceRoot?.AbsoluteUri;
        const string metadata = "$metadata#";

        // One pass through the segments to compute total size to avoid multiple array allocations (e.g. with StringBuilder)
        int totalSize = (absoluteUri?.Length ?? 0)
            + metadata.Length;

        if (odataUri.Path.Count > 0)
        {
            totalSize += odataUri.Path[0].Identifier.Length;
        }

        for (int i = 1; i < odataUri.Path.Count; i++)
        {
            totalSize += odataUri.Path[i].Identifier.Length + 1; // +1 for the separator
        }

        char[]? array = null;
        Span<char> buffer = totalSize <= 128 ?
            stackalloc char[totalSize] :
            (array = ArrayPool<char>.Shared.Rent(totalSize));

        buffer = buffer[..totalSize]; // Slice the buffer in case we rented a larger array than needed
        int offset = 0;
        absoluteUri.AsSpan().CopyTo(buffer);
        offset += absoluteUri?.Length ?? 0;
        metadata.AsSpan().CopyTo(buffer[offset..]);
        offset += metadata.Length;

        if (odataUri.Path.Count > 0)
        {
            odataUri.Path[0].Identifier.AsSpan().CopyTo(buffer[offset..]);
            offset += odataUri.Path[0].Identifier.Length;
        }

        for (int i = 1; i < odataUri.Path.Count; i++)
        {
            buffer[offset] = '/';
            odataUri.Path[i].Identifier.AsSpan().CopyTo(buffer[(offset + 1)..]);
            offset += 1 + odataUri.Path[i].Identifier.Length;
        }

        writer.WriteString("@odata.context"u8, buffer);

        if (array != null)
        {
            ArrayPool<char>.Shared.Return(array);
        }
    }
}
