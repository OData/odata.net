using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.OData.Serializer;

public partial class ODataPropertyInfo<TDeclaringType, TCustomState>
{
    public Func<TDeclaringType, IValueReader<TCustomState>, ODataReaderState<TCustomState>, bool>? ReadValue { get; init; }


    internal bool ReadProperty(TDeclaringType resource, ODataReaderState<TCustomState> state)
    {
        bool success = this.ReadPropertyValue(resource, state);
        Debug.Assert(success, "Resumability not yet supported.");

        return true;
    }

    internal protected virtual bool ReadPropertyValue(TDeclaringType resource, ODataReaderState<TCustomState> state)
    {
        Debug.Assert(this.ReadValue != null, "ReadValue should not be null.");
        return ReadValue(resource, DefaultJsonValueReader<TCustomState>.Instance, state);
    }
}
