using System.Diagnostics;

namespace Microsoft.OData.Serializer.Adapters;

internal class DictionaryDynamicPropertiesHandlerFactory<TCustomState> : DynamicPropertiesHandlerFactory<TCustomState>
{
    private static readonly Type[] _supportedGenericTypeDefs = [
        typeof(Dictionary<,>),
        typeof(IDictionary<,>),
        typeof(IReadOnlyDictionary<,>)];

    public override bool CanHandle(Type type)
    {
        // TODO: currently, custom type not supported. But could be supported in the future.
        if (!type.IsGenericType)
        {
            return false;
        }

        var typeDefinition = type.GetGenericTypeDefinition();

        foreach (var supportedTypeDef in _supportedGenericTypeDefs)
        {
            if (typeDefinition == supportedTypeDef)
            {
                return type.GenericTypeArguments[0] == typeof(string);
            }
        }

        return false;
    }

    public override IDynamicPropertiesHandler<TCustomState> CreateHandler(Type type)
    {
        var valueType = type.GenericTypeArguments[1];
        var handlerType = typeof(EnumerableDynamicPropertyHandler<,>).MakeGenericType(valueType, typeof(TCustomState));
        var handler = Activator.CreateInstance(handlerType) as IDynamicPropertiesHandler<TCustomState>;
        Debug.Assert(handler != null, "Handler should not be null");
        return handler!;
    }
}
