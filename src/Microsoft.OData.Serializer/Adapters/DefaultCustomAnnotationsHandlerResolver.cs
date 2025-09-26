using System.Collections.Concurrent;

namespace Microsoft.OData.Serializer.Adapters;

internal class DefaultCustomAnnotationsHandlerResolver<TCustomState> : ICustomAnnotationsHandlerResolver<TCustomState>
{
    private static readonly CustomAnnotationsHandlerFactory<TCustomState>[] _factories =
    [
        new DictionaryCustomAnnotationsHandlerFactory<TCustomState>(),
        new EnumerableCustomAnnotationsHandlerFactory<TCustomState>()
    ];

    // TODO: since this is expected to be read far more than written, explore
    // whether using a normal Dictionary behind a ReaderWriterLockSlim would be more efficient. 
    private readonly ConcurrentDictionary<Type, ICustomAnnotationsHandler<TCustomState>> _handlerCache = new();

    // TODO: perhaps we should pass the type instead so that annotations handlers are based on annotations type and not instance value.
    // The rationale would be that we do not expect the same ODataTypeInfo or ODataPropertyInfo to have a different annotations
    // container type for different instances. But this is an untested assumption. If we can reason that the annotations container
    // type will not change, then it makes it possible to cache the annotations handler on the ODataType/PropertyInfo after
    // the first time it's resolved.
    public ICustomAnnotationsHandler<TCustomState>? Resolve(Type annotationsContainerType)
    {
        if (_handlerCache.TryGetValue(annotationsContainerType, out var handler))
        {
            return handler;
        }

        foreach (var factory in _factories)
        {
            if (factory.CanHandle(annotationsContainerType))
            {
                var newHandler = factory.CreateHandler(annotationsContainerType);
                _handlerCache.TryAdd(annotationsContainerType, newHandler);
                return newHandler;
            }
        }

        return null;
    }
}
