using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.OData.Serializer.V3.Core;

public interface IODataWriterProvider
{
    public IODataWriter<T, TState> GetWriter<T, TState>(TState state); // TODO should we pass the state here?
}
