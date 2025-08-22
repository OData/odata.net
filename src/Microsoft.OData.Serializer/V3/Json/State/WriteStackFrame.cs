using Microsoft.OData.Serializer.V3.Adapters;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.Pipelines;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.OData.Serializer.V3.Json.State;

internal struct WriteStackFrame<TCustomState>
{
    public ODataTypeInfo? ResourceTypeInfo { get; set; }
    public Adapters.ODataPropertyInfo? PropertyInfo { get; set; }

    public bool IsContinuation { get; set; }

    public ResourceWriteProgress ResourceProgress { get; set; }

    public PropertyProgress PropertyProgress { get; set; }

    public int EnumeratorIndex { get; set; }

    public IEnumerator CurrentEnumerator { get; set; }

    public PipeReader? PipeReader { get; set; }
}
