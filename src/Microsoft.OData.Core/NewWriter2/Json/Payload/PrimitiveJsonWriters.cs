using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.OData.Core.NewWriter2;

internal class IntJsonwriter : IODataWriter<ODataJsonWriterContext, ODataJsonWriterStack, int>
{
    public ValueTask WriteAsync(int value, ODataJsonWriterStack state, ODataJsonWriterContext context)
    {
        var jsonWriter = context.JsonWriter;
        jsonWriter.WriteNumberValue(value);
        return ValueTask.CompletedTask;
    }
}

internal class BoolJsonWriter : IODataWriter<ODataJsonWriterContext, ODataJsonWriterStack, bool>
{
    public ValueTask WriteAsync(bool value, ODataJsonWriterStack state, ODataJsonWriterContext context)
    {
        var jsonWriter = context.JsonWriter;
        jsonWriter.WriteBooleanValue(value);
        return ValueTask.CompletedTask;
    }
}

internal class StringJsonWriter : IODataWriter<ODataJsonWriterContext, ODataJsonWriterStack, string>
{
    public ValueTask WriteAsync(string value, ODataJsonWriterStack state, ODataJsonWriterContext context)
    {
        var jsonWriter = context.JsonWriter;
        jsonWriter.WriteStringValue(value);
        return ValueTask.CompletedTask;
    }
}

internal class DecimalJsonWriter : IODataWriter<ODataJsonWriterContext, ODataJsonWriterStack, decimal>
{
    public ValueTask WriteAsync(decimal value, ODataJsonWriterStack state, ODataJsonWriterContext context)
    {
        var jsonWriter = context.JsonWriter;
        jsonWriter.WriteNumberValue(value);
        return ValueTask.CompletedTask;
    }
}

internal class DateTimeJsonWriter : IODataWriter<ODataJsonWriterContext, ODataJsonWriterStack, DateTime>
{
    public ValueTask WriteAsync(DateTime value, ODataJsonWriterStack state, ODataJsonWriterContext context)
    {
        var jsonWriter = context.JsonWriter;
#pragma warning disable CA1305 // Specify IFormatProvider
        jsonWriter.WriteStringValue(value.ToString("yyyy-MM-ddTHH:mm:ssZ")); // Ensure OData format is applied
#pragma warning restore CA1305 // Specify IFormatProvider
        return ValueTask.CompletedTask;
    }
}

internal class DateTimeOffsetJsonWriter : IODataWriter<ODataJsonWriterContext, ODataJsonWriterStack, DateTimeOffset>
{
    public ValueTask WriteAsync(DateTimeOffset value, ODataJsonWriterStack state, ODataJsonWriterContext context)
    {
        var jsonWriter = context.JsonWriter;
        jsonWriter.WriteStringValue(value); // TODO: ensure OData format is applied
        return ValueTask.CompletedTask;
    }
}

internal class ByteArrayJsonWriter : IODataWriter<ODataJsonWriterContext, ODataJsonWriterStack, byte[]>
{
    public ValueTask WriteAsync(byte[] value, ODataJsonWriterStack state, ODataJsonWriterContext context)
    {
        var jsonWriter = context.JsonWriter;
        jsonWriter.WriteBase64StringValue(value);
        return ValueTask.CompletedTask;
    }
}