using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.OData.Serializer.Core;

public interface IODataWriter<TState> : IODataWriter
{
    // Named this WriteObject to avoid conflict with the generic Write<T> in IODataWriter<T, TState>
    bool WriteObject(object value, TState state);
}
