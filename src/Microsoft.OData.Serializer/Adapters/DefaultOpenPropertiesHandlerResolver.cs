using System.Collections.Concurrent;

namespace Microsoft.OData.Serializer;

internal class DefaultOpenPropertiesHandlerResolver<TCustomState> : IOpenPropertiesHandlerResolver<TCustomState>
{
    private static readonly OpenPropertiesHandlerFactory<TCustomState>[] _factories =
    [
        new DictionaryOpenPropertiesHandlerFactory<TCustomState>()
    ];

    // TODO: since this is expected to be read far more than written, explore
    // whether using a normal Dictionary behind a ReaderWriterLockSlim would be more efficient. 
    private readonly ConcurrentDictionary<Type, IOpenPropertiesHandler<TCustomState>> _handlerCache = new();

    public IOpenPropertiesHandler<TCustomState>? Resolve(Type dynamicPropertiesContainerType)
    {
        if (_handlerCache.TryGetValue(dynamicPropertiesContainerType, out var handler))
        {
            return handler;
        }

        foreach (var factory in _factories)
        {
            if (factory.CanHandle(dynamicPropertiesContainerType))
            {
                var newHandler = factory.CreateHandler(dynamicPropertiesContainerType);
                _handlerCache.TryAdd(dynamicPropertiesContainerType, newHandler);
                return newHandler;
            }
        }

        return null;
    }
}
