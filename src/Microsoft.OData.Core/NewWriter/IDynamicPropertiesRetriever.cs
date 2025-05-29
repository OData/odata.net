using Microsoft.OData.Edm;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.OData.Core.NewWriter;

internal interface IDynamicPropertiesRetriever
{
    IEnumerable<(string PropertyName, IEdmTypeReference PropertyType, object Value)> GetDynamicProperties(object value, ODataWriterState state);
}

interface IDynamicPropertiesRetriever<T> : IDynamicPropertiesRetriever
{
    IEnumerable<(string PropertyName, IEdmTypeReference PropertyType, object Value)> GetDynamicProperties(T value, ODataWriterState state);
}