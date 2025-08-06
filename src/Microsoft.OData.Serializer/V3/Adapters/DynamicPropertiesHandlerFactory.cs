using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.OData.Serializer.V3.Adapters;

internal abstract class DynamicPropertiesHandlerFactory<TCustomState>
{
    public abstract bool CanHandle(Type type);

    public abstract IDynamicPropertiesHandler<TCustomState> CreateHandler(Type type);
}
