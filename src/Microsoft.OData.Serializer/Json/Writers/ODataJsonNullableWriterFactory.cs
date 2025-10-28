using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.OData.Serializer.Json.Writers;

internal class ODataJsonNullableWriterFactory<TCustomState> : ODataWriterFactory<TCustomState>
{
    private static readonly Type NullableGenericDefinition = typeof(Nullable<>);
    public override bool CanWrite(Type type)
    {
        return type.IsGenericType && type.GetGenericTypeDefinition() == NullableGenericDefinition;
    }

    public override IODataWriter CreateWriter(Type type, ODataSerializerOptions<TCustomState> options)
    {
        var genericArgument = type.GetGenericArguments().First();
        var writerType = typeof(ODataJsonNullableOfTWriter<,>).MakeGenericType(genericArgument, typeof(TCustomState));
        return (IODataWriter)Activator.CreateInstance(writerType);
    }
}
