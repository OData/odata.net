using Microsoft.OData.Edm;
using Microsoft.OData.Serializer.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Microsoft.OData.Serializer.Json;

public abstract class ODataResourceSetBaseJsonWriter<TCollection, TElement> : IODataWriter<ODataJsonWriterContext, ODataJsonWriterStack, TCollection>
{
    public async ValueTask WriteAsync(TCollection value, ODataJsonWriterStack state, ODataJsonWriterContext context)
    {
        if (state.IsTopLevel())
        {
            context.JsonWriter.WriteStartObject();

            await WritePreValueMetadata(value, state, context);

            context.JsonWriter.WritePropertyName("value");
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
        }

        context.JsonWriter.WriteStartArray();

        await WriteElements(value, state, context);

        context.JsonWriter.WriteEndArray();

        if (state.IsTopLevel())
        {
            context.JsonWriter.WriteEndObject();
        }
    }

    protected abstract ValueTask WriteElements(TCollection collection, ODataJsonWriterStack state, ODataJsonWriterContext context);

    protected virtual async ValueTask WriteElement(TElement element, ODataJsonWriterStack state, ODataJsonWriterContext context)
    {
        var frame = new ODataJsonWriterStackFrame()
        {
            SelectExpandClause = state.Current.SelectExpandClause,
            EdmType = state.Current.EdmType.AsElementType(),
        };

        state.Push(frame);

        var resourceWriter = context.GetValueWriter<TElement>(state);
        await resourceWriter.WriteAsync(element, state, context);

        state.Pop();
    }

    protected virtual async ValueTask WritePreValueMetadata(TCollection value, ODataJsonWriterStack state, ODataJsonWriterContext context)
    {
        // TODO: We should probably expose a ShouldWriteXXX method for metadata to give
        // users an easy way to control whether certain metadata should be written
        if (context.MetadataLevel >= ODataMetadataLevel.Minimal)
        {
            await WriteContextUrl(value, state, context);
        }

        // TODO: should this condition be implemented by the WriteCountProperty method?
        if (context.ODataUri.QueryCount.HasValue
            && context.ODataUri.QueryCount.Value)
        {
            await WriteCountProperty(value, state, context);
        }

        await WriteNextLinkProperty(value, state, context);
    }

    protected virtual async ValueTask WriteContextUrl(TCollection value, ODataJsonWriterStack state, ODataJsonWriterContext context)
    {
        var metadataWriter = context.GetMetadataWriter<TCollection>(state);
        await metadataWriter.WriteContextUrlAsync(value, state, context);
    }

    protected virtual async ValueTask WriteCountProperty(TCollection value, ODataJsonWriterStack state, ODataJsonWriterContext context)
    {
        if (this.HasCountValue(value, state, context, out long? count))
        {

            context.JsonWriter.WritePropertyName("@odata.count"u8);

            if (count.HasValue)
            {
                context.JsonWriter.WriteNumberValue(count.Value);
            }
            else
            {
                await this.WriteCountValue(value, state, context);
            }
        }
    }

    protected virtual bool HasCountValue(
        TCollection value,
        ODataJsonWriterStack state,
        ODataJsonWriterContext context,
        out long? count)
    {
        count = null;
        return false;
    }

    protected virtual ValueTask WriteCountValue(
        TCollection value,
        ODataJsonWriterStack state,
        ODataJsonWriterContext context)
    {
        return ValueTask.CompletedTask;
    }

    protected virtual ValueTask WriteNextLinkProperty(
        TCollection value,
        ODataJsonWriterStack state,
        ODataJsonWriterContext context)
    {
        if (HasNextLinkValue(value, state, context, out var nextLink))
        {
            context.JsonWriter.WritePropertyName("@odata.nextLink");
            if (nextLink == null)
            {
                // If nextLink is null, it means the caller acknowleges
                // there's a next link, but did not read the value yet.
                // So let's ask them to write it.
                WriteNextLinkValue(value, state, context);
            }
            else
            {
                context.JsonWriter.WriteStringValue(nextLink.AbsoluteUri);
            }
        }

        return ValueTask.CompletedTask;
    }

    protected virtual bool HasNextLinkValue(
        TCollection value,
        ODataJsonWriterStack state,
        ODataJsonWriterContext context,
        out Uri nextLink) // TODO: We should not couple ourselves to the Uri type, too expensive.
    {
        nextLink = null;
        return false;
    }

    protected virtual void WriteNextLinkValue(
        TCollection value,
        ODataJsonWriterStack state,
        ODataJsonWriterContext context)
    {
        // TODO: for this implementation of NextLinkRetriever, we assume HasNextLink always returns the nextLink
        // and therefore this method should not be called. But we implement it just in case.
        // Or should we throw an exception instead? Or have some strategy pattern that doesn't require this to be called when not needed?
        bool hasNextLink = HasNextLinkValue(value, state, context, out var nextLink);
        Debug.Assert(hasNextLink == true, "WriteNextLinkValue should only be called if HasNextLinkValue returned true.");
        Debug.Assert(nextLink != null);
        context.JsonWriter.WriteStringValue(nextLink.AbsoluteUri);
    }

    protected virtual bool HasNestedNextLinkValue(
        TCollection value,
        IEdmProperty prop,
        ODataJsonWriterStack state,
        ODataJsonWriterContext context,
        out Uri nextLink) // TODO: We should not couple ourselves to the Uri type, too expensive.
    {
        nextLink = null;
        return false;
    }

    protected virtual void WriteNestedNextLinkValue(
        TCollection value,
        IEdmProperty prop,
        ODataJsonWriterStack state,
        ODataJsonWriterContext context)
    {
        throw new NotImplementedException();
    }
}
