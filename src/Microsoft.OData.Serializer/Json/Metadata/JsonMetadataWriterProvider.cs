using Microsoft.OData.Edm;
using Microsoft.OData.Serializer.Core;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.OData.Serializer.Json;

public class JsonMetadataWriterProvider(
    IMetadataValueProvider<ODataJsonWriterContext, ODataJsonWriterStack, IEdmProperty> metadataValueProvider)
    : IMetadataWriterProvider<ODataJsonWriterContext, ODataJsonWriterStack, IEdmProperty>
{
    private readonly ConcurrentDictionary<Type, object> _writers = new();
    public IMetadataWriter<ODataJsonWriterContext, ODataJsonWriterStack, TValue, IEdmProperty> GetMetadataWriter<TValue>(
        ODataJsonWriterContext context,
        ODataJsonWriterStack state)
    {
        if (_writers.TryGetValue(typeof(TValue), out var writer))
        {
            return (IMetadataWriter<ODataJsonWriterContext, ODataJsonWriterStack, TValue, IEdmProperty>)writer;
        }

        var newWriter = new JsonMetadataWriter<TValue>(metadataValueProvider);
        _writers.TryAdd(typeof(TValue), newWriter);
        return newWriter;
    }
}
