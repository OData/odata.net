using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.OData.Core.NewWriter;

internal interface IPropertySelector<TResource, TProperty>
{
    IEnumerable<TProperty> GetProperties(TResource resource, ODataWriterState context);
}