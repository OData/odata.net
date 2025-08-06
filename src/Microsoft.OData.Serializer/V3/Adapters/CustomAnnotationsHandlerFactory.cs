using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.OData.Serializer.V3.Adapters;

internal abstract class CustomAnnotationsHandlerFactory<TCustomState>
{
    public abstract bool CanHandle(Type type);

    public abstract ICustomAnnotationsHandler<TCustomState> CreateHandler(Type type);
}
