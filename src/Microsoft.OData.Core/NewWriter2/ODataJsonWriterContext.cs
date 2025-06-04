using Microsoft.OData.Edm;
using System.Text.Json;

namespace Microsoft.OData.Core.NewWriter2;

internal class ODataJsonWriterContext
{
    public ODataUri ODataUri { get; set; }
    public IEdmModel Model { get; set; }
    public ODataPayloadKind PayloadKind { get; set; }
    public JsonSerializerOptions JsonSerializerOptions { get; set; }
    public Utf8JsonWriter JsonWriter { get; set; }
    public IResourceWriterProvider<ODataJsonWriterContext, ODataJsonWriterStack> ResourceWriterProvider { get; set; }
}
