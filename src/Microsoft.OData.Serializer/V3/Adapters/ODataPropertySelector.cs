using Microsoft.OData.Serializer.V3.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.OData.Serializer.V3.Adapters;

public abstract class ODataPropertySelector<TResource, TCustomState>
{
    internal abstract bool WriteProperties(TResource resource, ODataWriterState<TCustomState> state);
}
