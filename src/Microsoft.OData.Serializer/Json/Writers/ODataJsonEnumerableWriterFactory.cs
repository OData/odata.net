using System.Collections;
using System.Reflection;

namespace Microsoft.OData.Serializer;

internal class ODataJsonEnumerableWriterFactory<TCustomState> : ODataWriterFactory<TCustomState>
{
    private static readonly Type CustomStateType = typeof(TCustomState);
    private static readonly Type EnumerableType = typeof(IEnumerable);
    private static readonly Type EnumerableGenericDefinition = typeof(IEnumerable<>);
    public override bool CanWrite(Type type)
    {
        //if (TryGetEnumerableElementType(type, out _))
        //{
        //    return true;
        //}

        return type.IsAssignableTo(EnumerableType);
    }

    public override IODataWriter CreateWriter(Type type, ODataSerializerOptions<TCustomState> options)
    {
        if (TryGetEnumerableElementType(type, out Type? elementType))
        {

            var typeInfo = options.TryGetResourceInfo(type);

            var listType = typeof(IList<>).MakeGenericType(elementType!);
            if (listType.IsAssignableFrom(type))
            {
                var listWriterType = typeof(ODataJsonListWriter<,,>).MakeGenericType(type, elementType!, CustomStateType);
                return (IODataWriter)Activator.CreateInstance(listWriterType, [typeInfo]);
            }

            var readOnlyListType = typeof(IReadOnlyList<>).MakeGenericType(elementType!);
            if (readOnlyListType.IsAssignableFrom(type))
            {
                var readOnlyListWriterType = typeof(ODataJsonReadOnlyListWriter<,,>).MakeGenericType(type, elementType!, CustomStateType);
                return (IODataWriter)Activator.CreateInstance(readOnlyListWriterType, [typeInfo]);
            }

            var enumerableType = EnumerableGenericDefinition.MakeGenericType(elementType!);
            var enumerableWriterType = typeof(ODataJsonEnumerableWriter<,,>).MakeGenericType(type, elementType!, CustomStateType);
            return (IODataWriter)Activator.CreateInstance(enumerableWriterType, [typeInfo]);
        }

        // TODO: this would be non-generic cases
        throw new NotSupportedException($"The serializer type does not yet support the enumerable type: '{type.FullName}'");
    }

    private static bool TryGetEnumerableElementType(Type type, out Type? elementType)
    {
        // TODO: Consider checking common enumerable types first (IReadOnlyList<T>, IList<T>, List<T>, etc.) for performance

        elementType = null;
        if (type.IsGenericType)
        {
            // If type is generic, Type<T>, check whether it also implements IEnumerable<T>
            // to avoid scanning for interfaces in the common case.
            var args = type.GetGenericArguments();
            if (args.Length == 1)
            {
                elementType = args[0];

                var enumerableType = typeof(IEnumerable<>).MakeGenericType(elementType);

                if (enumerableType.IsAssignableFrom(type))
                {
                    return true;
                }
            }
        }

        if (TryScanInterfacesForEnumerableElementType(type, out elementType))
        {
            return true;
        }

        return false;
    }

    private static bool TryScanInterfacesForEnumerableElementType(Type type, out Type? elementType)
    {
        elementType = null;
        var enumerable = EnumerableType;
        if (!type.IsAssignableTo(enumerable))
        {
            return false;
        }

        var enumerableInterfaces = type.FindInterfaces(
            (t, _) => t.IsGenericType && t.GetGenericTypeDefinition() == EnumerableGenericDefinition,
            filterCriteria: null);

        if (enumerableInterfaces.Length == 0)
        {
            return false;
        }

        var enumerableInterface = enumerableInterfaces[0];
        elementType = enumerableInterface.GetGenericArguments()[0];
        return true;
    }
}
