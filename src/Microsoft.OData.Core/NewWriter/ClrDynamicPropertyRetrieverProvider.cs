using Microsoft.OData.Edm;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.OData.Core.NewWriter;

internal class ClrDynamicPropertyRetrieverProvider : IDynamicPropertiesRetrieverProvider
{
    public IDynamicPropertiesRetriever<T> GetDynamicPropertiesRetriever<T>(IEdmStructuredType edmType, ODataWriterContext context)
    {
        if (!edmType.IsOpen())
        {
            throw new InvalidOperationException("not an open type");
        }

        // TODO: It's bad for perf to create a retriever for the same type each time.
        // We should cache them. But the current retriever implementation also
        // caches the dynamic properties dictionary. This shows that this
        // whole design is bad because it almost forces wasteful caching.
        return new DictionaryDynamicPropertyRetriever<T>();
    }

    public IDynamicPropertiesRetriever GetDynamicPropertiesRetriever(IEdmStructuredType edmType, Type valueType, ODataWriterContext context)
    {
        if (!edmType.IsOpen())
        {
            throw new InvalidOperationException("not an open type");
        }

        var retrieverType = typeof(DictionaryDynamicPropertyRetriever<>).MakeGenericType(valueType);
        var retriever = Activator.CreateInstance(retrieverType);
        if (retriever == null)
        {
            throw new InvalidOperationException($"Failed to create dynamic properties retriever for type '{valueType}' with EDM type '{edmType}'.");
        }

        return retriever as IDynamicPropertiesRetriever;
    }
}
