using Microsoft.OData.Edm;
using Microsoft.OData.Serializer.Core;
using Microsoft.OData.Serializer.Json;
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

    internal Dictionary<Type, ODataResourceTypeInfo> ResourceTypeInfos { get; } = new();

    public void AddTypeInfo<T>(ODataResourceTypeInfo<T, TCustomState> resourceTypeInfo)
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

    public ODataResourceTypeInfo<T, TCustomState>? TryGetResourceInfo<T>()
    {
        if (ResourceTypeInfos.TryGetValue(typeof(T), out var typeInfo))
        {
            return (ODataResourceTypeInfo<T, TCustomState>)typeInfo;
        }

        return null;
    }

    public ODataResourceTypeInfo? TryGetResourceInfo(Type type)
    {
        if (ResourceTypeInfos.TryGetValue(type, out var typeInfo))
        {
            return typeInfo;
        }

        return null;
    }
}
