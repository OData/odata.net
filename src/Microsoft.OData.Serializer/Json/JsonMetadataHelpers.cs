using System.Buffers;
using System.Text;
using System.Text.Json;
using System.Text.Unicode;

namespace Microsoft.OData.Serializer;

internal static class JsonMetadataHelpers
{
    public static void WritePropertyAnnotationName(
        Utf8JsonWriter writer,
        ReadOnlySpan<char> propertyName,
        ReadOnlySpan<byte> annotationName) // annotationName should include @separator
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

        Span<byte> buffer = maxCombinedLength < StackAllocThreshold ?
            stackalloc byte[maxCombinedLength] : rentedArray = ArrayPool<byte>.Shared.Rent(maxCombinedLength);

        Utf8.FromUtf16(propertyName, buffer, out _, out var propertyBytesWritten);
        annotationName.CopyTo(buffer.Slice(propertyBytesWritten));

        var fullAnnotationName = buffer.Slice(0, propertyBytesWritten + annotationName.Length);
        writer.WritePropertyName(fullAnnotationName);

        if (rentedArray is not null)
        {
            ArrayPool<byte>.Shared.Return(rentedArray);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="writer"></param>
    /// <param name="propertyName">The name of the property the annotation belongs to.</param>
    /// <param name="annotationName">The annotation name, without the leading @ separator.</param>
    public static void WritePropertyAnnotationName(
        Utf8JsonWriter writer,
        ReadOnlySpan<char> propertyName,
        ReadOnlySpan<char> annotationName)
    {
        // TODO: if we have access to Utf8JsonWriter's underlying IBufferWriter,
        // we could write directly to and avoid performing this concatenation
        // but that then Utf8JsonWriter will not know about this write
        // and it may lead to corrupted writes or unexpected behaviour.
        // It becomes very error-prone and hard to maintain.

        const int StackAllocThreshold = 128;
        int combinedLength = propertyName.Length + annotationName.Length + 1;


        char[] rentedArray = null;

        Span<char> buffer = combinedLength < StackAllocThreshold ?
            stackalloc char[combinedLength] : rentedArray = ArrayPool<char>.Shared.Rent(combinedLength);

        propertyName.CopyTo(buffer);
        buffer[propertyName.Length] = '@'; // Add the @ separator
        annotationName.CopyTo(buffer.Slice(propertyName.Length + 1));

        buffer = buffer[..combinedLength]; // Trim to the exact length since rented array may be larger
        writer.WritePropertyName(buffer);

        if (rentedArray is not null)
        {
            ArrayPool<char>.Shared.Return(rentedArray);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="writer"></param>
    /// <param name="propertyName">The name of the property the annotation belongs to.</param>
    /// <param name="annotationName">The annotation name, without the leading @ separator.</param>
    public static void WritePropertyAnnotationName(
        Utf8JsonWriter writer,
        ReadOnlySpan<byte> propertyName,
        ReadOnlySpan<byte> annotationName)
    {
        // TODO: if we have access to Utf8JsonWriter's underlying IBufferWriter,
        // we could write directly to and avoid performing this concatenation
        // but that then Utf8JsonWriter will not know about this write
        // and it may lead to corrupted writes or unexpected behaviour.
        // It becomes very error-prone and hard to maintain.

        const int StackAllocThreshold = 128;
        int combinedLength = propertyName.Length + annotationName.Length + 1;


        byte[]? rentedArray = null;

        Span<byte> buffer = combinedLength < StackAllocThreshold ?
            stackalloc byte[combinedLength] :
            rentedArray = ArrayPool<byte>.Shared.Rent(combinedLength);

        propertyName.CopyTo(buffer);
        buffer[propertyName.Length] = (byte)'@'; // Add the @ separator
        annotationName.CopyTo(buffer[(propertyName.Length + 1)..]);

        buffer = buffer[..combinedLength]; // Trim to the exact length since rented array may be larger
        writer.WritePropertyName(buffer);

        if (rentedArray is not null)
        {
            ArrayPool<byte>.Shared.Return(rentedArray);
        }
    }

    /// <summary>
    /// Writes a custom (non-standard) annotation name.
    /// This is should be a non-nested annotation name (without property prefix).
    /// Custom annotations should include a custom non-odata prefix such as "foo.bar".
    /// </summary>
    /// <param name="writer"></param>
    /// <param name="annotationName">The custom annotation name. Should inclued a namespace and should not include the leading @.</param>
    public static void WriteCustomAnnotationName(
        Utf8JsonWriter writer,
        ReadOnlySpan<char> annotationName)
    {
        ValidateCustomAnnotationName(annotationName);

        const int StackAllocThreshold = 128;
        int combinedLength = annotationName.Length + 1;


        char[] rentedArray = null;

        Span<char> buffer = combinedLength < StackAllocThreshold ?
            stackalloc char[combinedLength] : rentedArray = ArrayPool<char>.Shared.Rent(combinedLength);

        buffer[0] = '@';
        annotationName.CopyTo(buffer[1..]);
        buffer = buffer[..combinedLength]; // Trim to the exact length since rented array may be larger

        writer.WritePropertyName(buffer);

        if (rentedArray is not null)
        {
            ArrayPool<char>.Shared.Return(rentedArray);
        }
    }

    /// <summary>
    /// Writes a custom (non-standard) annotation name.
    /// This is should be a non-nested annotation name (without property prefix).
    /// Custom annotations should include a custom non-odata prefix such as "foo.bar".
    /// </summary>
    /// <param name="writer"></param>
    /// <param name="annotationName">The custom annotation name. Should inclued a namespace and should not include the leading @.</param>
    public static void WriteCustomAnnotationName(
        Utf8JsonWriter writer,
        ReadOnlySpan<byte> annotationName)
    {

        ////var dotIndex = annotationName.IndexOf('.');
        ////if (dotIndex < 1)
        ////{
        ////    throw new ArgumentException("Custom annotation names must include a prefix and a dot, e.g. 'foo.bar'.", nameof(annotationName));
        ////}

        ////if (annotationName.StartsWith("odata."))
        ////{
        ////    throw new ArgumentException("Custom annotation names must not start with 'odata.' prefix.", nameof(annotationName));
        ////}
        ///
        // TODO: should validate this. With UTF-8, when we search for a ., we should make sure it's not part of a multi-byte character.

        ValidateCustomAnnotationName(annotationName);


        const int StackAllocThreshold = 128;
        int combinedLength = annotationName.Length + 1;


        byte[] rentedArray = null;

        Span<byte> buffer = combinedLength < StackAllocThreshold ?
            stackalloc byte[combinedLength] : rentedArray = ArrayPool<byte>.Shared.Rent(combinedLength);

        buffer[0] = (byte)'@';
        annotationName.CopyTo(buffer[1..]);
        buffer = buffer[..combinedLength]; // Trim to the exact length since rented array may be larger

        writer.WritePropertyName(buffer);

        if (rentedArray is not null)
        {
            ArrayPool<byte>.Shared.Return(rentedArray);
        }
    }

    public static void WriteCustomPropertyAnnotationName(
        Utf8JsonWriter writer,
        ReadOnlySpan<char> propertyName,
        ReadOnlySpan<char> annotationName)
    {
        // TODO: if we have access to Utf8JsonWriter's underlying IBufferWriter,
        // we could write directly to and avoid performing this concatenation
        // but that then Utf8JsonWriter will not know about this write
        // and it may lead to corrupted writes or unexpected behaviour.
        // It becomes very error-prone and hard to maintain.

        ValidateCustomAnnotationName(annotationName);

        WritePropertyAnnotationName(writer, propertyName, annotationName);
    }

    public static void WriteCustomPropertyAnnotationName(
        Utf8JsonWriter writer,
        ReadOnlySpan<byte> propertyName,
        ReadOnlySpan<byte> annotationName)
    {

        // TODO: validate custom annotation name
        ValidateCustomAnnotationName(annotationName);

        WritePropertyAnnotationName(writer, propertyName, annotationName);
    }
    
    private static void ValidateCustomAnnotationName(ReadOnlySpan<char> annotationName)
    {
        if (annotationName.IndexOf('.') < 1)
        {
            throw new ArgumentException($"Custom annotation {annotationName} names must include a namespace and a dot, e.g. 'foo.bar'.");
        }

        if (annotationName.StartsWith("odata."))
        {
            throw new ArgumentException($"Custom annotation {annotationName} names must not start with 'odata.' prefix.");
        }

        if (annotationName[0] == '@')
        {
            throw new ArgumentException($"Custom annotation {annotationName} names must not start with '@' character.");
        }
    }

    private static void ValidateCustomAnnotationName(ReadOnlySpan<byte> annotationName)
    {
        //if (annotationName.IndexOf('.') < 1)
        //{
        //    throw new ArgumentException($"Custom annotation {annotationName} names must include a namespace and a dot, e.g. 'foo.bar'.");
        //}

        //if (annotationName.StartsWith("odata."))
        //{
        //    throw new ArgumentException($"Custom annotation {annotationName} names must not start with 'odata.' prefix.", nameof(annotationName));
        //}

        if (annotationName[0] == (byte)'@')
        {
            // TODO: use a more efficient approach to print name.
            throw new ArgumentException($"Custom annotation {Encoding.Unicode.GetString(annotationName)} names must not start with '@' character.");
        }
    }
}
