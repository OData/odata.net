using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.OData.Serializer.V3.Adapters;

internal class EnumerableCustomAnnotationsHandlerFactory<TCustomState> : CustomAnnotationsHandlerFactory<TCustomState>
{
    private static readonly Type enumerableTypeDef = typeof(IEnumerable<>);
    private static readonly Type keyValuePairTypeDef = typeof(KeyValuePair<,>);

    public override bool CanHandle(Type type)
    {
        // supports IEnumerable<KeyValuePair<string, TValue>>
        if (!type.IsGenericType)
        {
            return false;
        }

        var typeDefinition = type.GetGenericTypeDefinition();
        if (typeDefinition != enumerableTypeDef)
        {
            return false;
        }

        var itemType = type.GenericTypeArguments[0];
        if (itemType.IsGenericType && itemType.GetGenericTypeDefinition() == keyValuePairTypeDef)
        {
            return itemType.GenericTypeArguments[0] == typeof(string);
        }

        return false;
    }

    public override ICustomAnnotationsHandler<TCustomState> CreateHandler(Type type)
    {
        var valueType = type.GenericTypeArguments[0].GenericTypeArguments[1];
        var handlerType = typeof(EnumerableCustomAnnotationsHandler<,>).MakeGenericType(valueType, typeof(TCustomState));
        var handler = Activator.CreateInstance(handlerType) as ICustomAnnotationsHandler<TCustomState>;
        Debug.Assert(handler != null, "Handler should not be null");
        return handler;
    }
}
