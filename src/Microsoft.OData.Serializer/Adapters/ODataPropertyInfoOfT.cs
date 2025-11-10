using System.Diagnostics;

namespace Microsoft.OData.Serializer;

#pragma warning disable CA1005 // Avoid excessive parameters on generic types
public class ODataPropertyInfo<TDeclaringType, TCustomState> :
    ODataPropertyInfo,
    IValueWriter<TCustomState>,
    ICountWriter<TCustomState>,
    INextLinkWriter<TCustomState>,
    IAnnotationWriter<TCustomState>,
    IStreamValueWriter<TCustomState>
#pragma warning restore CA1005 // Avoid excessive parameters on generic types
{
    // TODO: this property name conflicts with the method WriteValue from IValueWriter. Perhaps IValueWriter.WriteValue should be renamed?
    // TODO: should we support resumability in custom writes?
    public Func<TDeclaringType, IValueWriter<TCustomState>, ODataWriterState<TCustomState>, bool> WriteValue { get; init; }

    public Func<TDeclaringType, IStreamValueWriter<TCustomState>, ODataWriterState<TCustomState>, ValueTask> WriteValueAsync { get; init; }

    // TODO: this tight coupling between attribute handling and the core serializer internals is not ideal and should be revised.
    /// <summary>
    /// Custom writer provided by the user via [ODataPropertyValueWriter] attribute.
    /// Should extend one of:
    /// ODataPropertyValueWriter<..> variants or ODataAsyncPropertyValueWriter<..> variants.
    /// </summary>
    internal object? CustomPropertyValueWriter { get; set; }

    // The value of this delegate is generated automatically by the library based on the
    // [ODataPropertyValueWriter] attribute. The second object parameter represents the actual writer
    // type provided by the user. It should extend one of:
    // ODataPropertyValueWriter<..> variants or ODataAsyncPropertyValueWriter<..> variants.
    // TODO: this tight coupling between attribute handling and the core serializer internals is not ideal and should be revised.
    internal Func<TDeclaringType, object, IStreamValueWriter<TCustomState>, ODataWriterState<TCustomState>, ValueTask>?
        WriteValueWithCustomWriterAsync { get; set; }

    // TODO: implement a shorthand GetValue hook of type Func<TDeclaringType, ODataJsonWriterState<TCustomState>, TValue>
    // which would require an extra generic type parameter TValue that's a bit hard to defined since we'd
    // also want a default cause that only takes 2 parameters <TDeclaringType, TValue>
    // which would conflict with the current <TDeclaringType, TCustomState> definition.

    // TODO: Should nested count handlers be defined on the property info or the type info?
    // We already have count handlers on the type info, keeping them there would provide
    // uniformity. It might be cause of confusion to have to set it multiple places.
    // The challenge with having it on the type info is that when comes to writing
    // nested annotations, e.g. PropertyName@odata.count, we need to write this before
    // the value. We need to write this annotation before the value, so at this time
    // we have not resolved type info and we don't know what annotation handlers
    // it exposes. But we have access to the property info. Having it on the
    // property info also makes it convenient to add annotations to primitive properties
    // without having to define type infos for primitive values.
    // The other approach is to have them in the type infos, which means when writing
    // the property, we don't know what annotations are available until we resolve
    // the property value's writer. So the value writer would need to handle
    // writing property names, meaning each IODataWriter will need to check
    // the state to see if there's a property in scope, or if it's top-level, etc.
    // there might a cost to performing these if-checks on the hot path, but not sure it's significant.
    // It simplifies the configuration for the end user, the annotations are always on the type info.

    // TODO: I considered whether the position should be a func or property. I should evaluate cost of calling the func vs property/field.
    // Do we expect the position to change per value? I think for a given type or property, the position of the annotation is likely
    // to be fixed. Consult with team.
    public AnnotationPosition CountPosition { get; init; } = AnnotationPosition.Auto;

    public Func<TDeclaringType, ODataWriterState<TCustomState>, long?>? GetCount { get; init; }


    public Action<TDeclaringType, ICountWriter<TCustomState>, ODataWriterState<TCustomState>>? WriteCount { get; init; }

    public AnnotationPosition NextLinkPosition { get; init; } = AnnotationPosition.Auto;

    public Func<TDeclaringType, ODataWriterState<TCustomState>, string?>? GetNextLink { get; init; }

    public Action<TDeclaringType, INextLinkWriter<TCustomState>, ODataWriterState<TCustomState>>? WriteNextLink { get; init; }

    public Func<TDeclaringType, ODataWriterState<TCustomState>, object?>? GetCustomPreValueAnnotations { get; init; }

    public Action<TDeclaringType, IAnnotationWriter<TCustomState>, ODataWriterState<TCustomState>>? WriteCustomPreValueAnnotations { get; init; }

    public Func<TDeclaringType, ODataWriterState<TCustomState>, object?>? GetCustomPostValueAnnotations { get; init; }

    public Action<TDeclaringType, IAnnotationWriter<TCustomState>, ODataWriterState<TCustomState>>? WriteCustomPostValueAnnotations { get; init; }

    public Func<TDeclaringType, ODataWriterState<TCustomState>, bool>? ShouldSkip { get; init; }

    internal bool WriteProperty(TDeclaringType resource, ODataWriterState<TCustomState> state)
    {
        if (WriteValueAsync != null || WriteValueWithCustomWriterAsync != null)
        {
            // TODO: should probably just have WritePropertyValueAsync below.
            return this.WritePropertyAsync(resource, state);
        }

        if (state.Stack.Current.PropertyProgress < PropertyProgress.PreValueMetadata)
        {
            this.WritePreValueAnnotations(resource, state);
            state.Stack.Current.PropertyProgress = PropertyProgress.PreValueMetadata;
            // TODO: check whether to flush.
        }
        
        if (state.Stack.Current.PropertyProgress < PropertyProgress.Value)
        {
            bool success = this.WritePropertyValue(resource, state);
            if (!success)
            {
                return false;
            }

            state.Stack.Current.PropertyProgress = PropertyProgress.Value;
        }

        if (state.Stack.Current.PropertyProgress < PropertyProgress.PostValueMetadata)
        {
            this.WritePostValueAnnotations(resource, state);
            state.Stack.Current.PropertyProgress = PropertyProgress.PostValueMetadata;
        }

        return true;
    }

    internal bool WriteProperty<TValue>(TDeclaringType resource, TValue value, ODataWriterState<TCustomState> state)
    {
        if (state.Stack.Current.PropertyProgress < PropertyProgress.PreValueMetadata)
        {
            this.WritePreValueAnnotations(resource, state);
            state.Stack.Current.PropertyProgress = PropertyProgress.PreValueMetadata;
            // TODO: check whether to flush.
        }

        if (state.Stack.Current.PropertyProgress < PropertyProgress.Value)
        {
            bool success = this.WritePropertyValue(resource, value, state);
            if (!success)
            {
                return false;
            }

            state.Stack.Current.PropertyProgress = PropertyProgress.Value;
        }

        if (state.Stack.Current.PropertyProgress < PropertyProgress.PostValueMetadata)
        {
            this.WritePostValueAnnotations(resource, state);
            state.Stack.Current.PropertyProgress = PropertyProgress.PostValueMetadata;
        }

        return true;
    }

    internal bool WritePropertyAsync(TDeclaringType resource, ODataWriterState<TCustomState> state)
    {
        Debug.Assert(this.WriteValueAsync != null || this.WriteValueWithCustomWriterAsync != null,
            "WriteValueAsync should not be null.");

        if (state.Stack.Current.PropertyProgress < PropertyProgress.PreValueMetadata)
        {
            this.WritePreValueAnnotations(resource, state);
            state.Stack.Current.PropertyProgress = PropertyProgress.PreValueMetadata;
            // TODO: check whether to flush.
        }

        if (state.Stack.Current.PropertyProgress < PropertyProgress.Value)
        {
            ValueTask task = this.WritePropertyValueAsync(resource, state);

            // Store task in the state. The root of the serializer will await this task before
            // resuming the write.
            // It will also advance the PropertyProgress to Value once the task is complete.
            // This will ensure that we don't attempt to write the property value again when resuming.
            state.Stack.Current.PendingTask = task;
            
            
            return false;
        }

        if (state.Stack.Current.PropertyProgress < PropertyProgress.PostValueMetadata)
        {
            this.WritePostValueAnnotations(resource, state);
            state.Stack.Current.PropertyProgress = PropertyProgress.PostValueMetadata;
        }

        return true;
    }

    internal ValueTask WritePropertyValueAsync(TDeclaringType resource, ODataWriterState<TCustomState> state)
    {
        Debug.Assert(this.WriteValueAsync != null || this.WriteValueWithCustomWriterAsync != null, 
            "WriteValueAsync should not be null.");

        // TODO: We pass this ODataPropertyInfo as the IStreamValueWriter parameter
        // so that the property name is written only if the caller invokes Write* methods.
        // Since ODataPropertyInfo knows the property to write.
        // Alternatively, we could store the property name in the state and have the StreamValueWriter
        // retrieve it from the state and write it before the writing the value.
        // I'll benchmark the two and settle on the one that's more performant.
        if (this.WriteValueAsync != null)
        {
            return WriteValueAsync(resource, this, state);
        }

        Debug.Assert(this.CustomPropertyValueWriter != null, "CustomPropertyValueWriter should not be null if WriteValueWithCustomWriterAsync is set.");
        Debug.Assert(WriteValueWithCustomWriterAsync != null);
        return WriteValueWithCustomWriterAsync(resource, this.CustomPropertyValueWriter, this, state);
    }

    

    private void WritePreValueAnnotations(TDeclaringType resource, ODataWriterState<TCustomState> state)
    {
        if (this.CountPosition != AnnotationPosition.PostValue)
        {
            this.WriteCountInternal(resource, state);
        }

        if (this.NextLinkPosition != AnnotationPosition.PostValue)
        {
            this.WriteNextLinkInternal(resource, state);
        }

        if (this.GetCustomPreValueAnnotations != null)
        {
            var customAnnotations = this.GetCustomPreValueAnnotations(resource, state);
            if (customAnnotations != null)
            {
                // TODO: Should we allow GetCustomPreValueAnnotations to return
                // a different type on different calls? If not, we could cache the handler on the property info
                var handler = state.GetCustomAnnotationsHandler(customAnnotations.GetType());
                handler.WriteAnnotations(customAnnotations, this, state);
            }
        }
        else if (this.WriteCustomPreValueAnnotations != null)
        {
            // annotations writer.
            WriteCustomPreValueAnnotations(resource, this, state);
        }
    }

    private void WritePostValueAnnotations(TDeclaringType resource, ODataWriterState<TCustomState> state)
    {
        if (this.CountPosition == AnnotationPosition.PostValue)
        {
            this.WriteCountInternal(resource, state);
        }

        if (this.NextLinkPosition == AnnotationPosition.PostValue)
        {
            this.WriteNextLinkInternal(resource, state);
        }

        if (this.GetCustomPostValueAnnotations != null)
        {
            var customAnnotations = this.GetCustomPostValueAnnotations(resource, state);
            if (customAnnotations != null)
            {
                // TODO: Should we allow GetCustomPreValueAnnotations to return
                // a different type on different calls? If not, we could cache the handler on the property info
                var handler = state.GetCustomAnnotationsHandler(customAnnotations.GetType());
                handler.WriteAnnotations(customAnnotations, this, state);
            }
        }
        else if (this.WriteCustomPostValueAnnotations != null)
        {
            this.WriteCustomPostValueAnnotations(resource, this, state);
        }
    }

    internal protected virtual bool WritePropertyValue(TDeclaringType resource, ODataWriterState<TCustomState> state)
    {
        Debug.Assert(this.WriteValue != null, "WriteValue should not be null.");
        return WriteValue(resource, this, state);
    }

    internal protected bool WritePropertyValue<TValue>(TDeclaringType resource, TValue value, ODataWriterState<TCustomState> state)
    {
        if (state.Stack.Current.PropertyProgress < PropertyProgress.Name)
        {
            state.JsonWriter.WritePropertyName(this.Utf8Name.Span);
            state.Stack.Current.PropertyProgress = PropertyProgress.Name;
        }

        return state.WriteValue(value);
    }

    private void WriteCountInternal(TDeclaringType resource, ODataWriterState<TCustomState> state)
    {
        if (this.GetCount != null)
        {
            var count = this.GetCount(resource, state);
            if (count.HasValue)
            {
                JsonMetadataHelpers.WritePropertyAnnotationName(state.JsonWriter, this.Utf8Name.Span, "odata.count"u8);
                state.JsonWriter.WriteNumberValue(count.Value);
            }
        }
        else
        {
            this.WriteCount?.Invoke(resource, this, state);
        }
    }

    private void WriteNextLinkInternal(TDeclaringType resource, ODataWriterState<TCustomState> state)
    {
        if (this.GetNextLink != null)
        {
            var nextLink = this.GetNextLink(resource, state);
            if (!string.IsNullOrEmpty(nextLink))
            {
                JsonMetadataHelpers.WritePropertyAnnotationName(state.JsonWriter, this.Utf8Name.Span, "odata.nextLink"u8);
                state.JsonWriter.WriteStringValue(nextLink);
            }
        }
        else
        {
            this.WriteNextLink?.Invoke(resource, this, state);
        }
    }

    void ICountWriter<TCustomState>.WriteCount(long count, ODataWriterState<TCustomState> state)
    {
        var jsonWriter = state.JsonWriter;
        JsonMetadataHelpers.WritePropertyAnnotationName(jsonWriter, this.Utf8Name.Span, "odata.count"u8);
        jsonWriter.WriteNumberValue(count);
    }

    void INextLinkWriter<TCustomState>.WriteNextLink(ReadOnlySpan<char> nextLink, ODataWriterState<TCustomState> state)
    {
        var jsonWriter = state.JsonWriter;
        JsonMetadataHelpers.WritePropertyAnnotationName(jsonWriter, this.Utf8Name.Span, "odata.nextLink"u8);
        jsonWriter.WriteStringValue(nextLink);
    }

    void INextLinkWriter<TCustomState>.WriteNextLink(ReadOnlySpan<byte> nextLink, ODataWriterState<TCustomState> state)
    {
        var jsonWriter = state.JsonWriter;
        JsonMetadataHelpers.WritePropertyAnnotationName(jsonWriter, this.Utf8Name.Span, "odata.nextLink"u8);
        jsonWriter.WriteStringValue(nextLink);
    }

    void INextLinkWriter<TCustomState>.WriteNextLink(Uri nextLink, ODataWriterState<TCustomState> state)
    {
        var jsonWriter = state.JsonWriter;
        JsonMetadataHelpers.WritePropertyAnnotationName(jsonWriter, this.Utf8Name.Span, "odata.nextLink"u8);
        jsonWriter.WriteStringValue(nextLink.AbsoluteUri);
    }

    bool IValueWriter<TCustomState>.WriteValue<T>(T value, ODataWriterState<TCustomState> state)
    {
        if (state.Stack.Current.PropertyProgress < PropertyProgress.Name)
        {
            var jsonWriter = state.JsonWriter;
            jsonWriter.WritePropertyName(this.Utf8Name.Span);
            state.Stack.Current.PropertyProgress = PropertyProgress.Name;
        }

        return state.WriteValue(value);
    }

    // The IAnnotationwriter.WriteAnnotation methods are only called when writing custom annotations
    void IAnnotationWriter<TCustomState>.WriteAnnotation<TValue>(ReadOnlySpan<char> name, TValue value, ODataWriterState<TCustomState> state)
    {
        var jsonWriter = state.JsonWriter;
        JsonMetadataHelpers.WriteCustomPropertyAnnotationName(jsonWriter, this.Name, name);
        var success = state.WriteValue(value);
        Debug.Assert(success, "Annotation value write did not complete. Resumable writes are not currently supported for annotations.");
    }

    void IAnnotationWriter<TCustomState>.WriteAnnotation<TValue>(ReadOnlySpan<byte> name, TValue value, ODataWriterState<TCustomState> state)
    {
        var jsonWriter = state.JsonWriter;
        JsonMetadataHelpers.WriteCustomPropertyAnnotationName(jsonWriter, this.Utf8Name.Span, name);
        var success = state.WriteValue(value);
        Debug.Assert(success, "Annotation value write did not complete. Resumable writes are not currently supported for annotations.");
    }

    void IStreamValueWriter<TCustomState>.WriteValue<T>(T value, ODataWriterState<TCustomState> state)
    {
        EnsurePropertyWrittenInsideAsyncWriteState(state);
        StreamValueWriter<TCustomState>.Instance.WriteValue(value, state);
    }

    ValueTask IStreamValueWriter<TCustomState>.WriteValueAsync<T>(T value, ODataWriterState<TCustomState> state)
    {
        EnsurePropertyWrittenInsideAsyncWriteState(state);
        return StreamValueWriter<TCustomState>.Instance.WriteValueAsync(value, state);
    }

    void IStreamValueWriter<TCustomState>.WriteStringSegment(ReadOnlySpan<char> value, bool isFinalBlock, ODataWriterState<TCustomState> state)
    {
        EnsurePropertyWrittenInsideAsyncWriteState(state);
        StreamValueWriter<TCustomState>.Instance.WriteStringSegment(value, isFinalBlock, state);
    }

    void IStreamValueWriter<TCustomState>.WriteBinarySegment(ReadOnlySpan<byte> value, bool isFinalBlock, ODataWriterState<TCustomState> state)
    {
        EnsurePropertyWrittenInsideAsyncWriteState(state);
        StreamValueWriter<TCustomState>.Instance.WriteBinarySegment(value, isFinalBlock, state);
    }

    bool IStreamValueWriter<TCustomState>.ShouldFlush(ODataWriterState<TCustomState> state)
    {
        return StreamValueWriter<TCustomState>.Instance.ShouldFlush(state);
    }

    ValueTask IStreamValueWriter<TCustomState>.FlushAsync(ODataWriterState<TCustomState> state)
    {
        return StreamValueWriter<TCustomState>.Instance.FlushAsync(state);
    }

    ValueTask IStreamValueWriter<TCustomState>.FlushIfBufferFullAsync(ODataWriterState<TCustomState> state)
    {
        return StreamValueWriter<TCustomState>.Instance.FlushIfBufferFullAsync(state);
    }

    private void EnsurePropertyWrittenInsideAsyncWriteState(ODataWriterState<TCustomState> state)
    {
        // This method should only be called from inside an async property value writer.
        // In this case, we're in inside a WriteValueAsync method, so we expect the serializer
        // to be suspended with a pending task until the property value writing is complete.
        // This method is called from within the property value writer to ensure the property name
        // is written before writing the value.
        // Since the serialzer is suspended, we should access Stack.LastSuspendedFrame, which remains
        // stable until the property value writing is complete, unlike Stack.Current which may
        // change or be popped as the serializer unwinds to the root.
        Debug.Assert(state.Stack.HasSuspendedFrames());
        if (state.Stack.LastSuspendedFrame.PropertyProgress < PropertyProgress.Name)
        {
            state.JsonWriter.WritePropertyName(this.Utf8Name.Span);
            state.Stack.LastSuspendedFrame.PropertyProgress = PropertyProgress.Name;
        }
    }
}
