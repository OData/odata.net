using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.OData.Core.NewWriter;

internal interface IPropertySelectorFactory<TResource, TProperty>
{
    IPropertySelector<TResource, TProperty> GetPropertySelector();
}
