using Microsoft.OData.Serializer.Json.State;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.OData.Serializer.Adapters;

public abstract class ODataPropertySelector<TResource, TCustomState>
{
    internal abstract bool WriteProperties(TResource resource, ODataWriterState<TCustomState> state);
}
