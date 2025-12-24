using System.Diagnostics;
using System.Text.Json;

namespace Microsoft.OData.Serializer;

internal partial class ODataResourceJsonWriter<T, TCustomState> : IValueReader<TCustomState>
{
    public override bool Read(ODataReaderState<TCustomState> state, out T value)
    {
        var reader = state.GetJsonReader();
        reader.Read();
        if (reader.TokenType == JsonTokenType.Null)
        {
            value = default!;
            return true;
        }
        if (reader.TokenType != JsonTokenType.StartObject)
        {
            throw new Exception($"Expected start of object token but found {reader.TokenType}.");
        }

        if (typeInfo.CreateInstance == null)
        {
            throw new Exception($"The type {typeof(T).FullName} cannot be deserialized since no instance factory has been defined. Consider setting the {nameof(ODataTypeInfo<T>.CreateInstance)} property on the type info..");
        }
        
        T instance = typeInfo.CreateInstance(state);
        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
            {
                break;
            }

            if (reader.TokenType != JsonTokenType.PropertyName)
            {
                throw new ODataException($"Expected property name token but found {reader.TokenType}.");
            }

            string? propertyName = reader.GetString();
            if (propertyName == null)
            {
                throw new ODataException("Property name cannot be null."); // TODO should include location in the text.
            }

            if (propertyName == "@odata.context")
            {
                // TODO: Skip odata.context annotation
                reader.Read();
                Debug.Assert(reader.TokenType == JsonTokenType.String);
                reader.GetString();
                continue;
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

            state.SaveJsonReaderState(ref reader);
            // TODO should handle case where read fails due to buffer end, and need to fetch more data from stream into buffer.
            propertyInfo.ReadValue(instance, valueReader, state);
            reader = state.GetJsonReader();
        }

        state.SaveJsonReaderState(ref reader);
        value = instance;
        return true;
    }

    bool IValueReader<TCustomState>.GetBoolean(ODataReaderState<TCustomState> state)
    {
        var reader = state.GetJsonReader();
        Debug.Assert(reader.Read());
        var value = reader.GetBoolean();
        state.SaveJsonReaderState(ref reader);
        return value;
    }

    int IValueReader<TCustomState>.GetInt32(ODataReaderState<TCustomState> state)
    {
        var reader = state.GetJsonReader();
        Debug.Assert(reader.Read());
        var value = reader.GetInt32();
        state.SaveJsonReaderState(ref reader);
        return value;
    }

    string? IValueReader<TCustomState>.GetString(ODataReaderState<TCustomState> state)
    {
        var reader = state.GetJsonReader();
        Debug.Assert(reader.Read());
        var value = reader.GetString();
        state.SaveJsonReaderState(ref reader);
        return value;
    }
}
