using Microsoft.OData;
using Microsoft.OData.Edm;
using System.Net.NetworkInformation;
using System.Text.Json;
using System.Threading.Tasks;

namespace Microsoft.OData.Core.NewWriter2;

public class ODataJsonWriterContext
{
    public required ODataUri ODataUri { get; set; }
    public required IEdmModel Model { get; set; }
    public required ODataMetadataLevel MetadataLevel { get; set; }
    public required ODataPayloadKind PayloadKind { get; set; }
    public required ODataVersion ODataVersion { get; set; }
    public JsonSerializerOptions JsonSerializerOptions { get; set; }
    internal Utf8JsonWriter JsonWriter { get; set; }
    internal IResourceWriterProvider<ODataJsonWriterContext, ODataJsonWriterStack>
        ValueWriterProvider { private get; set; }
    internal IResourcePropertyWriterProvider<ODataJsonWriterContext, ODataJsonWriterStack, IEdmProperty>
        ResourcePropertyWriterProvider { private get; set; }
    internal IMetadataWriterProvider<ODataJsonWriterContext, ODataJsonWriterStack, IEdmProperty> MetadataWriterProvider { private get; set; }
    internal IPropertyValueWriterProvider<ODataJsonWriterContext, ODataJsonWriterStack, IEdmProperty>
        PropertyValueWriterProvider { private get; set; }

    public IMetadataWriter<ODataJsonWriterContext, ODataJsonWriterStack, T, IEdmProperty> GetMetadataWriter<T>(ODataJsonWriterStack state)
    {
        return MetadataWriterProvider.GetMetadataWriter<T>(this, state);
    }

    public IResourcePropertyWriter<T, IEdmProperty, ODataJsonWriterStack, ODataJsonWriterContext> GetPropertyWriter<T>(ODataJsonWriterStack state)
    {
        return ResourcePropertyWriterProvider.GetPropertyWriter<T>(state, this);
    }

    public IPropertyValueWriter<ODataJsonWriterContext, ODataJsonWriterStack, T, IEdmProperty> GetPropertyValueWriter<T>(T resource, IEdmProperty property, ODataJsonWriterStack state)
    {
        return PropertyValueWriterProvider.GetPropertyValueWriter<T>(resource, property, state, this);
    }

    public IODataWriter<ODataJsonWriterContext, ODataJsonWriterStack, T> GetValueWriter<T>(ODataJsonWriterStack state)
    {
        return ValueWriterProvider.GetResourceWriter<T>(this, state);
    }

    public ValueTask WriteValueAsync<T>(T value, ODataJsonWriterStack state)
    {
        var writer = GetValueWriter<T>(state);
        return writer.WriteAsync(value, state, this);
    }
}
