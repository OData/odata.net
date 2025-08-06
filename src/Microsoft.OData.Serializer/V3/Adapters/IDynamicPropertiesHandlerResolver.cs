using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.OData.Serializer.V3.Adapters;

internal interface IDynamicPropertiesHandlerResolver<TCustomState>
{
    IDynamicPropertiesHandler<TCustomState>? Resolve(Type dynamicPropertiesContainerType);
}
