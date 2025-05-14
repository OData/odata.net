using Microsoft.OData.Edm;
using Microsoft.OData.UriParser;
using System;
using System.Collections.Generic;
using System.Text;

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
            var selectedPropertyNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
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

            }

            return selectedProperties;
        }

        throw new Exception($"Failed to get properties because the type '{edmType}' is not a structured type.");
    }
}
