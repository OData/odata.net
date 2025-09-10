using Microsoft.OData.Serializer.V3.Utils;
using Microsoft.OData.UriParser;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Text.Json;

namespace Microsoft.OData.Serializer.V3.ContextUrl;

internal static class ContextUrlHelper
{
    public static void WriteContextUrlProperty(ODataPayloadKind payloadKind, ODataUri uri, Utf8JsonWriter jsonWriter)
    {
        switch (payloadKind)
        {
            case ODataPayloadKind.ResourceSet:
                WriteResourceSetContextUrl(uri, jsonWriter);
                break;

            default:
                // No context URL for other payload kinds
                throw new NotImplementedException();
        }
    }

    internal static void WriteResourceSetContextUrl(ODataUri odataUri, Utf8JsonWriter jsonWriter)
    {
        if (odataUri == null)
        {
            return;
        }

        if (odataUri.Path.Count == 1)
        {
            WriteContextUrlPropertyForSimpleEntitySet(odataUri, jsonWriter);
            return;
        }

        var writer = jsonWriter;

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

    internal static void WriteContextUrlPropertyForSimpleEntitySet(ODataUri odataUri, Utf8JsonWriter jsonWriter)
    {
        Debug.Assert(odataUri.SelectAndExpand == null);
        Debug.Assert(odataUri.Apply == null);

        var absoluteUri = odataUri.ServiceRoot?.AbsoluteUri ?? string.Empty;
        
        const string metadata = "$metadata#";

        Debug.Assert(odataUri.Path.Count == 1);
        var segment = odataUri.Path[0] as EntitySetSegment;
        Debug.Assert(segment != null);

        int totalLength = absoluteUri.Length + metadata.Length + segment.EntitySet.Name.Length;

        (string AbsoluteUri, string SegmentName, Utf8JsonWriter Writer) state = (absoluteUri, segment.EntitySet.Name, jsonWriter);

        ShortLivedArrayHelpers.WriteCharArray(totalLength, state, static (uriStringBuffer, state) =>
        {
            var builder = new SpanStringBuilder(uriStringBuffer);
            builder.Append(state.AbsoluteUri);
            builder.Append(metadata);
            builder.Append(state.SegmentName);

            state.Writer.WriteString("@odata.context", builder.WrittenSpan);
        });
    }

    internal static void WriteContextUrlPropertyForEntitySetWithSelectExpand(ODataUri odataUri, Utf8JsonWriter writer)
    {
        var absoluteUri = odataUri.ServiceRoot?.AbsoluteUri;
        const string metadata = "$metadata#";
    }
}
