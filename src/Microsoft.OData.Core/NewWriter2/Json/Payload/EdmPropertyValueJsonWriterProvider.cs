using Microsoft.OData.Edm;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.OData.Core.NewWriter2;

internal class EdmPropertyValueJsonWriterProvider : IPropertyValueWriterProvider<ODataJsonWriterContext, ODataJsonWriterStack, IEdmProperty>
{
    private readonly ConcurrentDictionary<Type, object> _writers = new();

    public void Add<TResource>(IPropertyValueWriter<ODataJsonWriterContext, ODataJsonWriterStack, TResource, IEdmProperty> writer)
    {
        _writers[typeof(TResource)] = writer;
    }

    public IPropertyValueWriter<ODataJsonWriterContext, ODataJsonWriterStack, TResource, IEdmProperty> GetPropertyValueWriter<TResource>(
        TResource resource,
        IEdmProperty property,
        ODataJsonWriterStack state,
        ODataJsonWriterContext context)
    {
        // TODO: Should we dynamically create the writer if it doesn't exist?
        if (!_writers.TryGetValue(typeof(TResource), out var writerObj))
        {
            throw new InvalidOperationException($"No property value writer registered for resource type {typeof(TResource)}.");
        }
        
        return (IPropertyValueWriter<ODataJsonWriterContext, ODataJsonWriterStack, TResource, IEdmProperty>)writerObj;
    }
}
