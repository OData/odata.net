using Microsoft.OData.Serializer.Core;
using Microsoft.OData.UriParser;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.OData.Serializer.Json;

public abstract class ODataResourceBasePlainJsonWriter<T, TProperty> : IODataWriter<ODataJsonWriterContext, ODataJsonWriterStack, T>
{
    public async ValueTask WriteAsync(T value, ODataJsonWriterStack state, ODataJsonWriterContext context)
    {
        // TODO: should 
        // TODO: handle null, should we check value == null? or should have an IsNull method
        var jsonWriter = context.JsonWriter;
        jsonWriter.WriteStartObject();

        // Note: do we need to account for metadata written before/after
        if (context.MetadataLevel >= ODataMetadataLevel.Minimal)
        {
            // TODO: Write context URL if needed

            await this.WriteEtagProperty(value, state, context);
        }

        await WriteProperties(value, state, context);

        jsonWriter.WriteEndObject();
    }

    protected abstract ValueTask WriteProperties(T value, ODataJsonWriterStack state, ODataJsonWriterContext context);

    // TODO: Requiring the implementer to return a string is too restrictive and
    // may negatively impact performance.
    // For example
    // - the implementer only has a ReadOnlySpan<char> or char[], so they need to allocate to return a string
    // - the implementer has a StringBuilder or sequence of string fragments, so they need to call string.Join or equivalent
    // - the implementer stores the property name in a larger string, so they have to call string.Substring()
    // - the implementer has a Utf-8 encoded ReadOnlySpan<byte> or byte[], so they have to transcode and allocate to string,
    //   even though we end up re-encoding back to UTF-8 bytes before we write it to the output.
    // We need a way to allow the implementer to extract the property name but not force them to allocate it.
    // Do we expose to them a context.WritePropertyName that accepts multiple overloads? Or do we
    // give them a buffer writer that they can write into? But we don't want to expose the writer directly,
    // if we expose the Utf8JsonWriter's buffer writer, it might get corrupted.
    // We also need to take into account that sometimes we want to prepend the property name as a prefix
    // to a property annotation like "Address@odata.etag", in which case we don't want them to write
    // the property name directly.
    protected abstract string GetPropertyName(
        T value,
        TProperty resourceProperty,
        ODataJsonWriterStack state,
        ODataJsonWriterContext context);

    protected abstract void WritePropertyName(
        T value,
        TProperty resourceProperty,
        ODataJsonWriterStack state,
        ODataJsonWriterContext context);

    protected abstract bool IsPropertyCollection(
        T value,
        TProperty resourceProperty,
        ODataJsonWriterStack state,
        ODataJsonWriterContext context);

    protected abstract bool IsPropertyStructured(
        T value,
        TProperty resourceProperty,
        ODataJsonWriterStack state,
        ODataJsonWriterContext context);

    protected abstract bool IsPropertyStructuredCollection(
        T value,
        TProperty resourceProperty,
        ODataJsonWriterStack state,
        ODataJsonWriterContext context);

    protected void WritePropertyName(
        string propertyName,
        ODataJsonWriterStack state,
        ODataJsonWriterContext context)
    {
        context.JsonWriter.WritePropertyName(propertyName);
    }

    protected void WritePropertyName(
        ReadOnlySpan<char> propertyName,
        ODataJsonWriterStack state,
        ODataJsonWriterContext context)
    {
        context.JsonWriter.WritePropertyName(propertyName);
    }

    protected void WritePropertyName(
        ReadOnlySpan<byte> propertyName,
        ODataJsonWriterStack state,
        ODataJsonWriterContext context)
    {
        context.JsonWriter.WritePropertyName(propertyName);
    }


    // TODO: should this be virtual? protected?
    // use case: caller could inherit this class to control the order of properties or decide
    // to auto-select or always-hide properties regardless of SelectExpandClause
    // having access to this method means they don't have to deal with the details
    // of preparing the state for nested properties
    //protected virtual async ValueTask WriteProperty(
    //    T resource,
    //    TProperty propertyToWrite,
    //    ODataJsonWriterStack state,
    //    ODataJsonWriterContext context)
    //{
    //    // TODO: We don't have access to the SelectExpand or EdmType,
    //    // but we don't know if we need them either, so do we still
    //    // need to push a state node here? Perhaps
    //    // we should allow the user to write custom logic before
    //    // and after each property, then they can decide whether or not
    //    // they want to add anything to the state.
    //    // Checking if property is structured or structured collection
    //    // could be expensive. How can we rethink this?
    //    if (this.IsPropertyStructured(resource, propertyToWrite, state, context)
    //        || this.IsPropertyStructuredCollection(resource, propertyToWrite, state, context))
    //    {
    //        // If the property is a complex type, we need to write it as a nested object
    //        var nestedState = new ODataJsonWriterStackFrame
    //        {
    //            SelectExpandClause = selectExpand,
    //            EdmType = propertyToWrite
    //        };

