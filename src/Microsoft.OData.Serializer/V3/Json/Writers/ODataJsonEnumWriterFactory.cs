using Microsoft.OData.Serializer.V3.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.OData.Serializer.V3.Json.Writers;

internal class ODataJsonEnumWriterFactory : ODataWriterFactory
{
    public override bool CanWrite(Type type)
    {
        return type.IsEnum;
    }

    public override IODataWriter CreateWriter(Type type, ODataSerializerOptions options)
    {
        var writerType = typeof(ODataJsonEnumWriter<>).MakeGenericType(type);
        return (IODataWriter)Activator.CreateInstance(writerType);
    }
}
