using Microsoft.OData.Serializer.V3.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.OData.Serializer.V3.Json.Writers;

internal class ODataJsonEnumWriterFactory<TCustomState> : ODataWriterFactory<TCustomState>
{
    private static readonly Type CustomStateType = typeof(TCustomState);
    public override bool CanWrite(Type type)
    {
        return type.IsEnum;
    }

    public override IODataWriter CreateWriter(Type type, ODataSerializerOptions<TCustomState> options)
    {
        var writerType = typeof(ODataJsonEnumWriter<,>).MakeGenericType(type, CustomStateType);
        return (IODataWriter)Activator.CreateInstance(writerType);
    }
}
