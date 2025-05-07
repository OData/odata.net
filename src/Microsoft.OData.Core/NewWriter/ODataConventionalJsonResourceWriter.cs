using Microsoft.OData.Edm;
using Microsoft.OData.UriParser;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Microsoft.OData.Core.NewWriter;

internal class ODataConventionalJsonResourceWriter<T>(
    Utf8JsonWriter jsonWriter,
    IPropertySelector<T, IEdmProperty> propertySelector,
    IResourcePropertyWriter<T, IEdmProperty> propertyWriter) : IODataWriter<T>
{
    public async ValueTask WriteAsync(T payload, ODataWriterState context)
    {
        // write start
        jsonWriter.WriteStartObject();
        
        // write annotations

        // find list of properties to write
        var properties = propertySelector.GetProperties(payload, context);
        // for each property, write it out
        foreach (var property in properties)
        {
            // Does this need to be async?
            await propertyWriter.WriteProperty(payload, property, context);
        }


        // write end
        jsonWriter.WriteEndObject();
    }
}
