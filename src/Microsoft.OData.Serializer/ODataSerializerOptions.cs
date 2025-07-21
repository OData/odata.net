using Microsoft.OData.Edm;
using Microsoft.OData.Serializer.Core;
using Microsoft.OData.Serializer.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Microsoft.OData.Serializer;

public sealed class ODataSerializerOptions
{
    private IMetadataWriterProvider<ODataJsonWriterContext, ODataJsonWriterStack, IEdmProperty> _metadataWriterProvider = null;
    private EdmPropertyValueJsonWriterProvider _propertyValueWriterProvider = new EdmPropertyValueJsonWriterProvider();
    private ResourceJsonWriterProvider _valueWriterProvider = new ResourceJsonWriterProvider();

    public ODataMetadataLevel MetadataLevel { get; set; } = ODataMetadataLevel.Minimal;
    public ODataVersion ODataVersion { get; set; } = ODataVersion.V4;
    public IResourceWriterProvider<ODataJsonWriterContext, ODataJsonWriterStack> ValueWriterProvider => _valueWriterProvider;
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

    public IPropertyValueWriterProvider<ODataJsonWriterContext, ODataJsonWriterStack, IEdmProperty> PropertyValueWriterProvider
    {
        get => _propertyValueWriterProvider;
        set => _propertyValueWriterProvider = value as EdmPropertyValueJsonWriterProvider ?? new EdmPropertyValueJsonWriterProvider();
    }

    public void AddPropertyValueWriter<TResource>(Func<TResource, IEdmProperty, ODataJsonWriterStack, ODataJsonWriterContext, ValueTask> writeProperty)
    {
        _propertyValueWriterProvider.Add(writeProperty);
    }

    public void AddValueWriter<T>(IODataWriter<ODataJsonWriterContext, ODataJsonWriterStack, T> writer)
    {
        _valueWriterProvider.AddValueWriter<T>(writer);
    }
}
