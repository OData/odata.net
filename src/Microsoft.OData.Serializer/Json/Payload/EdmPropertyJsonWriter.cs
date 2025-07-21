using Microsoft.OData.Edm;
using Microsoft.OData.Serializer.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.OData.Serializer.Json;

internal class EdmPropertyJsonWriter<TResource> :
    IResourcePropertyWriter<TResource, IEdmProperty, ODataJsonWriterStack, ODataJsonWriterContext>
{
    public async ValueTask WriteProperty(TResource resource, IEdmProperty property, ODataJsonWriterStack state, ODataJsonWriterContext context)
    {
        var jsonWriter = context.JsonWriter;

        if (property.Type.IsCollection())
        {
            // The challenge here is that we don't know the value of the type of the property at this point.
            // We could move this logic to the value writer, but we would have written the property name already.
            // That's only reasonable if we either move all property writing logic to the value writer and do away with the property writer,
            // Or we create a metadata provider that is based on the IEdmType rather than the input type.
            // Alternative, we get a type mapper that maps the IEdmType to the CLR type (and vice versa).
            // The challenge with an Edm to CLR type mapper is that, while it's likely that you'd have only one Edm type for a given CLR type,
            // the reverse is less likely (e.g. IEnumerable<Customer>, IList<Customer>, CustomCustomerCollection,
            // could all map to Edmetc. all map to the same Edm type).
            // Another alternative, the metadata resource's metadata writer could be responsible for
            // writing annotations for its properties.
            var metadataWriter = context.GetMetadataWriter<TResource>(state);
            await metadataWriter.WriteNestedCountPropertyAsync(resource, property, state, context);
            await metadataWriter.WriteNestedNextLinkPropertyAsync(resource, property, state, context);
        }

        // if property is collection, we should write annotations if available
        jsonWriter.WritePropertyName(property.Name);

        // TODO: handle scenario where we don't need to write the value, just annotations.
        // write property value
        var valueWriter = context.GetPropertyValueWriter(resource, property, state);
        await valueWriter.WritePropertyValue(resource, property, state, context);
    }
}
