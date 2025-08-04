using Microsoft.OData.Edm;
using Microsoft.OData.Serializer.V3.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.OData.Serializer.V3.Adapters;

public class ODataResourceTypeInfo<T, TCustomState> : ODataResourceTypeInfo
{
    private IReadOnlyList<ODataPropertyInfo<T, TCustomState>>? _properties;
    // TODO use alternate key and also support Utf8-based lookups if necessary
    private Dictionary<string, ODataPropertyInfo<T, TCustomState>> _propertiesCache = new();
    public override Type Type { get; init; } = typeof(T);

    public IReadOnlyList<ODataPropertyInfo<T, TCustomState>>? Properties
    {
        get
        {
            return _properties;
        }
        set
        {
            // TODO: detect duplicates and throw if any
            _properties = value;
            foreach (var prop in _properties)
            {
                _propertiesCache[prop.Name] = prop;
            }

        }
    }

    public Func<T, ODataJsonWriterState<TCustomState>, bool>? HasCount { get; init; }
    public Func<T, ODataJsonWriterState<TCustomState>, ValueTask>? WriteCount { get; init; }

    public Func<T, ODataJsonWriterState<TCustomState>, bool>? HasNextLink { get; init; }

    public Func<T, ODataJsonWriterState<TCustomState>, ValueTask>? WriteNextLink { get; init; }

    public Func<T, ODataJsonWriterState<TCustomState>, bool>? HasEtag { get; init; }

    public Func<T, ODataJsonWriterState<TCustomState>, ValueTask>? WriteEtag { get; init; }

    public Func<T, IPropertyWriter<T, TCustomState>, ODataJsonWriterState<TCustomState>, ValueTask>? WriteProperties { get; init; }

    /// <summary>
    /// Custom hook that's called before the value is written. If the value
    /// represents an resource/object, this is called before the opening brace is written.
    /// TODO: Determine whether this is called before annotations even for nested annotations of primitive values.
    /// Ideally, I think it should.
    /// </summary>
    public Action<T, ODataJsonWriterState<TCustomState>>? OnSerializing { get; init; }

    public ODataPropertyInfo<T, TCustomState> GetPropertyInfo(string name)
    {
        // TODO: we should throw instead of returning
        if (_propertiesCache.TryGetValue(name, out var prop))
        {
            return prop;
        }
        
        throw new Exception($"Property '{name}' not found in type '{Type.Name}'");
    }

}
