using Microsoft.OData.Serializer.V3.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.OData.Serializer.V3.Adapters;

public interface ICustomAnnotationsHandler<TCustomState>
{
    ValueTask WriteAnnotations(object annotations, IAnnotationWriter<TCustomState> writer, ODataJsonWriterState<TCustomState> state);
}
