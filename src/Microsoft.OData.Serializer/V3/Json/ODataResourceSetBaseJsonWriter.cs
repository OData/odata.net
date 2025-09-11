using Microsoft.OData.Serializer.V3.Adapters;
using Microsoft.OData.Serializer.V3.ContextUrl;
using Microsoft.OData.Serializer.V3.Json.State;
using Microsoft.OData.Serializer.V3.Json.Writers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.OData.Serializer.V3.Json;

#pragma warning disable CA1005 // Avoid excessive parameters on generic types
public abstract class ODataResourceSetBaseJsonWriter<TCollection, TElement, TCustomState>(ODataTypeInfo<TCollection, TCustomState>? typeInfo = null) :
    ODataJsonWriter<TCollection, TCustomState>, ICountWriter<TCustomState>, INextLinkWriter<TCustomState>
#pragma warning restore CA1005 // Avoid excessive parameters on generic types
{
    public override bool Write(TCollection value, ODataWriterState<TCustomState> state)
    {
        Adapters.ODataPropertyInfo? parentProperty = state.IsTopLevel()
            ? null
            : state.Stack.Current.PropertyInfo;
        state.Stack.Push();

        var jsonWriter = state.JsonWriter;

        var progress = state.Stack.Current.ResourceProgress;

        if (progress < ResourceWriteProgress.PreValueMetadata)
        {
            state.Stack.Current.ResourceTypeInfo = typeInfo;
            state.Stack.Current.PropertyInfo = parentProperty;

            typeInfo?.OnSerializing?.Invoke(value, state);

            if (state.IsTopLevel())
            {
                jsonWriter.WriteStartObject();

                WritePreValueMetadata(value, state);

                state.JsonWriter.WritePropertyName("value");
            }
            else
            {
                // In a previous iteration, this was writing nested annotations
                // in this case, annotations are stored on the parent object's propertyInfo
                // and they're prefixed using the property name. But I've changed
                // the model, now the nested annotations of the property that current
                // value belongs to are written by the parent writer.
                // I'm still evaluating to see which models makes more sense.
                //await WritePreValueMetadata(value, parentProperty, state);
            }


            state.JsonWriter.WriteStartArray();

            state.Stack.Current.ResourceProgress = ResourceWriteProgress.PreValueMetadata;

            if (state.ShouldFlush())
            {
                state.Stack.Pop(false);
                return false;
            }
        }

        if (progress < ResourceWriteProgress.Value)
        {
            if (WriteElements(value, state))
            {
                state.Stack.Current.ResourceProgress = ResourceWriteProgress.Value;

                if (state.ShouldFlush())
                {
                    state.Stack.Pop(false);
                    return false;
                }
            }
            else
            {
                state.Stack.Pop(false);
                return false;
            }
        }

        if (progress < ResourceWriteProgress.PostValueMetadata)
        {
            state.JsonWriter.WriteEndArray();

            if (state.IsTopLevel())
            {
                WritePostValueAnnotaitons(value, state);
                state.JsonWriter.WriteEndObject();
            }

            state.Stack.Current.ResourceProgress = ResourceWriteProgress.PostValueMetadata;
            if (state.ShouldFlush())
            {
                state.Stack.Pop(false);
                return false;
            }
        }

        typeInfo?.OnSerialized?.Invoke(value, state);
        state.Stack.Pop(true);
        return true;
    }

    protected abstract bool WriteElements(TCollection value, ODataWriterState<TCustomState> state);

    protected virtual void WritePreValueMetadata(TCollection value, ODataWriterState<TCustomState> state)
    {
        // Since this is only called when top-level, let's also write the context URL
        if (state.MetadataLevel >= ODataMetadataLevel.Minimal)
        {
            WriteContextUrl(state);
        }

        //// TODO: should this condition be implemented by the WriteCountProperty method?
        //if (context.ODataUri.QueryCount.HasValue
        //    && context.ODataUri.QueryCount.Value)
        //{
        //    await WriteCountProperty(value, state, context);
        //}

        WritePreValueAnnotaitons(value, state);
    }

    protected virtual void WritePreValueAnnotaitons(TCollection value, ODataWriterState<TCustomState> state)
    {
        if (typeInfo?.CountPosition != AnnotationPosition.PostValue)
        {
            WriteCountProperty(value, state);
        }
        if (typeInfo?.NextLinkPosition != AnnotationPosition.PostValue)
        {
            WriteNextLinkProperty(value, state);
        }

        if (typeInfo?.GetCustomPreValueAnnotations != null)
        {
            var annotations = typeInfo.GetCustomPreValueAnnotations(value, state);
            if (annotations is not null)
            {
                // TODO: Should we allow GetCustomPreValueAnnotations to return
                // a different type on different calls? If not, we could cache the handler on the writer or type info.
                var jsonWriter = state.JsonWriter;
                var handler = state.GetCustomAnnotationsHandler(annotations.GetType());
                handler.WriteAnnotations(annotations, CustomInstanceAnnotationWriter<TCustomState>.Instance, state);
            }
        }
        else if (typeInfo?.WriteCustomPreValueAnnotations != null)
        {
            typeInfo.WriteCustomPreValueAnnotations(value, CustomInstanceAnnotationWriter<TCustomState>.Instance, state);
        }
    }

    protected virtual void WritePostValueAnnotaitons(TCollection value, ODataWriterState<TCustomState> state)
    {
        if (typeInfo?.CountPosition == AnnotationPosition.PostValue)
        {
            WriteCountProperty(value, state);
        }

        if (typeInfo?.NextLinkPosition == AnnotationPosition.PostValue)
        {
            WriteNextLinkProperty(value, state);
        }

        // TODO: perhaps this logic should be moved to the type info?
        if (typeInfo?.GetCustomPostValueAnnotations != null)
        {
            var annotations = typeInfo.GetCustomPostValueAnnotations(value, state);
            if (annotations is not null)
            {
                // TODO: Should we allow GetCustomPreValueAnnotations to return
                // a different type on different calls? If not, we could cache the handler on the type info or writer.
                var jsonWriter = state.JsonWriter;
                var handler = state.GetCustomAnnotationsHandler(annotations.GetType());
                handler.WriteAnnotations(annotations, CustomInstanceAnnotationWriter<TCustomState>.Instance, state);
            }
        }
        else if (typeInfo?.WriteCustomPostValueAnnotations != null)
        {
            typeInfo.WriteCustomPostValueAnnotations(value, CustomInstanceAnnotationWriter<TCustomState>.Instance, state);
        }
    }

    private static void WriteContextUrl(ODataWriterState<TCustomState> state)
    {
        if (state.PayloadKind == ODataPayloadKind.ResourceSet && state.IsTopLevel())
        {
            ContextUrlHelper.WriteContextUrlProperty(state.PayloadKind, state.ODataUri, state.JsonWriter);
        }

        // TODO: nested context and other payload kinds
    }

    protected virtual void WriteCountProperty(TCollection value, ODataWriterState<TCustomState> state)
    {
        var jsonWriter = state.JsonWriter;
        if (typeInfo?.GetCount != null)
        {
            var count = typeInfo.GetCount(value, state);
            if (count.HasValue)
            {
                jsonWriter.WritePropertyName("@odata.count"u8);
                jsonWriter.WriteNumberValue(count.Value);
            }
        }
        else
        {
            typeInfo?.WriteCount?.Invoke(value, this, state);
        }
    }

    protected virtual void WriteNextLinkProperty(TCollection value, ODataWriterState<TCustomState> state)
    {

        var jsonWriter = state.JsonWriter;
        if (typeInfo?.GetNextLink != null)
        {
            var nextLink = typeInfo.GetNextLink(value, state);
            if (!string.IsNullOrEmpty(nextLink))
            {
                jsonWriter.WritePropertyName("@odata.nextLink"u8);
                jsonWriter.WriteStringValue(nextLink);
            }
        }
        else
        {
            typeInfo?.WriteNextLink?.Invoke(value, this, state);
        }
    }

    public void WriteCount(long count, ODataWriterState<TCustomState> state)
    {
        var jsonWriter = state.JsonWriter;
        jsonWriter.WritePropertyName("odata.count"u8);
        jsonWriter.WriteNumberValue(count);
    }

    public void WriteNextLink(ReadOnlySpan<char> nextLink, ODataWriterState<TCustomState> state)
    {
        var jsonWriter = state.JsonWriter;
        jsonWriter.WritePropertyName("odata.nextLink"u8);
        jsonWriter.WriteStringValue(nextLink);
    }

    public void WriteNextLink(ReadOnlySpan<byte> nextLink, ODataWriterState<TCustomState> state)
    {
        var jsonWriter = state.JsonWriter;
        jsonWriter.WritePropertyName("odata.nextLink"u8);
        jsonWriter.WriteStringValue(nextLink);
    }

    public void WriteNextLink(Uri nextLink, ODataWriterState<TCustomState> state)
    {
        var jsonWriter = state.JsonWriter;
        jsonWriter.WritePropertyName("odata.nextLink"u8);
        jsonWriter.WriteStringValue(nextLink.AbsoluteUri);
    }
}

