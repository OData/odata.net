using Microsoft.OData.Serializer.Json.State;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.OData.Serializer.Adapters;

internal interface ICustomPropertyAnnotationsHandler<TCustomState>
{
    void WriteCustomPropertyAnnotations(
        ReadOnlySpan<char> propertyName,
        object annotations,
        ODataWriterState<TCustomState> state);
}
