using Microsoft.OData.Edm;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.OData.Core.NewWriter2;

internal class EdmPropertyJsonWriter<TResource> :
    IResourcePropertyWriter<TResource, IEdmProperty, ODataJsonWriterStack, ODataJsonWriterContext>
{
    public async ValueTask WriteProperty(TResource resource, IEdmProperty property, ODataJsonWriterStack state, ODataJsonWriterContext context)
    {
        var jsonWriter = context.JsonWriter;
        // if property is collection, we should write annotations if available
        jsonWriter.WritePropertyName(property.Name);

        // write property value
        var valueWriter = context.GetPropertyValueWriter(resource, property, state);
        await valueWriter.WritePropertyValue(resource, property, state, context);
    }
}
