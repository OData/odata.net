using System.Collections;
using System.Reflection;

namespace Microsoft.OData.Serializer;

internal class ODataJsonEnumerableWriterFactory<TCustomState> : ODataWriterFactory<TCustomState>
{
    private static readonly Type CustomStateType = typeof(TCustomState);
    public override bool CanWrite(Type type)
    {
        if (TryGetEnumerableElementType(type, out _))
        {
            return true;
        }

        return type.IsAssignableTo(typeof(IEnumerable));
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

            var enumerableType = typeof(IEnumerable<>).MakeGenericType(elementType!);
            var enumerableWriterType = typeof(ODataJsonEnumerableWriter<,,>).MakeGenericType(type, elementType!, CustomStateType);
            return (IODataWriter)Activator.CreateInstance(enumerableWriterType, [typeInfo]);
        }

        // TODO: this would be non-generic cases
        throw new NotSupportedException($"Non-genereic enumerable types not yet supports {type.FullName}");
    }

    private static bool TryGetEnumerableElementType(Type type, out Type? elementType)
    {
        elementType = null;
        if (!type.IsGenericType)
        {
            if (TryGetNonGenericEnumerableElementType(type, out elementType))
            {
                return true;
            }
        }

        var args = type.GetGenericArguments();
        if (args.Length != 1)
        {
            return false;
        }

        elementType = args[0];

        var enumerableType = typeof(IEnumerable<>).MakeGenericType(elementType);
        
        return enumerableType.IsAssignableFrom(type);
    }

    private static bool TryGetNonGenericEnumerableElementType(Type type, out Type? elementType)
    {
        elementType = null;
        var enumerable = typeof(IEnumerable);
        if (!type.IsAssignableTo(enumerable))
        {
            return false;
        }

        var enumerableInterfaces = type.FindInterfaces(
            (t, _) => t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IEnumerable<>),
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
