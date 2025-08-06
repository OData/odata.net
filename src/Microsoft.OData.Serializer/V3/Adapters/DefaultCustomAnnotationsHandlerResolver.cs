using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.OData.Serializer.V3.Adapters;

internal class DefaultCustomAnnotationsHandlerResolver<TCustomState> : ICustomAnnotationsHandlerResolver<TCustomState>
{
    private static readonly CustomAnnotationsHandlerFactory<TCustomState>[] _factories =
    [
        new DictionaryCustomAnnotationsHandlerFactory<TCustomState>(),
        new EnumerableCustomAnnotationsHandlerFactory<TCustomState>()
    ];

    // TODO: perhaps we should pass the type instead so that annotations handlers are based on annotations type and not instance value.
    // The rationale would be that we do not expect the same ODataTypeInfo or ODataPropertyInfo to have a different annotations
    // container type for different instances. But this is an untested assumption. If we can reason that the annotations container
    // type will not change, then it makes it possible to cache the annotations handler on the ODataType/PropertyInfo after
    // the first time it's resolved.
    public ICustomAnnotationsHandler<TCustomState>? Resolve(object annotations)
    {
        foreach (var factory in _factories)
        {
            if (factory.CanHandle(annotations.GetType()))
            {
                return factory.CreateHandler(annotations.GetType());
            }
        }

        return null;
    }
}
