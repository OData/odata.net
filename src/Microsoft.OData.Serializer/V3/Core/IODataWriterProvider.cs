using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.OData.Serializer.V3.Core;

public interface IODataWriterProvider<TState>
{
    public IODataWriter<T, TState> GetWriter<T>(); // TODO should we pass the state here?
}
