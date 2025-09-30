
using Microsoft.OData.Edm;

namespace Microsoft.OData.Serializer;

public class ODataSerializerOptions<TCustomState>
{
    public ODataMetadataLevel MetadataLevel { get; set; } = ODataMetadataLevel.Minimal;
    public ODataVersion ODataVersion { get; set; } = ODataVersion.V4;
    public int BufferSize { get; set; } = 16 * 1024;

    // TODO: this setting should be per serialization session rather than global.
    /// <summary>
    /// When set to true, Edm.Int64 and Edm.Decimal values will be serialized as strings,
    /// otherwise they will be serialized as JSON numbers.
    /// </summary>
    public bool Ieee754Compatible { get; set; } = false;

    public IODataTypeMapper TypeMapper { get; set; } = new DefaultODataTypeMapper();

    internal ICustomAnnotationsHandlerResolver<TCustomState> CustomAnnotationsHandlerResolver { get; set; } = new DefaultCustomAnnotationsHandlerResolver<TCustomState>();
    internal IOpenPropertiesHandlerResolver<TCustomState> DynamicPropertiesHandlerResolver { get; set; } = new DefaultOpenPropertiesHandlerResolver<TCustomState>();

    internal Dictionary<Type, ODataTypeInfo> ResourceTypeInfos { get; } = new();

    public void AddTypeInfo<T>(ODataTypeInfo<T, TCustomState> typeInfo)
    {
        if (typeInfo is null)
        {
            throw new ArgumentNullException(nameof(typeInfo));
        }

        if (ResourceTypeInfos.ContainsKey(typeof(T)))
        {
            throw new InvalidOperationException($"Resource type info for {typeof(T).FullName} is already registered.");
        }

        ResourceTypeInfos[typeof(T)] = typeInfo;

        if (typeInfo.Properties != null)
        {
            var fixedEdmType = typeInfo.EdmType; // The type info always maps to the same IEdmType
            foreach (var property in typeInfo.Properties)
            {
                if (fixedEdmType != null && fixedEdmType is IEdmEntityType fixedEntityType)
                {
                    var fixedEdmProperty = fixedEntityType.FindProperty(property.Name);
                    property.FixedEdmProperty = fixedEdmProperty;
                }
            }
        }
        
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
