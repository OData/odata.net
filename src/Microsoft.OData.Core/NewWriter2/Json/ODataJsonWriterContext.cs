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
    public required IMetadataWriterProvider<ODataJsonWriterContext, ODataJsonWriterStack> MetadataWriterProvider { get; set; }
    public required IPropertyValueWriterProvider<ODataJsonWriterContext, ODataJsonWriterStack, IEdmProperty> PropertyValueWriterProvider { get; set; }
}
