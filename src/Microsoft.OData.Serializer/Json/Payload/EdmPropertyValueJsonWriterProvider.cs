using Microsoft.OData.Edm;
using Microsoft.OData.Serializer.Core;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.OData.Serializer.Json;

internal class EdmPropertyValueJsonWriterProvider : IPropertyValueWriterProvider<ODataJsonWriterContext, ODataJsonWriterStack, IEdmProperty>
{
    private readonly ConcurrentDictionary<Type, object> _writers = new();

    public void Add<TResource>(IPropertyValueWriter<ODataJsonWriterContext, ODataJsonWriterStack, TResource, IEdmProperty> writer)
    {
        _writers[typeof(TResource)] = writer;
    }

    public void Add<TResource>(Action<TResource, IEdmProperty, ODataJsonWriterStack, ODataJsonWriterContext> writerAction)
    {
        Add(new EdmPropertyValueWriterWithAction<TResource>(writerAction));
    }

    public void Add<TResource>(Func<TResource, IEdmProperty, ODataJsonWriterStack, ODataJsonWriterContext, ValueTask> writeAsyncFunc)
    {
        Add(new EdmPropertyValueWriterWithAsyncFunc<TResource>(writeAsyncFunc));
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

    class EdmPropertyValueWriterWithAction<TResource> : IPropertyValueWriter<ODataJsonWriterContext, ODataJsonWriterStack, TResource, IEdmProperty>
    {
        private readonly Action<TResource, IEdmProperty, ODataJsonWriterStack, ODataJsonWriterContext> _writeAction;
        public EdmPropertyValueWriterWithAction(Action<TResource, IEdmProperty, ODataJsonWriterStack, ODataJsonWriterContext> writeAction)
        {
            _writeAction = writeAction ?? throw new ArgumentNullException(nameof(writeAction));
        }

        public ValueTask WritePropertyValue(TResource resource, IEdmProperty property, ODataJsonWriterStack state, ODataJsonWriterContext context)
        {
            _writeAction(resource, property, state, context);
            return ValueTask.CompletedTask;
        }
    }

    class EdmPropertyValueWriterWithAsyncFunc<TResource> : IPropertyValueWriter<ODataJsonWriterContext, ODataJsonWriterStack, TResource, IEdmProperty>
    {
        private readonly Func<TResource, IEdmProperty, ODataJsonWriterStack, ODataJsonWriterContext, ValueTask> _writeAsyncFunc;
        public EdmPropertyValueWriterWithAsyncFunc(Func<TResource, IEdmProperty, ODataJsonWriterStack, ODataJsonWriterContext, ValueTask> writeAsyncFunc)
        {
            _writeAsyncFunc = writeAsyncFunc ?? throw new ArgumentNullException(nameof(writeAsyncFunc));
        }
        public ValueTask WritePropertyValue(TResource resource, IEdmProperty property, ODataJsonWriterStack state, ODataJsonWriterContext context)
        {
            return _writeAsyncFunc(resource, property, state, context);
        }
    }

}
