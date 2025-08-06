using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.OData.Serializer.V3.Adapters;

internal interface ICustomAnnotationsHandlerResolver<TCustomState>
{
    ICustomAnnotationsHandler<TCustomState>? Resolve(object annotations);
}
