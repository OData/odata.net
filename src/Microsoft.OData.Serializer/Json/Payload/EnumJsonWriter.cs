using Microsoft.OData.Serializer.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.OData.Serializer.Json;

// TODO: ideally this should be a factory that generates efficient, non-boxing writer for each enum type
// But for convenience, this is just a placeholder
[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "This class is instantiated via reflection.")]
internal class EnumJsonWriter<T> : IODataWriter<ODataJsonWriterContext, ODataJsonWriterStack, T> where T :Enum
{
    public ValueTask WriteAsync(T value, ODataJsonWriterStack state, ODataJsonWriterContext context)
    {
        var jsonWriter = context.JsonWriter;
        // Write the enum value as a string
        jsonWriter.WriteStringValue(value.ToString());
        return ValueTask.CompletedTask;
    }
}