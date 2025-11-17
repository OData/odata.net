using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.OData.Serializer;

public abstract class ODataElementSelector<TResource, TCustomState>
{
    internal ODataElementSelector() // internal constructor to prevent external subclassing
    {
    }

    internal abstract bool WriteElements(TResource resource, ODataWriterState<TCustomState> state);
}
