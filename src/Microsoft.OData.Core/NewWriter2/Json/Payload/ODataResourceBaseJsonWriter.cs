using Microsoft.OData.Edm;
using Microsoft.OData.UriParser;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.OData.Core.NewWriter2.Json.Payload;

// Allows extensibility through inheritance
public abstract class ODataResourceBaseJsonWriter<T> : IODataWriter<ODataJsonWriterContext, ODataJsonWriterStack, T>
{
    ConcurrentDictionary<IEdmStructuredType, List<IEdmProperty>> cachedEdmProperties = new();

    public async ValueTask WriteAsync(T value, ODataJsonWriterStack state, ODataJsonWriterContext context)
    {
        // TODO: should 
        // TODO: handle null, should we check value == null? or should have an IsNull method
        var jsonWriter = context.JsonWriter;
        jsonWriter.WriteStartObject();

        // Note: do we need to account for metadata written before/after
        if (context.MetadataLevel >= ODataMetadataLevel.Minimal)
        {
            // Write context URL if needed
            var metadataWriter = context.GetMetadataWriter<T>(state);
            await metadataWriter.WriteEtagPropertyAsync(value, state, context);
        }

        await WriteProperties(value, state, context);

        jsonWriter.WriteEndObject();
    }

    // TODO: consider making this method virtual to allow derived classes to customize the order of properties
    private async ValueTask WriteProperties(T value, ODataJsonWriterStack state, ODataJsonWriterContext context)
    {
        // AspNetCoreOData serializes properties in a certain deterministic order: strucural properties first, the dynamic properties, etc.
        // However, according to the spec, the order is considered insignificant. So in this case we'll go with
        // arbitrary order for performance. But there might be cases where we want to have consistent order with AspNetCore
        // or other libraries, or when handling streaming scenarios where it may be beneficial to serialize
        // properties as they arrive from the source

        // We also use the SelectExpandClause and IEdmModel to determine which properties to write
        // Another approach is to go through all the properties of the resource, and then for each
        // one decide whether to write if its expanded or selected if the caller wants some properties to always be selected.

        var edmType = state.Current.EdmType as IEdmStructuredType;
        var selectExpand = state.Current.SelectExpandClause;

        if (selectExpand == null || selectExpand.AllSelected == true)
        {
            var properties = this.GetEdmProperties(edmType);
            // If all properties are selected, we can write all structural properties
            for (int i = 0; i < properties.Count; i++)
            {
                var property = properties[i];
                if (property.PropertyKind == EdmPropertyKind.Structural)
                {
                    await WriteProperty(value, property, null, state, context);
                }
            }

            // This was too costly due to repeated enumerate allocations
            //foreach (var property in edmType.Properties())
            //{
            //    if (property.PropertyKind == EdmPropertyKind.Structural)
            //    {
            //        await WriteProperty(propertyWriter, value, property, null, state, context);
            //    }
            //}

        }
        else
        {
            // TODO: in the current tests, we expect structural properties before navigation properties.
            // For some reason, the ODataUri parsers return SelectExpandClause with navigation properties first.
            // However, if the order of properties is not significant, we could combine structural
            // and navigation properties in a single loop for better performance.
            foreach (var item in selectExpand.SelectedItems)
            {
                if (item is PathSelectItem pathSelectItem)
                {
                    var propertySegment = pathSelectItem.SelectedPath.LastSegment as PropertySegment;
                    var property = propertySegment.Property;
                    await WriteProperty(value, property, pathSelectItem.SelectAndExpand, state, context);
                }
            }

        }

        if (selectExpand != null)
        {
            foreach (var item in selectExpand.SelectedItems)
            {
                if (item is ExpandedNavigationSelectItem expandedItem)
                {
                    var propertySegment = expandedItem.PathToNavigationProperty.LastSegment as NavigationPropertySegment;
                    var property = propertySegment.NavigationProperty;
                    await WriteProperty(value, property, expandedItem.SelectAndExpand, state, context);
                }
            }
        }

        // TODO: handle dynamic properties
    }

    // TODO: should this be virtual? protected?
    // use case: caller could inherit this class to control the order of properties or decide
    // to auto-select or always-hide properties regardless of SelectExpandClause
    // having access to this method means they don't have to deal with the details
    // of preparing the state for nested properties
    protected virtual async ValueTask WriteProperty(
        T resource,
        IEdmProperty propertyToWrite,
        SelectExpandClause selectExpand,
        ODataJsonWriterStack state,
        ODataJsonWriterContext context)
    {
        if (propertyToWrite.Type.IsStructured() || propertyToWrite.Type.IsStructuredCollection())
        {
            // If the property is a complex type, we need to write it as a nested object
            var nestedState = new ODataJsonWriterStackFrame
            {
                SelectExpandClause = selectExpand,
                EdmType = propertyToWrite.Type.Definition
            };

            state.Push(nestedState);
            await WriteProperty(resource, propertyToWrite, state, context);
            state.Pop();
        }
        else
        {
            // Do we need custom state for non-nested properties?
            await WriteProperty(resource, propertyToWrite, state, context);
        }
    }

    protected virtual async ValueTask WriteProperty(
        T resource,
        IEdmProperty propertyToWrite,
        ODataJsonWriterStack state,
        ODataJsonWriterContext context)
    {
        var jsonWriter = context.JsonWriter;

        if (propertyToWrite.Type.IsCollection())
        {
            // TODO: wh
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
        jsonWriter.WritePropertyName(propertyToWrite.Name);

        // TODO: handle scenario where we don't need to write the value, just annotations.
        // write property value
        await WritePropertyValue(resource, propertyToWrite, state, context);
    }

    protected virtual ValueTask WriteNestedCountProperty(
        T resource,
        IEdmProperty propertyToWrite,
        ODataJsonWriterStack state,
        ODataJsonWriterContext context)
    {
        return ValueTask.CompletedTask;
    }

    protected virtual ValueTask WriteNestedNextLinkProperty(
        T resource,
        IEdmProperty propertyToWrite,
        ODataJsonWriterStack state,
        ODataJsonWriterContext context)
    {
        return ValueTask.CompletedTask;
    }

    protected abstract ValueTask WritePropertyValue(
        T resource,
        IEdmProperty propertyToWrite,
        ODataJsonWriterStack state,
        ODataJsonWriterContext context);

    private List<IEdmProperty> GetEdmProperties(IEdmStructuredType type)
    {
        if (cachedEdmProperties.TryGetValue(type, out var properties))
        {
            return properties;
        }

        properties = type.Properties().ToList();
        cachedEdmProperties[type] = properties;
        return properties;
    }
}
