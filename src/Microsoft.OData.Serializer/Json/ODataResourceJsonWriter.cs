using System.Diagnostics;

namespace Microsoft.OData.Serializer;

internal class ODataResourceJsonWriter<T, TCustomState>(ODataTypeInfo<T, TCustomState> typeInfo)
    : ODataWriter<T, ODataWriterState<TCustomState>>,
    IResourcePopertyWriter<T, TCustomState>,
    IEtagWriter<TCustomState>,
    IODataIdWriter<TCustomState>
{
    public override bool Write(T value, ODataWriterState<TCustomState> state)
    {
        ODataPropertyInfo? parentProperty = state.IsTopLevel() ? null : state.Stack.Current.PropertyInfo;

        // if we're resuming, then we should not push the stack again.
        // but how do we know if we're resuming?
        // if continuation is true, but what if that's the previous state?
        state.Stack.Push();
        var jsonWriter = state.JsonWriter;

        var resourceProgress = state.Stack.Current.ResourceProgress;

        if (resourceProgress < ResourceWriteProgress.StartToken)
        {
            state.Stack.Current.ResourceTypeInfo = typeInfo;

            typeInfo.OnSerializing?.Invoke(value, state);



            // resumability.
            // v1: apply only to properties, i.e. writing annotations should write without pausing.
            // for each property, keep track of index. When resuming, skip to the property.
            // We need to know which writer we're currently dealing with, so that we can call resume on it.
            // We need to remember which value we were writing. How do we do that without boxing?
            // We don't need to box, it's the caller's responsibility to ensure they pass the same value.
            // So we should store enough state that we can resume from the same point.
            // So in this class, let's assume we have the same writer we started from.

            jsonWriter.WriteStartObject();

            state.Stack.Current.ResourceProgress = ResourceWriteProgress.StartToken;
        }

        if (resourceProgress < ResourceWriteProgress.PreValueMetadata)
        {
            

            // No need to write the property name since there's a value.
            // But how do we handle annotation-only resources?
            // For simplicity, we don't support suspending between annotations.
            // We expect that the annotations are will be few and short and should be within the buffer size
            // most of the time.
            WritePreValueMetadata(value, state);

            state.Stack.Current.ResourceProgress = ResourceWriteProgress.PreValueMetadata;


            if (state.ShouldFlush())
            {
                state.Stack.Current.IsContinuation = true;
                state.Stack.Pop(false);
                return false;
            }
        }

        if (resourceProgress < ResourceWriteProgress.Value)
        {
            if (typeInfo.PropertySelector != null)
            {
                bool success = typeInfo.PropertySelector.WriteProperties(value, state);
                if (!success)
                {
                    state.Stack.Current.IsContinuation = true;
                    state.Stack.Pop(false);
                    return false;
                }

            }
            else if (typeInfo.WriteProperties != null)
            {
                
                bool success = typeInfo.WriteProperties(value, this, state);
                if (!success)
                {
                    // Need to see explore how user code could handle resumability, especially
                    // considering that they do not have access to everything in the state.
                    // But they do have access to custom state, so they could store anything they want there.
                    // But they don't control when the stack is pushed or popped.
                    // TODO: how to communicate to downstream writers that they should not be resumable then?
                    throw new Exception("Resumable writes are currently not supported for custom WriteProperties hook.");
                    // assume the properties writer has stored enough state to resume writing later
                    //state.Stack.Current.IsContinuation = true;
                    //state.Stack.Pop(false);
                    //return false;
                }
            }
            else
            {
                // Library-driven iteration and selection.
                bool success = WriteProperties(value, typeInfo, state);
                if (!success)
                {
                    state.Stack.Current.IsContinuation = true;
                    state.Stack.Pop(false);
                    return false;
                }
            }

            state.Stack.Current.ResourceProgress = ResourceWriteProgress.Value;

            if (state.ShouldFlush())
            {
                state.Stack.Current.IsContinuation = true;
                state.Stack.Pop(false);
                return false;
            }
        }

        if (resourceProgress < ResourceWriteProgress.PostValueMetadata)
        {
            WritePostValueMetadata(value, state);

            state.Stack.Current.ResourceProgress = ResourceWriteProgress.PostValueMetadata;

            // TODO: does it make sense to flush here since we're almost at the end
            // of the value and the parent writer would have likely flushed
            // after this either way?
            if (state.ShouldFlush())
            {
                state.Stack.Current.IsContinuation = true;
                state.Stack.Pop(false);
                return false;
            }
        }

        jsonWriter.WriteEndObject();

        state.Stack.Current.ResourceProgress = ResourceWriteProgress.EndToken;

        typeInfo.OnSerialized?.Invoke(value, state);

        state.Stack.Pop(true);

        return true;
    }

    private static bool WriteProperties(
        T value,
        ODataTypeInfo<T, TCustomState> typeInfo,
        ODataWriterState<TCustomState> state)
    {
        Debug.Assert(typeInfo.Properties != null || typeInfo.GetOpenProperties != null, "TypeInfo should have properties defined.");

        if (typeInfo.Properties != null)
        {
            // resume from last property.
            var propIndex = state.Stack.Current.EnumeratorIndex;
            for (int i = propIndex; i < typeInfo.Properties.Count; i++)
            {
                state.Stack.Current.EnumeratorIndex = i;
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

                bool success = WriteProperty(value, propertyInfo, state);
                if (!success)
                {
                    return false;
                }

                // when we implement re-entrancy, we should
                // return if property was not written completely.
                //if (!propertyWritten)
                //{
                //    // assume the property has stored enough state to resume writing later
                //    return false;
                //}

                state.Stack.EndProperty();

                if (state.ShouldFlush())
                {
                    state.Stack.Current.EnumeratorIndex = i + 1;
                    return false;
                }
            }
        }

        object? dynamicProperties = typeInfo.GetOpenProperties?.Invoke(value, state);
        if (dynamicProperties != null)
        {
            // TODO: For now we don't support resumable writes for dynamic properties, but we should.

            // Should allow a GetDynamicProperties to return a different container type for the same typeInfo?
            // If it can't change, then we could cache the handler on the typeInfo.
            var handler = state.GetDynamicPropertiesHandler(dynamicProperties.GetType());
            handler.WriteDynamicProperties(
                value,
                dynamicProperties,
                typeInfo.GetPropertyPreValueAnnotations,
                typeInfo.GetPropertyPostValueAnnotations,
                // TODO: Get dynamic property writer from options?
                DefaultDynamicPropertyWriter<TCustomState>.Instance,
                state);
        }

        return true;
    }

    private static bool WriteProperty(T resource, ODataPropertyInfo<T, TCustomState> propertyInfo, ODataWriterState<TCustomState> state)
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

    private void WritePreValueMetadata(T value, ODataWriterState<TCustomState> state)
    {
        if (state.IsTopLevel())
        {
            WriteContextUrl(state);
        }

        WriteId(value, state);
        WriteEtag(value, state);

        if (typeInfo.GetCustomPreValueAnnotations != null)
        {
            var annotations = typeInfo.GetCustomPreValueAnnotations(value, state);
            if (annotations != null)
            {
                // TODO: Should we allow GetCustomPreValueAnnotations to return
                // a different type on different calls? If not, we could cache the handler on the type info or resource writer.
                var handler = state.GetCustomAnnotationsHandler(annotations.GetType());
                handler.WriteAnnotations(annotations, CustomInstanceAnnotationWriter<TCustomState>.Instance, state);
            }
        }
        else if (typeInfo.WriteCustomPreValueAnnotations != null)
        {
            typeInfo.WriteCustomPreValueAnnotations(value, CustomInstanceAnnotationWriter<TCustomState>.Instance, state);
        }
    }

    private void WritePostValueMetadata(T value, ODataWriterState<TCustomState> state)
    {
        if (typeInfo.GetCustomPostValueAnnotations != null)
        {
            var annotations = typeInfo.GetCustomPostValueAnnotations(value, state);
            if (annotations != null)
            {
                // TODO: Should we allow GetCustomPostValueAnnotations to return
                // a different type on different calls? If not, we could cache the handler on the type info or resource writer.
                var handler = state.GetCustomAnnotationsHandler(annotations.GetType());
                handler.WriteAnnotations(annotations, CustomInstanceAnnotationWriter<TCustomState>.Instance, state);
            }
        }
        else if (typeInfo.WriteCustomPostValueAnnotations != null)
        {
            typeInfo.WriteCustomPostValueAnnotations(value, CustomInstanceAnnotationWriter<TCustomState>.Instance, state);
        }
    }

    private static void WriteContextUrl(ODataWriterState<TCustomState> state)
    {
        var jsonWriter = state.JsonWriter;
        ContextUrlWriter.WriteContextUrlProperty(state.PayloadKind, state.ODataUri, state.JsonWriter);
    }

    private void WriteId(T value, ODataWriterState<TCustomState> state)
    {
        var jsonWriter = state.JsonWriter;
        if (typeInfo.GetODataId != null)
        {
            var id = typeInfo.GetODataId(value, state);
            if (id != null)
            {
                jsonWriter.WritePropertyName("@odata.id"u8);
                jsonWriter.WriteStringValue(id);
            }
        }

        if (typeInfo.WriteODataId != null)
        {
            typeInfo.WriteODataId(value, this, state);
        }
    }

    private void WriteEtag(T value, ODataWriterState<TCustomState> state)
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

    bool IResourcePopertyWriter<T, TCustomState>.WriteProperty<TValue>(T resource, string name, TValue value, ODataWriterState<TCustomState> state)
    {
        var property = typeInfo.GetPropertyInfo(name);
        return (this as IResourcePopertyWriter<T, TCustomState>).WriteProperty(resource, property, value, state);
    }

    bool IResourcePopertyWriter<T, TCustomState>.WriteProperty<TValue>(T resource, ODataPropertyInfo<T, TCustomState> propertyInfo, TValue value, ODataWriterState<TCustomState> state)
    {
        return propertyInfo.WriteProperty(resource, value, state);
    }

    bool IResourcePopertyWriter<T, TCustomState>.WriteProperty(T resource, ODataPropertyInfo<T, TCustomState> propertyInfo, ODataWriterState<TCustomState> state)
    {
        return WriteProperty(resource, propertyInfo, state);
    }

    // TODO: The etag writer should probably be a singleton since it doesn't access any state.

    void IEtagWriter<TCustomState>.WriteEtag(ReadOnlySpan<char> etag, ODataWriterState<TCustomState> state)
    {
        var jsonWriter = state.JsonWriter;
        jsonWriter.WritePropertyName("@odata.etag"u8);
        jsonWriter.WriteStringValue(etag);
    }

    void IEtagWriter<TCustomState>.WriteEtag(ReadOnlySpan<byte> etag, ODataWriterState<TCustomState> state)
    {
        var jsonWriter = state.JsonWriter;
        jsonWriter.WritePropertyName("@odata.etag"u8);
        jsonWriter.WriteStringValue(etag);
    }

    void IODataIdWriter<TCustomState>.WriteId(ReadOnlySpan<char> id, ODataWriterState<TCustomState> state)
    {
        var jsonWriter = state.JsonWriter;
        jsonWriter.WritePropertyName("@odata.id"u8);
        jsonWriter.WriteStringValue(id);
    }

    void IODataIdWriter<TCustomState>.WriteId(ReadOnlySpan<byte> id, ODataWriterState<TCustomState> state)
    {
        var jsonWriter = state.JsonWriter;
        jsonWriter.WritePropertyName("@odata.id"u8);
        jsonWriter.WriteStringValue(id);
    }

    void IODataIdWriter<TCustomState>.WriteId(Uri id, ODataWriterState<TCustomState> state)
    {
        var jsonWriter = state.JsonWriter;
        jsonWriter.WritePropertyName("@odata.id"u8);
        jsonWriter.WriteStringValue(id.OriginalString);
    }
}
