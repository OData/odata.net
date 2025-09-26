using Microsoft.OData.Serializer.Json.State;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.OData.Serializer.Adapters;

internal interface IDynamicPropertiesHandler<TCustomState>
{
    // TODO: support resumability for dynamic properties
    void WriteDynamicProperties<TResource>(
        TResource resource,
        object dynamicProperties,
        Func<TResource, string, ODataWriterState<TCustomState>, object?>? getPropertyPreValueAnnotations,
        Func<TResource, string, ODataWriterState<TCustomState>, object?>? getPropertyPostValueAnnotations,
        IDynamicPropertyWriter<TCustomState> writer,
        ODataWriterState<TCustomState> state);
}
