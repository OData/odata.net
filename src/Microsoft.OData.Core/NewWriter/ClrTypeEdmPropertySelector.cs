using Microsoft.OData.Edm;
using Microsoft.OData.UriParser;
using System;
using System.Collections.Generic;

namespace Microsoft.OData.Core.NewWriter;

internal class ClrTypeEdmPropertySelector<T> : IPropertySelector<T, IEdmProperty>
{
    public IEnumerable<IEdmProperty> GetProperties(T resource, ODataWriterState context)
    {
        // TODO: handle nested properties/nested selects
        var edmType = context.EdmType;
        if (context.SelectAndExpand == null || context.SelectAndExpand.AllSelected)
        {
            if (edmType is IEdmStructuredType structuredType)
            {
                return structuredType.Properties();
            }
        }
        else
        {
            // retrieve properties from the SelectExpandClause
            var selectedItems = context.SelectAndExpand.SelectedItems;
            List<IEdmProperty> selectedProperties = [];
            foreach (var item in selectedItems)
            {

                if (item is PathSelectItem pathSelectItem)
                {
                    var path = pathSelectItem.SelectedPath;
                    var propertySegment = path.FirstSegment as PropertySegment;
                    if (propertySegment != null)
                    {
                        selectedProperties.Add(propertySegment.Property);
                    }
                    
                }
                else if (item is ExpandedNavigationSelectItem navigationItem)
                {
                    // Handle expanded navigation properties if needed
                    var path = navigationItem.PathToNavigationProperty;
                    var propertySegment = path?.FirstSegment as NavigationPropertySegment;
                    if (propertySegment != null) {
                        selectedProperties.Add(propertySegment.NavigationProperty);
                    }
                }


            }

            return selectedProperties;
        }

        throw new Exception($"Failed to get properties because the type '{edmType}' is not a structured type.");
    }
}
