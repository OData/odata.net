using Microsoft.OData.Edm;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.OData.Core.NewWriter;

internal interface IDynamicPropertiesRetrieverProvider
{
    IDynamicPropertiesRetriever<T> GetDynamicPropertiesRetriever<T>(IEdmStructuredType edmType, ODataWriterContext context);

    IDynamicPropertiesRetriever GetDynamicPropertiesRetriever(IEdmStructuredType edmType, Type valueType, ODataWriterContext context);
}
