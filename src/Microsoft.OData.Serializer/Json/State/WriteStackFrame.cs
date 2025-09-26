using Microsoft.OData.Serializer.Adapters;
using System.Collections;

namespace Microsoft.OData.Serializer.Json.State;

internal struct WriteStackFrame<TCustomState>
{
    public ODataTypeInfo? ResourceTypeInfo { get; set; }
    public Adapters.ODataPropertyInfo? PropertyInfo { get; set; }

    public bool IsContinuation { get; set; }

    public ResourceWriteProgress ResourceProgress { get; set; }

    public PropertyProgress PropertyProgress { get; set; }

    public int EnumeratorIndex { get; set; }

    public IEnumerator CurrentEnumerator { get; set; }

    public object? StreamingValueSource { get; set; }
    public ResourceCleanupState CleanUpState { get; set; }
    public ValueTask? PendingTask { get; set; }
}
