using Microsoft.OData.Serializer.V3.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.OData.Serializer.V3.Adapters;

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
