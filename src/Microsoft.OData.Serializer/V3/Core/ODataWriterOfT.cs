using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.OData.Serializer.V3.Core;

public abstract class ODataWriter<T, TState> : IODataWriter<T, TState>
{
    public Type? Type { get; } = typeof(T);

    public virtual bool CanWrite(Type type)
    {
        return type == this.Type;
    }

    public abstract ValueTask Write(T value, TState state);
}
