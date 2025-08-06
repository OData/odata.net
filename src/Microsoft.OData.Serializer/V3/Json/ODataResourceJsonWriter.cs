using Microsoft.OData.Serializer.Core;
using Microsoft.OData.Serializer.V3.Adapters;
using Microsoft.OData.Serializer.V3.Core;
using Microsoft.OData.Serializer.V3.Json.Writers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.OData.Serializer.V3.Json;

internal class ODataResourceJsonWriter<T, TCustomState>(ODataTypeInfo<T, TCustomState> typeInfo)
    : ODataWriter<T, ODataJsonWriterState<TCustomState>>,
    IPropertyWriter<T, TCustomState>,
    IEtagWriter<TCustomState>
{
    public override async ValueTask Write(T value, ODataJsonWriterState<TCustomState> state)
    {
        Adapters.ODataPropertyInfo? parentProperty = state.Stack.Current.PropertyInfo;

        state.Stack.Push();
        state.Stack.Current.ResourceTypeInfo = typeInfo;

        typeInfo.OnSerializing?.Invoke(value, state);

        var jsonWriter = state.JsonWriter;

        jsonWriter.WriteStartObject();

        // No need to write the property name since there's a value.
        // But how do we handle annotation-only resources?
        await WritePreValueAnnotations(value, state);

        if (typeInfo.WriteProperties != null)
        {
            // User-driven iteration and selection.
            await typeInfo.WriteProperties(value, this, state);
        }
        else
        {
            // Library-drive iteration and selection.
            await WriteProperties(value, typeInfo, state);
        }

        await WritePostValueAnnotations(value, state);

        jsonWriter.WriteEndObject();

        typeInfo.OnSerialized?.Invoke(value, state);

        state.Stack.Pop();
    }

    private static async ValueTask WriteProperties(
        T value,
        ODataTypeInfo<T, TCustomState> typeInfo,
        ODataJsonWriterState<TCustomState> state)
    {
        Debug.Assert(typeInfo.Properties != null, "TypeInfo should have properties defined.");
        for (int i = 0; i < typeInfo.Properties.Count; i++)
        {
            var propertyInfo = typeInfo.Properties[i];
            state.Stack.Current.PropertyInfo = propertyInfo;

            // This is crude property skipping logic. Might not be efficient in all cases.
            // For example if the propertys are key/vals in a dictionary and we only
            // write those properties in the dictionary, then we'll do a lookup here
            // and another lookup when writing the value.
            // Perf would be even worse if the properties are stored in an IEnumerable<(Property, Value)>
            // without higher than O(1) lookup.
            if (propertyInfo.ShouldSkip?.Invoke(value, state) == true)
            {
                continue; // skip this property
            }

            await WriteProperty(value, propertyInfo, state);

            // when we implement re-entrancy, we should
            // return if property was not written completely.
            //if (!propertyWritten)
            //{
            //    // assume the property has stored enough state to resume writing later
            //    return false;
            //}

            if (state.ShouldFlush())
            {
                // return false; // TODO: store state and return false if re-entrancy implemented
                // TODO: flush or return if re-entrancy implemented
            }

            // if async source needs more data, we should return false as well
        }

        object? dynamicProperties = typeInfo.GetDynamicProperties?.Invoke(value, state);
        if (dynamicProperties != null)
        {
            // Should allow a GetDynamicProperties to return a different container type for the same typeInfo?
            // If it can't change, then we could cache the handler on the typeInfo.
            var handler = state.GetDynamicPropertiesHandler(dynamicProperties.GetType());
            await handler.WriteDynamicProperties(dynamicProperties, DefaultDynamicPropertyWriter<TCustomState>.Instance, state);
        }
    }

    private static ValueTask WriteProperty(T resource, ODataPropertyInfo<T, TCustomState> propertyInfo, ODataJsonWriterState<TCustomState> state)
    {
        var jsonWriter = state.JsonWriter;
        // add this property to the state, including current index
        // TODO: check annotations.

        return propertyInfo.WriteProperty(resource, state);

        //await WritePropertyAnnotations(resource, propertyInfo, state);

        //// How do we handle property selection?
        //// examples:
        //// - resource exposes a SelectProperties() that returns IEnumerable<ODataPropertyInfo<TDeclaringType>>
        //// - resource or property exposes a ShouldWrite() or ShouldSkipProperty() that is evaluated per property?
        ////   - this might rely on the implementation accessing the property value twice (e.g. to check whether null or missing)
        ////   - should we assume that property access is O(1)? If not, might not be a good idea to force multiple lookups of the same value
        //jsonWriter.WritePropertyName(propertyInfo.Utf8Name.Span);

        //// Another option is for propertyInfo to expose a GetXX method, e.g.
        //// if propertyInfo.IsString(), string value = propertyInfo.GetString(value);
        //// then we take care of writing it.
        //// This would work well if property if O(1), but what if O(n)?
        //// e.g. what if value is an IEnumerable<(Property, Value)> (e.g. ODataResource.Properties)
        //// This will make this loop quadratic.
        //// What if the value handed us its properties via common representation
        //// resourceInfo.GetProperties(value) that returns IEnumerable<CustomPropertyRepresentation>
        //// problem is if we make the property repersentation generic, we might have to make the entire
        //// stack generic on that property representation.
        //// be we could take the property representation and call an adapter to retrieve a value.
        //// But what if it's a POCO class, what's the native representation? A string?
        //// so it would check the string property name in an if statement chain like in v2??
        //// If we expose WriteValue, are we sure the user will preserve the state correctly
        //// on resumable writes?
        ////bool propertyWritten = propertyInfo.WriteValue(value, state);
        //await propertyInfo.WriteValue(resource, propertyInfo, state);

        // when we implement re-entrancy, we should
        // return if property was not written completely.
        //if (!propertyWritten)
        //{
        //    // assume the property has stored enough state to resume writing later
        //    return false;
        //}
    }

    private async ValueTask WritePreValueAnnotations(T value, ODataJsonWriterState<TCustomState> state)
    {
        if (typeInfo.EtagPosition != AnnotationPosition.PostValue)
        {
            WriteEtag(value, state);
        }

        if (typeInfo.GetCustomPreValueAnnotations != null)
        {
            var annotations = typeInfo.GetCustomPreValueAnnotations(value, state);
            if (annotations != null)
            {
                // TODO: Should we allow GetCustomPreValueAnnotations to return
                // a different type on different calls? If not, we could cache the handler on the type info or resource writer.
                var handler = state.GetCustomAnnotationsHandler(annotations.GetType());
                await handler.WriteAnnotations(annotations, CustomInstanceAnnotationWriter<TCustomState>.Instance, state);
            }
        }
        else if (typeInfo.WriteCustomPreValueAnnotations != null)
        {
            await typeInfo.WriteCustomPreValueAnnotations(value, CustomInstanceAnnotationWriter<TCustomState>.Instance, state);
        }
    }

    private ValueTask WritePostValueAnnotations(T value, ODataJsonWriterState<TCustomState> state)
    {
        if (typeInfo.EtagPosition == AnnotationPosition.PostValue)
        {
            WriteEtag(value, state);
        }

        return ValueTask.CompletedTask;
    }

    private void WriteEtag(T value, ODataJsonWriterState<TCustomState> state)
    {
        var jsonWriter = state.JsonWriter;
        if (typeInfo.GetEtag != null)
        {
            var etag = typeInfo.GetEtag(value, state);
            if (etag != null)
            {
                jsonWriter.WritePropertyName("@odata.etag"u8);
                jsonWriter.WriteStringValue(etag);
            }
        }

        if (typeInfo.WriteEtag != null)
        {
            typeInfo.WriteEtag(value, this, state);
        }
    }

    ValueTask IPropertyWriter<T, TCustomState>.WriteProperty<TValue>(T resource, string name, TValue value, ODataJsonWriterState<TCustomState> state)
    {
        var property = typeInfo.GetPropertyInfo(name);
        return (this as IPropertyWriter<T, TCustomState>).WriteProperty(resource, property, value, state);
    }

    ValueTask IPropertyWriter<T, TCustomState>.WriteProperty<TValue>(T resource, ODataPropertyInfo<T, TCustomState> propertyInfo, TValue value, ODataJsonWriterState<TCustomState> state)
    {
        return propertyInfo.WriteProperty(resource, value, state);
    }

    ValueTask IPropertyWriter<T, TCustomState>.WriteProperty(T resource, ODataPropertyInfo<T, TCustomState> propertyInfo, ODataJsonWriterState<TCustomState> state)
    {
        return WriteProperty(resource, propertyInfo, state);
    }

    // TODO: The etag writer should probably be a singleton since it doesn't access any state.

    void IEtagWriter<TCustomState>.WriteEtag(ReadOnlySpan<char> etag, ODataJsonWriterState<TCustomState> state)
    {
        var jsonWriter = state.JsonWriter;
        jsonWriter.WritePropertyName("@odata.etag"u8);
        jsonWriter.WriteStringValue(etag);
    }

    void IEtagWriter<TCustomState>.WriteEtag(ReadOnlySpan<byte> etag, ODataJsonWriterState<TCustomState> state)
    {
        var jsonWriter = state.JsonWriter;
        jsonWriter.WritePropertyName("@odata.etag"u8);
        jsonWriter.WriteStringValue(etag);
    }
}