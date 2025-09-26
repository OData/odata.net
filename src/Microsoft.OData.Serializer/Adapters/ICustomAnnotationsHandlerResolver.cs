using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.OData.Serializer.Adapters;

internal interface ICustomAnnotationsHandlerResolver<TCustomState>
{
    ICustomAnnotationsHandler<TCustomState>? Resolve(Type annotationsContainerType);
}
