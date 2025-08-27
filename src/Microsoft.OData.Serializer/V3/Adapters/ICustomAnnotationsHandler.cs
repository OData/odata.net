using Microsoft.OData.Serializer.V3.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.OData.Serializer.V3.Adapters;

public interface ICustomAnnotationsHandler<TCustomState>
{
    // TODO: should we return bool instead and make these resumable as well?
    void WriteAnnotations(object annotations, IAnnotationWriter<TCustomState> writer, ODataWriterState<TCustomState> state);
}
