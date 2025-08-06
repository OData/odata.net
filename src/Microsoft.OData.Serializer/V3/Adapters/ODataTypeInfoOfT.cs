using Microsoft.OData.Edm;
using Microsoft.OData.Serializer.V3.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.OData.Serializer.V3.Adapters;

public class ODataTypeInfo<T, TCustomState> : ODataTypeInfo
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

    // We expose two approaches to writing annotations:
    // - shorthand GetXXXValue that returns the value, here we select a common, but "cheap" type to represent the value.
    // - WriteXXXValue that accepts a constrained writer that can write the value. The write may have overloads that support different types for more flexibility and performance.
    // - The user should pick only on of these per annotation. Ideally, when initializing the serialzer options, we should throw if both are set for the same annotation.

    public Func<T, ODataJsonWriterState<TCustomState>, long?>? GetCount { get; init; }
    public Action<T, ICountWriter<TCustomState>, ODataJsonWriterState<TCustomState>>? WriteCount { get; init; }

    // Opted to use string for next link as the "common" representation, since Uri is expensive and I don't want to force it as the default.
    // Need to review this with the team because some may argue that Uri as default makes more sense since the intent is clearer and we're sure
    // what we're writing is a valid uri.
    public Func<T, ODataJsonWriterState<TCustomState>, string>? GetNextLink { get; init; }

    public Action<T, INextLinkWriter<TCustomState>, ODataJsonWriterState<TCustomState>>? WriteNextLink { get; init; }

    public Func<T, ODataJsonWriterState<TCustomState>, string>? GetEtag { get; init; }

    public Action<T, IEtagWriter<TCustomState>, ODataJsonWriterState<TCustomState>>? WriteEtag { get; init; }

    public Func<T, IPropertyWriter<T, TCustomState>, ODataJsonWriterState<TCustomState>, ValueTask>? WriteProperties { get; init; }

    /// <summary>
    /// Custom hook that's called before the value is written. If the value
    /// represents an resource/object, this is called before the opening brace is written.
    /// TODO: Determine whether this is called before annotations even for nested annotations of primitive values.
    /// Ideally, I think it should.
    /// </summary>
    public Action<T, ODataJsonWriterState<TCustomState>>? OnSerializing { get; init; }

    /// <summary>
    /// Custom hook that's called after the value is written. If the value represents a resource/object,
    /// this is called after the closing brace is written.
    /// </summary>

    public Action<T, ODataJsonWriterState<TCustomState>>? OnSerialized { get; init; }

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
