using Microsoft.OData.Serializer.V3.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.OData.Serializer.Core;

public abstract class ODataWriterFactory<TCustomState> : IODataWriter
{
    public Type? Type => null;

    public abstract bool CanWrite(Type type);

    public abstract IODataWriter CreateWriter(Type type, ODataSerializerOptions<TCustomState> options);
}
