using Microsoft.OData.Serializer.V3.Adapters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.OData.Serializer.V3.Json;

public abstract class ODataResourceSetBaseJsonWriter<TCollection, TElement>(ODataResourceTypeInfo<TCollection>? typeInfo = null) : ODataJsonWriter<TCollection>
{
    public override async ValueTask Write(TCollection value, ODataJsonWriterState state)
    {
        Adapters.ODataPropertyInfo? parentProperty = state.IsTopLevel()
            ? null
            : state.Stack.Current.PropertyInfo;
        state.Stack.Push();
        state.Stack.Current.ResourceTypeInfo = typeInfo;
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

    protected abstract ValueTask WriteElements(TCollection value, ODataJsonWriterState state);

    protected virtual async ValueTask WritePreValueMetadata(TCollection value, Adapters.ODataPropertyInfo? propertyInfo, ODataJsonWriterState state)
    {
        // TODO: We should probably expose a ShouldWriteXXX method for metadata to give
        // users an easy way to control whether certain metadata should be written
        if (state.IsTopLevel() && state.MetadataLevel >= ODataMetadataLevel.Minimal)
        {
            await WriteContextUrl(value, state);
        }

        //// TODO: should this condition be implemented by the WriteCountProperty method?
        //if (context.ODataUri.QueryCount.HasValue
        //    && context.ODataUri.QueryCount.Value)
        //{
        //    await WriteCountProperty(value, state, context);
        //}

        await WriteNextLinkProperty(value, propertyInfo,  state);
        await WriteCountProperty(value, propertyInfo, state);
    }

    protected virtual ValueTask WriteContextUrl(TCollection value, ODataJsonWriterState state)
    {
        if (state.PayloadKind == ODataPayloadKind.ResourceSet && state.IsTopLevel())
        {
            ContextUrlHelper.WriteContextUrlProperty(state.PayloadKind, state);
        }

        // TODO: nested context and other payload kinds

        return ValueTask.CompletedTask;
    }

    protected virtual ValueTask WriteCountProperty(TCollection value, Adapters.ODataPropertyInfo? propertyInfo, ODataJsonWriterState state)
    {
        var jsonWriter = state.JsonWriter;
        if (typeInfo?.HasCount == null)
        {
            // What should be the default?
            return ValueTask.CompletedTask;
        }

        if (typeInfo.HasCount(value, state))
        {
            // TODO: Currently this is handled in the parent type info. But perhaps it's better to handle it here.
            //if (propertyInfo != null)
            //{
            //    JsonMetadataHelpers.WritePropertyAnnotationName(jsonWriter, propertyInfo.Utf8Name.Span, "odata.count"u8);
            //}
            //else
            //{
            //    state.JsonWriter.WritePropertyName("@odata.count"u8);
            //}
            
            state.JsonWriter.WritePropertyName("odata.count"u8);

            // TODO: this should be validated when type info is registered.
            if (typeInfo.WriteCount == null)
            {
                throw new Exception("WriteCount function must be provided if HasCount returns true");
            }

            return typeInfo.WriteCount(value, state);
        }

        return ValueTask.CompletedTask;
    }

    protected virtual ValueTask WriteNextLinkProperty(TCollection value, Adapters.ODataPropertyInfo? propertyInfo, ODataJsonWriterState state)
    {

        var jsonWriter = state.JsonWriter;
        if (typeInfo?.HasNextLink == null)
        {
            // What should be the default?
            return ValueTask.CompletedTask;
        }

        if (typeInfo.HasNextLink(value, state))
        {
            if (propertyInfo != null)
            {
                JsonMetadataHelpers.WritePropertyAnnotationName(jsonWriter, propertyInfo.Utf8Name.Span, "odata.nextLink"u8);
            }
            else
            {
                state.JsonWriter.WritePropertyName("@odata.nextLink"u8);
            }

            // TODO: this should be validated when type info is registered.
            if (typeInfo.WriteNextLink == null)
            {
                throw new Exception("WriteNextLink function must be provided if HastNextLink returns true");
            }

            return typeInfo.WriteNextLink(value, state);
        }

        return ValueTask.CompletedTask;
    }
}

