using Microsoft.OData.Edm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.OData.Serializer.Core;

public interface IODataWriterProvider<TState>
{
    public IODataWriter<T, TState> GetWriter<T>(IEdmModel? model); // TODO should we pass the state here?
}
