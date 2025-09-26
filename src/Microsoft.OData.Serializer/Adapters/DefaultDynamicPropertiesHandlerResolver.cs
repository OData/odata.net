using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.OData.Serializer.Adapters;

internal class DefaultDynamicPropertiesHandlerResolver<TCustomState> : IDynamicPropertiesHandlerResolver<TCustomState>
{
    private static readonly DynamicPropertiesHandlerFactory<TCustomState>[] _factories =
    [
        new DictionaryDynamicPropertiesHandlerFactory<TCustomState>()
    ];

    // TODO: since this is expected to be read far more than written, explore
    // whether using a normal Dictionary behind a ReaderWriterLockSlim would be more efficient. 
    private readonly ConcurrentDictionary<Type, IDynamicPropertiesHandler<TCustomState>> _handlerCache = new();

    public IDynamicPropertiesHandler<TCustomState>? Resolve(Type dynamicPropertiesContainerType)
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