    //        state.Push(nestedState);
    //        await WriteProperty(resource, propertyToWrite, state, context);
    //        state.Pop();
    //    }
    //    else
    //    {
    //        // Do we need custom state for non-nested properties?
    //        await WriteProperty(resource, propertyToWrite, state, context);
    //    }
    //}

    protected virtual async ValueTask WriteProperty(
        T resource,
        TProperty propertyToWrite,
        ODataJsonWriterStack state,
        ODataJsonWriterContext context)
    {
        var jsonWriter = context.JsonWriter;

        if (this.IsPropertyCollection(resource, propertyToWrite, state, context))
        {
            // The challenge here is that we don't know the value of the type of the property at this point.
            // We could move this logic to the value writer, but we would have written the property name already.
            // That's only reasonable if we either move all property writing logic to the value writer and do away with the property writer,
            // Or we create a metadata provider that is based on the IEdmType rather than the input type.
            // Alternative, we get a type mapper that maps the IEdmType to the CLR type (and vice versa).
            // The challenge with an Edm to CLR type mapper is that, while it's likely that you'd have only one Edm type for a given CLR type,
            // the reverse is less likely (e.g. IEnumerable<Customer>, IList<Customer>, CustomCustomerCollection,
            // could all map to Edmetc. all map to the same Edm type).
            // Another alternative, the metadata resource's metadata writer could be responsible for
            // writing annotations for its properties.
            await WriteNestedCountProperty(resource, propertyToWrite, state, context);
            await WriteNestedNextLinkProperty(resource, propertyToWrite, state, context);
        }

        // if property is collection, we should write annotations if available
        this.WritePropertyName(resource, propertyToWrite, state, context);

        // TODO: handle scenario where we don't need to write the value, just annotations.
        // write property value
        await WritePropertyValue(resource, propertyToWrite, state, context);
    }

    protected virtual async ValueTask WriteEtagProperty(
        T value,
        ODataJsonWriterStack state,
        ODataJsonWriterContext context)
    {
        if (HasEtagValue(value, state, context, out string etagValue))
        {
            context.JsonWriter.WritePropertyName("@odata.etag"u8);

            if (etagValue != null)
            {
                context.JsonWriter.WriteStringValue(etagValue);
            }
            else
            {
                // If etagValue is null, it means the caller acknowledges
                // there's an etag, but did not read the value yet.
                // So let's ask them to write it.
                // TODO: I'm not really pleased with this pattern
                await WriteEtagValue(value, state, context);
            }

        }
    }

    protected virtual bool HasEtagValue(T value, ODataJsonWriterStack state, ODataJsonWriterContext context, out string etagValue)
    {
        etagValue = null;
        return false;
    }
    protected virtual ValueTask WriteEtagValue(T value, ODataJsonWriterStack state, ODataJsonWriterContext context)
    {
        bool hasEtag = HasEtagValue(value, state, context, out var etagValue);
        Debug.Assert(hasEtag);
        Debug.Assert(etagValue != null);
        context.JsonWriter.WriteStringValue(etagValue);
        return ValueTask.CompletedTask;
    }

    protected virtual async ValueTask WriteNestedCountProperty(
        T value,
        TProperty resourceProperty,
        ODataJsonWriterStack state,
        ODataJsonWriterContext context)
    {
        if (HasNestedCountValue(value, resourceProperty, state, context, out long? count))
        {

            JsonMetadataHelpers.WritePropertyAnnotationName(
                context.JsonWriter,
                this.GetPropertyName(value, resourceProperty, state, context),
                "@odata.count"u8);

            if (count.HasValue)
            {
                context.JsonWriter.WriteNumberValue(count.Value);
            }
            else
            {
                await WriteNestedCountValue(value, resourceProperty, state, context);
            }
        }
    }

    protected virtual bool HasNestedCountValue(
        T value,
        TProperty resourceProperty,
        ODataJsonWriterStack state,
        ODataJsonWriterContext context,
        out long? count)
    {
        count = null;
        return false;
    }

    protected virtual ValueTask WriteNestedCountValue(
        T value,
        TProperty propertyToWrite,
        ODataJsonWriterStack state,
        ODataJsonWriterContext context)
    {
        bool hasCount = HasNestedCountValue(value, propertyToWrite, state, context, out var count);
        Debug.Assert(hasCount);
        Debug.Assert(count.HasValue);
        context.JsonWriter.WriteNumberValue(count.Value);
        return ValueTask.CompletedTask;
    }


    protected virtual ValueTask WriteNestedNextLinkProperty(
        T resource,
        TProperty propertyToWrite,
        ODataJsonWriterStack state,
        ODataJsonWriterContext context)
    {
        return ValueTask.CompletedTask;
    }

    protected abstract ValueTask WritePropertyValue(
        T resource,
        TProperty propertyToWrite,
        ODataJsonWriterStack state,
        ODataJsonWriterContext context);
}
