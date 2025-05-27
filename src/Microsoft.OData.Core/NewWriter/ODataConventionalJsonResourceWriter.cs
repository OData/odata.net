using Microsoft.OData.Edm;
using Microsoft.OData.UriParser;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Microsoft.OData.Core.NewWriter;

internal class ODataConventionalJsonResourceWriter<T>(
    Utf8JsonWriter jsonWriter,
    IPropertySelector<T, IEdmProperty> propertySelector,
    IResourcePropertyWriter<T, IEdmProperty> propertyWriter) : IODataWriter<T>
{
    public async ValueTask WriteAsync(T payload, ODataWriterState state)
    {
        // write start
        jsonWriter.WriteStartObject();
        
        // write annotations

        // TODO: instead of fetching properties, we can iterate over the
        // select expand properties directly so we can properly
        // propagate the nested expand close
        // find list of properties to write
        //var properties = propertySelector.GetProperties(payload, state);
        // for each property, write it out
        //foreach (var property in properties)
        //{
        //    // Does this need to be async?
        //    await propertyWriter.WriteProperty(payload, property, state);
        //}

        var edmType = state.EdmType;
        if (state.SelectAndExpand == null || state.SelectAndExpand.AllSelected)
        {
            if (edmType is IEdmStructuredType structuredType)
            {
                foreach (var edmProperty in structuredType.Properties())
                {
                    if (edmProperty.Type.IsComplex())
                    {
                        var nestedState = new ODataWriterState
                        {
                            WriterContext = state.WriterContext,
                            EdmType = edmProperty.Type.AsComplex().Definition,
                            SelectAndExpand = null // all nested properties are selected
                        };

                        await propertyWriter.WriteProperty(payload, edmProperty, nestedState);
                    }
                    else
                    {
                        // Does this need to be async?
                        await propertyWriter.WriteProperty(payload, edmProperty, state);
                    }
                }
            }
        }
        else
        {
            // retrieve properties from the SelectExpandClause
            var selectedItems = state.SelectAndExpand.SelectedItems;
            List<ExpandedNavigationSelectItem> navigationItems = null;
            foreach (var item in selectedItems)
            {

                if (item is PathSelectItem pathSelectItem)
                {
                    var path = pathSelectItem.SelectedPath;
                    var propertySegment = path.FirstSegment as PropertySegment;
                    if (propertySegment != null)
                    {
                        if (propertySegment.Property.Type.IsComplex())
                        {
                            // If the property is complex, we need to write it as a nested object.
                            // We can use the same property writer, but we need to create a new state
                            // for the nested complex type.
                            var nestedState = new ODataWriterState
                            {
                                WriterContext = state.WriterContext,
                                EdmType = propertySegment.Property.Type.AsComplex().Definition,
                                SelectAndExpand = pathSelectItem.SelectAndExpand
                            };

                            await propertyWriter.WriteProperty(payload, propertySegment.Property, nestedState);
                        }
                        else if (propertySegment.Property.Type.IsCollection())
                        {
                            // If the property is a collection, we should handle it as an array.
                            // The property writer should handle this correctly.
                            await propertyWriter.WriteProperty(payload, propertySegment.Property, state);
                        }
                        else
                        {
                            // For primitive properties, we can write them directly.
                            await propertyWriter.WriteProperty(payload, propertySegment.Property, state);
                        }
                    }

                }
                else if (item is ExpandedNavigationSelectItem navigationItem)
                {
                    // To preserve the order of property writes, we collect them in a separate list.
                    // However, if this is not required by the spec, we should consider writing them
                    // immediately for better performance.
                    if (navigationItems == null)
                    {
                        navigationItems = new List<ExpandedNavigationSelectItem>();
                    }

                    navigationItems.Add(navigationItem);
                    
                    
                }
            }

            if (navigationItems != null)
            {
                // We write nav properties after structured properties to preserve
                // the same order as AspNetCoreOData. But if it's not required by the spec,
                // we should consider writing them as they appear for better performance.
                foreach (var navigationItem in navigationItems)
                {
                    var propertySegment = navigationItem.PathToNavigationProperty.FirstSegment as NavigationPropertySegment;
                    var nestedState = new ODataWriterState
                    {
                        WriterContext = state.WriterContext,
                        EdmType = propertySegment.EdmType.AsElementType(),
                        SelectAndExpand = navigationItem.SelectAndExpand
                    };

                    await propertyWriter.WriteProperty(payload, propertySegment.NavigationProperty, nestedState);
                }
            }
            
        }


        // write end
        jsonWriter.WriteEndObject();
    }

    public ValueTask WriteAsync(object payload, ODataWriterState context)
    {
        return WriteAsync((T)payload, context);
    }
}
