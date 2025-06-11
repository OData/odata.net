using Microsoft.OData;
using Microsoft.OData.Core.NewWriter2.Core.Resource;
using Microsoft.OData.Edm;
using System.Text.Json;

namespace Microsoft.OData.Core.NewWriter2;

internal class ODataJsonWriterContext
{
    public required ODataUri ODataUri { get; set; }
    public required IEdmModel Model { get; set; }
    public required ODataMetadataLevel MetadataLevel { get; set; }
    public required ODataPayloadKind PayloadKind { get; set; }
    public required ODataVersion ODataVersion { get; set; }
    public JsonSerializerOptions JsonSerializerOptions { get; set; }
    public required Utf8JsonWriter JsonWriter { get; set; }
    public required IResourceWriterProvider<ODataJsonWriterContext, ODataJsonWriterStack> ResourceWriterProvider { get; set; }
    public required IResourcePropertyWriterProvider<ODataJsonWriterContext, ODataJsonWriterStack, IEdmProperty> ResourcePropertyWriterProvider { get; set; }
    public required IMetadataWriterProvider<ODataJsonWriterContext, ODataJsonWriterStack> MetadataWriterProvider { get; set; }
    public required IPropertyValueWriterProvider<ODataJsonWriterContext, ODataJsonWriterStack, IEdmProperty> PropertyValueWriterProvider { get; set; }

    public IMetadataWriter<ODataJsonWriterContext, ODataJsonWriterStack, T> GetMetadataWriter<T>(ODataJsonWriterStack state)
    {
        return MetadataWriterProvider.GetMetadataWriter<T>(this, state);
    }

    public IPropertyValueWriter<ODataJsonWriterContext, ODataJsonWriterStack, T, IEdmProperty> GetPropertyValueWriter<T>(T resource, IEdmProperty property, ODataJsonWriterStack state)
    {
        return PropertyValueWriterProvider.GetPropertyValueWriter<T>(resource, property, state, this);
    }
}
