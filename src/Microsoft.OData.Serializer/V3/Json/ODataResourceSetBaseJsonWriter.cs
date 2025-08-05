using Microsoft.OData.Serializer.V3.Adapters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.OData.Serializer.V3.Json;

#pragma warning disable CA1005 // Avoid excessive parameters on generic types
public abstract class ODataResourceSetBaseJsonWriter<TCollection, TElement, TCustomState>(ODataResourceTypeInfo<TCollection, TCustomState>? typeInfo = null) :
    ODataJsonWriter<TCollection, TCustomState>, ICountWriter<TCustomState>, INextLinkWriter<TCustomState>
#pragma warning restore CA1005 // Avoid excessive parameters on generic types
{
    public override async ValueTask Write(TCollection value, ODataJsonWriterState<TCustomState> state)
    {
        Adapters.ODataPropertyInfo? parentProperty = state.IsTopLevel()
            ? null
            : state.Stack.Current.PropertyInfo;
        state.Stack.Push();
        state.Stack.Current.ResourceTypeInfo = typeInfo;
        state.Stack.Current.PropertyInfo = parentProperty;

        typeInfo?.OnSerializing?.Invoke(value, state);

        if (state.IsTopLevel())
        {
            state.JsonWriter.WriteStartObject();

            await WritePreValueMetadata(value, state);

            state.JsonWriter.WritePropertyName("value");
        }
        else
        {
            // In a previous iteration, this was writing nested annotations
            // in this case, annotations are stored on the parent object's propertyInfo
            // and they're prefixed using the property name. But I've changed
            // the model, now the nested annotations of the property that current
            // valu belongs to are written by the parent writer.
            // I'm still evaluating to see which models makes more sense.
            //await WritePreValueMetadata(value, parentProperty, state);
        }

        state.JsonWriter.WriteStartArray();

        await WriteElements(value, state);

        state.JsonWriter.WriteEndArray();

        if (state.IsTopLevel())
        {
            state.JsonWriter.WriteEndObject();
        }

        typeInfo?.OnSerialized?.Invoke(value, state);

        state.Stack.Pop();
    }

    protected abstract ValueTask WriteElements(TCollection value, ODataJsonWriterState<TCustomState> state);

    protected virtual  ValueTask WritePreValueMetadata(TCollection value, ODataJsonWriterState<TCustomState> state)
    {
        if (state.IsTopLevel() && state.MetadataLevel >= ODataMetadataLevel.Minimal)
        {
            WriteContextUrl(value, state);
        }

        //// TODO: should this condition be implemented by the WriteCountProperty method?
        //if (context.ODataUri.QueryCount.HasValue
        //    && context.ODataUri.QueryCount.Value)
        //{
        //    await WriteCountProperty(value, state, context);
        //}

        WriteNextLinkProperty(value, state);
        WriteCountProperty(value, state);

        return ValueTask.CompletedTask;
    }

    protected virtual void WriteContextUrl(TCollection value, ODataJsonWriterState<TCustomState> state)
    {
        if (state.PayloadKind == ODataPayloadKind.ResourceSet && state.IsTopLevel())
        {
            ContextUrlHelper.WriteContextUrlProperty(state.PayloadKind, state.ODataUri, state.JsonWriter);
        }

        // TODO: nested context and other payload kinds
    }

    protected virtual void WriteCountProperty(TCollection value, ODataJsonWriterState<TCustomState> state)
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

    protected virtual void WriteNextLinkProperty(TCollection value, ODataJsonWriterState<TCustomState> state)
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

    public void WriteCount(long count, ODataJsonWriterState<TCustomState> state)
    {
        var jsonWriter = state.JsonWriter;
        jsonWriter.WritePropertyName("odata.count"u8);
        jsonWriter.WriteNumberValue(count);
    }

    public void WriteNextLink(ReadOnlySpan<char> nextLink, ODataJsonWriterState<TCustomState> state)
    {
        var jsonWriter = state.JsonWriter;
        jsonWriter.WritePropertyName("odata.nextLink"u8);
        jsonWriter.WriteStringValue(nextLink);
    }

    public void WriteNextLink(ReadOnlySpan<byte> nextLink, ODataJsonWriterState<TCustomState> state)
    {
        var jsonWriter = state.JsonWriter;
        jsonWriter.WritePropertyName("odata.nextLink"u8);
        jsonWriter.WriteStringValue(nextLink);
    }

    public void WriteNextLink(Uri nextLink, ODataJsonWriterState<TCustomState> state)
    {
        var jsonWriter = state.JsonWriter;
        jsonWriter.WritePropertyName("odata.nextLink"u8);
        jsonWriter.WriteStringValue(nextLink.AbsoluteUri);
    }
}

