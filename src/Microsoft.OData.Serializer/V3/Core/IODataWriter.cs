using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.OData.Serializer.V3.Core;

public interface IODataWriter
{
    Type? Type { get; }
    bool CanWrite(Type type);
}
