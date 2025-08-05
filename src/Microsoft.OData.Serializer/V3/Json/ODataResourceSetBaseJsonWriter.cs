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
        if (state.IsTopLevel())
        {
            state.JsonWriter.WriteStartObject();

            await WritePreValueMetadata(value, null, state);

            state.JsonWriter.WritePropertyName("value");
        }
        else
        {
            // TODO: if this is the value of a property, we should write annotations for the property
            // before the array start. Should we write the annotations here in the parent property writer.
            // Since the parent property writer has already written the property name by the time we get here,
            // then we cannot write annotations here.
            // But this creates an issue. The annotations are written by different components depending
            // on whether this is a top-level write or not.
            // Perhaps this component should only be responsible for writing the array and the top-level
            // annotations moved to some parent component
            await WritePreValueMetadata(value, parentProperty, state);
        }

        state.JsonWriter.WriteStartArray();

        await WriteElements(value, state);

        state.JsonWriter.WriteEndArray();

        if (state.IsTopLevel())
        {
            state.JsonWriter.WriteEndObject();
        }

        state.Stack.Pop();
    }

    protected abstract ValueTask WriteElements(TCollection value, ODataJsonWriterState<TCustomState> state);

    protected virtual  ValueTask WritePreValueMetadata(TCollection value, Adapters.ODataPropertyInfo? propertyInfo, ODataJsonWriterState<TCustomState> state)
    {
        // TODO: We should probably expose a ShouldWriteXXX method for metadata to give
        // users an easy way to control whether certain metadata should be written
        if (state.IsTopLevel() && state.MetadataLevel >= ODataMetadataLevel.Minimal)
        {
            return WriteContextUrl(value, state);
        }

        //// TODO: should this condition be implemented by the WriteCountProperty method?
        //if (context.ODataUri.QueryCount.HasValue
        //    && context.ODataUri.QueryCount.Value)
        //{
        //    await WriteCountProperty(value, state, context);
        //}

        WriteNextLinkProperty(value, propertyInfo,  state);
        WriteCountProperty(value, propertyInfo, state);

        return ValueTask.CompletedTask;
    }

    protected virtual ValueTask WriteContextUrl(TCollection value, ODataJsonWriterState<TCustomState> state)
    {
        if (state.PayloadKind == ODataPayloadKind.ResourceSet && state.IsTopLevel())
        {
            ContextUrlHelper.WriteContextUrlProperty(state.PayloadKind, state.ODataUri, state.JsonWriter);
        }

        // TODO: nested context and other payload kinds

        return ValueTask.CompletedTask;
    }

    protected virtual void WriteCountProperty(TCollection value, Adapters.ODataPropertyInfo? propertyInfo, ODataJsonWriterState<TCustomState> state)
    {
        var jsonWriter = state.JsonWriter;
        if (typeInfo?.GetCount != null)
        {
            var count = typeInfo.GetCount(value, state);
            if (count.HasValue)
            {
                jsonWriter.WritePropertyName("odata.count"u8);
                jsonWriter.WriteNumberValue(count.Value);
            }
        }
        else
        {
            typeInfo?.WriteCount?.Invoke(value, this, state);
        }
    }

    protected virtual void WriteNextLinkProperty(TCollection value, Adapters.ODataPropertyInfo? propertyInfo, ODataJsonWriterState<TCustomState> state)
    {

        var jsonWriter = state.JsonWriter;
        if (typeInfo?.GetNextLink != null)
        {
            var nextLink = typeInfo.GetNextLink(value, state);
            if (!string.IsNullOrEmpty(nextLink))
            {
                jsonWriter.WritePropertyName("odata.nextLink"u8);
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

