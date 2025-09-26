using Microsoft.OData.Edm;

namespace Microsoft.OData.Serializer;

public class ODataTypeInfo<T, TCustomState> : ODataTypeInfo
{
    private IReadOnlyList<ODataPropertyInfo<T, TCustomState>>? _properties;
    // TODO use alternate key and also support Utf8-based lookups if necessary
    private Dictionary<string, ODataPropertyInfo<T, TCustomState>> _propertiesCache = new();
    public override Type Type { get; init; } = typeof(T);

    /// <summary>
    /// Gets or sets a function that returns the EDM type associated with the CLR type
    /// represented by this <see cref="ODataTypeInfo"/>. This should be set
    /// if the CLR type can map to different EDM types based on the value or state.
    /// If the CLR type always maps to the same EDM type, consider using the
    /// <see cref="ODataTypeInfo.ODataTypeInfo"/> property instead.
    /// </summary>
    public Func<T, ODataWriterState<TCustomState>, IEdmType>? GetEdmType { get; set; }

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

    public ODataPropertySelector<T, TCustomState>? PropertySelector { get; set; }

    // We expose two approaches to writing annotations:
    // - shorthand GetXXXValue that returns the value, here we select a common, but "cheap" type to represent the value.
    // - WriteXXXValue that accepts a constrained writer that can write the value. The write may have overloads that support different types for more flexibility and performance.
    // - The user should pick only on of these per annotation. Ideally, when initializing the serialzer options, we should throw if both are set for the same annotation.

    // TODO: I considered whether the position should be a func or property. I should evaluate cost of calling the func vs property/field.
    // Do we expect the position to change per value? I think for a given type or property, the position of the annotation is likely
    // to be fixed. Consult with team.
    public AnnotationPosition CountPosition { get; init; } = AnnotationPosition.Auto;
    public Func<T, ODataWriterState<TCustomState>, long?>? GetCount { get; init; }
    public Action<T, ICountWriter<TCustomState>, ODataWriterState<TCustomState>>? WriteCount { get; init; }

    public AnnotationPosition NextLinkPosition { get; init; } = AnnotationPosition.Auto;
    public Func<T, ODataWriterState<TCustomState>, string>? GetNextLink { get; init; }

    public Action<T, INextLinkWriter<TCustomState>, ODataWriterState<TCustomState>>? WriteNextLink { get; init; }

    // Etag is always written before the value, so it doesn't have a position override.

    public Func<T, ODataWriterState<TCustomState>, string?>? GetEtag { get; init; }

    public Action<T, IEtagWriter<TCustomState>, ODataWriterState<TCustomState>>? WriteEtag { get; init; }

    // The Id annotation is always written before the value, so it doesn't have a position override.
    public Func<T, ODataWriterState<TCustomState>, string?>? GetODataId { get; init; }

    public Action<T, IODataIdWriter<TCustomState>, ODataWriterState<TCustomState>>? WriteODataId { get; init; }

    public Func<T, IResourcePopertyWriter<T, TCustomState>, ODataWriterState<TCustomState>, bool>? WriteProperties { get; init; }

    public Func<T, ODataWriterState<TCustomState>, object?>? GetCustomPreValueAnnotations { get; init; }

    public Func<T, IAnnotationWriter<TCustomState>, ODataWriterState<TCustomState>, ValueTask>? WriteCustomPreValueAnnotations { get; init; }

    public Func<T, ODataWriterState<TCustomState>, object?>? GetCustomPostValueAnnotations { get; init; }

    public Func<T, IAnnotationWriter<TCustomState>, ODataWriterState<TCustomState>, ValueTask>? WriteCustomPostValueAnnotations { get; init; }

    public Func<T, ODataWriterState<TCustomState>, object?>? GetDynamicProperties { get; init; }

    // TODO: add WriteAnnotations variants
    public Func<T, string, ODataWriterState<TCustomState>, object?>? GetPropertyPreValueAnnotations { get; init; }
    public Func<T, string, ODataWriterState<TCustomState>, object?>? GetPropertyPostValueAnnotations { get; init; }

    /// <summary>
    /// Custom hook that's called before the value is written. If the value
    /// represents an resource/object, this is called before the opening brace is written.
    /// TODO: Determine whether this is called before annotations even for nested annotations of primitive values.
    /// Ideally, I think it should.
    /// </summary>
    public Action<T, ODataWriterState<TCustomState>>? OnSerializing { get; init; }

    /// <summary>
    /// Custom hook that's called after the value is written. If the value represents a resource/object,
    /// this is called after the closing brace is written.
    /// </summary>

    public Action<T, ODataWriterState<TCustomState>>? OnSerialized { get; init; }

    public ODataPropertyInfo<T, TCustomState> GetPropertyInfo(string name)
    {
        // TODO: we should throw instead of returning
        if (_propertiesCache.TryGetValue(name, out var prop))
        {
            return prop;
        }
        
        throw new Exception($"Property '{name}' not found in type '{Type.Name}'");
    }

    public IEdmType? GetEffectiveEdmType(T value, ODataWriterState<TCustomState> state)
    {
        IEdmType? edmType = EdmType ?? GetEdmType?.Invoke(value, state);
        return edmType;
    }
}
