using Microsoft.OData.Edm;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.OData.Core.NewWriter2;

internal class EdmPropertyJsonWriterProvider :
    IResourcePropertyWriterProvider<ODataJsonWriterContext, ODataJsonWriterStack, IEdmProperty>
{
    private readonly ConcurrentDictionary<Type, object> _writers = new ConcurrentDictionary<Type, object>();

    public void Add<TResource>(
        IResourcePropertyWriter<TResource, IEdmProperty, ODataJsonWriterStack, ODataJsonWriterContext> writer)
    {
        if (writer == null) throw new ArgumentNullException(nameof(writer));
        _writers[typeof(TResource)] = writer;
    }

    public IResourcePropertyWriter<TResource, IEdmProperty, ODataJsonWriterStack, ODataJsonWriterContext> GetPropertyWriter<TResource>(
        ODataJsonWriterStack state,
        ODataJsonWriterContext context)
    {
        // TODO: should we create dynamically?
        var writer = _writers.GetOrAdd(typeof(TResource), _ => new EdmPropertyJsonWriter<TResource>());
        return writer as EdmPropertyJsonWriter<TResource>;
    }
}
