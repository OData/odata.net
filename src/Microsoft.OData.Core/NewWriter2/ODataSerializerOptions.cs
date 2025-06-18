using Microsoft.OData.Edm;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace Microsoft.OData.Core.NewWriter2;

internal sealed class ODataSerializerOptions
{
    private IMetadataWriterProvider<ODataJsonWriterContext, ODataJsonWriterStack, IEdmProperty> _metadataWriterProvider = null;
    public ODataMetadataLevel MetadataLevel { get; set; } = ODataMetadataLevel.Minimal;
    public ODataVersion ODataVersion { get; set; } = ODataVersion.V4;
    public IResourceWriterProvider<ODataJsonWriterContext, ODataJsonWriterStack> ValueWriterProvider { get; set; }
        = new ResourceJsonWriterProvider();
    public JsonSerializerOptions JsonSerializerOptions { get; set; } = JsonSerializerOptions.Default;

    public IResourcePropertyWriterProvider<ODataJsonWriterContext, ODataJsonWriterStack, IEdmProperty> ResourcePropertyWriterProvider { get; set; }
        = new EdmPropertyJsonWriterProvider();

    public IMetadataValueProvider<ODataJsonWriterContext, ODataJsonWriterStack, IEdmProperty> MetadataValueProvider { get; set; } =
        new JsonMetadataValueProvider();

    public IMetadataWriterProvider<ODataJsonWriterContext, ODataJsonWriterStack, IEdmProperty> MetadataWriterProvider
    {
        get => _metadataWriterProvider ??= new JsonMetadataWriterProvider(this.MetadataValueProvider);
        set => _metadataWriterProvider = value;
    }

    public IPropertyValueWriterProvider<ODataJsonWriterContext, ODataJsonWriterStack, IEdmProperty> PropertyValueWriterProvider { get; set; }
        = new EdmPropertyValueJsonWriterProvider();
}
