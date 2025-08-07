using Microsoft.OData.Serializer.V3.Json;
using Microsoft.OData.Serializer.V3.Json.State;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.OData.Serializer.V3.Adapters;

#pragma warning disable CA1005 // Avoid excessive parameters on generic types
public class ODataPropertyInfo<TDeclaringType, TCustomState> :
    ODataPropertyInfo,
    IValueWriter<TCustomState>,
    ICountWriter<TCustomState>,
    INextLinkWriter<TCustomState>,
    IAnnotationWriter<TCustomState>
#pragma warning restore CA1005 // Avoid excessive parameters on generic types
{
    // TODO: this property name conflicts with the method WriteValue from IValueWriter. Perhaps IValueWriter.WriteValue should be renamed?
    // TODO: should we support resumability in custom writes?
    public Func<TDeclaringType, IValueWriter<TCustomState>, ODataJsonWriterState<TCustomState>, bool> WriteValue { get; init; }

    // TODO: implement a shorthad GetValue hook of type Func<TDeclaringType, ODataJsonWriterState<TCustomState>, TValue>
    // which would require an extra generic type parameter TValue that's a bit hard to defined since we'd
    // also want a default cause that only takes 2 parameters <TDeclaringType, TValue>
    // which would conflict with the current <TDeclaringType, TCustomState> definition.

    // TODO: Should nested count handlers be defined on the property info or the type info?
    // We already have count handlers on the type info, keeping them there would provide
    // uniformit. It might be cause of confusion to have to set it multiple places.
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

    public Func<TDeclaringType, ODataJsonWriterState<TCustomState>, long?>? GetCount { get; init; }


    public Action<TDeclaringType, ICountWriter<TCustomState>, ODataJsonWriterState<TCustomState>>? WriteCount { get; init; }

    public AnnotationPosition NextLinkPosition { get; init; } = AnnotationPosition.Auto;

    public Func<TDeclaringType, ODataJsonWriterState<TCustomState>, string?>? GetNextLink { get; init; }

    public Action<TDeclaringType, INextLinkWriter<TCustomState>, ODataJsonWriterState<TCustomState>>? WriteNextLink { get; init; }

    public Func<TDeclaringType, ODataJsonWriterState<TCustomState>, object?>? GetCustomPreValueAnnotations { get; init; }

    public Action<TDeclaringType, IAnnotationWriter<TCustomState>, ODataJsonWriterState<TCustomState>>? WriteCustomPreValueAnnotations { get; init; }

    public Func<TDeclaringType, ODataJsonWriterState<TCustomState>, object?>? GetCustomPostValueAnnotations { get; init; }

    public Action<TDeclaringType, IAnnotationWriter<TCustomState>, ODataJsonWriterState<TCustomState>>? WriteCustomPostValueAnnotations { get; init; }

    public Func<TDeclaringType, ODataJsonWriterState<TCustomState>, bool>? ShouldSkip { get; init; }

    internal bool WriteProperty(TDeclaringType resource, ODataJsonWriterState<TCustomState> state)
    {
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

    internal bool WriteProperty<TValue>(TDeclaringType resource, TValue value, ODataJsonWriterState<TCustomState> state)
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

    private void WritePreValueAnnotations(TDeclaringType resource, ODataJsonWriterState<TCustomState> state)
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

    private void WritePostValueAnnotations(TDeclaringType resource, ODataJsonWriterState<TCustomState> state)
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

    internal protected virtual bool WritePropertyValue(TDeclaringType resource, ODataJsonWriterState<TCustomState> state)
    {
        return WriteValue(resource, this, state);
    }

    internal protected bool WritePropertyValue<TValue>(TDeclaringType resource, TValue value, ODataJsonWriterState<TCustomState> state)
    {
        if (state.Stack.Current.PropertyProgress < PropertyProgress.Name)
        {
            state.JsonWriter.WritePropertyName(this.Utf8Name.Span);
            state.Stack.Current.PropertyProgress = PropertyProgress.Name;
        }

        return state.WriteValue(value);
    }

    private void WriteCountInternal(TDeclaringType resource, ODataJsonWriterState<TCustomState> state)
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

    private void WriteNextLinkInternal(TDeclaringType resource, ODataJsonWriterState<TCustomState> state)
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

    void ICountWriter<TCustomState>.WriteCount(long count, ODataJsonWriterState<TCustomState> state)
    {
        var jsonWriter = state.JsonWriter;
        JsonMetadataHelpers.WritePropertyAnnotationName(jsonWriter, this.Utf8Name.Span, "odata.count"u8);
        jsonWriter.WriteNumberValue(count);
    }

    void INextLinkWriter<TCustomState>.WriteNextLink(ReadOnlySpan<char> nextLink, ODataJsonWriterState<TCustomState> state)
    {
        var jsonWriter = state.JsonWriter;
        JsonMetadataHelpers.WritePropertyAnnotationName(jsonWriter, this.Utf8Name.Span, "odata.nextLink"u8);
        jsonWriter.WriteStringValue(nextLink);
    }

    void INextLinkWriter<TCustomState>.WriteNextLink(ReadOnlySpan<byte> nextLink, ODataJsonWriterState<TCustomState> state)
    {
        var jsonWriter = state.JsonWriter;
        JsonMetadataHelpers.WritePropertyAnnotationName(jsonWriter, this.Utf8Name.Span, "odata.nextLink"u8);
        jsonWriter.WriteStringValue(nextLink);
    }

    void INextLinkWriter<TCustomState>.WriteNextLink(Uri nextLink, ODataJsonWriterState<TCustomState> state)
    {
        var jsonWriter = state.JsonWriter;
        JsonMetadataHelpers.WritePropertyAnnotationName(jsonWriter, this.Utf8Name.Span, "odata.nextLink"u8);
        jsonWriter.WriteStringValue(nextLink.AbsoluteUri);
    }

    bool IValueWriter<TCustomState>.WriteValue<T>(T value, ODataJsonWriterState<TCustomState> state)
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
    void IAnnotationWriter<TCustomState>.WriteAnnotation<TValue>(ReadOnlySpan<char> name, TValue value, ODataJsonWriterState<TCustomState> state)
    {
        var jsonWriter = state.JsonWriter;
        JsonMetadataHelpers.WriteCustomPropertyAnnotationName(jsonWriter, this.Name, name);
        var success = state.WriteValue(value);
        Debug.Assert(success, "Annotation value write did not complete. Resumable writes are not currently supported for annotations.");
    }

    void IAnnotationWriter<TCustomState>.WriteAnnotation<TValue>(ReadOnlySpan<byte> name, TValue value, ODataJsonWriterState<TCustomState> state)
    {
        var jsonWriter = state.JsonWriter;
        JsonMetadataHelpers.WriteCustomPropertyAnnotationName(jsonWriter, this.Utf8Name.Span, name);
        var success = state.WriteValue(value);
        Debug.Assert(success, "Annotation value write did not complete. Resumable writes are not currently supported for annotations.");
    }
}
