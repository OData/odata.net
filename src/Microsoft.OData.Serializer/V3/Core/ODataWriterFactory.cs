using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.OData.Serializer.V3.Core;

public abstract class ODataWriterFactory : IODataWriter
{
    public abstract bool CanWrite(Type type);

    public abstract IODataWriter CreateWriter(Type type); // TODO pass options or context
}
