using Microsoft.OData.Serializer.V3.Adapters;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Microsoft.OData.Serializer.V3;

public class ODataSerializerOptions<TCustomState>
{
    public ODataMetadataLevel MetadataLevel { get; set; } = ODataMetadataLevel.Minimal;
    public ODataVersion ODataVersion { get; set; } = ODataVersion.V4;
    public int BufferSize { get; set; } = 16 * 1024;

    public IODataTypeMapper TypeMapper { get; set; } = new DefaultODataTypeMapper();

    internal ICustomAnnotationsHandlerResolver<TCustomState> CustomAnnotationsHandlerResolver { get; set; } = new DefaultCustomAnnotationsHandlerResolver<TCustomState>();
    internal IDynamicPropertiesHandlerResolver<TCustomState> DynamicPropertiesHandlerResolver { get; set; } = new DefaultDynamicPropertiesHandlerResolver<TCustomState>();

    internal Dictionary<Type, ODataTypeInfo> ResourceTypeInfos { get; } = new();

    public void AddTypeInfo<T>(ODataTypeInfo<T, TCustomState> resourceTypeInfo)
    {
        if (resourceTypeInfo is null)
        {
            throw new ArgumentNullException(nameof(resourceTypeInfo));
        }

        if (ResourceTypeInfos.ContainsKey(typeof(T)))
        {
            throw new InvalidOperationException($"Resource type info for {typeof(T).FullName} is already registered.");
        }
        ResourceTypeInfos[typeof(T)] = resourceTypeInfo;
    }

    public ODataTypeInfo<T, TCustomState>? TryGetResourceInfo<T>()
    {
        if (ResourceTypeInfos.TryGetValue(typeof(T), out var typeInfo))
        {
            return (ODataTypeInfo<T, TCustomState>)typeInfo;
        }

        return null;
    }

    public ODataTypeInfo? TryGetResourceInfo(Type type)
    {
        if (ResourceTypeInfos.TryGetValue(type, out var typeInfo))
        {
            return typeInfo;
        }

        return null;
    }
}
