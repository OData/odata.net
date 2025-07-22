using System;
using System.Buffers;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Unicode;

namespace Microsoft.OData.Serializer.Json;

internal static class JsonMetadataHelpers
{
    public static void WritePropertyAnnotationName(
        Utf8JsonWriter writer,
        ReadOnlySpan<char> propertyName,
        ReadOnlySpan<byte> annotationName)
    {
        // TODO: if we have access to Utf8JsonWriter's underlying IBufferWriter,
        // we could write directly to and avoid performing this concatenation
        // but that then Utf8JsonWriter will not know about this write
        // and it may lead to corrupted writes or unexpected behaviour.
        // It becomes very error-prone and hard to maintain.

        const int StackAllocThreshold = 128;
        int combinedLength = propertyName.Length + annotationName.Length;
        int maxCombinedLength = propertyName.Length * 6 + annotationName.Length; // worst case for UTF-8 encoding


        byte[] rentedArray = null;

        Span<byte> buffer = combinedLength < StackAllocThreshold ?
            stackalloc byte[combinedLength] : ArrayPool<byte>.Shared.Rent(combinedLength);

        Utf8.FromUtf16(propertyName, buffer, out _, out var propertyBytesWritten);
        annotationName.CopyTo(buffer.Slice(propertyBytesWritten));

        var fullAnnotationName = buffer.Slice(0, propertyBytesWritten + annotationName.Length);
        writer.WritePropertyName(fullAnnotationName);

        if (rentedArray is not null)
        {
            ArrayPool<byte>.Shared.Return(rentedArray);
        }
    }
}
