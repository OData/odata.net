using System;
using System.Buffers;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace Microsoft.OData.Serializer;

// TODO: consider making this a struct to avoid the extra alloc, but we'll need
// to refactor all usage to ensure it's always used by ref.
/// <summary>
/// Wrapper around Utf8JsonWriter. This wrapper only exists so that we can manually insert commas
/// after we have bypassed the Utf8JsonWriter to write large (binary) strings in chunks directly
/// to the underlying buffer writer. Since the Utf8JsonWriter is not aware of these writes,
/// it will not insert commas as expected and it will lead to invalid JSON.
/// This workaround is temporary and we should remove it once we move to .NET 10 and can
/// use the built-in chunk writing APIs of Utf8JsonWriter.
/// </summary>
/// <remarks>
/// While this wrapper allows us to correctly insert separators after a value
/// was written to the buffer directly, it does not handle all edge cases.
/// It handles only commas between property-value pairs in objects,
/// it does not handle commas between array elements. The expectation
/// is that writing large values inside arrays is rare and can
/// wait for the production release.
/// </remarks>
/// <param name="writer"></param>
/// <param name="bufferWriter"></param>
internal class Utf8JsonWriterWrapper(Utf8JsonWriter writer, PooledByteBufferWriter bufferWriter)
{
    private bool _lastWriteBypassedWriter = false;
    public int BytesPending => writer.BytesPending;
    public void WritePropertyName(ReadOnlySpan<char> propertyName)
    {
        WriteCommaManuallyIfNecesary();
        writer.WritePropertyName(propertyName);
    }

    public void WritePropertyName(ReadOnlySpan<byte> propertyName)
    {
        WriteCommaManuallyIfNecesary();
        writer.WritePropertyName(propertyName);
    }
    public void WriteString(ReadOnlySpan<byte> propertyName, ReadOnlySpan<byte> value)
    {
        WriteCommaManuallyIfNecesary();
        writer.WriteString(propertyName, value);
    }

    public void WriteString(ReadOnlySpan<byte> propertyName, ReadOnlySpan<char> value)
    {
        WriteCommaManuallyIfNecesary();
        writer.WriteString(propertyName, value);
    }

    public void WriteStringValue(ReadOnlySpan<char> value) => writer.WriteStringValue(value);
    public void WriteStringValue(ReadOnlySpan<byte> value) => writer.WriteStringValue(value);
    public void WriteStringValue(Guid value) => writer.WriteStringValue(value);
    public void WriteStringValue(DateTimeOffset value) => writer.WriteStringValue(value);
    public void WriteStringValue(DateTime value) => writer.WriteStringValue(value);
    public void WriteBase64StringValue(ReadOnlySpan<byte> value) => writer.WriteBase64StringValue(value);
    public void WriteBooleanValue(bool value) => writer.WriteBooleanValue(value);
    public void WriteNumberValue(int value) => writer.WriteNumberValue(value);
    public void WriteNumberValue(long value) => writer.WriteNumberValue(value);
    public void WriteNumberValue(double value) => writer.WriteNumberValue(value);
    public void WriteNumberValue(decimal value) => writer.WriteNumberValue(value);
    public void WriteNumberValue(float value) => writer.WriteNumberValue(value);
    public void WriteNumberValue(uint value) => writer.WriteNumberValue(value);
    public void WriteNumberValue(ulong value) => writer.WriteNumberValue(value);
    public void WriteNumberValue(short value) => writer.WriteNumberValue(value);
    public void WriteNumberValue(ushort value) => writer.WriteNumberValue(value);
    public void WriteNumberValue(byte value) => writer.WriteNumberValue(value);
    public void WriteNumberValue(sbyte value) => writer.WriteNumberValue(value);

    public void WriteNullValue() => writer.WriteNullValue();
    public void WriteStartObject()
    {
        writer.WriteStartObject();
        ResetManualSeparatorSignal();
    }

    public void WriteEndObject()
    {
        writer.WriteEndObject();
        ResetManualSeparatorSignal();
    }
    public void WriteStartArray() => writer.WriteStartArray();
    public void WriteEndArray() => writer.WriteEndArray();
    
    public void Flush() => writer.Flush();

    public void SignalWriteThatBypassedWriter()
    {
        _lastWriteBypassedWriter = true;
    }

    private void ResetManualSeparatorSignal()
    {
        _lastWriteBypassedWriter = false;
    }

    private void WriteCommaManuallyIfNecesary()
    {
        if (_lastWriteBypassedWriter)
        {
            Flush();
            bufferWriter.Write([(byte)',']);
            _lastWriteBypassedWriter = false;
        }
    }
}
