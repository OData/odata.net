using System.Diagnostics;
using System.Text.Json;

namespace Microsoft.OData.Serializer;

internal partial class ODataResourceJsonWriter<T, TCustomState> : IValueReader<TCustomState>
{
    public override bool Read(ODataReaderState<TCustomState> state, out T value)
    {
        var scope = state.GetJsonReaderScope();
        if (scope.Reader.TokenType == JsonTokenType.Null)
        {
            value = default!;
            return true;
        }
        if (scope.Reader.TokenType != JsonTokenType.StartObject)
        {
            throw new Exception($"Expected start of object token but found {scope.Reader.TokenType}.");
        }

        if (typeInfo.CreateInstance == null)
        {
            throw new Exception($"The type {typeof(T).FullName} cannot be deserialized since no instance factory has been defined. Consider setting the {nameof(ODataTypeInfo<T>.CreateInstance)} property on the type info..");
        }
        
        T instance = typeInfo.CreateInstance(state);
        while (scope.Reader.Read())
        {
            if (scope.Reader.TokenType == JsonTokenType.EndObject)
            {
                break;
            }

            if (scope.Reader.TokenType != JsonTokenType.PropertyName)
            {
                throw new ODataException($"Expected property name token but found {scope.Reader.TokenType}.");
            }

            string? propertyName = scope.Reader.GetString();
            if (propertyName == null)
            {
                throw new ODataException("Property name cannot be null."); // TODO should include location in the text.
            }

            if (!scope.Reader.Read()) // TODO: we may have reached the end of the buffer. We save the state and request more data.
            {
                throw new ODataException("Unexpected end of JSON while reading property value.");
            }

            var propertyInfo = typeInfo.GetPropertyInfo(propertyName);
            if (propertyInfo == null)
            {
                // No matching property found.
                // TODO: should check if this property is defined in the model
                // TODO: check if this typeInfo allows unknown/open properties

                throw new ODataException($"Could not find property '{propertyName}' on type '{typeof(T).FullName}'.");
            }
            
            if (propertyInfo.ReadValue == null)
            {
                throw new ODataException($"Could not deserialize property {propertyInfo.Name} of type {Type?.FullName}. Consider definining the ReadValue property.");
            }

            IValueReader<TCustomState> valueReader = this;
            scope.Dispose();

            // TODO should handle case where read fails due to buffer end, and need to fetch more data from stream into buffer.
            propertyInfo.ReadValue(instance, valueReader, state);
            scope = state.GetJsonReaderScope();
        }

        scope.Dispose();
        value = instance;
        return true;
    }

    bool IValueReader<TCustomState>.GetBoolean(ODataReaderState<TCustomState> state)
    {
        using var scope = state.GetJsonReaderScope();
        return scope.Reader.GetBoolean();
    }

    int IValueReader<TCustomState>.GetInt32(ODataReaderState<TCustomState> state)
    {
        using var scope = state.GetJsonReaderScope();
        return scope.Reader.GetInt32();
    }

    string? IValueReader<TCustomState>.GetString(ODataReaderState<TCustomState> state)
    {
        using var scope = state.GetJsonReaderScope();
        return scope.Reader.GetString();
    }
}
