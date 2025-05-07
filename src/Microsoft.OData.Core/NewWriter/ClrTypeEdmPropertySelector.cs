using Microsoft.OData.Edm;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.OData.Core.NewWriter;

internal class ClrTypeEdmPropertySelector<T> : IPropertySelector<T, IEdmProperty>
{
    public IEnumerable<IEdmProperty> GetProperties(T resource, ODataWriterState context)
    {
        var edmType = context.EdmType;
        if (edmType is IEdmStructuredType structuredType)
        {
            return structuredType.Properties();
        }

        throw new Exception($"Failed to get properties because the type '{edmType}' is not a structured type.");
    }
}
